using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;

using System.Collections.Generic;
using PeakChatOps.API.AI;
using PeakChatOps.UI;

namespace PeakChatOps.Core.MsgChain;

public static class AIChatMessageChain
{
    public static void EnsureInitialized()
    {
        // 订阅 AI 聊天消息总线
        EventBusRegistry.AIChatMessageBus.Subscribe("ai://chat", HandleAIChatMessageAsync);
        EventBusRegistry.AIChatMessageBus.Subscribe("ai://translate", HandleAITranslateMessageAsync);
        // 启动 runner
        var cts = CentralCmdRouter.GetOrCreateBusCts();
        EventBusRegistry.AIChatMessageBus.RunAsync("ai://chat", cts.Token).Forget();
    }

    // AI 聊天消息处理器

    private static async UniTask HandleAIChatMessageAsync(AIChatMessageEvent evt)
    {
        if (evt == null)
            return;
        if (evt.Role != AIChatRole.user)
            if (evt.Role == AIChatRole.assistant)
            {
                return;
            }
            else
            {
                return;
            }
        if (string.IsNullOrWhiteSpace(evt.Message))
            return;

        // per-invocation timer key to measure elapsed time for AI processing
        var __timerKey = $"AIChat::{(evt.UserId ?? evt.Sender ?? Guid.NewGuid().ToString())}::{Guid.NewGuid():N}";

        string __devlog_model = null;
        try
        {

        // 获取AI回复之前处理下拓展指令（优先使用类型化 AIExtra）
        if (evt.Extra != null)
        {
            bool handledPre = false;
            if (evt.Extra.TryGetValue("AI", out var aiObj) && aiObj is AIExtra aiExtra)
            {
                var aiAtPre = aiExtra.AtCommand?.ToLowerInvariant();
                switch (aiAtPre)
                {
                    case "clear":
                        AIChatContextLogger.Instance?.Clear();
                        return;
                    case "send":
                        var presetPrompt = PeakChatOpsPlugin.promptSend.Value;
                        if (!string.IsNullOrWhiteSpace(aiExtra.PromptAppend))
                            evt.Message = $"{evt.Message}({aiExtra.PromptAppend})";
                        else
                            evt.Message = $"{evt.Message}({presetPrompt})";
                        handledPre = true;
                        break;
                    default:
                        break;
                }
            }
            if (!handledPre && evt.Extra.TryGetValue("at", out var atObjPre) && atObjPre is string atValPre)
            {
                switch (atValPre.ToLowerInvariant())
                {
                    case "clear":
                        AIChatContextLogger.Instance?.Clear();
                        return;
                    case "send":
                        var presetPrompt = PeakChatOpsPlugin.promptSend.Value;
                        evt.Message = $"{evt.Message}({presetPrompt})";
                        break;
                    default:
                        break;
                }
            }
        }

        // 获取AI回复：在后台线程执行阻塞调用并 await，直接捕获异常
        string aiReply = null;
        string aiReplyError = null;
        try
        {
            var apiKey = PeakChatOpsPlugin.aiApiKey?.Value;
            var endpoint = PeakChatOpsPlugin.aiEndpoint?.Value;
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                aiReply = "未配置OpenAI/Ollama API Key或Endpoint，请先在设置中填写。";
            }
            else
            {
                string prompt = evt.Message;
                // capture configured model for reporting
                __devlog_model = PeakChatOpsPlugin.aiModel?.Value;
                string model = __devlog_model;
                if (string.IsNullOrWhiteSpace(model))
                    model = "gpt-oss:120b-cloud";

                var messages = AIChatContextLogger.Instance.BuildContextMessages(prompt, evt.UserId) ?? new List<Dictionary<string, object>>();

                int maxTokens = PeakChatOpsPlugin.aiMaxTokens?.Value ?? 1024;
                double temperature = PeakChatOpsPlugin.aiTemperature?.Value ?? 0.7;
                double topP = PeakChatOpsPlugin.aiTopP?.Value ?? 1.0;
                int n = PeakChatOpsPlugin.aiN?.Value ?? 1;
                using var client = new OpenAIClient(apiKey, endpoint, maxTokens, temperature, topP, n);
                var chatApi = new API.AI.Apis.OpenAIChatApi(client);

                var response = await chatApi.CreateChatCompletionAsync(model, messages, maxTokens);

                var (content, reasoning) = AIChatContextLogger.ParseOpenAICompletionResponseWithReasoning(response!);

                // displayText：用于本地显示（包含 reasoning）；sendText：仅用于发送（仅 content）
                string displayText;
                if (content != null && reasoning != null)
                {
                    // 仅在配置允许时显示推理；推理以更小字体和淡灰色显示（TextMeshPro 富文本）
                    if (PeakChatOpsPlugin.aiShowResponse?.Value ?? true)
                    {
                        var reasoningFormatted = $"<size=80%><color=#C0C0C0>{reasoning}</color></size>";
                        displayText = content + "\n<color=#888>[R]</color>: " + reasoningFormatted;
                    }
                    else
                    {
                        // 跳过推理显示，仅显示 content
                        displayText = content;
                    }
                }
                else if (content != null)
                {
                    displayText = content;
                }
                else if (reasoning != null)
                {
                    // 仅有推理时本地显示推理（小号淡灰）
                    displayText = $"<size=70%><color=#C0C0C0>{reasoning}</color></size>";
                }
                else
                {
                    displayText = "(AI无回复, No response)";
                }

                // sendText 仅为 content（推理永不发送）
                var sendText = content;

                // 如果使用了 reasoning（content 为 null），记录以便追踪
                if (content == null && reasoning != null)
                {
                    DevLog.File($"[AIContext] Reply taken from reasoning field (not sent). Preview: {reasoning?.Substring(0, Math.Min(200, reasoning.Length))}");

                }

                // 记录到上下文：优先记录 content（若无则记录 displayText）

                AIChatContextLogger.Instance?.LogAssistant(content ?? displayText, PeakChatOpsPlugin.aiModel?.Value ?? "AI");

                // aiReply 用于默认本地显示（保留原样）
                aiReply = displayText;
                
            }
        }
        catch (Exception ex)
        {
            aiReply = $"AI回复失败: {ex.Message}";
            aiReplyError = ex.ToString();

        }

    // 在AI回答之后按命令处理（优先使用类型化 AIExtra）
        if (evt.Extra != null)
        {
            bool handledPost = false;
            if (evt.Extra.TryGetValue("AI", out var aiObjPost) && aiObjPost is AIExtra aiExtraPost)
            {
                var aiAtPost = aiExtraPost.AtCommand?.ToLowerInvariant();
                switch (aiAtPost)
                {
                    case "clear":
                        AIChatContextLogger.Instance?.Clear();
                        return;
                    case "send":
                        if (!string.IsNullOrWhiteSpace(aiReply))
                        {
                            var chatEvt = new ChatMessageEvent(
                                sender: evt.Sender + " And " + PeakChatOpsPlugin.aiModel?.Value,
                                message: aiReply,
                                userId: evt.UserId,
                                isDead: Character.localCharacter.data.dead,
                                extra: evt.Extra
                            );
                            EventBusRegistry.ChatMessageBus.Publish("sander://self", chatEvt).Forget();
                        }
                        return;
                    default:
                        PeakChatOpsUI.Instance.AddMessage($"[AI] 未知的AI扩展指令: {aiAtPost}");
                        handledPost = true;
                        break;
                }
            }
            if (!handledPost && evt.Extra.TryGetValue("at", out var atObjPost) && atObjPost is string atValPost)
            {
                switch (atValPost.ToLowerInvariant())
                {
                    case "clear":
                        AIChatContextLogger.Instance?.Clear();
                        return;
                    case "send":
                        if (!string.IsNullOrWhiteSpace(aiReply))
                        {
                            var chatEvt = new ChatMessageEvent(
                                sender: evt.Sender + " And " + PeakChatOpsPlugin.aiModel?.Value,
                                message: aiReply,
                                userId: evt.UserId,
                                isDead: Character.localCharacter.data.dead,
                                extra: evt.Extra
                            );
                            EventBusRegistry.ChatMessageBus.Publish("sander://self", chatEvt).Forget();
                        }
                        return;
                    default:
                        PeakChatOpsUI.Instance.AddMessage($"[AI] 未知的extra.at指令: {atValPost}");
                        break;
                }
            }
        }
        else
        {
            // 默认行为：显示在本地UI
            if (!string.IsNullOrWhiteSpace(aiReply))
            {
                    PeakChatOpsUI.Instance.AddMessage($"<color=#00BFFF>[{PeakChatOpsPlugin.aiModel?.Value ?? "ollama"}]</color>: {aiReply}");
            }
            return;
        }
        // 防御性兜底，所有路径都返回
    return;
        }
        finally
        {
            // 
        }
    }

    // AI 翻译消息链
    private static async UniTask HandleAITranslateMessageAsync(AIChatMessageEvent evt)
    {
        try
        {
            // AI自动翻译
            string richText = evt.Message;
            using var client = new OpenAIClient(PeakChatOpsPlugin.aiApiKey?.Value, PeakChatOpsPlugin.aiEndpoint?.Value);
            string systemPrompt = string.IsNullOrWhiteSpace(PeakChatOpsPlugin.promptTranslate?.Value) ? $"You are a helpful assistant that translates messages to the player：{evt.Sender}'s language." : PeakChatOpsPlugin.promptTranslate.Value;
            var messages = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        { "role", "system" },
                        { "content", systemPrompt }
                    },
                    new Dictionary<string, object>()
                    {
                        { "role", "user" },
                        { "content", evt.Message }
                    }
                };

            var jsonBodyObj = new
            {
                model = string.IsNullOrWhiteSpace(PeakChatOpsPlugin.aiModel?.Value) ? "gpt-oss:120b-cloud" : PeakChatOpsPlugin.aiModel.Value,
                messages = messages,
                max_tokens = 256
            };
            var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(jsonBodyObj);

            var response = await client.PostAsync("completions", jsonBody);

            var (tContent, tReasoning) = AIChatContextLogger.ParseOpenAICompletionResponseWithReasoning(response);
            string translation = tContent ?? tReasoning ?? "(AI无回复, No response)";

            if (!string.IsNullOrWhiteSpace(translation))
            {
                richText += $"\n<color=#FFA500>[翻译/Translation]</color>: {translation}";
            }

            PeakChatOpsUI.Instance.AddMessage(richText);
        }
        catch (Exception ex)
        {
            PeakChatOpsUI.Instance.AddMessage($"<color=#FF0000>[AI翻译异常]</color>: {ex.Message}");
            DevLog.UI($"[AI翻译异常] {ex}");
        }
    return;
    }
}
