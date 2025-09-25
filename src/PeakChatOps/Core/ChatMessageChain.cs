using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace PeakChatOps.Core;

public static class ChatMessageChain
{
    public static void EnsureInitialized()
    {
        // 订阅 ChatMessageBus
        EventBusRegistry.ChatMessageBus.Subscribe("sander://other", HandleRemoteChatMessageAsync);
        EventBusRegistry.ChatMessageBus.Subscribe("sander://self", HandleLocalChatMessageAsync);
        EventBusRegistry.ChatMessageBus.Subscribe("sander://whisper", HandleWhisperChatMessageAsync);
        // 启动 runner
        var cts = CentralCmdRouter.GetOrCreateBusCts();
        UniEventBusRunner.RunChannelLoop(EventBusRegistry.ChatMessageBus, "sander://other", cts.Token).Forget();
        UniEventBusRunner.RunChannelLoop(EventBusRegistry.ChatMessageBus, "sander://self", cts.Token).Forget();
        UniEventBusRunner.RunChannelLoop(EventBusRegistry.ChatMessageBus, "sander://system", cts.Token).Forget();
    }

    #region 接收消息（网络端）后处理器
    // 其他玩家消息的入口
    private static UniTask HandleRemoteChatMessageAsync(ChatMessageEvent evt)
    {
        if (evt == null)
            return UniTask.CompletedTask;
        else if (string.IsNullOrWhiteSpace(evt.Message))
            return UniTask.CompletedTask;
        if (evt.Extra != null && evt.Extra.TryGetValue("system", out var isSystem) && isSystem is bool b && b)
        {
            DevLog.UI("[ChatMessageChain] Received system message: " + evt.Message + " UserID: " + evt.UserId);
            PeakOpsUI.instance.AddMessage(evt.Message);
        }

        string colorHex = "#FFFFFF";
        if (evt.IsDead)
            colorHex = "#888888";
        else if (evt.Extra != null && evt.Extra.TryGetValue("color", out var cObj) && cObj is string cStr)
            colorHex = cStr;
        string deadMark = evt.IsDead ? " <b><color=#FF0000>(DEAD)</color></b>" : "";
        string richText = $"<color={colorHex}>[{evt.Sender}]</color>{deadMark}: {evt.Message}";

        PeakOpsUI.instance.AddMessage(richText);

        // 检查是不是ping消息 判断：包含Extra.TargetActors 是不是包含自己的 actorNumber 并且 Extra.CmdName == "ping"
        try
        {
            var extra = evt.Extra;
            if (extra != null && extra.TryGetValue("TargetActors", out var targetObj) && targetObj is int[] targetActors
                && extra.TryGetValue("CmdName", out var cmdNameObj) && cmdNameObj is string cmdName && cmdName == "ping")
            {
                int myActorNumber = Photon.Pun.PhotonNetwork.LocalPlayer?.ActorNumber ?? -1;
                if (Array.IndexOf(targetActors, myActorNumber) >= 0)
                {
                    // 构造Pong消息，目标为对方
                    var pongExtra = new Dictionary<string, object>();
                    pongExtra["CmdName"] = "ping"; // ping才是命令，pong只是回应
                    pongExtra["TargetActors"] = new int[] { evt.Extra.ContainsKey("UserActorNumber") ? (int)evt.Extra["UserActorNumber"] : (evt.UserId == null ? -1 : myActorNumber) };
                    string userName = Photon.Pun.PhotonNetwork.LocalPlayer?.NickName ?? "Unknown";
                    string userId = Photon.Pun.PhotonNetwork.LocalPlayer?.UserId ?? "?";
                    string pongMsg = $"<color=#32CD32>[PeakChatOps Pong]</color> <b>{userName}</b> (ID: {userId}) at {DateTime.Now:HH:mm:ss}";
                    var pongEvt = new ChatMessageEvent(
                        userName,
                        pongMsg,
                        userId,
                        isDead: false,
                        extra: pongExtra
                    );
                    // 回复Pong
                    HandleWhisperChatMessageAsync(pongEvt).Forget();
                }
            }
        }
        catch (Exception ex)
        {
            PeakOpsUI.instance.AddMessage($"<color=#FF0000>[PongError]</color>: {ex.Message}");
        }
        return UniTask.CompletedTask;
    }

    #endregion

    #region 发送消息（网络端）前处理器

    private static async UniTask HandleLocalChatMessageAsync(ChatMessageEvent evt)

    {
        string colorHex = "#7CFC00";
        string richText = $"<color={colorHex}>[You]</color>: {evt.Message}";
        try
        {
            object[] payload = new object[]
            {
                    evt.Sender,
                    evt.Message,
                    evt.UserId,
                    evt.IsDead,
                    evt.Extra ?? new Dictionary<string, object>()
            };
            Photon.Pun.PhotonNetwork.RaiseEvent(
                EventCodes.ChatEventCode,
                payload,
                new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others },
                ExitGames.Client.Photon.SendOptions.SendReliable
            );
        }
        catch (Exception ex)
        {
            PeakOpsUI.instance.AddMessage($"<color=#FF0000>[Error]</color>: {ex.Message}");
        }
        PeakOpsUI.instance.AddMessage(richText);
        await UniTask.CompletedTask;
    }
    
    private static UniTask HandleWhisperChatMessageAsync(ChatMessageEvent evt)
    {
        try
        {
            object[] payload = new object[]
            {
                evt.Sender,
                evt.Message,
                evt.UserId,
                evt.IsDead,
                evt.Extra ?? new System.Collections.Generic.Dictionary<string, object>()
            };
            var options = new Photon.Realtime.RaiseEventOptions();
            if (evt.Extra != null && evt.Extra.TryGetValue("TargetActors", out var targetObj) && targetObj is int[] targetActors && targetActors.Length > 0)
            {
                options.TargetActors = targetActors;
            }
            else if (evt.Extra != null && evt.Extra.TryGetValue("TargetNames", out var nameObj) && nameObj is string[] targetNames && targetNames.Length > 0)
            {
                options.TargetActors = System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Where(
                        System.Linq.Enumerable.Select(targetNames, name => ChatApiUtil.NameToActorId(name)),
                        actorNumber => actorNumber > 0)
                );
            }
            Photon.Pun.PhotonNetwork.RaiseEvent(
                EventCodes.ChatEventCode,
                payload,
                options,
                ExitGames.Client.Photon.SendOptions.SendReliable
            );
        }
        catch (Exception ex)
        {
            PeakOpsUI.instance.AddMessage($"<color=#FF0000>[Error]</color>: {ex.Message}");
        }
        return UniTask.CompletedTask;
    }
}

#endregion
