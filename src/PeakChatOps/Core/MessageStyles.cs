namespace PeakChatOps.Core;

/// <summary>
/// 统一的消息显示样式配置
/// </summary>
public static class MessageStyles
{
    // ===== 颜色常量 =====
    
    /// <summary>用户输入/命令 - 蓝色</summary>
    public const string ColorCommand = "#5BA3F5";
    
    /// <summary>AI 回复 - 青色</summary>
    public const string ColorAI = "#00D9FF";
    
    /// <summary>成功/确认 - 绿色</summary>
    public const string ColorSuccess = "#4ADE80";
    
    /// <summary>错误/失败 - 红色</summary>
    public const string ColorError = "#F87171";
    
    /// <summary>警告 - 黄色</summary>
    public const string ColorWarning = "#FBBF24";
    
    /// <summary>系统消息 - 紫色</summary>
    public const string ColorSystem = "#A78BFA";
    
    /// <summary>翻译 - 橙色</summary>
    public const string ColorTranslate = "#FB923C";
    
    /// <summary>死亡状态 - 灰色</summary>
    public const string ColorDead = "#9CA3AF";
    
    /// <summary>本地玩家消息 - 翡翠绿</summary>
    public const string ColorPlayerLocal = "#34D399";
    
    /// <summary>远程玩家消息 - 浅灰白</summary>
    public const string ColorPlayerRemote = "#E5E7EB";
    
    /// <summary>次要信息 - 中灰色</summary>
    public const string ColorSecondary = "#9CA3AF";
    
    /// <summary>Reasoning 信息 - 深灰色</summary>
    public const string ColorReasoning = "#6B7280";
    
    /// <summary>普通文本 - 浅灰白（默认内容颜色）</summary>
    public const string ColorText = "#D1D5DB";
    
    /// <summary>AI 回复内容 - 淡青色</summary>
    public const string ColorTextAI = "#7DD3FC";
    
    /// <summary>命令内容 - 淡蓝色</summary>
    public const string ColorTextCommand = "#93C5FD";
    
    /// <summary>成功内容 - 淡绿色</summary>
    public const string ColorTextSuccess = "#86EFAC";
    
    /// <summary>错误内容 - 淡红色</summary>
    public const string ColorTextError = "#FCA5A5";
    
    /// <summary>警告内容 - 淡黄色</summary>
    public const string ColorTextWarning = "#FDE047";
    
    /// <summary>系统内容 - 淡紫色</summary>
    public const string ColorTextSystem = "#C4B5FD";
    
    /// <summary>翻译内容 - 淡橙色</summary>
    public const string ColorTextTranslate = "#FDBA74";

    // ===== 字体大小常量 =====
    
    /// <summary>标签字体大小</summary>
    public const string SizeLabel = "90%";
    
    /// <summary>次要信息字体大小</summary>
    public const string SizeSecondary = "85%";
    
    /// <summary>Reasoning 字体大小</summary>
    public const string SizeReasoning = "80%";

    // ===== 样式模板方法 =====
    
    /// <summary>生成带颜色和大小的标签</summary>
    public static string Label(string text, string color, string size = SizeLabel)
    {
        return $"<color={color}><size={size}>{text}</size></color>";
    }
    
    /// <summary>生成命令标签</summary>
    public static string CommandLabel(string command)
    {
        return Label($"[{command}]", ColorCommand);
    }
    
    /// <summary>生成 AI 标签</summary>
    public static string AILabel(string modelName = "AI")
    {
        return Label($"[{modelName}]", ColorAI);
    }
    
    /// <summary>生成成功标签</summary>
    public static string SuccessLabel(string text = "Success")
    {
        return Label($"[{text}]", ColorSuccess);
    }
    
    /// <summary>生成错误标签</summary>
    public static string ErrorLabel(string text = "Error")
    {
        return Label($"<b>[{text}]</b>", ColorError);
    }
    
    /// <summary>生成警告标签</summary>
    public static string WarningLabel(string text = "Warning")
    {
        return Label($"[{text}]", ColorWarning);
    }
    
    /// <summary>生成系统标签</summary>
    public static string SystemLabel(string text = "System")
    {
        return Label($"[{text}]", ColorSystem);
    }
    
    /// <summary>生成翻译标签</summary>
    public static string TranslateLabel()
    {
        return Label("[翻译]", ColorTranslate);
    }
    
    /// <summary>生成玩家标签</summary>
    public static string PlayerLabel(string playerName, bool isLocal, bool isDead = false)
    {
        var color = isDead ? ColorDead : (isLocal ? ColorPlayerLocal : ColorPlayerRemote);
        var deadMark = isDead ? " <b><color=" + ColorError + ">(DEAD)</color></b>" : "";
        return $"<color={color}><size={SizeLabel}>[{playerName}]</size></color>{deadMark}";
    }
    
    /// <summary>生成次要信息文本</summary>
    public static string SecondaryText(string text)
    {
        return $"<color={ColorSecondary}><size={SizeSecondary}>{text}</size></color>";
    }
    
    /// <summary>生成 Reasoning 文本</summary>
    public static string ReasoningText(string text)
    {
        return $"<color={ColorReasoning}><size={SizeReasoning}>{text}</size></color>";
    }
    
    // ===== 内容着色方法 =====
    
    /// <summary>为普通文本内容着色</summary>
    public static string ColoredContent(string content, string color = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;
        
        var textColor = color ?? ColorText;
        return $"<color={textColor}>{content}</color>";
    }
    
    /// <summary>为 AI 回复内容着色</summary>
    public static string AIContent(string content)
    {
        return ColoredContent(content, ColorTextAI);
    }
    
    /// <summary>为命令内容着色</summary>
    public static string CommandContent(string content)
    {
        return ColoredContent(content, ColorTextCommand);
    }
    
    /// <summary>为成功内容着色</summary>
    public static string SuccessContent(string content)
    {
        return ColoredContent(content, ColorTextSuccess);
    }
    
    /// <summary>为错误内容着色</summary>
    public static string ErrorContent(string content)
    {
        return ColoredContent(content, ColorTextError);
    }
    
    /// <summary>为警告内容着色</summary>
    public static string WarningContent(string content)
    {
        return ColoredContent(content, ColorTextWarning);
    }
    
    /// <summary>为系统内容着色</summary>
    public static string SystemContent(string content)
    {
        return ColoredContent(content, ColorTextSystem);
    }
    
    /// <summary>为翻译内容着色</summary>
    public static string TranslateContent(string content)
    {
        return ColoredContent(content, ColorTextTranslate);
    }
    
    /// <summary>为玩家消息内容着色</summary>
    public static string PlayerContent(string content, bool isLocal = false)
    {
        var color = isLocal ? ColorPlayerLocal : ColorText;
        return ColoredContent(content, color);
    }
}
