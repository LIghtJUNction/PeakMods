using System;
using System.Collections.Generic;

namespace PeakChatOps.Core.MsgChain;

// 所有 Extra 包的集中定义
public class PingExtra
{
    public string CmdName { get; set; } = "ping";
    public int UserActorNumber { get; set; }
    public int[] TargetActors { get; set; } = Array.Empty<int>();
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["CmdName"] = CmdName,
            ["UserActorNumber"] = UserActorNumber,
            ["TargetActors"] = TargetActors
        };
    }
}
public class WhisperExtra
{
    // 兼容 PingExtra 风格的结构（可包含命令名与发起者 actor）
    public string CmdName { get; set; } = "whisper";
    public int UserActorNumber { get; set; }
    // 直发 actor 列表
    public int[] TargetActors { get; set; } = Array.Empty<int>();
    // 或者以名字为准的目标列表
    public string[] TargetNames { get; set; } = Array.Empty<string>();
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["CmdName"] = CmdName,
            ["UserActorNumber"] = UserActorNumber,
            ["TargetActors"] = TargetActors,
            ["TargetNames"] = TargetNames
        };
    }
}
// AI 相关的扩展包定义（用于 AIChatMessageChain）
public class AIExtra
{
    // 命令名（默认 ai）
    public string CmdName { get; set; } = "ai";
    // 可选指令，例如 "clear"、"send" 等
    public string AtCommand { get; set; } = string.Empty;
    // 额外的提示词或参数
    public string PromptAppend { get; set; } = string.Empty;
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["CmdName"] = CmdName,
            ["at"] = AtCommand,
            ["promptAppend"] = PromptAppend
        };
    }
}

