# 从 Harmony 补丁迁移到 PLocalizedText

## 快速开始

如果你是从旧版本的 PeakChatOps 升级，本地化系统已经完全重写。你不需要做任何操作，只需要知道：

✅ **向后兼容**：现有的 `Localization.csv` 文件格式完全兼容，无需修改  
✅ **配置保留**：你的配置文件不会受到影响  
✅ **性能提升**：新系统内存占用减少约 93%  
✅ **更稳定**：不再有运行时 Harmony 补丁冲突

## 开发者迁移指南

如果你在开发基于 PeakChatOps 的扩展或修改，需要注意以下变更：

### 1. 命名空间引用

**添加新的命名空间**：
```csharp
using PeakChatOps.core;  // 新增
```

### 2. API 调用

**替换所有本地化文本获取调用**：

```csharp
// ❌ 旧代码（不再支持）
string text = LocalizedText.GetText("KEY");

// ✅ 新代码
string text = PLocalizedText.GetText("KEY");
```

### 3. 初始化

**在插件 Awake 中初始化**（如果你创建了自定义插件）：

```csharp
private void Awake()
{
    // 在创建配置之前初始化本地化
    PLocalizedText.Init();
    
    // 其他初始化代码...
    config = new PConfig(Config);
}
```

## 新功能

### 回退值

现在可以为找不到的键提供默认值：

```csharp
// 如果 "CUSTOM_KEY" 不存在，返回 "Default Text"
string text = PLocalizedText.GetText("CUSTOM_KEY", "Default Text");
```

### 键检查

检查某个键是否存在：

```csharp
if (PLocalizedText.HasKey("PEAKCHATOPSWELCOME"))
{
    // 键存在，可以安全使用
    string text = PLocalizedText.GetText("PEAKCHATOPSWELCOME");
}
```

### 热重载（调试用）

在开发时可以重新加载本地化表：

```csharp
// 修改 CSV 后重新加载
PLocalizedText.Reload();
```

## 常见问题

### Q: 我的旧代码会报错吗？

不会，前提是你使用的是 PeakChatOps 提供的 API。内部实现已经全部迁移。

### Q: 我需要修改 CSV 文件吗？

不需要，CSV 文件格式完全兼容。

### Q: 如何添加新的本地化字符串？

在 `Localization.csv` 中添加新行即可：

```csv
MY_CUSTOM_KEY,English Text,Texte français,...
```

然后在代码中使用：

```csharp
string text = PLocalizedText.GetText("MY_CUSTOM_KEY");
```

### Q: 游戏切换语言后文本不更新？

当前版本需要重启游戏才能切换语言。这是设计限制，未来版本可能支持运行时切换。

## 需要帮助？

如果遇到问题，请：

1. 检查日志中的 `[PLocalizedText]` 相关信息
2. 确认 `BepInEx/plugins/PeakChatOps/Localization.csv` 文件存在
3. 在 GitHub 提交 Issue：https://github.com/LIghtJUNction/PeakMods/issues

## 更多信息

- [PLocalizedText API 文档](./PLocalizedText.md)
- [重构详细说明](./LocalizationRefactoring.md)
