using System;
using UnityEngine;

namespace PeakChatOps.Core
{
    /// <summary>
    /// 消息包 - 包含所有消息传递所需的信息
    /// </summary>
    public class MessagePacket
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// 发送者ID
        /// </summary>
        public string SenderId { get; set; }
        
        /// <summary>
        /// 发送者名称
        /// </summary>
        public string SenderName { get; set; }
        
        /// <summary>
        /// 接收者ID（null表示发送给所有人）
        /// </summary>
        public string ReceiverId { get; set; }
        
        /// <summary>
        /// 接收者名称（null表示发送给所有人）
        /// </summary>
        public string ReceiverName { get; set; }
        
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Normal;
        
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 是否为私人消息
        /// </summary>
        public bool IsPrivate => !string.IsNullOrEmpty(ReceiverId);
        
        /// <summary>
        /// 是否为广播消息
        /// </summary>
        public bool IsBroadcast => string.IsNullOrEmpty(ReceiverId);
        
        /// <summary>
        /// 消息优先级
        /// </summary>
        public MessagePriority Priority { get; set; } = MessagePriority.Normal;
        
        /// <summary>
        /// 创建广播消息包
        /// </summary>
        public static MessagePacket CreateBroadcast(string content, string senderId, string senderName, MessageType type = MessageType.Normal)
        {
            return new MessagePacket
            {
                Content = content,
                SenderId = senderId,
                SenderName = senderName,
                Type = type
            };
        }
        
        /// <summary>
        /// 创建私人消息包
        /// </summary>
        public static MessagePacket CreatePrivate(string content, string senderId, string senderName, string receiverId, string receiverName, MessageType type = MessageType.Normal)
        {
            return new MessagePacket
            {
                Content = content,
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = receiverId,
                ReceiverName = receiverName,
                Type = type
            };
        }
        
        /// <summary>
        /// 创建系统消息包
        /// </summary>
        public static MessagePacket CreateSystem(string content, string receiverId = null, string receiverName = null)
        {
            return new MessagePacket
            {
                Content = content,
                SenderId = "SYSTEM",
                SenderName = "System",
                ReceiverId = receiverId,
                ReceiverName = receiverName,
                Type = MessageType.System,
                Priority = MessagePriority.High
            };
        }
    }
    
    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        Normal,     // 普通聊天
        System,     // 系统消息
        Command,    // 命令消息
        Whisper,    // 私聊
        Broadcast,  // 广播
        Error       // 错误消息
    }
    
    /// <summary>
    /// 消息优先级
    /// </summary>
    public enum MessagePriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }
}