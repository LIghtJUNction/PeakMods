using System;

#nullable enable
namespace PeakChatOps.API.Commands;

public class EchoCommand : ICmdProvider
{
  public void Register()
  {
    CmdX.Register(new Cmd()
    {
      Name = "echo",
      Description = "回显输入内容",
      HelpInfo = "用法: /echo <内容>\n将你输入的内容原样返回。",
      Handler = (Func<string[], string>) (args => EchoCommand.Echo(args))
    });
  }

  public static string Echo(string[] args)
  {
    return args == null || args.Length == 0 ? "请输入要回显的内容。" : string.Join(" ", args);
  }
}
