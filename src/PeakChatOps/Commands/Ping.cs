
#nullable enable
using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
namespace PeakChatOps.Commands;

[PCOCommand("ping", "pong!", "用法: /ping\n使用同一模块的人会回答Pong!")]
public class PingCommand
{
    // 注册
    public PingCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://ping", Handle);
        DevLog.UI("[Cmd] PingCommand subscribed to cmd://ping");
    }
    public static async UniTask Handle(CmdMessageEvent evt)
    {
        // 1. 解析参数
        string[] args = evt.Args ?? Array.Empty<string>();
        string target = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]) ? args[0].Trim() : "other";
        int myActorNumber = Photon.Pun.PhotonNetwork.LocalPlayer?.ActorNumber ?? -1;
        string userName = Photon.Pun.PhotonNetwork.LocalPlayer?.NickName ?? "Unknown";
        string userId = Photon.Pun.PhotonNetwork.LocalPlayer?.UserId ?? "?";
        var extra = new System.Collections.Generic.Dictionary<string, object>();
        // Use a top-level "Ping" object for easier parsing by message-chain handlers
        var pingPayload = new Core.MsgChain.PingExtra
        {
            UserActorNumber = myActorNumber
        };
        int[] targetActors;
        if (string.Equals(target, "other", StringComparison.OrdinalIgnoreCase))
        {
            // 发送给所有人（TargetActors = 所有在线玩家ActorNumber，排除自己）
            var allActors = Photon.Pun.PhotonNetwork.PlayerList;
            targetActors = System.Linq.Enumerable.ToArray(
                System.Linq.Enumerable.Select(
                    System.Linq.Enumerable.Where(
                        allActors,
                        p => p != null && p.ActorNumber != myActorNumber
                    ),
                    p => p.ActorNumber
                )
            );
        }
        else
        {
            // 私聊指定玩家
            int actorNumber = -1;
            try
            {
                actorNumber = ChatApiUtil.NameToActorId(target);
            }
            catch { }
            if (actorNumber <= 0)
            {
                var errEvt = new CmdExecResultEvent(
                    evt.Command,
                    args,
                    userId,
                    stdout: null,
                    stderr: $"未找到玩家 '{target}'，无法私发ping。",
                    success: false
                );
                await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
                return;
            }
            targetActors = new int[] { actorNumber };
        }
        pingPayload.TargetActors = targetActors;
        extra["Ping"] = pingPayload;

        // 2. 发送ping消息
        string richPing = $"<color=#00BFFF>[PeakChatOps Ping]</color> <b>{userName}</b> (ID: {userId}) at {DateTime.Now:HH:mm:ss}";
        var pingEvt = new ChatMessageEvent(
            userName,
            richPing,
            userId,
            isDead: false,
            extra: extra
        );

        // 远程分发
        if (string.Equals(target, "other", StringComparison.OrdinalIgnoreCase))
        {
            await EventBusRegistry.ChatMessageBus.Publish("sander://self", pingEvt);
        }
        else
        {
            await EventBusRegistry.ChatMessageBus.Publish("sander://whisper", pingEvt);
        }

        // 3. 输出命令执行结果到本地
        var resultEvt = new CmdExecResultEvent(
            evt.Command,
            args,
            userId,
            stdout: $"已发送 ping 请求到: {(string.Equals(target, "other", StringComparison.OrdinalIgnoreCase) ? "所有人" : target)}，请等待Pong响应...",
            stderr: null,
            success: true
        );
        await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        await UniTask.CompletedTask;
    }
}
