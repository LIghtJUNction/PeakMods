using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("echo", "回显输入内容", "用法: /echo <内容>\n将你输入的内容原样返回。")]
public class EchoCommand
{
    // New message-driven handler signature. Plugins/commands register handlers
    // on EventBusRegistry.CmdMessageBus with channel "cmd://echo".
    // 注册
    public EchoCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://echo", Handle);
        DevLog.UI("[Cmd] EchoCommand subscribed to cmd://echo");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            var args = evt.Args ?? Array.Empty<string>();
            var res = args.Length == 0 ? "请输入要回显的内容。" : string.Join(" ", args);
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: res, stderr: null, success: true);
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
