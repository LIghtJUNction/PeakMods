using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using PeakChatOps.API.AI;

namespace PeakChatOps.Core;

public static class AIChatMessageChain
{
    public static void EnsureInitialized()
    {
        // 订阅 AI 聊天消息总线
        EventBusRegistry.AIChatMessageBus.Subscribe("ai://chat", HandleAIChatMessageAsync);
        EventBusRegistry.AIChatMessageBus.Subscribe("ai://translate", HandleAITranslateMessageAsync);
        // 启动 runner
        var cts = CentralCmdRouter.GetOrCreateBusCts();
        UniEventBusRunner.RunChannelLoop(EventBusRegistry.AIChatMessageBus, "ai://chat", cts.Token).Forget();
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

        // 获取AI回复之前处理下拓展指令
        if (evt.Extra != null && evt.Extra.TryGetValue("at", out var atObjPre) && atObjPre is string atValPre)
        {
            switch (atValPre.ToLowerInvariant())
            {
                case "clear":
                    AIChatContextLogger.Instance?.Clear();
                    return;
                case "send":
                    // 这里修改提示词，加上预设的提示词
                    var presetPrompt = PeakChatOpsPlugin.promptSend.Value;
                    evt.Message = $"{evt.Message}({presetPrompt})";
                    break;
                default:
                    break;
            }
        }

        // 获取AI回复（用AsyncUtil后台包装，避免阻塞主线程）
        string aiReply = null;
        string aiReplyError = null;
        var (result, error) = await AsyncUtil.RunInBackground(() => {
            var apiKey = PeakChatOpsPlugin.aiApiKey?.Value;
            var endpoint = PeakChatOpsPlugin.aiEndpoint?.Value;
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                return "未配置OpenAI/Ollama API Key或Endpoint，请先在设置中填写。";
            }
            string prompt = evt.Message;
            var model = PeakChatOpsPlugin.aiModel?.Value;
            if (string.IsNullOrWhiteSpace(model))
                model = "gpt-oss:120b-cloud";
            var messages = AIChatContextLogger.Instance.BuildContextMessages(prompt, evt.UserId) ?? new List<Dictionary<string, object>>();
            int maxTokens = PeakChatOpsPlugin.aiMaxTokens?.Value ?? 1024;
            double temperature = PeakChatOpsPlugin.aiTemperature?.Value ?? 0.7;
            double topP = PeakChatOpsPlugin.aiTopP?.Value ?? 1.0;
            int n = PeakChatOpsPlugin.aiN?.Value ?? 1;
            using var client = new OpenAIClient(apiKey, endpoint, maxTokens, temperature, topP, n);
            var chatApi = new API.AI.Apis.OpenAIChatApi(client);
            var response = chatApi.CreateChatCompletion(model, messages, maxTokens);
            var reply = AIChatContextLogger.ParseOpenAICompletionResponse(response!);
            AIChatContextLogger.Instance?.LogAssistant(reply, PeakChatOpsPlugin.aiModel?.Value ?? "AI");
            return reply;
        });
        if (error != null)
        {
            aiReply = $"AI回复失败: {error.Message}";
            aiReplyError = error.ToString();
        }
        else
        {
            aiReply = result;
        }

        // 在AI回答之后按命令处理
        if (evt.Extra != null && evt.Extra.TryGetValue("at", out var atObjPost) && atObjPost is string atValPost)
        {
            switch (atValPost.ToLowerInvariant())
            {
                case "clear":
                    // 清理上下文后不显示回答
                    AIChatContextLogger.Instance?.Clear();
                    return;
                case "send":
                    // AI的回答直接发送，而不是显示在本地
                    if (!string.IsNullOrWhiteSpace(aiReply))
                    {
                        var chatEvt = new ChatMessageEvent(
                            sender: evt.Sender + " And " + PeakChatOpsPlugin.aiModel?.Value, // 发送者+模型名
                            message: aiReply,
                            userId: evt.UserId,
                            isDead: Character.localCharacter.data.dead,
                            extra: null
                        );
                        // 发布到self渠道
                        EventBusRegistry.ChatMessageBus.Publish("sander://self", chatEvt).Forget();
                    }
                    return;
                default:
                    MainThreadDispatcher.Run(() => PeakOpsUI.instance.AddMessage($"[AI] 未知的extra.at指令: {atValPost}"));
                    break;
            }
        }
        else
        {
            // 默认行为：显示在本地UI
            if (!string.IsNullOrWhiteSpace(aiReply))
            {
                MainThreadDispatcher.Run(() => {
                    PeakOpsUI.instance.AddMessage($"<color=#00BFFF>[{PeakChatOpsPlugin.aiModel?.Value ?? "ollama"}]</color>: {aiReply}");
                });
            }
            return;
        }
        // 防御性兜底，所有路径都返回
    return;
    }

    // AI 翻译消息链
    private static UniTask HandleAITranslateMessageAsync(AIChatMessageEvent evt)
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

            var response = client.Post("completions", jsonBody);

            string translation = AIChatContextLogger.ParseOpenAICompletionResponse(response);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                richText += $"\n<color=#FFA500>[翻译/Translation]</color>: {translation}";
            }

            MainThreadDispatcher.Run(() => PeakOpsUI.instance.AddMessage(richText));
        }
        catch (Exception ex)
        {
            MainThreadDispatcher.Run(() => PeakOpsUI.instance.AddMessage($"<color=#FF0000>[AI翻译异常]</color>: {ex.Message}"));
            DevLog.UI($"[AI翻译异常] {ex}");
        }
        return UniTask.CompletedTask;
    }
}
