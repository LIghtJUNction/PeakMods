namespace PeakChatOps.Core
{
    /// <summary>
    /// 消息处理器接口 - 简化版本
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// 处理器名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 处理优先级（数值越小优先级越高）
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// 判断是否可以处理此消息包
        /// </summary>
        bool CanHandle(MessagePacket packet);
        
        /// <summary>
        /// 处理消息包，返回处理后的消息包（可能为null表示不需要继续传递）
        /// </summary>
        MessagePacket Handle(MessagePacket packet);
    }
}