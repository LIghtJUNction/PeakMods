using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("hide", "立即隐藏聊天框", "用法: /hide\n立即将聊天框隐藏。")]
public class HideCommand
{
    public HideCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://hide", Handle);
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            if (PeakOpsUI.instance != null)
            {
                MainThreadDispatcher.Run(() => PeakOpsUI.instance.HideNow());
                var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: "聊天框已隐藏。", stderr: null, success: true);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
            }
            else
            {
                var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: "聊天界面未初始化。", stderr: null, success: true);
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
            }
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: $"隐藏聊天框时发生错误: {ex.Message}", success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }
}
