#if DEBUG
using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using System.Diagnostics;
#nullable enable
namespace PeakChatOps.Commands;


[PCOCommand("dev", "开发者命令", "用法: /dev <内容>\n将你输入的内容原样返回。")]

public class DevCommand
{

    // 注册
    public DevCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://dev", Handle);
        DevLog.UI("[Cmd] DevCommand subscribed to cmd://dev");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {

        var args = evt.Args ?? Array.Empty<string>();
        // 测试
        await UniTask.SwitchToMainThread();
    }

}

#endif