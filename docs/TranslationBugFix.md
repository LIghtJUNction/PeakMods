# AI 翻译功能修复报告

## 问题描述

用户报告在使用 AI 翻译功能时总是显示"AI无回复, No response"，而 `/ai` 聊天命令工作正常。

## 根本原因

翻译功能和聊天功能调用的 API endpoint **不一致**：

### 问题代码（翻译功能）
```csharp
// ❌ 错误：直接调用 "completions" endpoint
var response = await client.PostAsync("completions", jsonBody);
```

### 正确代码（聊天功能）
```csharp
// ✅ 正确：使用 OpenAIChatApi 调用 "chat/completions"
using var client = new OpenAIClient(apiKey, endpoint);
var chatApi = new API.AI.Apis.OpenAIChatApi(client);
var response = await chatApi.CreateChatCompletionAsync(model, messages, maxTokens);
```

## API Endpoint 差异

| 功能 | 旧的翻译代码 | 聊天代码（正确） |
|------|-------------|-----------------|
| Endpoint | `completions` | `chat/completions` |
| 实际路径 | `/v1/completions` | `/v1/chat/completions` |
| API 类型 | Legacy Completions API | Chat Completions API |

OpenAI 的 **Chat Completions API** (`chat/completions`) 是现代推荐的接口，支持多轮对话和 system/user/assistant 角色。

## 修复方案

### 1. 统一使用 `OpenAIChatApi`

将翻译功能改为使用相同的 API 封装类：

```csharp
// 创建 client 和 chatApi
using var client = new OpenAIClient(apiKey, endpoint);
var chatApi = new API.AI.Apis.OpenAIChatApi(client);

// 调用 CreateChatCompletionAsync
var response = await chatApi.CreateChatCompletionAsync(model, messages, 256);
```

### 2. 添加调试日志

增加详细的日志输出，便于诊断问题：

```csharp
DevLog.File($"[Translate] 开始翻译: {evt.Message}");
DevLog.File($"[Translate] 发送请求到 {endpoint} | Model: {model}");
DevLog.File($"[Translate] 解析结果 - Content: {contentPreview} | Reasoning: {reasoningPreview}");
```

### 3. 改进错误处理

- 检查 API Key 和 Endpoint 配置
- 检查 response 是否为 null
- 提供更明确的错误提示

### 4. 本地化硬编码文本

添加 `AI_NO_RESPONSE` 到 CSV：

```csv
AI_NO_RESPONSE,No response from AI,Pas de réponse de l'IA,...,AI 无回复,...
```

替换硬编码：
```csharp
// ❌ 旧代码
var translation = tContent ?? tReasoning ?? "(AI无回复, No response)";

// ✅ 新代码
var translation = tContent ?? tReasoning ?? PLocalizedText.GetText("AI_NO_RESPONSE");
```

## 修改文件

1. **AIChatMessageChain.cs**
   - 重写 `HandleAiTranslateMessageAsync` 方法
   - 使用 `OpenAIChatApi` 替代直接调用 `client.PostAsync`
   - 添加调试日志
   - 改进错误处理

2. **Localization.csv**
   - 添加 `AI_NO_RESPONSE` 键及其15种语言翻译

## 测试验证

### 编译结果
```
还原完成(0.6)
PeakChatOps 已成功 (0.6 秒) → artifacts\bin\PeakChatOps\debug\com.github.LIghtJUNction.PeakChatOps.dll
在 1.4 秒内生成 已成功
```

### 预期行为

现在翻译功能应该能够：
1. ✅ 正确调用 `chat/completions` API
2. ✅ 解析 AI 返回的翻译内容
3. ✅ 显示本地化的错误提示
4. ✅ 输出详细的调试日志

### 调试日志示例

成功翻译：
```
[Translate] 开始翻译: Hello, how are you?
[Translate] 发送请求到 http://localhost:11434/v1 | Model: gpt-oss:120b-cloud
[Translate] 解析结果 - Content: 你好，你好吗？ | Reasoning: null
```

失败情况：
```
[Translate] 开始翻译: Test message
[Translate] 发送请求到 http://localhost:11434/v1 | Model: gpt-oss:120b-cloud
[Translate] ❌ API 返回 null 响应
```

## 相关代码

### OpenAIEndpoint 枚举

```csharp
public enum OpenAIEndpoint
{
    ChatCompletions,  // → "chat/completions" ✅
    Completions,      // → "completions" (legacy)
    // ...
}
```

### CreateChatCompletionAsync 实现

```csharp
public async UniTask<string> CreateChatCompletionAsync(
    string model, 
    List<Dictionary<string, object>> messages, 
    int maxTokens = 128)
{
    var body = new
    {
        model = model,
        messages = messages,
        max_tokens = maxTokens
    };
    var json = JsonConvert.SerializeObject(body);
    return await _client.PostAsync(Endpoint.ToPath(), json);
}
```

其中 `Endpoint.ToPath()` 返回 `"chat/completions"`。

## 总结

问题的核心是翻译功能使用了**错误的 API endpoint**。通过统一使用 `OpenAIChatApi` 和 `chat/completions` endpoint，翻译功能现在应该能够正常工作了。

添加的调试日志将帮助快速诊断任何后续问题。
