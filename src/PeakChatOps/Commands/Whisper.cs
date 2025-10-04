#nullable enable
using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
using System.Linq;
using System.Collections.Generic;

namespace PeakChatOps.Commands;

[PCOCommand("whisper", "私聊", "用法: /whisper <playerName> <message>")]
public class WhisperCommand
{
    public WhisperCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://whisper", Handle);
        DevLog.UI("[Cmd] WhisperCommand subscribed to cmd://whisper");
    }

    public static async UniTask Handle(CmdMessageEvent evt)
    {
        string[] args = evt.Args ?? Array.Empty<string>();
        string userId = Photon.Pun.PhotonNetwork.LocalPlayer?.UserId ?? "?";

        if (args.Length < 2)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, args, userId, stdout: null, stderr: "用法: /whisper <playerName> <message>", success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
            return;
        }

        string targetName = args[0].Trim();
        var message = string.Join(' ', args.Skip(1)).Trim();

        // Resolve target actor
        int actorNumber = -1;
        try { actorNumber = ChatApiUtil.NameToActorId(targetName); } catch { }
        if (actorNumber <= 0)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, args, userId, stdout: null, stderr: $"未找到玩家 '{targetName}'，无法私发消息。", success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
            return;
        }

        string myName = Photon.Pun.PhotonNetwork.LocalPlayer?.NickName ?? "Unknown";
        string richMsg = $"<color=#FFD700>[Whisper]</color> <b>{myName}</b> -> <b>{targetName}</b>: {message}";

        // Prepare typed WhisperExtra
        var whisperExtra = new Core.MsgChain.WhisperExtra
        {
            UserActorNumber = Photon.Pun.PhotonNetwork.LocalPlayer?.ActorNumber ?? -1,
            TargetActors = new int[] { actorNumber },
            TargetNames = new string[] { targetName }
        };

        var extra = new Dictionary<string, object>
        {
            ["Whisper"] = whisperExtra
        };

        var chatEvt = new ChatMessageEvent(myName, richMsg, userId, isDead: false, extra: extra);

        // Publish to whisper channel so central handler raises Photon event to target actor
        await EventBusRegistry.ChatMessageBus.Publish("sander://whisper", chatEvt);

        var resultEvt = new CmdExecResultEvent(evt.Command, args, userId, stdout: $"已向 {targetName} 私发消息。", stderr: null, success: true);
        await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        await UniTask.CompletedTask;
    }
}

