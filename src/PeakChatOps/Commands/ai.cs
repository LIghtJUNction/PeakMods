
using System;
using PeakChatOps.API;
using PeakChatOps.API.AI;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using System.Linq;
using System.Collections.Generic;

#nullable enable
namespace PeakChatOps.Commands;

/// <summary>
/// /ai 命令：将用户输入路由到 AI 聊天消息链，不直接调用AI接口。
/// 支持参数：/ai <内容>
/// </summary>
[PCOCommand("ai", "AI助手", "开发中")]
public class AICommand
{
    /// <summary>
    /// 注册命令到消息总线
    /// </summary>
    public AICommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://ai", Handle);
        DevLog.UI("[Cmd] AICommand subscribed to cmd://ai");
    }

    /// <summary>
    /// 处理/ai命令，将用户输入封装为AIChatMessageEvent并路由到AI消息链
    /// </summary>
    /// <param name="evt">命令事件</param>
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            DevLog.UI("[AI] Step 1: Handler entered");
            if (evt.Args == null || evt.Args.Length == 0 || evt.Args.All(string.IsNullOrWhiteSpace))
            {
                var msg = "请输入要发送给AI的内容，如 /ai Hello AI!";
                var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: msg, stderr: null, success: false);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
                return;
            }

            string prompt = string.Join(" ", evt.Args).Trim();
            DevLog.UI($"[AI] Step 2: prompt = {prompt}");

            // 只负责路由，不做AI接口调用
            var aiMsg = new AIChatMessageEvent(
                sender: PeakChatOpsPlugin.aiModel?.Value ?? "ollama",
                message: prompt,
                userId: evt.UserId,
                role: AIChatRole.user
            );
            DevLog.UI($"[AI] Step 3: AIChatMessageEvent = sender={aiMsg.Sender}, userId={aiMsg.UserId}, message={aiMsg.Message}");
            await EventBusRegistry.AIChatMessageBus.Publish("ai://chat", aiMsg);
            DevLog.UI("[AI] Step 4: AIChatMessageEvent published to ai://chat");
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
    }
}

