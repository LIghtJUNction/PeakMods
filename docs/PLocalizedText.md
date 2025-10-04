# PLocalizedText 自定义本地化系统

## 概述

`PLocalizedText` 是 PeakChatOps 的自定义本地化文本管理类，用于替代原有的游戏本地化系统补丁方案。该类提供了更简洁、高效的本地化文本加载和管理功能。

## 设计理念

### 为什么创建自定义类？

1. **避免 Harmony 补丁冲突**：不再需要对游戏的 `LocalizedText` 类进行 Harmony 补丁，避免潜在的方法重载冲突和运行时错误
2. **内存优化**：只加载当前语言的文本，而不是加载所有语言的数据
3. **独立性**：Mod 的本地化系统完全独立于游戏本地化系统，互不干扰
4. **易于维护**：代码结构清晰，便于调试和扩展

## 功能特性

### 核心功能

- ✅ 自动检测游戏当前语言
- ✅ 从 `Localization.csv` 加载本地化文本
- ✅ 仅加载当前语言的文本（节省内存）
- ✅ 支持 CSV 字段引号和逗号转义
- ✅ 提供回退机制（找不到键时返回键名或自定义默认值）
- ✅ 支持热重载（调试时可重新加载本地化表）

### API 方法

#### `Init()`
初始化本地化系统，加载 CSV 文件。

```csharp
PLocalizedText.Init();
```

**注意**：该方法应在插件 `Awake()` 中调用，且在创建配置对象之前调用。

#### `GetText(string key, string? fallback = null)`
获取本地化文本。

```csharp
// 基本用法
string welcomeMsg = PLocalizedText.GetText("PEAKCHATOPSWELCOME");

// 带回退值
string customMsg = PLocalizedText.GetText("CUSTOM_KEY", "Default text");
```

**参数**：
- `key`：本地化键
- `fallback`（可选）：找不到键时返回的默认值，默认为键本身

**返回值**：本地化后的文本

#### `HasKey(string key)`
检查某个键是否存在。

```csharp
if (PLocalizedText.HasKey("PEAKCHATOPSWELCOME"))
{
    // 键存在
}
```

#### `GetAllKeys()`
获取所有已加载的键。

```csharp
var keys = PLocalizedText.GetAllKeys();
foreach (var key in keys)
{
    Debug.Log(key);
}
```

#### `Reload()`
重新加载本地化表（用于调试）。

```csharp
PLocalizedText.Reload();
```

## CSV 文件格式

### 文件位置
```
BepInEx/plugins/PeakChatOps/Localization.csv
```

### 文件结构

第一行为语言列表：
```csv
CURRENT_LANGUAGE,English,Français,Italiano,Deutsch,Español (España),Español (LatAm),Português (BR),Русский,Українська,简体中文,繁体中文,日本語,한국어,Polski,Türkçe,ENDLINE
```

后续行为键值对：
```csv
PEAKCHATOPSWELCOME,Welcome to PeakChatOps,Bienvenue sur PeakChatOps,...
```

### 字段说明

- **第一列**：本地化键（必须唯一）
- **后续列**：各语言的翻译文本
- **最后一列**：`ENDLINE`（可选，作为行结束标记）

### 转义规则

- 包含逗号的文本需要用引号包裹：`"Hello, World"`
- 引号本身需要双写：`"He said ""Hello"""`

## 使用示例

### 在配置中使用

```csharp
public PConfig(ConfigFile config)
{
    DeathMessage = config.Bind(
        "preset", "DeathMessage",
        PLocalizedText.GetText("DEATH_MESSAGE"),
        PLocalizedText.GetText("DEATH_MESSAGE_DESCRIPTION")
    );
}
```

### 在运行时使用

```csharp
public void ShowWelcomeMessage()
{
    string msg = PLocalizedText.GetText("PEAKCHATOPSWELCOME");
    ChatSystem.Instance.SendLocalMessage(msg);
}
```

