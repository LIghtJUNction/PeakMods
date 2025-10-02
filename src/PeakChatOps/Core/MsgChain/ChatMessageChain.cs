using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Client.Photon;
using PeakChatOps.UI;

namespace PeakChatOps.Core.MsgChain;

// 加载ChatMessageChain > AIChatMessageChain
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
        EventBusRegistry.ChatMessageBus.RunAsync("sander://other", cts.Token).Forget();
        EventBusRegistry.ChatMessageBus.RunAsync("sander://self", cts.Token).Forget();
        EventBusRegistry.ChatMessageBus.RunAsync("sander://system", cts.Token).Forget();

        // 启动AIChatMessageChain
        AIChatMessageChain.EnsureInitialized();

    }

    #region 接收消息（网络端）后处理器
    // 其他玩家消息的入口
    private static UniTask HandleRemoteChatMessageAsync(ChatMessageEvent evt)
    {
        if (evt == null)
            return UniTask.CompletedTask;
        else if (string.IsNullOrWhiteSpace(evt.Message))
            return UniTask.CompletedTask;
        // 记录其他玩家发来的消息到AI上下文
        AIChatContextLogger.Instance?.LogUser(evt.Message, evt.Sender, evt.UserId);
        if (evt.Extra != null && evt.Extra.GetExtraValue<bool>("system", false))
        {
            DevLog.UI("[ChatMessageChain] Received system message: " + evt.Message + " UserID: " + evt.UserId);
            PeakChatOpsUI.Instance.AddMessage(evt.Sender, evt.Message);
        }

        string colorHex = "#FFFFFF";
        if (evt.IsDead)
            colorHex = "#888888";
        else
        {
            var cStr = evt.Extra?.GetExtraValue<string>("color", null);
            if (!string.IsNullOrEmpty(cStr)) colorHex = cStr;
        }
        string deadMark = evt.IsDead ? " <b><color=#FF0000>(DEAD)</color></b>" : "";


        // Ping handling delegated to PingHandler
        try
        {
            Handle.PingHandler.HandlePing(evt);
        }
        catch (Exception ex)
        {
            try { PeakChatOpsUI.Instance.AddMessage("<color=#FF0000>[PongError]</color>: ", ex.Message); } catch { }
        }

        // 检查是否启用AI自动翻译，若已启用则委托 AITranslateHandler 处理并早退
        if (Handle.AITranslateHandler.TryHandleAutoTranslate(evt))
        {
            return UniTask.CompletedTask;
        }
        PeakChatOpsUI.Instance.AddMessage($"<color={colorHex}>[{evt.Sender}]</color>{deadMark}", evt.Message);
        return UniTask.CompletedTask;
    }

    #endregion

    #region 发送消息（网络端）前处理器

    private static async UniTask HandleLocalChatMessageAsync(ChatMessageEvent evt)
    {
        // 记录本地用户直接发送的消息到AI上下文
        if (evt != null && !string.IsNullOrWhiteSpace(evt.Message))
        {
            AIChatContextLogger.Instance?.LogUser(evt.Message, evt.Sender, evt.UserId);
        }
        string colorHex = "#7CFC00";

        try
        {
        object[] payload = new object[]
        {
            evt.Sender,
            evt.Message,
            evt.UserId,
            evt.IsDead,
            ConvertExtraToHashtable(evt.Extra)
        };
            Photon.Pun.PhotonNetwork.RaiseEvent(
                EventCodes.ChatEventCode,
                payload,
                new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others },
                SendOptions.SendReliable
            );
        }
        catch (Exception ex)
        {
            PeakChatOpsUI.Instance.AddMessage("<color=#FF0000>[Error]</color>", ex.Message);
        }
        PeakChatOpsUI.Instance.AddMessage($"<color={colorHex}>[You]</color>", evt.Message);
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
                ConvertExtraToHashtable(evt.Extra)
            };

            var options = new Photon.Realtime.RaiseEventOptions();

            // Prefer typed WhisperExtra if provided under the "Whisper" key
            if (evt.Extra != null && evt.Extra.TryGetValue("Whisper", out var whisperObj))
            {
                try
                {
                    if (whisperObj is WhisperExtra wExtra)
                    {
                        if (wExtra.TargetActors != null && wExtra.TargetActors.Length > 0)
                        {
                            options.TargetActors = wExtra.TargetActors;
                        }
                        else if (wExtra.TargetNames != null && wExtra.TargetNames.Length > 0)
                        {
                            options.TargetActors = System.Linq.Enumerable.ToArray(
                                System.Linq.Enumerable.Where(
                                    System.Linq.Enumerable.Select(wExtra.TargetNames, name => ChatApiUtil.NameToActorId(name)),
                                    actorNumber => actorNumber > 0)
                            );
                        }
                    }
                    // Some senders may place a dictionary form; check for that as fallback
                    else if (whisperObj is IDictionary<string, object> dict)
                    {
                        if (dict.TryGetValue("TargetActors", out var ta) && ta is int[] targetActors && targetActors.Length > 0)
                        {
                            options.TargetActors = targetActors;
                        }
                        else if (dict.TryGetValue("TargetNames", out var tn) && tn is string[] targetNames && targetNames.Length > 0)
                        {
                            options.TargetActors = System.Linq.Enumerable.ToArray(
                                System.Linq.Enumerable.Where(
                                    System.Linq.Enumerable.Select(targetNames, name => ChatApiUtil.NameToActorId(name)),
                                    actorNumber => actorNumber > 0)
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    DevLog.UI($"[ChatMessageChain] Failed to parse WhisperExtra: {ex}");
                }
            }

            // If no Whisper payload or it didn't yield targets, fall back to legacy top-level keys
            if ((options.TargetActors == null || options.TargetActors.Length == 0) && evt.Extra != null)
            {
                if (evt.Extra.TryGetValue("TargetActors", out var targetObj) && targetObj is int[] targetActors && targetActors.Length > 0)
                {
                    options.TargetActors = targetActors;
                }
                else if (evt.Extra.TryGetValue("TargetNames", out var nameObj) && nameObj is string[] targetNames && targetNames.Length > 0)
                {
                    options.TargetActors = System.Linq.Enumerable.ToArray(
                        System.Linq.Enumerable.Where(
                            System.Linq.Enumerable.Select(targetNames, name => ChatApiUtil.NameToActorId(name)),
                            actorNumber => actorNumber > 0)
                    );
                }
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
            PeakChatOpsUI.Instance.AddMessage("<color=#FF0000>[Error]</color>", ex.Message);
        }
        return UniTask.CompletedTask;
    }

    // Convert in-memory extras (Dictionary<string, object> or DTOs with ToDictionary) to Photon Hashtable
    private static Hashtable ConvertExtraToHashtable(IDictionary<string, object> extra)
    {
        var ht = new Hashtable();
        if (extra == null)
            return ht;

        foreach (var kv in extra)
        {
            var key = kv.Key;
            var val = kv.Value;
            if (val == null)
            {
                ht[key] = null;
                continue;
            }

            // primitive and array types that Photon supports directly
            if (val is string || val is bool || val is byte || val is sbyte || val is short || val is ushort
                || val is int || val is uint || val is long || val is ulong || val is float || val is double
                || val is byte[] || val is int[] || val is string[] || val is object[])
            {
                ht[key] = val;
                continue;
            }

            // nested dictionary
            if (val is IDictionary<string, object> dict)
            {
                ht[key] = ConvertExtraToHashtable(dict);
                continue;
            }

            // support DTOs that expose ToDictionary()
            try
            {
                var mi = val.GetType().GetMethod("ToDictionary", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (mi != null && typeof(IDictionary<string, object>).IsAssignableFrom(mi.ReturnType))
                {
                    var nested = mi.Invoke(val, null) as IDictionary<string, object>;
                    if (nested != null)
                    {
                        ht[key] = ConvertExtraToHashtable(nested);
                        continue;
                    }
                }
            }
            catch { /* fallthrough to string fallback */ }

            // fallback: store string representation
            ht[key] = val.ToString();
        }

        return ht;
    }
}

#endregion
