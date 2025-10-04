using System.Collections.Generic;
using System.IO;
using BepInEx;
#nullable enable

namespace PeakChatOps.core;

/// <summary>
/// 自定义本地化文本类，用于加载和管理 Mod 的本地化字符串
/// </summary>
public static class PLocalizedText
{
    /// <summary>
    /// 存储本地化文本的字典：key -> 翻译后的文本
    /// 仅存储当前语言的文本，节省内存
    /// </summary>
    private static Dictionary<string, string> _textTable = new();

    /// <summary>
    /// 当前语言索引（对应CSV列）
    /// </summary>
    private static int _currentLangIndex = -1;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    private static bool _initialized;

    /// <summary>
    /// 初始化本地化系统，加载 Localization.csv
    /// </summary>
    public static void Init()
    {
        if (_initialized)
        {
            PeakChatOpsPlugin.Logger.LogWarning("[PLocalizedText] Already initialized, skipping.");
            return;
        }

        var csvPath = Path.Combine(PeakChatOpsPlugin.PluginPath, "Localization.csv");

        if (!File.Exists(csvPath))
        {
            PeakChatOpsPlugin.Logger.LogError($"[PLocalizedText] CSV file not found: {csvPath}");
            _initialized = true; // 标记为已初始化，避免重复尝试
            return;
        }

        try
        {
            LoadLocalizationCsv(csvPath);
            _initialized = true;
            PeakChatOpsPlugin.Logger.LogInfo($"[PLocalizedText] Loaded {_textTable.Count} localization entries for language index {_currentLangIndex}");
        }
        catch (System.Exception ex)
        {
            PeakChatOpsPlugin.Logger.LogError($"[PLocalizedText] Failed to load CSV: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载 CSV 文件并解析当前语言的文本
    /// </summary>
    private static void LoadLocalizationCsv(string csvPath)
    {
        var lines = File.ReadAllLines(csvPath);
        if (lines.Length == 0)
        {
            PeakChatOpsPlugin.Logger.LogWarning("[PLocalizedText] CSV file is empty.");
            return;
        }

        // 第一行是语言列表
        var header = ParseCsvLine(lines[0]);
        
        // 确定当前语言索引
        _currentLangIndex = GetCurrentLanguageIndex(header);
        
        if (_currentLangIndex < 0 || _currentLangIndex >= header.Count)
        {
            PeakChatOpsPlugin.Logger.LogWarning($"[PLocalizedText] Invalid language index: {_currentLangIndex}, defaulting to 0 (English)");
            _currentLangIndex = 0;
        }

        // 解析后续行
        for (int i = 1; i < lines.Length; i++)
        {
            var columns = ParseCsvLine(lines[i]);
            if (columns.Count <= _currentLangIndex)
            {
                PeakChatOpsPlugin.Logger.LogWarning($"[PLocalizedText] Line {i + 1} has insufficient columns, skipping.");
                continue;
            }

            var key = columns[0];
            var value = columns[_currentLangIndex];

            if (!string.IsNullOrEmpty(key) && key != "ENDLINE")
            {
                _textTable[key] = value;
            }
        }
    }

    /// <summary>
    /// 获取当前游戏语言对应的CSV列索引
    /// </summary>
    private static int GetCurrentLanguageIndex(List<string> header)
    {
        // 从游戏的本地化系统获取当前语言
        string currentLanguage;
        try
        {
            currentLanguage = LocalizedText.GetText("CURRENT_LANGUAGE");
        }
        catch
        {
            // 如果游戏本地化系统未加载，默认使用英语
            PeakChatOpsPlugin.Logger.LogWarning("[PLocalizedText] Could not get current language from game, defaulting to English");
            return 1; // English 列索引通常是 1
        }

        // 在 header 中查找匹配的语言列
        for (int i = 0; i < header.Count; i++)
        {
            if (header[i].Equals(currentLanguage, System.StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        // 如果未找到，尝试匹配简体中文
        if (currentLanguage.Contains("中文") || currentLanguage.Contains("Chinese"))
        {
            for (int i = 0; i < header.Count; i++)
            {
                if (header[i].Contains("简体中文") || header[i].Contains("Chinese"))
                {
                    return i;
                }
            }
        }

        // 默认返回英语列（通常是索引 1）
        PeakChatOpsPlugin.Logger.LogWarning($"[PLocalizedText] Language '{currentLanguage}' not found in header, defaulting to English");
        return 1;
    }

    /// <summary>
    /// 解析 CSV 行，处理引号和逗号
    /// </summary>
    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var currentField = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        // 添加最后一个字段
        result.Add(currentField.ToString());

        return result;
    }

    /// <summary>
    /// 获取本地化文本
    /// </summary>
    /// <param name="key">本地化键</param>
    /// <param name="fallback">如果未找到键，返回的默认值（默认为键本身）</param>
    /// <returns>本地化后的文本</returns>
    public static string GetText(string key, string? fallback = null)
    {
        if (!_initialized)
        {
            PeakChatOpsPlugin.Logger.LogWarning("[PLocalizedText] Not initialized yet, returning key as-is");
            return fallback ?? key;
        }

        if (_textTable.TryGetValue(key, out var value))
        {
            return value;
        }

        // 如果找不到，记录警告并返回 fallback 或键本身
        PeakChatOpsPlugin.Logger.LogWarning($"[PLocalizedText] Key '{key}' not found in localization table");
        return fallback ?? key;
    }

    /// <summary>
    /// 检查某个键是否存在
    /// </summary>
    public static bool HasKey(string key)
    {
        return _textTable.ContainsKey(key);
    }

    /// <summary>
    /// 获取所有已加载的键
    /// </summary>
    public static IEnumerable<string> GetAllKeys()
    {
        return _textTable.Keys;
    }

    /// <summary>
    /// 重新加载本地化表（用于调试或热更新）
    /// </summary>
    public static void Reload()
    {
        _initialized = false;
        _textTable.Clear();
        Init();
    }
}
