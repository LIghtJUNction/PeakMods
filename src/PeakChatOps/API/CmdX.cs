using System.Collections.Generic;

namespace PeakChatOps.API;

public class Cmd
{
    public string Name = string.Empty;
    public string Description = string.Empty;
    public string HelpInfo = string.Empty;
    public System.Func<string[], string> Handler = _ => "";

    public Cmd() { }
}


public static class CmdX
{
    /// <summary>
    /// 命令前缀，可动态设置，默认为 "/"
    /// </summary>
    public static string Prefix { get; set; } = "/";
    /// <summary>
    /// 主动调用所有命令注册类的 EnsureRegistered，确保命令被注册。
    /// </summary>
    public static void EnsureRegistered()
    {
        // 自动发现并注册所有实现ICmdProvider的命令类（约定命名空间）
        var asm = typeof(CmdX).Assembly;
        foreach (var type in asm.GetTypes())
        {
            if (type.Namespace == "PeakChatOps.API.Commands" && typeof(ICmdProvider).IsAssignableFrom(type) && !type.IsAbstract)
            {
                var instance = System.Activator.CreateInstance(type) as ICmdProvider;
                instance?.Register();
            }
        }
    }


    private static readonly Dictionary<string, Cmd> _cmds = new();

    public static void Register(Cmd cmd)
    {
        if (cmd == null || string.IsNullOrWhiteSpace(cmd.Name) || cmd.Handler == null)
            return;
        _cmds[cmd.Name.ToLower()] = cmd;
    }

    public static string exec(string commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine)) return "请输入命令。";
        var parts = commandLine.Trim().Split(' ');
        if (parts.Length == 0) return "无效命令。";
        var cmdName = parts[0].StartsWith(Prefix)
            ? parts[0].Substring(Prefix.Length)
            : parts[0];
        cmdName = cmdName.ToLower();
        var args = parts.Length > 1 ? parts.SubArray(1, parts.Length - 1) : new string[0];
        if (_cmds.TryGetValue(cmdName, out var cmd))
            return cmd.Handler(args);
        return $"未知命令: {cmdName}，输入{Prefix}help 查看所有命令。";
    }

// .NET Standard 2.0 没有内置数组切片，这里提供扩展方法
}

public static class ArrayExtensions
{
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        var result = new T[length];
        System.Array.Copy(data, index, result, 0, length);
        return result;
    }
}
// 命令自动注册接口
public interface ICmdProvider
{
    void Register();
}