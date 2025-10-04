using System;
using System.Collections.Generic;
using System.Linq;
using PeakChatOps.Commands;
using PeakChatOps.Core;
using Photon.Pun;

namespace PeakChatOps.UI;

/// <summary>
/// 自动补全辅助类，用于匹配命令和玩家名
/// </summary>
public static class AutoCompleteHelper
{
    /// <summary>
    /// 补全项类型
    /// </summary>
    public enum SuggestionType
    {
        Command,
        PlayerName
    }

    /// <summary>
    /// 补全建议项
    /// </summary>
    public class Suggestion
    {
        public string Text { get; set; }
        public SuggestionType Type { get; set; }
        public string Description { get; set; }

        public Suggestion(string text, SuggestionType type, string description = "")
        {
            Text = text;
            Type = type;
            Description = description;
        }

        /// <summary>
        /// 获取显示文本（带颜色和图标）
        /// </summary>
        public string GetDisplayText()
        {
            var icon = Type == SuggestionType.Command ? "⚡" : "👤";
            var color = Type == SuggestionType.Command ? MessageStyles.ColorCommand : MessageStyles.ColorPlayerLocal;
            var typeLabel = Type == SuggestionType.Command ? "命令" : "玩家";
            
            if (!string.IsNullOrEmpty(Description))
            {
                return $"<color={color}>{icon} {Text}</color> {MessageStyles.SecondaryText($"[{typeLabel}] {Description}")}";
            }
            return $"<color={color}>{icon} {Text}</color> {MessageStyles.SecondaryText($"[{typeLabel}]")}";
        }
    }

    /// <summary>
    /// 获取所有可用的补全建议
    /// </summary>
    public static List<Suggestion> GetAllSuggestions()
    {
        var suggestions = new List<Suggestion>();

        // 添加命令
        try
        {
            if (Cmdx.CommandMetas != null)
            {
                foreach (var cmd in Cmdx.CommandMetas)
                {
                    if (cmd != null && !string.IsNullOrEmpty(cmd.Name))
                    {
                        var description = cmd.Description ?? "";
                        suggestions.Add(new Suggestion(cmd.Name, SuggestionType.Command, description));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DevLog.File($"[AutoComplete] Failed to load commands: {ex.Message}");
        }

        // 添加玩家名
        try
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
            {
                var players = PhotonNetwork.PlayerList;
                if (players != null)
                {
                    foreach (var player in players)
                    {
                        if (player != null && !string.IsNullOrEmpty(player.NickName))
                        {
                            var isLocal = player.Equals(PhotonNetwork.LocalPlayer);
                            var description = isLocal ? "你" : $"Actor#{player.ActorNumber}";
                            suggestions.Add(new Suggestion(player.NickName, SuggestionType.PlayerName, description));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DevLog.File($"[AutoComplete] Failed to load players: {ex.Message}");
        }

        return suggestions;
    }

    /// <summary>
    /// 根据输入文本匹配建议
    /// </summary>
    /// <param name="input">用户输入的文本</param>
    /// <param name="maxResults">最大返回结果数</param>
    /// <returns>匹配的建议列表</returns>
    public static List<Suggestion> GetMatchingSuggestions(string input, int maxResults = 10)
    {
        DevLog.File($"[AutoComplete] GetMatchingSuggestions called with input: '{input}', maxResults: {maxResults}");
        
        if (string.IsNullOrWhiteSpace(input))
        {
            DevLog.File("[AutoComplete] Input is null or whitespace, returning empty list");
            return new List<Suggestion>();
        }

        var allSuggestions = GetAllSuggestions();
        DevLog.File($"[AutoComplete] GetAllSuggestions returned {allSuggestions.Count} total suggestions");
        
        var query = input.Trim();

        // 1. 精确前缀匹配（优先级最高）
        var exactPrefixMatches = allSuggestions
            .Where(s => s.Text.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.Text.Length)
            .ThenBy(s => s.Text)
            .ToList();

        // 2. 包含匹配（优先级次之）
        var containsMatches = allSuggestions
            .Where(s => !s.Text.StartsWith(query, StringComparison.OrdinalIgnoreCase) && 
                        s.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            .OrderBy(s => s.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase))
            .ThenBy(s => s.Text.Length)
            .ToList();

        // 3. 模糊匹配（优先级最低）
        var fuzzyMatches = allSuggestions
            .Where(s => s.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0 &&
                        IsFuzzyMatch(s.Text, query))
            .OrderByDescending(s => CalculateFuzzyScore(s.Text, query))
            .ToList();

        // 合并结果
        var results = new List<Suggestion>();
        results.AddRange(exactPrefixMatches);
        results.AddRange(containsMatches);
        results.AddRange(fuzzyMatches);

        // 去重并限制数量
        return results
            .GroupBy(s => s.Text.ToLowerInvariant())
            .Select(g => g.First())
            .Take(maxResults)
            .ToList();
    }

    /// <summary>
    /// 检查是否模糊匹配（字符顺序匹配）
    /// </summary>
    private static bool IsFuzzyMatch(string text, string query)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(query))
            return false;

        text = text.ToLowerInvariant();
        query = query.ToLowerInvariant();

        int queryIndex = 0;
        foreach (char c in text)
        {
            if (c == query[queryIndex])
            {
                queryIndex++;
                if (queryIndex >= query.Length)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 计算模糊匹配分数（分数越高越匹配）
    /// </summary>
    private static int CalculateFuzzyScore(string text, string query)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(query))
            return 0;

        text = text.ToLowerInvariant();
        query = query.ToLowerInvariant();

        int score = 0;
        int queryIndex = 0;
        int consecutiveMatches = 0;

        foreach (char c in text)
        {
            if (queryIndex < query.Length && c == query[queryIndex])
            {
                queryIndex++;
                consecutiveMatches++;
                score += 10 + consecutiveMatches; // 连续匹配加分
            }
            else
            {
                consecutiveMatches = 0;
            }
        }

        return score;
    }

    /// <summary>
    /// 从输入文本中提取最后一个词（用于补全）
    /// </summary>
    public static string GetLastWord(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var trimmed = input.TrimEnd();
        var lastSpaceIndex = trimmed.LastIndexOf(' ');
        
        if (lastSpaceIndex < 0)
            return trimmed;
        
        return trimmed.Substring(lastSpaceIndex + 1);
    }

    /// <summary>
    /// 应用补全（替换最后一个词）
    /// </summary>
    public static string ApplyCompletion(string input, string completion)
    {
        if (string.IsNullOrWhiteSpace(input))
            return completion;

        var trimmed = input.TrimEnd();
        var lastSpaceIndex = trimmed.LastIndexOf(' ');
        
        if (lastSpaceIndex < 0)
            return completion;
        
        return trimmed.Substring(0, lastSpaceIndex + 1) + completion;
    }
}
