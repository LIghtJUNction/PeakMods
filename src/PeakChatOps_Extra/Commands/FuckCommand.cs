using System;
using PeakChatOps.API;

#nullable enable
namespace PeakChatOps.Extra.Commands;

public class FuckCommand : ICmdProvider
{
    public static void Register()
    {
        CmdX.Register(new Cmd()
        {
            Name = "fuck",
            Description = "回显输入内容",
            HelpInfo = "用法: /fuck <内容>\n将你输入的内容fuck后返回。",
            Handler = (Func<string[], string>)(args => FuckCommand.Fuck(args))
        });
    }

    public static string Fuck(string[] args)
    {
        return args == null || args.Length == 0 ? "请输入要回显的内容。" : string.Join(" ", args) + " fuck!";
    }
}
