
using System;
using UnityEngine.SceneManagement;
using PeakChatOps.API;
#nullable enable
namespace PeakChatOps.Commands;

public class ExitCommand : PCmd
{
    public ExitCommand()
    {
        Name = "exit";
        Description = "退出游戏并返回主菜单";
        HelpInfo = "用法: /exit\n立即返回主菜单。";
        Handler = args => Exit();
    }

    public static string Exit()
    {
        try
        {
            SceneManager.LoadScene("MainMenu");
            return "已返回主菜单。";
        }
        catch (Exception ex)
        {
            return "退出失败: " + ex.Message;
        }
    }
}
