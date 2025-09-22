using System;
using PeakChatOps.API;
#nullable enable
namespace PeakChatOps.Commands;

public class EchoCommand : PCmd
{
    public EchoCommand()
    {
        Name = "echo";
        Description = "回显输入内容";
        HelpInfo = "用法: /echo <内容>\n将你输入的内容原样返回。";
        Handler = Echo;
    }

    public static string Echo(string[] args)
    {
        return args == null || args.Length == 0 ? "请输入要回显的内容。" : string.Join(" ", args);
    }
}
