Shader "Hidden/TerrainScannerTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _ScanRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 获取原始颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 在屏幕中心画一个扩散的红色圆圈
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float radius = (_ScanRange * 0.01); // 随 range 扩散
                
                // 如果在圆圈范围内，显示红色
                if (dist < radius && dist > radius - 0.05)
                {
                    return fixed4(1, 0, 0, 1); // 纯红色圆环
                }
                
                return col; // 原始颜色
            }
            ENDCG
        }
    }
}
