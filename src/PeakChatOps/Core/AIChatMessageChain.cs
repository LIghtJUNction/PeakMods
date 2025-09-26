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
    private static UniTask HandleAIChatMessageAsync(AIChatMessageEvent evt)
    {
        if (evt == null)
            return UniTask.CompletedTask;

        DevLog.UI($"[AIChatMessageChain] evt.Message = '{evt.Message}'");
        if (string.IsNullOrWhiteSpace(evt.Message))
            return UniTask.CompletedTask;

        // 判断是否为用户输入（role=user），需要调用AI接口
        if (evt.Role == AIChatRole.user)
        {
            try
            {
                var apiKey = PeakChatOpsPlugin.aiApiKey?.Value;
                DevLog.UI($"[AI] Step 2: apiKey = '{apiKey}'");
                var endpoint = PeakChatOpsPlugin.aiEndpoint?.Value;
                DevLog.UI($"[AI] Step 3: endpoint = '{endpoint}'");

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
                {
                    var msg = "未配置OpenAI/Ollama API Key或Endpoint，请先在设置中填写。";
                    DevLog.UI(msg);
                    var resultEvt = new CmdExecResultEvent("ai", new string[] { evt.Message }, evt.UserId, stdout: msg, stderr: null, success: false);
                    EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt).Forget();
                    return UniTask.CompletedTask;
                }

                string prompt = evt.Message;
                var model = PeakChatOpsPlugin.aiModel?.Value;
                if (string.IsNullOrWhiteSpace(model))
                    model = "gpt-oss:120b-cloud";
                DevLog.UI($"[AI] Step 4: OpenAIClient created, prompt = {prompt}, model = {model}");
                // 构建OpenAI chat/messages格式
                var messages = AIChatContextLogger.Instance.BuildContextMessages(prompt, evt.UserId);
                if (messages == null)
                {
                    DevLog.UI($"[AI] BuildContextMessages 返回null，自动创建空消息列表。Instance是否未初始化？");
                    messages = new List<Dictionary<string, object>>();
                }
                UniTask.Void(async () =>
                {
                    string stdout;
                    string stderr = null;
                    bool success = true;
                    try
                    {
                        // 读取全局AI参数
                        int maxTokens = 1024;
                        double temperature = 0.7;
                        double topP = 1.0;
                        int n = 1;
                        try { maxTokens = PeakChatOpsPlugin.aiMaxTokens?.Value ?? 1024; } catch { }
                        try { temperature = PeakChatOpsPlugin.aiTemperature?.Value ?? 0.7; } catch { }
                        try { topP = PeakChatOpsPlugin.aiTopP?.Value ?? 1.0; } catch { }
                        try { n = PeakChatOpsPlugin.aiN?.Value ?? 1; } catch { }

                        using var client = new OpenAIClient(apiKey, endpoint, maxTokens, temperature, topP, n);
                        var chatApi = new API.AI.Apis.OpenAIChatApi(client);
                        var response = await System.Threading.Tasks.Task.Run(() =>
                        {
                            return chatApi.CreateChatCompletion(model, messages, maxTokens);
                        });
                        DevLog.UI($"[AI] Step 5.0: Raw response = {response}");
                        var parsed = AIChatContextLogger.ParseOpenAICompletionResponse(response!);
                        DevLog.UI($"[AI] Step 5.2: Parsed AI content = {parsed}");
                        stdout = parsed;
                        DevLog.UI("[AI] Step 5: Completion success");
                    }
                    catch (Exception ex)
                    {
                        stdout = $"AI回复失败: {ex.Message}";
                        stderr = ex.ToString();
                        success = false;
                        DevLog.UI($"[AI] Step 6: Completion异常: {ex}");
                    }

                    var resultEvt2 = new CmdExecResultEvent("ai", new string[] { evt.Message }, evt.UserId, stdout: stdout, stderr: stderr, success: success);
                    EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt2).Forget();
                    DevLog.UI("[AI] Step 7: CmdExecResultEvent published");

                    // 直接推送AI助手消息到UI（避免递归自发自收）
                    if (!string.IsNullOrWhiteSpace(stdout))
                    {
                        PeakOpsUI.instance.AddMessage($"<color=#00BFFF>[{PeakChatOpsPlugin.aiModel?.Value ?? "ollama"}]</color>: {stdout}");
                        DevLog.UI($"[AI] Step 8: AI助手消息已推送到UI");
                    }
                    return;
                });
                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
                var errEvt = new CmdExecResultEvent("ai", new string[] { evt.Message }, evt.UserId, stdout: null, stderr: ex.Message, success: false);
                EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt).Forget();
                return UniTask.CompletedTask;
            }
        }

        // 如果是AI助手消息（role=assistant），直接推送到UI
        PeakOpsUI.instance.AddMessage($"<color=#00BFFF>[{evt.Sender}]</color>: {evt.Message}");
        return UniTask.CompletedTask;
    }


    // AI 翻译消息链
    private static UniTask HandleAITranslateMessageAsync(AIChatMessageEvent evt)
    {
        // AI翻译
        // 检查是否是当前语言
        if (!ChatApiUtil.IsMessageInCurrentLanguage(evt.Message, ChatApiUtil.GetCurrentLanguage()))
        {
            string richText = evt.Message;
            using var client = new OpenAIClient(PeakChatOpsPlugin.aiApiKey?.Value, PeakChatOpsPlugin.aiEndpoint?.Value);
            string systemPrompt = string.IsNullOrWhiteSpace(PeakChatOpsPlugin.promptTranslate?.Value) ? $"You are a helpful assistant that translates messages to the user's language{ChatApiUtil.GetCurrentLanguage()}." : PeakChatOpsPlugin.promptTranslate.Value;
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

            PeakOpsUI.instance.AddMessage(richText);
            return UniTask.CompletedTask;
        }
        
        return UniTask.CompletedTask;
    }




}
