using PeakChatOps.API;

namespace PeakChatOps.Extra;
public class TpCommand : PCmd
{
  public TpCommand()
  {
    Name = "tp";
    Description = "传送A 到 B";
    HelpInfo = "用法: /tp <目标玩家> <坐标>";
    Handler = Tp;
  }

  public static string Tp(string[] args)
  {
    if (args == null || args.Length < 2)
      return "用法: /tp <目标玩家> <内容>";

    string targetPlayer = args[0];
    string content = string.Join(" ", args[1..]);
    return $"将内容 \"{content}\" 传送给玩家 {targetPlayer}。";
  }
}