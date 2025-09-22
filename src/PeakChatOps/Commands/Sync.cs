using System;
using PeakChatOps.API;
using PeakChatOps.Core;
#nullable enable
namespace PeakChatOps.Commands;

public class SyncCommand : PCmd
{
    public SyncCommand()
    {
        Name = "sync";
        Description = "同步状态";
        HelpInfo = "用法: /sync";
        Handler = args => Sync();
    }

    public static string Sync()
    {
        // 重新加载命令
        Cmdx.LoadPCmd();
        Cmdx.Prefix = PeakChatOpsPlugin.CmdPrefix.Value;
        // 刷新配置 更新UI
        PeakOpsUI.instance.RefreshUI();
        
        return "同步完成";
    }
}


