// ReSharper disable InconsistentNaming
using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;

using System.Collections.Generic;
using PeakChatOps.API.AI;
using PeakChatOps.UI;
using PeakChatOps.Commands;
using PeakChatOps.core;

namespace PeakChatOps.Core.MsgChain;

public static class AIChatMessageChain
{
    public static void EnsureInitialized()
    {
        EventBusRegistry.AIChatMessageBus.Subscribe("ai://chat", HandleAiChatMessageAsync);
        EventBusRegistry.AIChatMessageBus.Subscribe("ai://translate", HandleAiTranslateMessageAsync);

        var cts = CentralCmdRouter.GetOrCreateBusCts();
        EventBusRegistry.AIChatMessageBus.RunAsync("ai://chat", cts.Token).Forget();
        EventBusRegistry.AIChatMessageBus.RunAsync("ai://translate", cts.Token).Forget();
    }

    private static async UniTask HandleAiChatMessageAsync(AIChatMessageEvent evt)
    {
        if (evt == null)
            return;

        if (evt.Role != AIChatRole.user)
            return;

        if (string.IsNullOrWhiteSpace(evt.Message))
            return;

        if (evt.Extra != null && evt.Extra.TryGetValue("AI", out var aiObj) && aiObj is AIExtra aiExtra)
        {
            var atCommand = aiExtra.AtCommand?.ToLowerInvariant();
            switch (atCommand)
            {
                case "clear":
                    AIChatContextLogger.Instance?.Clear();
                    PeakChatOpsUI.Instance?.AddMessage(MessageStyles.AILabel(), 
                        MessageStyles.SystemContent("上下文已清空 (Context cleared)"));
                    return;
                case "send":
                    var presetPrompt = PeakChatOpsPlugin.config.PromptSend.Value;
                    evt.Message = !string.IsNullOrWhiteSpace(aiExtra.PromptAppend)
                        ? $"{evt.Message}({aiExtra.PromptAppend})"
                        : $"{evt.Message}({presetPrompt})";
                    break;
                default:
                    var cmdMeta = Cmdx.CommandMetas.Find(c =>
                        c.Name.Equals(atCommand, StringComparison.OrdinalIgnoreCase));
                    if (cmdMeta != null)
                        evt.Message = $"{evt.Message} (Command Info: {cmdMeta.Name} - {cmdMeta.Description})";
                    break;
            }
        }

        // 在修改后记录完整的消息到上下文
        AIChatContextLogger.Instance?.LogUser(evt.Message, evt.Sender, evt.UserId);

        string aiReply;
        bool hasError = false;
        try
        {
            var apiKeyHash = PeakChatOpsPlugin.config.AiApiKey?.Value;
            var apiKey = PConfig.GetActualApiKey(apiKeyHash); // 从 hash 获取实际 key
            var endpoint = PeakChatOpsPlugin.config.AiEndpoint?.Value;
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                aiReply = "未配置 API Key 或 Endpoint，请先在设置中填写。";
                hasError = true;
            }
            else
            {
                var prompt = evt.Message;
                var model = PeakChatOpsPlugin.config.AiModel?.Value;
                if (string.IsNullOrWhiteSpace(model))
                    model = "gpt-oss:120b-cloud";

                var logger = AIChatContextLogger.Instance;
                List<Dictionary<string, object>> messages;
                if (logger != null)
                {
                    messages = logger.BuildContextMessages(prompt, evt.UserId);
                    DevLog.File($"[AI] 构建上下文消息数: {messages.Count}");
                }
                else
                {
                    messages = new List<Dictionary<string, object>>();
                    DevLog.File("[AI] ⚠️ AIChatContextLogger.Instance 为 null，使用空消息列表");
                }

                var maxTokens = PeakChatOpsPlugin.config.AiMaxTokens?.Value ?? 1024;
                var temperature = PeakChatOpsPlugin.config.AiTemperature?.Value ?? 0.7;
                var topP = PeakChatOpsPlugin.config.AiTopP?.Value ?? 1.0;
                var n = PeakChatOpsPlugin.config.AiN?.Value ?? 1;

                DevLog.File($"[AI] 发送请求到 {endpoint} | Model: {model} | MaxTokens: {maxTokens}");

                using var client = new OpenAIClient(apiKey, endpoint, maxTokens, temperature, topP, n);
                var chatApi = new API.AI.Apis.OpenAIChatApi(client);

                var response = await chatApi.CreateChatCompletionAsync(model, messages, maxTokens);
                
                if (response == null)
                {
                    DevLog.File("[AI] ❌ API 返回 null 响应");
                    aiReply = "AI API 返回空响应，请检查网络连接或 API 配置";
                    hasError = true;
                }
                else
                {
                    var (content, reasoning) = AIChatContextLogger.ParseOpenAICompletionResponseWithReasoning(response);

                    string displayText;
                    if (content != null && reasoning != null)
                    {
                        if (PeakChatOpsPlugin.config.AiShowResponse?.Value ?? true)
                        {
                            var reasoningFormatted = MessageStyles.ReasoningText(reasoning);
                            var reasoningLabel = MessageStyles.SecondaryText("[R]");
                            displayText = content + "\n" + reasoningLabel + ": " + reasoningFormatted;
                        }
                        else
                        {
                            displayText = content;
                        }
                    }
                    else if (content != null)
                    {
                        displayText = content;
                    }
                    else if (reasoning != null)
                    {
                        displayText = MessageStyles.ReasoningText(reasoning);
                    }
                    else
                    {
                        DevLog.File("[AI] ⚠️ content 和 reasoning 都为 null");
                        displayText = PLocalizedText.GetText("AI_NO_RESPONSE");
                        hasError = true;
                    }

                    if (content == null && reasoning != null)
                    {
                        var preview = reasoning.Substring(0, Math.Min(200, reasoning.Length));
                        DevLog.File($"[AIContext] Reply taken from reasoning field (not sent). Preview: {preview}");
                    }

                    if (!hasError)
                    {
                        AIChatContextLogger.Instance?.LogAssistant(content ?? displayText,
                            PeakChatOpsPlugin.config.AiModel?.Value ?? "AI");
                    }

                    aiReply = displayText;
                }
            }
        }
        catch (Exception ex)
        {
            aiReply = $"AI 请求异常: {ex.Message}";
            hasError = true;
            DevLog.File($"[AI] Exception: {ex}");
        }

