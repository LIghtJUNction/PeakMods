# Guide: Creating a PeakChatOps Command Extension Module

This guide explains how to create your own command extension module for PeakChatOps, using the official API and extension mechanism.

## 1. Create a New Extension Project
- Create a new DLL project (e.g., `PeakChatOps.Extra`).
- Reference `PeakChatOps.API` in your project.

## 2. Write Your Command Class
- Each command should inherit from `PCmd`.
- In the constructor, set `Name`, `Description`, `HelpInfo`, and assign the `Handler` delegate.
- The `Handler` is the actual logic, with the signature: `string Handler(string[] args)`.

**Example: TpCommand**
```csharp
public class TpCommand : PCmd
{
  public TpCommand()
  {
    Name = "tp";
    Description = "Teleport A to B";
    HelpInfo = "Usage: /tp <target player> <position>";
    Handler = Tp;
  }

  public static string Tp(string[] args)
  {
    if (args == null || args.Length < 2)
      return "Usage: /tp <target player> <content>";
    string targetPlayer = args[0];
    string content = string.Join(" ", args[1..]);
    return $"Teleporting '{content}' to player {targetPlayer}.";
  }
}
```

## 3. Automatic Registration
- You do **not** need to manually register your command. The main module will automatically discover and load all classes inheriting from `PCmd`.

## 4. Build and Deploy
- Build your DLL.
- Place it in the extension folder or the `plugins` directory as required by the main plugin. It will be loaded automatically.

## 5. Test and Extend
- Start the game and test your command (e.g., `/tp`).
- Add more command classes as needed. You can implement argument parsing, permission checks, custom responses, etc.

---

**Tips:**
- Command arguments are passed as the `args` array.
- You can leverage the main module's UI, localization, and message chain for advanced features.
- See the `PeakChatOps.Extra` source code for more examples.

For more details or advanced usage, refer to the main project documentation or open an issue!
