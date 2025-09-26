
using System;
using System.Collections.Generic;
using PeakChatOps.API;

namespace PeakChatOps.Core;

/// <summary>
/// AI 聊天上下文记录器，支持追加消息、裁剪历史、构建上下文字符串。
/// </summary>
public class AIChatContextLogger
{
    /// <summary>
    /// 导出当前聊天记录到本地文件（覆盖上一次的文件）
    /// </summary>
    /// <param name="filePath">要保存的完整文件路径</param>
    public void ExportHistoryToFile(string filePath)
    {
        try
        {
            var lines = new List<string>();
            foreach (var msg in _history)
            {
                lines.Add($"[{msg.Role}] {msg.Sender}({msg.UserId}): {msg.Message}");
            }
            System.IO.File.WriteAllLines(filePath, lines);
        }
        catch (System.Exception ex)
        {
            PeakChatOpsPlugin.Logger.LogError($"[AIContext] 导出聊天记录到文件失败: {filePath}，错误信息: {ex.Message}");
        }
    }
   /// <summary>
    /// 记录一条系统消息到上下文
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="sender">发送者名称,仅用于显示在UI上</param>
    /// <param name="userId">用户ID</param>
    public void LogSystem(string message, string sender = "system", string userId = "system")
    {
        Add(new AIChatMessageEvent(sender, message, userId, AIChatRole.system));
    }


    /// <summary>
    /// 记录一条用户消息到上下文
    /// </summary>
    public void LogUser(string message, string sender = "PlayerName", string userId = "uuid-user")
    {
        Add(new AIChatMessageEvent(sender, message, userId, AIChatRole.user));
    }

    /// <summary>
    /// 记录一条AI助手消息到上下文
    /// </summary>
    public void LogAssistant(string message, string sender = "ModelName", string userId = "assistant")
    {
        Add(new AIChatMessageEvent(sender, message, userId, AIChatRole.assistant));
    }

    /// <summary>
    /// 全局唯一实例（可用于全局上下文管理）
    /// </summary>
    public static AIChatContextLogger Instance { get; private set; }

    /// <summary>
    /// 创建并设置全局唯一实例
    /// </summary>
    public static void CreateGlobalInstance(int maxHistory = 30)
    {
        Instance = new AIChatContextLogger(maxHistory);
    }

    private readonly LinkedList<AIChatMessageEvent> _history = new();
    private readonly List<Dictionary<string, object>> _messages = new();
    private int _maxHistory;

    public AIChatContextLogger(int maxHistory = 30)
    {
        _maxHistory = maxHistory > 0 ? maxHistory : 30;
    }

    /// <summary>
    /// 运行时同步配置的最大历史条数
    /// </summary>
    public void SyncMaxHistoryFromConfig()
    {
        int configVal = 30;
        try { configVal = PeakChatOpsPlugin.aiContextMaxCount?.Value ?? 30; } catch { }
        if (configVal > 0) _maxHistory = configVal;
    }

    /// <summary>
    /// 追加一条AI聊天消息到上下文
    /// </summary>
    public void Add(AIChatMessageEvent msg)
    {
        _history.AddLast(msg);
        _messages.Add(new Dictionary<string, object>
        {
            ["role"] = msg.Role.ToString(),
            ["content"] = msg.Message,
            ["name"] = msg.UserId
        });
        // 保证两者长度一致
        while (_history.Count > _maxHistory)
        {
            _history.RemoveFirst();
        }
        while (_messages.Count > _history.Count)
        {
            _messages.RemoveAt(0);
        }
    }

    /// <summary>
    /// 获取当前上下文历史（OpenAI兼容消息字典列表，适合直接用作messages参数）
    /// </summary>
    public List<Dictionary<string, object>> GetHistory()
    {
        return new List<Dictionary<string, object>>(_messages);
    }

    /// <summary>
    /// 构建OpenAI兼容的消息字典列表（历史+当前用户输入）
    /// </summary>
    /// <param name="userPrompt">当前用户输入</param>
    /// <param name="userName">用户名（可选）</param>
    /// <returns>OpenAI chat/messages格式的历史+当前输入</returns>
    public List<Dictionary<string, object>> BuildContextMessages(string userPrompt, string userName = "玩家")
    {
        // 历史消息深拷贝，防止外部修改
        var messages = new List<Dictionary<string, object>>(_messages);
        // 追加当前用户输入
        messages.Add(new Dictionary<string, object>
        {
            ["role"] = "user",
            ["content"] = userPrompt,
            ["name"] = userName
        });
        return messages;
    }

    /// <summary>
    /// 清空历史
    /// </summary>
    public void Clear()
    {
        _history.Clear();
        _messages.Clear();
    }

    /// <summary>
    /// 解析OpenAI completions响应，提取text字段（使用Newtonsoft.Json）
    /// </summary>
    public static string ParseOpenAICompletionResponse(string response)
    {
        try
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(response);
            var choices = obj["choices"] as Newtonsoft.Json.Linq.JArray;
            if (choices != null && choices.Count > 0)
            {
                foreach (var choice in choices)
                {
                    // 1. chat 格式
                    var content = choice["message"]?["content"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(content))
                        return content.Trim();
                    // 2. text 字段（老接口）
                    var text = choice["text"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                        return text.Trim();
                }
            }
            // 3. 兜底：输出原始choices内容，便于调试
            return "(AI无回复, No response) 原始: " + (choices != null ? choices.ToString() : "null");
        }
        catch (Exception ex)
        {
            return $"(AI响应解析失败, Parse error: {ex.Message})";
        }
    }

}
