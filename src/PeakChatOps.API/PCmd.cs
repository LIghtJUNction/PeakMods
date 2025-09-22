
namespace PeakChatOps.API;

public class PCmd
{
    public string Name = string.Empty;
    public string Description = string.Empty;
    public string HelpInfo = string.Empty;
    public System.Func<string[], string> Handler = _ => "";

    public PCmd() { }
}
