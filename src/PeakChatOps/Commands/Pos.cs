using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using PeakChatOps.UI;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("pos", "设置面板位置", "用法: /pos <位置:L-左侧，R-右侧，C-居中>\n将面板位置设置为指定位置。")]
public class PosCommand
{
    // New message-driven handler signature. Plugins/commands register handlers
    // on EventBusRegistry.CmdMessageBus with channel "cmd://pos".
    // 注册
    public PosCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://pos", Handle);
        DevLog.UI("[Cmd] PosCommand subscribed to cmd://pos");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            var args = evt.Args ?? Array.Empty<string>();
            
            // 提取位置参数
            var pos = args.Length > 0 ? args[0].ToUpper() : "";
            // 执行位置设置

            if (pos == "L")
            {
                PeakChatOpsUI.Instance.OnTopLeft();
            }
            else if (pos == "R")
            {
                PeakChatOpsUI.Instance.OnTopRight();
            }
            else if (pos == "C")
            {
                PeakChatOpsUI.Instance.OnCenter();
            }
            else
            {
                var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: "无效的位置参数。请使用 L（左侧），R（右侧），C（居中）。", success: false);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
                return;
            }
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }
}
