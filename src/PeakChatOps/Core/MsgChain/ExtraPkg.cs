using System;
using System.Collections.Generic;

namespace PeakChatOps.Core.MsgChain;

// 抽象基类，所有 Extra 包都应继承自此类
public abstract class ExtraPackage
{
    public abstract string CmdName { get; }
    public abstract Dictionary<string, object> ToDictionary();
}

// 所有 Extra 包的集中定义
public class PingExtra : ExtraPackage
{
    public override string CmdName { get; } = "ping";
    public int UserActorNumber { get; set; }
    public int[] TargetActors { get; set; } = Array.Empty<int>();
    public override Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["CmdName"] = CmdName,
            ["UserActorNumber"] = UserActorNumber,
            ["TargetActors"] = TargetActors
        };
    }
}
public class WhisperExtra : ExtraPackage
{
    // 兼容 PingExtra 风格的结构（可包含命令名与发起者 actor）
    public override string CmdName { get; } = "whisper";
    public int UserActorNumber { get; set; }
    // 直发 actor 列表
    public int[] TargetActors { get; set; } = Array.Empty<int>();
    // 或者以名字为准的目标列表
    public string[] TargetNames { get; set; } = Array.Empty<string>();
    public override Dictionary<string, object> ToDictionary()
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
public class AIExtra : ExtraPackage
{
    // 命令名（默认 ai）
    public override string CmdName { get; } = "ai";
    // 可选指令，例如 "clear"、"send" 等
    public string AtCommand { get; set; } = string.Empty;
    // 额外的提示词或参数
    public string PromptAppend { get; set; } = string.Empty;
    public override Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["CmdName"] = CmdName,
            ["at"] = AtCommand,
            ["promptAppend"] = PromptAppend
        };
    }
}

