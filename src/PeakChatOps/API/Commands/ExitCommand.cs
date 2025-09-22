
using System;
using UnityEngine.SceneManagement;

#nullable enable
namespace PeakChatOps.API.Commands;

public class ExitCommand : ICmdProvider
{
  public void Register()
  {
    CmdX.Register(new Cmd()
    {
      Name = "exit",
      Description = "退出游戏并返回主菜单",
      HelpInfo = "用法: /exit\n立即返回主菜单。",
      Handler = (Func<string[], string>) (args => ExitCommand.Exit())
    });
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
