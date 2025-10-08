# PeakMods

PeakMods 是一组针对游戏（Unity）插件/Mod 的集合和工具，包含用于地形扫描（TerrainScanner）和 PeakChatOps 等子项目。

本仓库包含若干 Unity 相关模块与原生工具，主要用于在游戏中进行地形扫描、标记渲染与调试辅助功能。

## 目录结构（节选）

- `src/` - C# 源代码（主要是 Unity 可复用模块和插件）
	- `TerrainScanner/` - 地形扫描功能的实现（ScanFeature、ScanRaySampling、ScanMarkRenderer 等）
	- `PeakChatOps/` - 聊天/操作扩展模块
- `unity/` - 示例/Unity 项目（编辑器工程、着色器、示例场景）
- `docs/` - 设计与迁移文档

## 快速开始（开发者）

以下命令在 Windows + PowerShell 下执行（项目使用 dotnet 与 Unity 开发工具链）：

构建 C# 项目（仅 .NET 打包/发布）：

```powershell
dotnet build -c Release -target:PackTS
```

（Unity 编辑器内的打包/测试请在 `unity/` 下打开对应的 Unity 工程）

## 在 Unity 中使用 TerrainScanner

1. 把 `src/TerrainScanner` 编译成 DLL 或将源码直接放入 Unity 项目 `Assets/Plugins`（按你的项目结构）。
2. 在场景中添加 `ActiveScan` 的 GameObject（或者在玩家的摄像机对象上挂 `ActiveScan`），默认按键触发扫描：`Q`。
	 - 触发逻辑位于 `src/TerrainScanner/DS/ActiveScan.cs`：按下 `Q` 时调用 `ScanFeature.ExecuteScan(transform)`。
3. 确保 `ScanConfig` 中的资源（`scanMaterial`、`markMaterial`、三种粒子预制件）已在运行时注入或在 `ScanConfigManager` 注册时提供。

## 主要功能与文件说明

- `ScanFeature`：Unity 的 `ScriptableRendererFeature`，负责触发扫描、控制 shader 动画、把标记数据交给渲染通道。
- `ScanRaySampling`：射线采样逻辑，生成地形命中回调（支持列/行遍历、逐行 yield 以避免卡帧）。
- `ScanMarkRenderer`：把采样到的标记打包到 GPU 的 ComputeBuffer 并通过 `DrawMeshInstancedIndirect` 绘制。
- `ParticleSpawner`：集中化的粒子实例化辅助方法。
- `ScanConfig`：运行时可调的配置项（包括采样距离、网格大小、粒子概率等）。

关键配置（`src/TerrainScanner/Config.cs`）示例：

- `horizontalCount`, `verticalCount`：采样网格尺寸（列、行）。
- `gridStep`：网格采样间距（米）。
- `sampling_originHeightOffset`：射线起点相对于摄像机的高度偏移（可提高以覆盖更高地形）。
- `sampling_maxDistanceShort` / `sampling_maxDistanceLong`：射线长度设定。

## 常见问题与排查

- 扫描到数据但只渲染部分标记：
	- 检查 `ScanMarkRenderer` 创建的 `ComputeBuffer` 大小（count/stride）是否与 `Marks` 结构一致；检查 GPU 着色器中 `Marks` 定义（字段顺序/类型）是否匹配。
	- 着色器内部不要在片元阶段写入深度（`SV_DEPTH`），这会造成实例间的深度冲突/遮挡问题。
	- 如果扫描数据非常多，考虑在上传前裁剪或设置上限（避免一次性上传超大量实例）。

- 射线高度不够：
	- 修改 `ScanConfig.sampling_originHeightOffset` 提高起点；必要时同时增加 `sampling_maxDistanceShort`。

## 调试与日志

- 插件使用 `TerrainScannerPlugin.Logger` 打日志（在 BepInEx/Unity 控制台输出）。
- 本仓库默认只保留错误级别日志以便在运行时更容易发现问题。如果需要临时更多信息，可在 `ScanMarkRenderer`、`ScanRaySampling` 中恢复 `LogInfo` 或 `LogWarning`。

## 开发建议

- 在修改采样或传播算法时，尽量：
	1. 在 `PerformGridSamples` 内使用 `await UniTask.Yield()` 分片执行，避免冻结主线程。
	2. 对每个格子使用量化键（例如 0.1m 精度）进行去重，防止浮点噪声引发重复入队。
	3. 对传播（propagation）设计上加入去重（queued/processed set）和处理上限（processedLimit），防止无限膨胀。

## 贡献

欢迎提交 Issue 和 Pull Request：

- 新特性或修复：提交到 `main` 分支前请先讨论大改动。
- 提交 PR 前请确保通过 `dotnet build`（或在 Unity 中本地测试）并附上简要变更说明。

## 联系

如需更详细帮助，请在仓库内打开 Issue，或把运行时日志（Unity 控制台）和重现场景说明附上，便于复现问题。

---

（本 README 旨在快速上手与排查核心问题；如需我帮你把 README 翻译成英文或加入屏幕截图/示例场景说明，我可以继续完善。）
