using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

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
    private const byte ChatEventCode = 81;

    private void Start()
    {
    PeakChatOpsPlugin.Logger.LogDebug("[ChatSystem.Start] Called, registering EventReceived");
    Instance = this;
    PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
    PeakChatOpsPlugin.Logger.LogDebug("[ChatSystem.OnDestroy] Unregistering EventReceived");
    PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
    PeakChatOpsPlugin.Logger.LogDebug($"[ChatSystem.OnEvent] photonEvent: code={photonEvent.Code}, sender={photonEvent.Sender}, customData={photonEvent.CustomData}");
        if (photonEvent.Code == ChatEventCode)
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
    }

    public void ReceiveChatMessage(MessageData msg)
    {
        if (PeakOpsUI.instance != null)
        {
            // 这里可插入消息处理器链
            MsgHandlerChain.IncomingMessageChain(msg);
            // PeakOpsUI.instance.AddMessage($"[{msg.UserId}]: {msg.Message}{extraInfo}");
        }
    }

    public void SendChatMessage(string message)
    {
        SendChatMessage(message, null);
    }

    // 新增重载，支持扩展字典
    public void SendChatMessage(string message, Dictionary<string, object> extra)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            PeakChatOpsPlugin.Logger.LogDebug($"[ChatSystem.SendChatMessage] called, message={message}");
            bool isDead = false;
            if (Character.localCharacter?.data != null)
            {
                isDead = Character.localCharacter.data.dead;
            }
            object[] payload;
            if (extra != null && extra.Count > 0)
            {
                payload = new object[] {
                    PhotonNetwork.LocalPlayer.NickName,
                    message,
                    PhotonNetwork.LocalPlayer.UserId,
                    isDead,
                    extra
                };
            }
            else
            {
                payload = new object[] {
                    PhotonNetwork.LocalPlayer.NickName,
                    message,
                    PhotonNetwork.LocalPlayer.UserId,
                    isDead
                };
            }
            PeakChatOpsPlugin.Logger.LogDebug("[ChatSystem.SendChatMessage] Calling MsgHandlerChain.OutgoingMessageChain");
            // 上行消息处理链
            MsgHandlerChain.OutgoingMessageChain(ChatEventCode, payload);
        }
    }
}
