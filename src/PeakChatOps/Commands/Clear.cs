using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using PeakChatOps.UI;
using UnityEngine.UIElements;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("clear", "清空聊天记录", "用法: /clear\n清空当前聊天记录。")]
public class ClearCommand
{
    // New message-driven handler signature. Plugins/commands register handlers
    // on EventBusRegistry.CmdMessageBus with channel "cmd://clear".
    // 注册
    public ClearCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://clear", Handle);
        DevLog.UI("[Cmd] ClearCommand subscribed to cmd://clear");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            // 清空聊天记录
            if (PeakChatOpsUI.Instance != null && PeakChatOpsUI.uIDocument != null)
            {
                var root = PeakChatOpsUI.uIDocument.rootVisualElement;
                var messageList = root.Q<ListView>("message-list");
                if (messageList != null)
                {
                    PeakChatOpsUI.messages.Clear();
                    messageList.Rebuild();
                }
            }
            // 发布结果事件，提示清空成功
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: "聊天记录已清空。", stderr: null, success: true);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }
}
