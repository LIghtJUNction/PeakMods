// 命令自动注册接口（如未全局定义则本地定义，防止找不到类型报错）

using System.Collections.Generic;
using PeakChatOps.API;

namespace PeakChatOps.Commands;

public class HelpCommand : PCmd
{
    public HelpCommand()
    {
        Name = "help";
        Description = "显示所有可用命令及说明";
        HelpInfo = "用法: /help\n列出所有命令和用法说明。";
        Handler = args => Help();
    }

    public static string Help()
    {
        var cmdsField = typeof(CmdX).GetField("_cmds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var dict = cmdsField?.GetValue(null) as IDictionary<string, Cmd>;
        if (dict == null || dict.Count == 0) return "无可用命令。";
        var lines = new List<string>();
        foreach (var cmd in dict.Values)
            lines.Add($"/{cmd.Name} - {cmd.Description}{(string.IsNullOrWhiteSpace(cmd.HelpInfo) ? "" : "\n  " + cmd.HelpInfo)}");
        return string.Join("\n", lines);
    }
}

