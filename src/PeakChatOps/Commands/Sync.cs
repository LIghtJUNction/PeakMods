using System;
using PeakChatOps.API;
#nullable enable
namespace PeakChatOps.Commands;

public class SyncCommand : PCmd
{
    public SyncCommand()
    {
        Name = "sync";
        Description = "重新扫描命令";
        HelpInfo = "用法: /sync";
        Handler = args => Sync();
    }

    public static string Sync()
    {
        CmdX.EnsureRegistered();
        return "命令已重新扫描。";
    }
}


