using System;

#nullable enable
namespace PeakChatOps.API.Commands;

public class SyncCommand : ICmdProvider
{
  public void Register()
  {
    CmdX.Register(new Cmd()
    {
      Name = "sync",
      Description = "重新扫描命令",
      HelpInfo = "用法: /sync",
      Handler = (Func<string[], string>) (args => SyncCommand.Sync())
    });
  }

  public static string Sync()
  {
        // 这里是重新扫描命令的逻辑
    CmdX.EnsureRegistered();

    return "命令已重新扫描。";
  }
}


