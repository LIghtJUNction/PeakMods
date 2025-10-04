
using System;
using System.Collections.Generic;
using PeakChatOps.API;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

#nullable enable
namespace PeakChatOps.Core;

/// <summary>
/// AI 聊天上下文记录器，支持追加消息、裁剪历史、构建上下文字符串。
/// </summary>
public class AIChatContextLogger
{

    /// <summary>
    /// 消息条目包装类，包含消息和固定标记
    /// </summary>
    private sealed class MessageEntry
    {
        public AIChatMessageEvent Message { get; }
        public Dictionary<string, object> Payload { get; }
        public bool IsPinned { get; }

        public MessageEntry(AIChatMessageEvent message, Dictionary<string, object> payload, bool isPinned)
        {
            Message = message;
            Payload = payload;
            IsPinned = isPinned;
        }
    }

    /// <summary>
    /// 导出当前聊天记录到本地文件（覆盖上一次的文件）
    /// </summary>
    /// <param name="filePath">要保存的完整文件路径</param>
    public void ExportHistoryToFile(string filePath)
    {
        try
        {
            var lines = new List<string>();
            foreach (var entry in _history)
            {
                var msg = entry.Message;
                var pinMark = entry.IsPinned ? " [PINNED]" : "";
                lines.Add($"[{msg.Role}] {msg.Sender}({msg.UserId}): {msg.Message}{pinMark}");
            }
            System.IO.File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
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
    /// <param name="pinned">是否固定此消息，固定后不参与历史裁剪</param>
    public void LogSystem(string message, string sender = "system", string userId = "system", bool pinned = false)
    {
        Add(new AIChatMessageEvent(sender, message, userId, AIChatRole.system), pinned);
    }


    /// <summary>
    /// 记录一条用户消息到上下文
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="sender">发送者名称</param>
    /// <param name="userId">用户ID</param>
    /// <param name="pinned">是否固定此消息，固定后不参与历史裁剪</param>
    public void LogUser(string message, string sender = "PlayerName", string userId = "uuid-user", bool pinned = false)
    {
        Add(new AIChatMessageEvent(sender, message, userId, AIChatRole.user), pinned);
    }

    /// <summary>
    /// 记录一条AI助手消息到上下文
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="sender">发送者名称</param>
    /// <param name="userId">用户ID</param>
    /// <param name="pinned">是否固定此消息，固定后不参与历史裁剪</param>
    public void LogAssistant(string message, string sender = "ModelName", string userId = "assistant", bool pinned = false)
    {
        Add(new AIChatMessageEvent(sender, message, userId, AIChatRole.assistant), pinned);
    }

    /// <summary>
    /// 全局唯一实例（可用于全局上下文管理）
    /// </summary>
    public static AIChatContextLogger? Instance { get; private set; }

    /// <summary>
    /// 创建并设置全局唯一实例
    /// </summary>
    public static void CreateGlobalInstance(int maxHistory = 30)
    {
        Instance = new AIChatContextLogger(maxHistory);
    }

    private readonly LinkedList<MessageEntry> _history = new();
    private readonly List<MessageEntry> _messages = new();
    private int _maxHistory;
    private int _nonPinnedCount; // 跟踪非固定消息数量

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
            try
            {
                configVal = PeakChatOpsPlugin.config.AiContextMaxCount?.Value ?? 30;
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogDebug($"[AIContext] SyncMaxHistoryFromConfig failed to read config: {ex.Message}");
            }
            if (configVal > 0)
            {
                _maxHistory = configVal;
                TrimExcess(); // 同步后立即裁剪
            }
        }

    /// <summary>
    /// 追加一条AI聊天消息到上下文
    /// </summary>
    /// <param name="msg">消息事件</param>
    /// <param name="pinned">是否固定此消息，固定后不参与历史裁剪</param>
    public void Add(AIChatMessageEvent msg, bool pinned = false)
    {
        var payload = new Dictionary<string, object>
        {
            ["role"] = msg.Role.ToString(),
            ["content"] = msg.Message,
            ["name"] = msg.UserId
        };

        var entry = new MessageEntry(msg, payload, pinned);
        _history.AddLast(entry);
        _messages.Add(entry);

        if (!pinned)
        {
            _nonPinnedCount++;
        }

        // 只裁剪非固定消息
        TrimExcess();
    }

    /// <summary>
    /// 裁剪多余的非固定消息，保持非固定消息数量不超过 _maxHistory
    /// </summary>
    private void TrimExcess()
    {
        // 只有当非固定消息数超过限制时才裁剪
        while (_nonPinnedCount > _maxHistory)
        {
            var node = _history.First;
            while (node != null)
            {
                if (!node.Value.IsPinned)
                {
                    _history.Remove(node);
                    _messages.Remove(node.Value);
                    _nonPinnedCount--;
                    break;
                }
                node = node.Next;
            }
        }
    }

    /// <summary>
    /// 构建OpenAI兼容的消息字典列表（返回已记录的历史消息）
    /// 注意：调用此方法前应先调用 LogUser 记录用户输入
    /// </summary>
    /// <param name="userPrompt">当前用户输入（已废弃，保留仅为兼容旧代码）</param>
    /// <param name="userName">用户名（已废弃，保留仅为兼容旧代码）</param>
    /// <returns>OpenAI chat/messages格式的历史消息列表</returns>
    public List<Dictionary<string, object>> BuildContextMessages(string userPrompt, string userName = "玩家")
    {
        // 历史消息浅拷贝（复制payload引用），防止外部修改列表本身
        var messages = new List<Dictionary<string, object>>(_messages.Count);
        foreach (var entry in _messages)
        {
            messages.Add(entry.Payload);
        }
        
        DevLog.UI($"[AIContext] BuildContextMessages: total messages count = {messages.Count}\n 最近4条: {string.Join(", ", messages.TakeLast(4).Select(m => m["content"]))}");
        return messages;
    }

    /// <summary>
    /// 清空历史
    /// </summary>
    public void Clear()
    {
        _history.Clear();
        _messages.Clear();
        _nonPinnedCount = 0;
    }

    /// <summary>
    /// 解析 OpenAI completions 响应，返回 content 与 reasoning（若存在）
    /// 返回的元组中 content 和 reasoning 都可能为 null。
    /// 兼顾 chat 格式（message.content、message.reasoning）与老接口（text）。
    /// </summary>
    public static (string? content, string? reasoning) ParseOpenAICompletionResponseWithReasoning(string response)
    {
        try
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(response);
            var choices = obj["choices"] as Newtonsoft.Json.Linq.JArray;
            if (choices != null && choices.Count > 0)
            {
                foreach (var choice in choices)
                {
                    // 优先尝试 message.content
                    var content = choice["message"]?["content"]?.ToString();
                    // 常见命名可能为 reasoning、thoughts 等，这里优先尝试 message.reasoning，然后兼容 thoughts
                    var reasoning = choice["message"]?["reasoning"]?.ToString() ?? choice["message"]?["thoughts"]?.ToString();
                    // 兼容老接口 text 字段
                    var text = choice["text"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(content))
                        return (content.Trim(), string.IsNullOrWhiteSpace(reasoning) ? null : reasoning.Trim());
                    if (!string.IsNullOrWhiteSpace(reasoning))
                        return (null, reasoning.Trim());
                    if (!string.IsNullOrWhiteSpace(text))
                        return (text.Trim(), null);
                }
            }
            return (null, null);
        }
        catch (Exception)
        {
            return (null, null);
        }
    }

