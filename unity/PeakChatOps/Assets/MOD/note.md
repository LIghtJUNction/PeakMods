渲染固定在地图上的做法
Unity 官方文档给出的通用套路，是先用深度纹理和逆视图投影矩阵 (UNITY_MATRIX_I_VP) 在后处理里还原每个像素的世界坐标，然后用这个坐标去做你真正想要的采样或写入[^1]。一旦拿到了 worldPos，就可以把它投到某个稳定的平面（比如 XZ）上计算 UV：float2 historyUV = (worldPos.xz - centerXZ) / worldExtent + 0.5;。这样历史纹理或标记就和地面绑定，不再随着摄像机剪裁平移。
当你用 RenderGraph/Blitter 处理屏幕纹理时，仍然可以采用同样思路：让 Pass 0 在片元阶段还原世界坐标，Pass 1 把 worldPos.xz 写进一张“世界坐标贴图”或栅格化的 RT。摄像机移动时，sample 的坐标始终来自世界，相当于做了一次“地面坐标→纹理坐标”的投影，渲染结果就不会晃动。
如果需要大范围覆盖，可以把世界坐标缩放成一个循环的网格（Modulo 或 frac），或者分块维护多个 RT。关键是不要再用屏幕空间 uv 做历史采样，而是用世界空间映射。
[^1]: Unity URP Manual《Reconstruct the world space positions of pixels from the depth texture》展示了完整的 HLSL 代码：先计算 UV = positionHCS.xy / _ScaledScreenParams.xy，采样 SampleSceneDepth(UV)，再调用 ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP) 得到世界坐标。https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@11.0/manual/writing-shaders-urp-reconstruct-world-position.html

让坡度计算与视角无关
深度+法线纹理里的法线是视图空间法线，所以你抬头低头时，其 y 分量会跟着摄像机旋转。这正是你看到颜色跳变的原因。
解决办法是把它们乘以当前帧的 cameraToWorldMatrix（或者在 C# 里传入矩阵），把视图空间法线转成世界空间法线，再和世界 Up 向量求夹角。Ronja 的后处理法线教程给出了完整示例：DecodeDepthNormal 得到 viewNormal 之后执行 normal = mul((float3x3)_viewToWorld, normal);，这样同一个地面位置无论摄像机怎么转，normal.y 都稳定[^2]。
拿到世界空间法线后，就可以用 float slope = degrees(acos(saturate(dot(worldNormal, float3(0,1,0))))); 计算坡度。Unity 社区的讨论也证实了这种“法线与全局 Up 向量的夹角”是最直接的坡度角度算法[^3]。
顺便检查一下：如果你在 Pass 1 更新历史图时又重新用了视图空间法线，需要同步改成世界空间法线，否则持久化的颜色仍然会漂移。
[^2]: Ronja Tutorials《Postprocessing with Normal Texture》详细说明了如何在后处理里把 DepthNormals 转成世界空间法线，并指出“如果只用视图空间法线，旋转摄像机时同一点的法线会改变”，所以必须传入 _viewToWorld 矩阵。https://www.ronja-tutorials.com/post/018-postprocessing-normal/

[^3]: Unity Discussions《Getting slope angle from normals》给出的答案是对法线与 Vector3.up 求角度来得到坡度；这在 Shader 中可以直接用 dot 或 Vector3.Angle 等价实现。https://discussions.unity.com/t/getting-slope-angle-from-normals/142841

