using PeakChatOps.API;

namespace PeakChatOps.Extra;
public class TestCommand : PCmd
{
  public TestCommand()
  {
    Name = "test";
    Description = "回显输入内容";
    HelpInfo = "用法: /test <内容>\n将你输入的内容原样返回。";
    Handler = Test;
  }

  public static string Test(string[] args)
  {
    return args == null || args.Length == 0 ? "请输入要测试的内容。" : string.Join(" ", args);
  }
}