using System;
using PeakChatOps.API;
using PeakChatOps.Core;
using Cysharp.Threading.Tasks;
using PeakChatOps.UI;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("sync", "同步状态", "用法: /sync")]
public class SyncCommand
{
    public SyncCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://sync", Handle);
    }

    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            // 重新加载命令
            Cmdx.LoadPCmd();
            Cmdx.Prefix = PeakChatOpsPlugin.CmdPrefix.Value;
            // 刷新配置 更新UI
            PeakChatOpsUI.Instance.RefreshUI();
            // 更新AI上下文日志设置
            AIChatContextLogger.Instance?.SyncMaxHistoryFromConfig();

            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: "同步完成", stderr: null, success: true);
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


