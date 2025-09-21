namespace PeakChatOps.Core
{
    /// <summary>
    /// 基础消息处理器
    /// </summary>
    public class BasicMessageHandler : IMessageHandler
    {
        public string Name => "Basic Message Handler";
        public int Priority => 100;
        
        public bool CanHandle(MessagePacket packet)
        {
            // 暂时不处理任何特殊消息，让系统使用默认处理
            return false;
        }
        
        public MessagePacket Handle(MessagePacket packet)
        {
            // 直接返回原消息包
            return packet;
        }
    }
}