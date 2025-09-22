using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;

#nullable enable
namespace PeakChatOps.API.Commands;

public class Cmdx : PCmd
{
    // 用于高效查找命令
    public static Dictionary<string, PCmd> _PCmdDict = new Dictionary<string, PCmd>(StringComparer.OrdinalIgnoreCase);
    
    public Cmdx()
    {
        Name = "cmdx";
        Description = "命令执行器";
        HelpInfo = "用法: /cmdx <cmd> <args>\n执行指定的命令。";
        Handler = CmdxHandler;
    }

    public static string CmdxHandler(string[] args)
    {
        // 查找命令
        if (args == null || args.Length == 0)
            return $"command not found";

        var cmdName = args[0];
        var cmdArgs = args.Length > 1 ? args[1..] : Array.Empty<string>();

        var cmd = GetCommand(cmdName);
        if (cmd == null)
            return $"command '{cmdName}' not found";
        return cmd.Handler(cmdArgs);
    }

    public static void LoadPCmd()
    {
        _PCmdDict.Clear();
        // 加载内置命令
        void AddCmd(PCmd cmd)
        {
            if (!string.IsNullOrWhiteSpace(cmd.Name))
                _PCmdDict[cmd.Name] = cmd;
        }
        AddCmd(new Cmdx());
        AddCmd(new EchoCommand());
        AddCmd(new ExitCommand());
        AddCmd(new HelpCommand());
        AddCmd(new SyncCommand());

        // 通过反射动态加载程序集中的所有PCmd子类
        string pluginsDir = Paths.PluginPath;
        foreach (var dir in Directory.GetDirectories(pluginsDir, "PeakChatOps*"))
        {
            // 排除自身目录（如 plugins/PeakChatOps 或 plugins/PeakChatOps/）
            var dirName = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.Equals(dirName, "PeakChatOps", StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var dll in Directory.GetFiles(dir, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(PCmd)) && !type.IsAbstract)
                        {
                            var instance = Activator.CreateInstance(type) as PCmd;
                            if (instance != null && !string.IsNullOrWhiteSpace(instance.Name))
                                _PCmdDict[instance.Name] = instance;
                        }
                    }
                }
                catch { /* 忽略加载失败的DLL */ }
            }
        }
    }

    public static PCmd? GetCommand(string cmdName)
    {
        if (string.IsNullOrWhiteSpace(cmdName))
            return null;
        _PCmdDict.TryGetValue(cmdName, out var cmd);
        return cmd;
    }
}
