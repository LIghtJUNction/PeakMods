using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace PeakChatOps.Core
{
    /// <summary>
    /// 统一的聊天系统 - 包含消息包处理和网络通信
    /// </summary>
    public class ChatSystem : MonoBehaviour, IOnEventCallback
    {
        private readonly List<IMessageHandler> _handlers = new();
        private static ChatSystem _instance;
        
        // 网络事件代码
        private const byte ChatEventCode = 81;
        private const byte WhisperEventCode = 82;
        
        public static ChatSystem Instance => _instance ??= CreateInstance();
        
        /// <summary>
        /// 创建单例实例
        /// </summary>
        private static ChatSystem CreateInstance()
        {
            var go = new GameObject("ChatSystem");
            GameObject.DontDestroyOnLoad(go);
            return go.AddComponent<ChatSystem>();
        }
        
        private void Start()
        {
            _instance = this;
            // 注册 Photon 事件监听
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        private void OnDestroy()
        {
            // 取消注册 Photon 事件监听
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
        /// <summary>
        /// Photon 事件接收处理
        /// </summary>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == ChatEventCode)
            {
                HandleChatEvent(photonEvent);
            }
            else if (photonEvent.Code == WhisperEventCode)
            {
                HandleWhisperEvent(photonEvent);
            }
        }
        
        /// <summary>
        /// 处理聊天事件
        /// </summary>
        private void HandleChatEvent(EventData eventData)
        {
            var data = (object[])eventData.CustomData;
            if (data.Length < 4) return;
            
            string nickname = data[0]?.ToString() ?? "Unknown";
            string message = data[1]?.ToString() ?? "";
            string userId = data[2]?.ToString() ?? "";
            bool isDead = bool.TryParse(data[3]?.ToString(), out var d) && d;
            
            // 显示消息
            if (PeakOpsUI.instance != null)
            {
                var formattedMessage = $"[{nickname}]: {message}";
                PeakOpsUI.instance.AddMessage(formattedMessage);
            }
        }
        
        /// <summary>
        /// 处理私聊事件
        /// </summary>
        private void HandleWhisperEvent(EventData eventData)
        {
            var data = (object[])eventData.CustomData;
            if (data.Length < 3) return;
            
            string senderName = data[0]?.ToString() ?? "Unknown";
            string message = data[1]?.ToString() ?? "";
            string timestamp = data[2]?.ToString() ?? "";
            
            // 显示私聊消息
            if (PeakOpsUI.instance != null)
            {
                var whisperMessage = $"🔒 私聊来自 {senderName}: {message}";
                PeakOpsUI.instance.AddMessage(whisperMessage);
            }
        }
        
        /// <summary>
        /// 注册消息处理器
        /// </summary>
        public void RegisterHandler(IMessageHandler handler)
        {
            if (_handlers.Contains(handler))
            {
                PeakChatOpsPlugin.Logger.LogWarning($"Handler {handler.Name} already registered");
                return;
            }
            
            _handlers.Add(handler);
            _handlers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            PeakChatOpsPlugin.Logger.LogInfo($"Registered handler: {handler.Name}");
        }
        
        /// <summary>
        /// 处理输入消息
        /// </summary>
        public void ProcessMessage(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;
            
            // 创建消息包
            var packet = CreateMessagePacket(input);
            
            // 处理消息包
            ProcessPacket(packet);
        }
        
        /// <summary>
        /// 处理消息包
        /// </summary>
        public void ProcessPacket(MessagePacket packet)
        {
            if (packet == null) return;
            
            try
            {
                // 找到第一个能处理的处理器
                var handler = _handlers.FirstOrDefault(h => h.CanHandle(packet));
                
                if (handler != null)
                {
                    PeakChatOpsPlugin.Logger.LogDebug($"Processing with handler: {handler.Name}");
                    var processedPacket = handler.Handle(packet);
                    
                    // 如果处理器返回了新的消息包，则发送它
                    if (processedPacket != null)
                    {
                        SendPacket(processedPacket);
                    }
                }
                else
                {
                    // 没有处理器，直接发送原始消息包
                    SendPacket(packet);
                }
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogError($"Error processing packet: {ex.Message}");
                SendError($"Processing error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 发送消息包
        /// </summary>
        private void SendPacket(MessagePacket packet)
        {
            if (packet?.Content == null) return;
            
            try
            {
                if (packet.IsPrivate)
                {
                    // 私人消息 - 只在本地显示
                    ShowLocalMessage(packet);
                }
                else
                {
                    // 广播消息 - 发送到网络
                    if (packet.Type == MessageType.System)
                    {
                        ShowLocalMessage(packet);
                    }
                    else
                    {
                        SendToNetwork(packet.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogError($"Error sending packet: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 在本地显示消息
        /// </summary>
        private void ShowLocalMessage(MessagePacket packet)
        {
            if (PeakOpsUI.instance == null) return;
            if (packet.Type == MessageType.System || packet.Type == MessageType.Error)
            {
                PeakOpsUI.instance.AddMessage(packet.Content);
            }
            else
            {
                var message = string.IsNullOrEmpty(packet.SenderName) || packet.SenderName == "SYSTEM" 
                    ? packet.Content 
                    : $"[{packet.SenderName}]: {packet.Content}";
                PeakOpsUI.instance.AddMessage(message);
            }
        }
        
        /// <summary>
        /// 发送到网络聊天
        /// </summary>
        private void SendToNetwork(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;
            
            try
            {
                bool isDead = false;
                if (Character.localCharacter?.data != null)
                {
                    isDead = Character.localCharacter.data.dead;
                }

                object[] payload = {
                    PhotonNetwork.LocalPlayer.NickName,
                    content,
                    PhotonNetwork.LocalPlayer.UserId,
                    isDead
                };

                PhotonNetwork.RaiseEvent(
                    ChatEventCode,
                    payload,
                    new RaiseEventOptions() { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable
                );
                
                PeakChatOpsPlugin.Logger.LogDebug($"Sent message to network: {content}");
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogError($"Error sending to network: {ex.Message}");
                SendError($"Failed to send message: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        public void SendWhisperMessage(string targetPlayerId, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            
            try
            {
                object[] payload = {
                    PhotonNetwork.LocalPlayer.NickName,
                    message,
                    DateTime.Now.ToString("HH:mm:ss")
                };

                // 发送给特定玩家
                var targetPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => p.UserId == targetPlayerId);
                if (targetPlayer != null)
                {
                    PhotonNetwork.RaiseEvent(
                        WhisperEventCode,
                        payload,
                        new RaiseEventOptions() { TargetActors = new int[] { targetPlayer.ActorNumber } },
                        SendOptions.SendReliable
                    );
                    
                    // 在本地也显示发送的私聊
                    var whisperMessage = $"🔒 私聊发给 {targetPlayer.NickName}: {message}";
                    if (PeakOpsUI.instance != null)
                    {
                        PeakOpsUI.instance.AddMessage(whisperMessage);
                    }
                }
                else
                {
                    SendError($"Player not found: {targetPlayerId}");
                }
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogError($"Error sending whisper: {ex.Message}");
                SendError($"Failed to send whisper: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 创建消息包
        /// </summary>
        private MessagePacket CreateMessagePacket(string input)
        {
            var senderId = GetCurrentPlayerId();
            var senderName = GetCurrentPlayerName();
            
            return MessagePacket.CreateBroadcast(input, senderId, senderName);
        }
        
        /// <summary>
        /// 发送错误消息
        /// </summary>
        public void SendError(string message)
        {
            var packet = MessagePacket.CreateSystem($"❌ {message}", GetCurrentPlayerId(), GetCurrentPlayerName());
            packet.Type = MessageType.Error;
            ShowLocalMessage(packet);
        }
        
        /// <summary>
        /// 发送系统消息
        /// </summary>
        public void SendSystemMessage(string message)
        {
            var packet = MessagePacket.CreateSystem(message);
            ShowLocalMessage(packet);
        }
        
        /// <summary>
        /// 获取当前玩家ID
        /// </summary>
        private string GetCurrentPlayerId()
        {
            try
            {
                var character = Character.localCharacter;
                return character?.GetInstanceID().ToString() ?? "0";
            }
            catch
            {
                return "0";
            }
        }
        
        /// <summary>
        /// 获取当前玩家名称
        /// </summary>
        private string GetCurrentPlayerName()
        {
            try
            {
                var character = Character.localCharacter;
                return character?.characterName ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        
        /// <summary>
        /// 获取发送者颜色
        /// </summary>
        private Color GetSenderColor(string senderId)
        {
            if (senderId == "SYSTEM") return Color.cyan;
            
            try
            {
                var character = Character.localCharacter;
                return character?.refs?.customization?.PlayerColor ?? Color.white;
            }
            catch
            {
                return Color.white;
            }
        }
        
        /// <summary>
        /// 清理所有处理器
        /// </summary>
        public void Clear()
        {
            _handlers.Clear();
        }
    }
}