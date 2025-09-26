
using System;
using UnityEngine.SceneManagement;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;

#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("exit", "游戏直接结束并返回主菜单", "用法: /exit\n返回主菜单，并记为失败")]
public class ExitCommand
{
    public ExitCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://exit", Handle);
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            // SceneManager.LoadScene("MainMenu");
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: "已返回主菜单。", stderr: null, success: true);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: "退出失败: " + ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }
}