## 语言检测逻辑

1. 首先尝试从游戏的 `LocalizedText.GetText("CURRENT_LANGUAGE")` 获取当前语言
2. 在 CSV 的第一行中查找匹配的语言列
3. 如果找不到精确匹配，尝试模糊匹配（如简体中文）
4. 如果仍然找不到，默认使用英语列（索引 1）

## 支持的语言

根据 CSV 表头，当前支持以下语言：

| 索引 | 语言                    | 代码   |
|------|-------------------------|--------|
| 0    | CURRENT_LANGUAGE        | -      |
| 1    | English                 | en     |
| 2    | Français                | fr     |
| 3    | Italiano                | it     |
| 4    | Deutsch                 | de     |
| 5    | Español (España)        | es-ES  |
| 6    | Español (LatAm)         | es-LA  |
| 7    | Português (BR)          | pt-BR  |
| 8    | Русский                 | ru     |
| 9    | Українська              | uk     |
| 10   | 简体中文                | zh-CN  |
| 11   | 繁体中文                | zh-TW  |
| 12   | 日本語                  | ja     |
| 13   | 한국어                  | ko     |
| 14   | Polski                  | pl     |
| 15   | Türkçe                  | tr     |

## 迁移指南

### 从 LocalizedText 迁移到 PLocalizedText

1. **添加命名空间**：
```csharp
using PeakChatOps.core;
```

2. **替换调用**：
```csharp
// 旧代码
LocalizedText.GetText("KEY")

// 新代码
PLocalizedText.GetText("KEY")
```

3. **在 Awake 中初始化**：
```csharp
private void Awake()
{
    // 在创建配置之前初始化
    PLocalizedText.Init();
    
    config = new PConfig(Config);
}
```

### 批量替换

使用 PowerShell 批量替换：
```powershell
(Get-Content "path/to/file.cs") -replace 'LocalizedText\.GetText', 'PLocalizedText.GetText' | Set-Content "path/to/file.cs"
```

## 调试技巧

### 查看加载状态

初始化时会输出日志：
```
[Info   : PeakChatOps] [PLocalizedText] Loaded 24 localization entries for language index 10
```

### 检查缺失的键

当请求的键不存在时，会输出警告：
```
[Warning: PeakChatOps] [PLocalizedText] Key 'MISSING_KEY' not found in localization table
```

### 热重载

在游戏运行时修改 CSV 后，可以调用：
```csharp
PLocalizedText.Reload();
```

## 性能优化

### 内存占用

- **旧方案**（Harmony 补丁）：加载所有语言，约 15 列 × 行数 × 平均字符数
- **新方案**（PLocalizedText）：仅加载当前语言，约 1 列 × 行数 × 平均字符数

**节省内存**：约 93% (15 列 → 1 列)

### 查找性能

使用 `Dictionary<string, string>` 存储，查找时间复杂度为 O(1)。

## 常见问题

### Q: CSV 文件找不到怎么办？

**A**: 确保文件路径正确：
```
BepInEx/plugins/PeakChatOps/Localization.csv
```
检查日志中的错误信息。

### Q: 如何添加新的本地化键？

**A**: 在 CSV 文件中添加新行，格式如下：
```csv
NEW_KEY,English Text,French Text,...
```
然后调用 `PLocalizedText.Reload()` 或重启游戏。

### Q: 游戏语言切换后，文本没有更新？

**A**: 当前版本在初始化时读取语言，切换后需要重启游戏。未来版本可以考虑监听语言变化事件。

## 未来改进方向

- [ ] 支持游戏运行时语言切换
- [ ] 支持多个 CSV 文件（分模块加载）
- [ ] 支持参数化文本（如 `"You have {0} items"`）
- [ ] 支持富文本标记
- [ ] 提供编辑器工具验证 CSV 完整性

## 许可证

该代码属于 PeakChatOps 项目，遵循项目许可证。
