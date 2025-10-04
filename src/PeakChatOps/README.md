# PeakChatOps
[![GitHub](https://img.shields.io/badge/GitHub-PeakChatOps-LIghtJUNction?style=for-the-badge&logo=GitHub)](https://github.com/LIghtJUNction/PeakMods)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/LIghtPeak/PeakChatOps?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/LIghtPeak/PeakChatOps?style=for-the-badge&logo=thunderstore&logoColor=white)](https://new.thunderstore.io/c/peak/p/LIghtPeak/PeakChatOps/)

## Welcome to PeakChatOps

This mod is a chat enhancement mod with the following features:
- Multiple extensible chat commands
- Scrollable message history without lag, with text copying support
- LLM integration for translation and other AI features
- Localization support (commands not fully localized yet)

## Usage Tips

- **Default key**: Press `Y` to open chat
- **Commands**: Start with `/`
- **Help**: `/help` to view available commands
- **AI Chat**: `/ai HI` - Chat with AI (local display only)
- **Translation**: `/ai translate 你好` - Translate to English, then `@send` to broadcast the AI response
- **Command Learning**: `/ai how to use this cmd: whisper @whisper` - Learn how to use commands (useful for foreign language commands)

- **Position**: `/pos T / R / C` - Change chat window position (Top/Right/Center)

> **Note**: While typing, you can freely click on text to copy content

## What
'
s New in 1.2.0 (vs 1.1.5)

I completely abandoned the old UI system and switched to Unity
'
s latest UXML/USS system, solving these issues:

1. **No more lag** when sending long texts - significantly improved experience
2. **Smoother scrolling** throughout the interface
3. **Better extensibility** for future features

## Customizing Chat Styles

Here
'
s how to customize the chat appearance:

1. **Open the Unity Project**
   - Navigate to the `unity` folder in this project
   - Open with Unity (same version as the project)

2. **Locate the UI Files**
   - In the Mod folder, you
'
ll find prefab files
   - Use UI Toolkit to open the `.uxml` files

3. **Customize Styles**
   - Edit the UXML/USS files to change appearance
   - **Important**: Don
'
t change component names, or you
'
ll need to update the source code bindings

4. **Build the Bundle**
   - Enable Unity Addressable System
   - Go to `Window → Asset Management → Addressables → Groups → Build`
   - Click Build to generate the bundle

5. **Deploy the Bundle**
   - Find the largest bundle file in: `unity project / Libraries / com.unity.addressables / aa / standalonewindows64 /`
   - Rename it to: `PeakChatOpsUI.peakbundle`
   - Place it in the mod folder

## Developing Custom Commands

Here
'
s how to create your own commands. Example: `Echo.cs`

```csharp
using System;
using PeakChatOps.API;
using Cysharp.Threading.Tasks;
using PeakChatOps.Core;
#nullable enable
namespace PeakChatOps.Commands;

[PCOCommand("echo", "Echo input content", "Usage: /echo <content>\nReturns your input as-is.")]
public class EchoCommand
{
    // New message-driven handler signature. Plugins/commands register handlers
    // on EventBusRegistry.CmdMessageBus with channel "cmd://echo".
    public EchoCommand()
    {
        EventBusRegistry.CmdMessageBus.Subscribe("cmd://echo", Handle);
        DevLog.UI("[Cmd] EchoCommand subscribed to cmd://echo");
    }

    public static async UniTask Handle(CmdMessageEvent evt)
    {
        try
        {
            var args = evt.Args ?? Array.Empty<string>();
            var res = args.Length == 0 ? "Please enter content to echo." : string.Join(" ", args);
            var resultEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: res, stderr: null, success: true);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", resultEvt);
        }
        catch (Exception ex)
        {
            var errEvt = new CmdExecResultEvent(evt.Command, evt.Args ?? Array.Empty<string>(), evt.UserId, stdout: null, stderr: ex.Message, success: false);
            await EventBusRegistry.CmdExecResultBus.Publish("cmd://", errEvt);
        }
        await UniTask.CompletedTask;
    }
}
```


## API Key 安全提示

- **API Key 已安全储存在本地，请放心使用。**
- **如果不放心，建议使用 ollama 提供的云模型。**

> **API Key is securely stored locally. You can use it with confidence.**
> **If you have concerns, it is recommended to use the cloud model provided by ollama.**


## Build

```bash
dotnet build -c Release -target:PackTS -v d
```



## Credits (in no particular order)
- [PeakTextChat](https://github.com/borealityy/PeakTextChat) (Inspiration)
- [BepInEx](https://github.com/BepInEx/BepInEx) (Framework)
- [PeakLib](https://github.com/PeakModding/PeakLib) (Utility library)

---
For questions or suggestions, feel free to open an issue or PR!
