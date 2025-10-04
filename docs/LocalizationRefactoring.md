# 本地化系统重构总结

## 重构日期
2025年10月4日

## 重构背景

原先的本地化方案使用 Harmony 补丁来覆盖游戏的 `LocalizedText.GetText` 方法，存在以下问题：

1. **方法重载冲突**：`LocalizedText.GetText` 有多个重载，Harmony 无法明确确定要补丁的目标方法，导致 `AmbiguousMatchException` 运行时错误
2. **维护复杂**：需要管理 Harmony 补丁的生命周期和执行顺序
3. **内存浪费**：加载所有语言的翻译数据，而实际只使用一种语言
4. **依赖游戏系统**：需要等待游戏的本地化系统加载完成才能工作

## 重构方案

创建独立的 `PLocalizedText` 类，完全替代 Harmony 补丁方案：

### 新增文件
- `src/PeakChatOps/Core/PLocalizedText.cs` - 自定义本地化文本类

### 删除文件
- `src/PeakChatOps/Patches/LocalizedTextOverlay.cs` - 原 Harmony 补丁类

### 修改文件
1. **PeakChatOps.cs**
   - 移除 `_harmony.PatchAll(typeof(LocalizationPatches))`
   - 移除 `LocalizedText.LoadMainTable(true)` 强制重载
   - 添加 `PLocalizedText.Init()` 初始化调用
   - 更新 `Help()` 调用，使用 `PLocalizedText.GetText`

2. **Config.cs**
   - 添加 `using PeakChatOps.core;`
   - 批量替换所有 `LocalizedText.GetText` → `PLocalizedText.GetText`

3. **PeakChatOpsUI.cs**
   - 添加 `using PeakChatOps.core;`
   - 更新欢迎消息加载：`LocalizedText.GetText` → `PLocalizedText.GetText`
   - 更新 Help 方法日志

4. **ai.cs**
   - 添加 `using PeakChatOps.core;`
   - 更新错误提示文本：`LocalizedText.GetText` → `PLocalizedText.GetText`

## 新系统特性

### 1. 内存优化
- **旧方案**：加载所有 15 种语言的数据
- **新方案**：仅加载当前语言的数据
- **节省内存**：约 93%（15列 → 1列）

### 2. 性能优化
- 使用 `Dictionary<string, string>` 存储，O(1) 查找时间
- 不需要 Harmony 运行时拦截和反射调用

### 3. 独立性
- 完全独立于游戏的本地化系统
- 不依赖游戏本地化表的加载状态
- 避免与游戏或其他 Mod 的补丁冲突

### 4. 语言检测
自动检测当前游戏语言的逻辑：
1. 从游戏 `LocalizedText.GetText("CURRENT_LANGUAGE")` 获取当前语言名
2. 在 CSV 第一行中查找匹配的语言列
3. 如果找不到精确匹配，尝试模糊匹配（如简体中文）
4. 默认回退到英语列（索引 1）

### 5. CSV 解析
自定义 CSV 解析器，支持：
- 引号包裹的字段（包含逗号）
- 引号转义（`""` 表示一个引号字符）
- 自动处理行尾的 `ENDLINE` 标记

## API 对比

### 旧方案
```csharp
// 使用游戏的 LocalizedText 类（通过 Harmony 补丁覆盖）
string text = LocalizedText.GetText("KEY");
```

### 新方案
```csharp
// 使用自定义 PLocalizedText 类
string text = PLocalizedText.GetText("KEY");

// 带回退值
string text = PLocalizedText.GetText("KEY", "Default Value");

// 检查键是否存在
if (PLocalizedText.HasKey("KEY")) { ... }

// 热重载（调试）
PLocalizedText.Reload();
```

## 编译结果

所有代码修改已通过编译测试：
```
还原完成(0.5)
PeakChatOps 已成功 (0.5 秒) → artifacts\bin\PeakChatOps\debug\com.github.LIghtJUNction.PeakChatOps.dll
在 1.2 秒内生成 已成功
```

## 测试计划

### 单元测试项
1. ✅ 编译通过
2. ⏳ 运行时初始化日志检查
3. ⏳ 各语言环境下文本正确显示
4. ⏳ 缺失键的回退机制
5. ⏳ CSV 解析特殊字符（引号、逗号）
6. ⏳ 配置项本地化文本显示

### 建议的游戏内测试
1. 启动游戏，检查日志中 `[PLocalizedText] Loaded X localization entries for language index Y`
2. 打开聊天界面，验证欢迎消息显示正确的语言
3. 打开配置界面，检查配置项描述是否正确本地化
4. 切换游戏语言，重启后验证文本是否切换
5. 测试 `/ai` 命令错误提示是否正确本地化

## 向后兼容性

### CSV 文件格式
完全兼容现有的 `Localization.csv` 格式，无需修改。

### 配置文件
不影响用户的配置文件，配置键和值保持不变。

### API 变更
仅内部实现变更，外部调用接口签名保持一致：
```csharp
// 旧：LocalizedText.GetText(key)
// 新：PLocalizedText.GetText(key)
```
仅需要修改命名空间引用。

## 已知限制

1. **静态语言**：初始化时确定语言，不支持运行时切换（需要重启游戏）
2. **单一 CSV**：目前仅支持单个 `Localization.csv`，不支持分模块加载
3. **无参数化**：不支持参数化文本（如 `"You have {0} items"`）

## 未来改进方向

1. **运行时语言切换**：监听游戏语言变化事件，自动重新加载
2. **多 CSV 支持**：支持子模块独立的本地化文件
3. **参数化文本**：支持占位符和格式化字符串
4. **富文本支持**：支持 Unity 富文本标记
5. **CSV 验证工具**：开发编辑器工具验证 CSV 完整性和一致性

## 相关文档

- [PLocalizedText API 文档](./PLocalizedText.md)
- [CSV 文件格式说明](../src/PeakChatOps/Localization.csv)

## 变更统计

- **新增代码**：~220 行（PLocalizedText.cs）
- **删除代码**：~70 行（LocalizedTextOverlay.cs）
- **修改文件**：4 个（PeakChatOps.cs, Config.cs, PeakChatOpsUI.cs, ai.cs）
- **替换调用**：21 处（`LocalizedText.GetText` → `PLocalizedText.GetText`）

## 结论

本次重构成功将本地化系统从 Harmony 补丁方案迁移到独立的自定义类方案，解决了原有的运行时冲突问题，同时带来了内存和性能优化。新系统更加独立、可维护，并为未来的功能扩展提供了良好的基础。
