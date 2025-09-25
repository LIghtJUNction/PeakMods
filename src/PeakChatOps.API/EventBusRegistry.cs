
namespace PeakChatOps.API;


[EventName("ChatMessage")]
[EventDescription("聊天消息事件，包含消息内容、发送者、用户ID、死亡状态等信息")]
public class ChatMessageEvent : IEvent
{
    public string Sender { get; set; }
    public string Message { get; set; }
    public string UserId { get; set; }
    public bool IsDead { get; set; }
    public System.Collections.Generic.Dictionary<string, object>? Extra { get; set; }

    public ChatMessageEvent(string sender, string message, string userId, bool isDead, System.Collections.Generic.Dictionary<string, object>? extra = null)
    {
        Sender = sender;
        Message = message;
        UserId = userId;
        IsDead = isDead;
        Extra = extra;
    }
}

[EventName("CmdMessage")]
[EventDescription("命令消息事件，包含命令内容、发送者、用户ID等信息")]
public class CmdMessageEvent
{
    public string Command { get; set; }
    public string[] Args { get; set; }
    public string UserId { get; set; }

    public CmdMessageEvent(string command, string[] args, string userId)
    {
        Command = command;
        Args = args;
        UserId = userId;
    }
}


public class CmdExecResultEvent
{
    // 原始命令名
    public string Command { get; set; }
    // 原始参数
    public string[] Args { get; set; }
    // 触发命令的用户
    public string UserId { get; set; }
    // 标准输出文本
    public string? Stdout { get; set; }
    // 标准错误文本
    public string? Stderr { get; set; }
    // 是否成功（返回码或 bool）
    public bool Success { get; set; }
    // 时间戳
    public System.DateTime Timestamp { get; set; }

    public CmdExecResultEvent(string command, string[] args, string userId, string? stdout = null, string? stderr = null, bool success = true)
    {
        Command = command;
        Args = args;
        UserId = userId;
        Stdout = stdout;
        Stderr = stderr;
        Success = success;
        Timestamp = System.DateTime.UtcNow;
    }
}

/// <summary>
/// 全局事件总线注册表，主模块和插件都可通过此类访问事件总线单例。
/// </summary>
public static class EventBusRegistry
{
    /// <summary>
    /// 通用 object 事件总线，主模块和插件可通过通道名约定事件类型和协议。
    /// </summary>
    public static readonly UniEventBus<object> CommonBus = new UniEventBus<object>();
    // 聊天消息事件总线
    public static readonly UniEventBus<ChatMessageEvent> ChatMessageBus = new UniEventBus<ChatMessageEvent>();
    // 命令消息总线
    public static readonly UniEventBus<CmdMessageEvent> CmdMessageBus = new UniEventBus<CmdMessageEvent>();

    public static readonly UniEventBus<CmdExecResultEvent> CmdExecResultBus = new UniEventBus<CmdExecResultEvent>();
}
