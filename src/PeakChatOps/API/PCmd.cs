using System;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.API;

[AttributeUsage(AttributeTargets.Class)]
public class PCOCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public string HelpInfo { get; }
    public PCOCommandAttribute(string name, string description = "", string helpInfo = "")
    {
        Name = name;
        Description = description;
        HelpInfo = helpInfo;
    }
}

// EXAMPLE :

// Example command metadata: clearer wording and punctuation
[PCOCommand("name", "Example: how to create a custom command.", "Example only — not executable. See PeakChatOps.API (NuGet) for details.")]
public class PCmd
{
    public PCmd()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://name", Handle);
    }

    public static UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            var result = ""; // TODO: implement command logic
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: result, stderr: null, success: true);
            // publish without awaiting to avoid creating async state machine in API assembly
            EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        return UniTask.CompletedTask;
    }
}
