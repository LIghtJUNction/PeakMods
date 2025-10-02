using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using PeakChatOps.API;
using System;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.Core;

// 消息数据结构
public class MessageData
{
    public string Nickname;
    public string Message;
    public string UserId;
    public bool IsDead;
    public Dictionary<string, object> Extra;

    public MessageData(string nickname, string message, string userId, bool isDead, Dictionary<string, object> extra = null)
    {
        Nickname = nickname;
        Message = message;
        UserId = userId;
        IsDead = isDead;
        Extra = extra;
    }
}

/// <summary>
/// 统一的聊天系统 - 包含消息包处理和网络通信（异步版本）
/// 注意：此类只保留异步 API（UniTask），旧的同步方法已移除。
/// </summary>
public class ChatSystem : MonoBehaviour
{
    public static ChatSystem Instance;

    // 事件处理字典
    private readonly Dictionary<byte, Action<EventData>> eventHandlers = new();
    // 静态并发队列：当 ChatSystem 实例尚未就绪时，可缓存要发送的消息，实例化后会排空并发送
    private static ConcurrentQueue<(string message, Dictionary<string, object> extra)> _staticPendingSends = new ConcurrentQueue<(string, Dictionary<string, object>)>();

    private void Start()
    {
        DevLog.UI("[ChatSystem.Start] Called, registering EventReceived");
        DevLog.File("[ChatSystem] Start called");
        Instance = this;
        // 实例化后尝试排空任何静态缓存的发送请求（例如 UI 提交时实例尚未就绪）
        FlushStaticPendingSendsAsync().Forget();
        // 注册事件处理器
        eventHandlers[EventCodes.ChatEventCode] = HandleChatEvent;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
        DevLog.UI("[ChatSystem.OnDestroy] Unregistering EventReceived");
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        DevLog.UI($"[ChatSystem.OnEvent] photonEvent: code={photonEvent.Code}, sender={photonEvent.Sender}, customData={photonEvent.CustomData}");
        if (eventHandlers.TryGetValue(photonEvent.Code, out var handler))
        {
            handler(photonEvent);
        }
        else
        {
            DevLog.UI($"事件[ChatSystem.OnEvent]：未注册的事件类型");
        }
    }

    // 接收远程玩家消息的入口
    private void HandleChatEvent(EventData photonEvent)
    {
        DevLog.File($"[ChatSystem] HandleChatEvent code={photonEvent.Code} sender={photonEvent.Sender}");
        var data = (object[])photonEvent.CustomData;
        if (data.Length < 4) return;
        // Normalize the extra payload: accept Dictionary<string, object> or Photon Hashtable
        Dictionary<string, object> extraDict = null;
        if (data.Length > 4 && data[4] != null)
        {
            if (data[4] is Dictionary<string, object> dict)
            {
                extraDict = dict;
            }
            else if (data[4] is ExitGames.Client.Photon.Hashtable ht)
            {
                extraDict = ConvertHashtableToDictionary(ht);
            }
            else if (data[4] is IDictionary objDict)
            {
                // fallback: try to copy entries where keys are strings
                extraDict = new Dictionary<string, object>();
                foreach (DictionaryEntry de in objDict)
                {
                    if (de.Key != null)
                    {
                        var key = de.Key.ToString();
                        extraDict[key] = de.Value;
                    }
                }
            }
        }

        var msg = new MessageData(
            data[0]?.ToString() ?? null,
            data[1]?.ToString() ?? null,
            data[2]?.ToString() ?? null,
            bool.TryParse(data[3]?.ToString(), out var d) && d,
            extraDict
        );
        // Photon 回调是同步的；以 fire-and-forget 的方式启动异步处理
        DevLog.File($"[ChatSystem] Received chat message from payload: nick={msg.Nickname} msg={msg.Message}");

    }

    // 本地发送消息的入口
    // 异步版本的发送实现，负责发布到 ChatMessageBus（以及网络发送）
    public async UniTask SendChatMessageAsync(string message, Dictionary<string, object> extra)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        DevLog.UI($"[ChatSystem.SendChatMessageAsync] called, message={message}");
        DevLog.File($"[ChatSystem] SendChatMessageAsync called message='{message}' extraKeys={(extra==null?0:extra.Count)}");
        string prefix = PeakChatOpsPlugin.config.CmdPrefix.Value;
        bool isDead = false;
        if (!string.IsNullOrEmpty(prefix) && message.StartsWith(prefix))
        {
            DevLog.File($"[ChatSystem] Detected command prefix. processing as command: {message}");
            if (Character.localCharacter?.data != null)
            {
                isDead = Character.localCharacter.data.dead;
            }
            var withoutPrefix = message.Substring(prefix.Length).Trim();
            var parts = withoutPrefix.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            var command = parts.Length > 0 ? parts[0] : string.Empty;
            var args = parts.Length > 1 ? parts[1..] : new string[0];
            var cmdEvt = new CmdMessageEvent(command, args, PhotonNetwork.LocalPlayer.UserId);
            await EventBusRegistry.CmdMessageBus.Publish("cmd://", cmdEvt);
            return;
        }

        if (Character.localCharacter?.data != null)
        {
            isDead = Character.localCharacter.data.dead;
        }

        var evt = new ChatMessageEvent(
            PhotonNetwork.LocalPlayer.NickName,
            message,
            PhotonNetwork.LocalPlayer.UserId,
            isDead,
            extra
        );
        DevLog.File($"[ChatSystem] Publishing chat event to bus: sender={evt.Sender} msg={evt.Message}");
        await EventBusRegistry.ChatMessageBus.Publish("sander://self", evt);
    }

    /// <summary>
    /// 将 Photon Hashtable 转换为 Dictionary&lt;string, object&gt;
    /// </summary>
    private static Dictionary<string, object> ConvertHashtableToDictionary(ExitGames.Client.Photon.Hashtable hashtable)
    {
        if (hashtable == null) return null;
        
        var dictionary = new Dictionary<string, object>();
        foreach (DictionaryEntry entry in hashtable)
        {
            if (entry.Key != null)
            {
                dictionary[entry.Key.ToString()] = entry.Value;
            }
        }
        return dictionary;
    }

    /// <summary>
    /// 异步排空静态缓存队列中的待发送消息
    /// </summary>
    private async UniTaskVoid FlushStaticPendingSendsAsync()
    {
        while (_staticPendingSends.TryDequeue(out var pending))
        {
            await SendChatMessageAsync(pending.message, pending.extra);
        }
    }
}
