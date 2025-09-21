#nullable disable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PeakChatOps.Core
{
    /// <summary>
    /// 聊天消息数据
    /// </summary>
    public class ChatMessage
    {
        public string Content { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public Vector3 PlayerPosition { get; set; } = Vector3.zero;
        public Vector3 PlayerRotation { get; set; } = Vector3.zero;
        public int PhotonViewId { get; set; } = -1;
        public bool IsLocalPlayer { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsCommand { get; set; } = false;
        public bool IsSystemMessage { get; set; } = false;
        public string SessionId { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// 聊天响应数据
    /// </summary>
    public class ChatResponse
    {
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = true;  // 默认私有消息
        public bool IsError { get; set; } = false;    // 是否错误消息
        public bool SuppressEcho { get; set; } = false;  // 是否抑制回显
        public bool SendToPublicChat { get; set; } = false;  // 是否发送到公共聊天
    }

    /// <summary>
    /// 聊天消息处理器接口
    /// </summary>
    public interface IChatMessageHandler
    {
        /// <summary>
        /// 处理器名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 处理器优先级（数字越小优先级越高）
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// 是否可以处理此消息
        /// </summary>
        bool CanHandle(ChatMessage message);
        
        /// <summary>
        /// 处理消息并返回响应
        /// </summary>
        ChatResponse Handle(ChatMessage message);
    }

    /// <summary>
    /// 聊天系统接口
    /// </summary>
    public interface IChatSystem
    {
        /// <summary>
        /// 注册消息处理器
        /// </summary>
        void RegisterHandler(IChatMessageHandler handler);
        
        /// <summary>
        /// 注销消息处理器
        /// </summary>
        void UnregisterHandler(IChatMessageHandler handler);
        
        /// <summary>
        /// 处理输入消息
        /// </summary>
        void ProcessMessage(string input);
        
        /// <summary>
        /// 发送输出消息
        /// </summary>
        void SendMessage(string content, bool isPrivate = false, bool isError = false);
        
        /// <summary>
        /// 消息处理事件
        /// </summary>
        event Action<ChatMessage, ChatResponse> OnMessageProcessed;
    }
}