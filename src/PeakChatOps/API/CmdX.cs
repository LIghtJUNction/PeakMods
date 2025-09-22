using System;
using System.Collections.Generic;
using System.Reflection;
using WebSocketSharp;
#nullable enable
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
  private static readonly Dictionary<string, Cmd> _cmds = new Dictionary<string, Cmd>();

  public static string Prefix { get; set; } = "/";

  public static void EnsureRegistered()
  {
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (typeof (ICmdProvider).IsAssignableFrom(type))
        {
          if (!type.IsAbstract)
          {
            try
            {
              if (Activator.CreateInstance(type) is ICmdProvider instance)
                instance.Register();
            }
            catch
            {
            }
          }
        }
      }
    }
  }

  public static void Register(Cmd cmd)
  {
    if (cmd == null || string.IsNullOrWhiteSpace(cmd.Name) || cmd.Handler == null)
      return;
    CmdX._cmds[cmd.Name.ToLower()] = cmd;
  }

  public static string exec(string commandLine)
  {
    if (string.IsNullOrWhiteSpace(commandLine))
      return "请输入命令。";
    string[] data = commandLine.Trim().Split(' ');
    if (data.Length == 0)
      return "无效命令。";
    string lower = (data[0].StartsWith(CmdX.Prefix) ? data[0].Substring(CmdX.Prefix.Length) : data[0]).ToLower();
    string[] strArray = data.Length > 1 ? data.SubArray<string>(1, data.Length - 1) : new string[0];
    Cmd cmd;
    if (CmdX._cmds.TryGetValue(lower, out cmd))
      return cmd.Handler(strArray);
    return $"未知命令: {lower}，输入{CmdX.Prefix}help 查看所有命令。";
  }
}

// 命令自动注册接口
public interface ICmdProvider
{
    void Register();
}