    /// <summary>
    /// 构建房间环境信息的一行提示词，包含房间名、玩家列表、房主与可选房间种子等（便于 AI 上下文使用）。
    /// 示例："room[MyRoom] players=3 [Alice|id=uid1|actor=1|local=false|master=true;Bob|id=uid2|actor=2|local=true|master=false] host=Alice,mapSeed=2025-10-04,time=1696464000"
    /// </summary>
    /// <param name="includeTimestamp">是否包含当前 UTC 时间戳</param>
    /// <returns>单行房间提示词</returns>
    public string BuildRoomEnvironmentPrompt(bool includeTimestamp = true)
    {
        try
        {
            var room = PhotonNetwork.CurrentRoom;
            var roomName = room?.Name ?? "unknown";
            var players = PhotonNetwork.PlayerList ?? Array.Empty<Photon.Realtime.Player>();
            var local = PhotonNetwork.LocalPlayer;
            var master = PhotonNetwork.MasterClient;

            var parts = new List<string>();
            foreach (var p in players)
            {
                var nick = !string.IsNullOrEmpty(p.NickName) ? p.NickName : $"Player{p.ActorNumber}";
                var userId = !string.IsNullOrEmpty(p.UserId) ? p.UserId : "unknown";
                var actor = p.ActorNumber;
                var isLocal = p.Equals(local);
                var isMaster = p.Equals(master);
                parts.Add($"{nick}|id={userId}|actor={actor}|local={isLocal.ToString().ToLower()}|master={isMaster.ToString().ToLower()}");
            }

            string playersStr = parts.Count > 0 ? string.Join(";", parts) : string.Empty;
            string seedVal = "none";
            try
            {
                if (room?.CustomProperties != null && room.CustomProperties.ContainsKey("seed"))
                    seedVal = room.CustomProperties["seed"]?.ToString() ?? "none";
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogDebug($"[AIContext] BuildRoomEnvironmentPrompt read seed: {ex.Message}");
            }

            var hostName = master?.NickName ?? "unknown";
            var timestamp = includeTimestamp ? $",time={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}" : string.Empty;

            var prompt = $"room[{roomName}] players={players.Length} [{playersStr}] host={hostName},mapSeed={seedVal}{timestamp}";
            return prompt;
        }
        catch (Exception ex)
        {
            PeakChatOpsPlugin.Logger.LogError($"[AIContext] BuildRoomEnvironmentPrompt failed: {ex.Message}");
            return "room=unknown players=0";
        }
    }


}
