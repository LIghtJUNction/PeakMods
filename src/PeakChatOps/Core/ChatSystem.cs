using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using PeakChatOps.API;
using System;

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
/// 统一的聊天系统 - 包含消息包处理和网络通信
/// </summary>
public class ChatSystem : MonoBehaviour
{
    public static ChatSystem Instance;
    // Chat event code moved to API: EventCodes.ChatEventCode

    // 事件处理字典
    private readonly Dictionary<byte, Action<EventData>> eventHandlers = new();

    private void Start()
    {
        DevLog.UI("[ChatSystem.Start] Called, registering EventReceived");
        Instance = this;
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

    private void HandleChatEvent(EventData photonEvent)
    {
        var data = (object[])photonEvent.CustomData;
        if (data.Length < 4) return;
        var msg = new MessageData(
            data[0]?.ToString() ?? null,
            data[1]?.ToString() ?? null,
            data[2]?.ToString() ?? null,
            bool.TryParse(data[3]?.ToString(), out var d) && d,
            (data.Length > 4 && data[4] is Dictionary<string, object> dict) ? dict : null
        );
        ReceiveChatMessage(msg);
    }

    public void ReceiveChatMessage(MessageData msg)
    {
        var evt = new ChatMessageEvent(
            msg.Nickname,
            msg.Message,
            msg.UserId,
            msg.IsDead,
            msg.Extra
        );
    _ = EventBusRegistry.ChatMessageBus.Publish("sander://other", evt);
    }

    public void SendChatMessage(string message)
    {
        SendChatMessage(message, new Dictionary<string, object>());
    }

    // 新增重载，支持扩展字典
    // local player send message // cmd
    public void SendChatMessage(string message, Dictionary<string, object> extra)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        DevLog.UI($"[ChatSystem.SendChatMessage] called, message={message}");
        string prefix = PeakChatOpsPlugin.CmdPrefix.Value;
        // 初始值
        bool isDead = false;
        // 检查是否为命令
        if (!string.IsNullOrEmpty(prefix) && message.StartsWith(prefix))
        {
            if (Character.localCharacter?.data != null)
            {
                isDead = Character.localCharacter.data.dead;
            }
            // 解析命令（去掉前缀）
            var withoutPrefix = message.Substring(prefix.Length).Trim();
            var parts = withoutPrefix.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = parts.Length > 0 ? parts[0] : string.Empty;
            var args = parts.Length > 1 ? parts[1..] : new string[0];
            var cmdEvt = new CmdMessageEvent(command, args, PhotonNetwork.LocalPlayer.UserId);
            _ = EventBusRegistry.CmdMessageBus.Publish("cmd://", cmdEvt); // 路由
            return;
        }
        // 普通消息：准备发送给其他玩家
        {
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
            _ = EventBusRegistry.ChatMessageBus.Publish("sander://self", evt);
        }
    }
}
