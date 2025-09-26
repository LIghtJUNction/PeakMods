using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;

using System.Collections.Generic;
using System.ClientModel;

#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("ai", "AI助手", "开发中")]
public class AICommand
{
    // 注册
    public AICommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://ai", Handle);
        DevLog.UI("[Cmd] AICommand subscribed to cmd://ai");
    }

    public static async UniTask Handle(CmdMessageEvent evt)
    {
        DevLog.UI("[Cmd] AICommand received.");

    }
}