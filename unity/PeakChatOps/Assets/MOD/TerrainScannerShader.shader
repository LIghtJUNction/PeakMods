Shader "Hidden/TerrainScanner"
{
    Properties
    {
        _MainTex("Source Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "TerrainScan"
            ZTest Always
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            // 全屏三角形顶点着色器
            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };
            
            Varyings Vert(Attributes input)
            {
                Varyings output;
                // 全屏三角形技巧
                float2 uv = float2((input.vertexID << 1) & 2, input.vertexID & 2);
                output.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);
                output.texcoord = uv;
                #if UNITY_UV_STARTS_AT_TOP
                    output.texcoord.y = 1.0 - output.texcoord.y;
                #endif
                return output;
            }
            
            // Shader 参数
            // 使用 _BlitTexture 而不是 _MainTex！RenderGraph 会自动绑定
            sampler2D _BlitTexture;
            sampler2D _CameraDepthTexture;
            sampler2D _ScanHistoryTexture; // 持久化扫描历史
            
            float3 _ScanCenter;
            float _ScanRange;
            float _ScanWidth;
            float4x4 _CamToWorld;
            float3 _CamWorldPos;
            float _CamFar;
            
            float4 frag(Varyings input) : SV_Target
            {
                // 采样源纹理（相机渲染结果）- 使用 _BlitTexture
                float2 uv = input.texcoord;
                float4 sceneColor = tex2D(_BlitTexture, uv);
                
                // 如果源纹理无效，直接返回
                if (sceneColor.a < 0.01)
                {
                    return sceneColor;
                }
                
                // 采样深度
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                
                // 跳过天空等远距离像素
                if (depth >= 0.9999 || depth <= 0.0001)
                {
                    return sceneColor;
                }
                
                // 转换为线性深度（视图空间）
                float linearDepth = LinearEyeDepth(depth);
                
                // 验证深度有效性
                if (linearDepth <= 0.0 || linearDepth > _CamFar)
                {
                    return sceneColor;
                }
                
                // 重建世界坐标（手动计算，不用不存在的函数）
                float2 screenPos = uv * 2.0 - 1.0;
                float3 viewPos = float3(screenPos.x, screenPos.y, 1.0) * linearDepth;
                float3 worldPos = mul(_CamToWorld, float4(viewPos, 1.0)).xyz;
                
                // 从深度法线纹理采样（_CameraDepthTexture 包含深度+法线）
                float4 depthNormalSample = tex2D(_CameraDepthTexture, uv);
                
                // 解码法线（Unity DepthNormals 编码）
                float3 viewNormal;
                float decodedDepth;
                DecodeDepthNormal(depthNormalSample, decodedDepth, viewNormal);
                
                // 视图空间法线转世界空间（使用传入的 _CamToWorld 矩阵）
                float3 worldNormal = mul((float3x3)_CamToWorld, viewNormal);
                worldNormal = normalize(worldNormal);
                
                // 确保法线朝上
                if (worldNormal.y < 0.0) worldNormal = -worldNormal;
                
                // 计算坡度（世界空间法线 Y 分量表示平坦程度）
                // worldNormal.y = 1 表示完全水平，= 0 表示垂直
                float slopeAngle = degrees(acos(saturate(worldNormal.y)));
                
                // 根据坡度确定颜色
                float3 terrainColor;
                if (slopeAngle < 30.0)
                {
                    terrainColor = float3(0.0, 1.0, 0.0); // 绿色 - 平地
                }
                else if (slopeAngle < 50.0)
                {
                    terrainColor = float3(1.0, 1.0, 0.0); // 黄色 - 可站立斜坡
                }
                else
                {
                    terrainColor = float3(1.0, 0.0, 0.0); // 红色 - 危险陡坡
                }
                
                // 计算到扫描中心的距离
                float dist = distance(worldPos, _ScanCenter);
                
                // 计算扫描波强度（只在波的边缘显示）
                float scanEdge = abs(dist - _ScanRange);
                float scanIntensity = 1.0 - saturate(scanEdge / _ScanWidth);
                scanIntensity = pow(scanIntensity, 2.0); // 让扫描波更锐利
                
                // 如果不在扫描波范围内，直接返回原场景
                if (scanIntensity < 0.01)
                {
                    return sceneColor;
                }
                
                // 生成稀疏的标记点（使用世界坐标的网格）
                float markerSpacing = 2.0; // 标记间距（世界单位）
                float2 gridUV = frac(worldPos.xz / markerSpacing);
                
                // 创建叉叉形状（两条对角线）
                float crossSize = 0.15; // 叉叉大小
                float crossThickness = 0.08; // 线条粗细
                
                // 对角线1：从左下到右上
                float diag1 = abs(gridUV.x - gridUV.y);
                // 对角线2：从左上到右下
                float diag2 = abs(gridUV.x + gridUV.y - 1.0);
                
                // 组合成叉叉，只在中心附近显示
                float centerDist = distance(gridUV, float2(0.5, 0.5));
                float crossMask = 0.0;
                if (centerDist < crossSize)
                {
                    crossMask = (1.0 - smoothstep(0.0, crossThickness, diag1)) + 
                                (1.0 - smoothstep(0.0, crossThickness, diag2));
                    crossMask = saturate(crossMask);
                }
                
                // 标记强度随扫描波衰减
                float markerIntensity = crossMask * scanIntensity * 0.7;
                
                // 读取历史扫描数据（RGBA: RGB=颜色, A=强度）
                float4 history = tex2D(_ScanHistoryTexture, uv);
                
                // 混合：场景 + 新扫描 + 历史扫描
                float3 finalColor = sceneColor.rgb;
                
                // 叠加新扫描的地形标记
                finalColor = lerp(finalColor, terrainColor, markerIntensity);
                
                // 叠加历史扫描（淡出效果）
                finalColor = lerp(finalColor, history.rgb, history.a * 0.6);
                
                // 叠加青色扫描波边缘光晕
                float3 scanColor = float3(0.0, 1.0, 1.0);
                finalColor += scanColor * scanIntensity * 0.3;
                
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        // Pass 1: 更新历史纹理（保存扫描结果）
        Pass
        {
            Name "UpdateScanHistory"
            ZTest Always
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex VertHistory
            #pragma fragment fragHistory
            
            #include "UnityCG.cginc"
            
            // 顶点着色器输入输出结构体
            struct AttributesHistory
            {
                uint vertexID : SV_VertexID;
            };
            
            struct VaryingsHistory
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };
            
            VaryingsHistory VertHistory(AttributesHistory input)
            {
                VaryingsHistory output;
                float2 uv = float2((input.vertexID << 1) & 2, input.vertexID & 2);
                output.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);
                output.texcoord = uv;
                #if UNITY_UV_STARTS_AT_TOP
                    output.texcoord.y = 1.0 - output.texcoord.y;
                #endif
                return output;
            }
            
            sampler2D _BlitTexture; // 历史纹理输入
            sampler2D _CameraDepthTexture;
            
            float3 _ScanCenter;
            float _ScanRange;
            float _ScanWidth;
            float4x4 _CamToWorld;
            float _CamFar;
            float _FadeSpeed; // 淡出速度
            
            float4 fragHistory(VaryingsHistory input) : SV_Target
            {
                float2 uv = input.texcoord;
                
                // 读取当前历史数据
                float4 history = tex2D(_BlitTexture, uv);
                
                // 采样深度
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                if (depth >= 0.9999 || depth <= 0.0001)
                {
                    // 天空等无效区域，淡出历史
                    history.a *= _FadeSpeed;
                    return history;
                }
                
                // 重建世界坐标
                float linearDepth = LinearEyeDepth(depth);
                float2 screenPos = uv * 2.0 - 1.0;
                float3 viewPos = float3(screenPos.x, screenPos.y, 1.0) * linearDepth;
                float3 worldPos = mul(_CamToWorld, float4(viewPos, 1.0)).xyz;
                
                // 解码法线
                float4 depthNormalSample = tex2D(_CameraDepthTexture, uv);
                float3 viewNormal;
                float decodedDepth;
                DecodeDepthNormal(depthNormalSample, decodedDepth, viewNormal);
                float3 worldNormal = mul((float3x3)_CamToWorld, viewNormal);
                worldNormal = normalize(worldNormal);
                if (worldNormal.y < 0.0) worldNormal = -worldNormal;
                
                // 计算坡度和颜色
                float slopeAngle = degrees(acos(saturate(worldNormal.y)));
                float3 terrainColor;
                if (slopeAngle < 30.0)
                    terrainColor = float3(0.0, 1.0, 0.0);
                else if (slopeAngle < 50.0)
                    terrainColor = float3(1.0, 1.0, 0.0);
                else
                    terrainColor = float3(1.0, 0.0, 0.0);
                
                // 计算扫描波强度
                float dist = distance(worldPos, _ScanCenter);
                float scanEdge = abs(dist - _ScanRange);
                float scanIntensity = 1.0 - saturate(scanEdge / _ScanWidth);
                scanIntensity = pow(scanIntensity, 2.0);
                
                // 如果在扫描波内，更新历史
                if (scanIntensity > 0.01)
                {
                    // 生成叉叉标记
                    float2 gridUV = frac(worldPos.xz / 2.0);
                    float diag1 = abs(gridUV.x - gridUV.y);
                    float diag2 = abs(gridUV.x + gridUV.y - 1.0);
                    float centerDist = distance(gridUV, float2(0.5, 0.5));
                    float crossMask = 0.0;
                    if (centerDist < 0.15)
                    {
                        crossMask = (1.0 - smoothstep(0.0, 0.08, diag1)) + 
                                    (1.0 - smoothstep(0.0, 0.08, diag2));
                        crossMask = saturate(crossMask);
                    }
                    
                    float markerIntensity = crossMask * scanIntensity;
                    
                    // 更新历史：颜色和强度
                    if (markerIntensity > 0.01)
                    {
                        history.rgb = terrainColor;
                        history.a = max(history.a, markerIntensity);
                    }
                }
                
                // 淡出历史
                history.a *= _FadeSpeed;
                
                return history;
            }
            ENDHLSL
        }
    }
}