using System;
using System.Collections.Generic;
using PeakChatOps.API;
using PeakChatOps.Core.MsgChain;
using Cysharp.Threading.Tasks;
using PeakChatOps.UI;

namespace PeakChatOps.Core.MsgChain.Handle;


public static class PingHandler
{
    // 处理来自远端的 Ping 消息并自动回复 Pong（在内部调用 Whisper 发送）
    public static void HandlePing(ChatMessageEvent evt)
    {
        try
        {
            if (evt == null || evt.Extra == null) return;
            if (!evt.Extra.TryGetValue("Ping", out var obj) || obj == null) return;

            // 优先把 obj 当作类型化的 PingExtra 处理；若不是则尝试字典回退
            PingExtra ping = null;
            if (obj is PingExtra p)
            {
                ping = p;
            }
            else if (obj is IDictionary<string, object> dict)
            {
                try
                {
                    var cmdName = dict.TryGetValue("CmdName", out var cnObj) ? cnObj as string : null;
                    var userActor = dict.TryGetValue("UserActorNumber", out var uaObj) && uaObj is int ua ? ua : -1;
                    var targetActors = dict.TryGetValue("TargetActors", out var taObj) && taObj is int[] tas ? tas : Array.Empty<int>();
                    ping = new PingExtra { CmdName = cmdName ?? "ping", UserActorNumber = userActor, TargetActors = targetActors };
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }

            if (ping == null || ping.CmdName != "ping" || ping.TargetActors == null) return;
            int myActorNumber = Photon.Pun.PhotonNetwork.LocalPlayer?.ActorNumber ?? -1;
            if (Array.IndexOf(ping.TargetActors, myActorNumber) < 0) return;

            int replyTo = ping.UserActorNumber > 0 ? ping.UserActorNumber : (evt.UserId == null ? -1 : myActorNumber);

            // 构造类型化的 ping 回复（PingExtra）并也构造 WhisperExtra 用于发送
            var pongPingExtra = new PingExtra
            {
                CmdName = "ping",
                UserActorNumber = myActorNumber,
                TargetActors = new int[] { replyTo }
            };

            var whisperExtra = new WhisperExtra
            {
                CmdName = "whisper",
                UserActorNumber = myActorNumber,
                TargetActors = new int[] { replyTo }
            };

            // 将类型化对象放到 extra 中（接收端优先使用类型化对象）
            var pongExtra = new Dictionary<string, object>
            {
                ["Ping"] = pongPingExtra,
                ["Whisper"] = whisperExtra
            };

            string userName = Photon.Pun.PhotonNetwork.LocalPlayer?.NickName ?? "Unknown";
            string userId = Photon.Pun.PhotonNetwork.LocalPlayer?.UserId ?? "?";
            string pongMsg = $"<color=#32CD32>[PeakChatOps Pong]</color> <b>{userName}</b> (ID: {userId}) at {DateTime.Now:HH:mm:ss}";
            var pongEvt = new ChatMessageEvent(
                userName,
                pongMsg,
                userId,
                isDead: false,
                extra: pongExtra // WhisperExtra
            );
            // 通过 ChatMessageBus 发布到 sander://whisper，让系统统一处理发送
            EventBusRegistry.ChatMessageBus.Publish("sander://whisper", pongEvt).Forget();
        }
        catch (Exception ex)
        {
            try { PeakChatOpsUI.Instance.AddMessage($"<color=#FF0000>[PongError]</color>: {ex.Message}"); } catch { }
        }
    }
}