        // 检查是否是 send 命令（需要发送到聊天而不是显示在 UI）
        var isSendCommand = false;
        if (evt.Extra != null && evt.Extra.TryGetValue("AI", out var aiObjPost) && aiObjPost is AIExtra aiExtraPost)
        {
            var atCommandPost = aiExtraPost.AtCommand?.ToLowerInvariant();
            if (atCommandPost == "send")
            {
                isSendCommand = true;
            }
        }

        // 显示 AI 回复
        if (!string.IsNullOrWhiteSpace(aiReply))
        {
            if (isSendCommand && !hasError)
            {
                // send 命令：发送到游戏聊天
                var chatEvt = new ChatMessageEvent(
                    sender: evt.Sender + " And " + (PeakChatOpsPlugin.config.AiModel?.Value ?? "AI"),
                    message: aiReply,
                    userId: evt.UserId,
                    isDead: Character.localCharacter?.data?.dead ?? false,
                    extra: evt.Extra
                );
                EventBusRegistry.ChatMessageBus.Publish("sander://self", chatEvt).Forget();
                
                // 同时在 UI 显示确认信息
                var preview = aiReply.Substring(0, Math.Min(50, aiReply.Length));
                var confirmMsg = MessageStyles.SuccessContent($"已发送到游戏聊天: {preview}...");
                PeakChatOpsUI.Instance?.AddMessage(MessageStyles.SuccessLabel("AI→Chat"), confirmMsg);
            }
            else
            {
                // 普通模式：显示在 UI
                var modelLabel = PeakChatOpsPlugin.config.AiModel?.Value ?? "AI";
                var prefix = hasError ? MessageStyles.ErrorLabel("AI Error") : MessageStyles.AILabel(modelLabel);
                var coloredReply = hasError ? MessageStyles.ErrorContent(aiReply) : MessageStyles.AIContent(aiReply);
                PeakChatOpsUI.Instance?.AddMessage(prefix, coloredReply);
            }
        }
        else
        {
            // 完全没有回复内容
            DevLog.File("[AI] ⚠️ aiReply 为空或 null");
            PeakChatOpsUI.Instance?.AddMessage(MessageStyles.WarningLabel("AI"), 
                MessageStyles.WarningContent("没有收到任何响应"));
        }
    }

    private static async UniTask HandleAiTranslateMessageAsync(AIChatMessageEvent evt)
    {
        try
        {
            DevLog.File($"[Translate] 开始翻译: {evt.Message}");
            
            var apiKeyHash = PeakChatOpsPlugin.config.AiApiKey?.Value;
            var apiKey = PConfig.GetActualApiKey(apiKeyHash);
            var endpoint = PeakChatOpsPlugin.config.AiEndpoint?.Value;
            
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                PeakChatOpsUI.Instance.AddMessage(MessageStyles.ErrorLabel("翻译错误"), 
                    MessageStyles.ErrorContent("未配置 API Key 或 Endpoint"));
                return;
            }

            using var client = new OpenAIClient(apiKey, endpoint);
            var chatApi = new API.AI.Apis.OpenAIChatApi(client);
            
            var systemPrompt = string.IsNullOrWhiteSpace(PeakChatOpsPlugin.config.PromptTranslate?.Value)
                ? PLocalizedText.GetText("TRANSLATE_PROMPT")
                : PeakChatOpsPlugin.config.PromptTranslate.Value;

            var messages = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "role", "system" },
                    { "content", systemPrompt }
                },
                new Dictionary<string, object>
                {
                    { "role", "user" },
                    { "content", evt.Message }
                }
            };

            var model = string.IsNullOrWhiteSpace(PeakChatOpsPlugin.config.AiModel?.Value)
                ? "gpt-oss:120b-cloud"
                : PeakChatOpsPlugin.config.AiModel.Value;

            DevLog.File($"[Translate] 发送请求到 {endpoint} | Model: {model}");
            
            var response = await chatApi.CreateChatCompletionAsync(model, messages, 256);
            
            if (response == null)
            {
                DevLog.File("[Translate] ❌ API 返回 null 响应");
                PeakChatOpsUI.Instance.AddMessage(MessageStyles.ErrorLabel("翻译错误"), 
                    MessageStyles.ErrorContent("API 返回空响应"));
                return;
            }

            var (tContent, tReasoning) = AIChatContextLogger.ParseOpenAICompletionResponseWithReasoning(response);
            
            var contentPreview = tContent != null ? tContent.Substring(0, Math.Min(50, tContent.Length)) : "null";
            var reasoningPreview = tReasoning != null ? tReasoning.Substring(0, Math.Min(50, tReasoning.Length)) : "null";
            DevLog.File($"[Translate] 解析结果 - Content: {contentPreview} | Reasoning: {reasoningPreview}");
            
            var translation = tContent ?? tReasoning ?? PLocalizedText.GetText("AI_NO_RESPONSE");

            PeakChatOpsUI.Instance.AddMessage(MessageStyles.TranslateLabel(), 
                MessageStyles.TranslateContent(translation));
        }
        catch (Exception ex)
        {
            PeakChatOpsUI.Instance.AddMessage(MessageStyles.ErrorLabel("翻译异常"), 
                MessageStyles.ErrorContent(ex.Message));
            DevLog.File($"[Translate] Exception: {ex}");
        }
    }
}
