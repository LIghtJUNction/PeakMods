using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using PeakChatOps.Core;
using PEAKLib.Core;
using PEAKLib.UI;
using UnityEngine.UI.ProceduralImage;
using MonoDetour;
using PeakChatOps.Patches;

namespace PeakChatOps;

[BepInAutoPlugin]
[BepInDependency("com.snosz.photoncustompropsutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(CorePlugin.Id)]
[BepInDependency(UIPlugin.Id)]
partial class PeakChatOpsPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger = null!;

    Harmony harmony = null!;

    public static ConfigEntry<float> configFontSize = null!;
    public static ConfigEntry<string> configChatSize = null!;
    public static ConfigEntry<float> configMessageFadeDelay = null!;
    public static ConfigEntry<float> configFadeDelay = null!;
    public static ConfigEntry<float> configHideDelay = null!;
    public static ConfigEntry<KeyCode> configKey = null!;
    public static ConfigEntry<UIAlignment> configPos = null!;
    public static ConfigEntry<bool> configRichTextEnabled = null!;
    public static ConfigEntry<bool> configIMGUI = null!;
    public static ConfigEntry<float> configBgOpacity = null!;
    public static ConfigEntry<bool> configFrameVisible = null!;
    public static ConfigEntry<bool> configHideInputField = null!;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        MonoDetourManager.InvokeHookInitializers(typeof(PeakChatOpsPlugin).Assembly);
        Logger.LogInfo($"PeakChatOps is loaded!");

        configKey = Config.Bind<KeyCode>(
                                "Display",
                                "ChatKey",
                                KeyCode.Y,
                                "The key that activates typing in chat"
                            );

        configIMGUI = Config.Bind<bool>(
                                "Display",
                                "UseIMGUI",
                                false,
                                "Use IMGUI for the text field (use if you're having problems with typing)"
            );

        configPos = Config.Bind<UIAlignment>(
                                "Display",
                                "ChatPosition",
                                UIAlignment.TopLeft,
                                "The position of the text chat"
                            );

        configChatSize = Config.Bind<string>(
                                "Display",
                                "ChatSize",
                                "500:300",
                                "The size of the text chat (formatted X:Y)"
                            );

        configFontSize = Config.Bind<float>(
                                "Display",
                                "ChatFontSize",
                                20f,
                                "Size of the chat's text"
                            );

        configBgOpacity = Config.Bind<float>(
                                "Display",
                                "ChatBackgroundOpacity",
                                0.3f,
                                "Opacity of the chat's background/shadow"
                            );

        configFrameVisible = Config.Bind<bool>(
                                "Display",
                                "ChatFrameVisible",
                                true,
                                "Whether the frame of the chat box is visible"
                            );

        configHideInputField = Config.Bind<bool>(
                                "Display",
                                "HideInputFieldWhenChatHidden",
                                true,
                                "Whether to hide the input field when chat is hidden (frees up space for other UI)"
                            );

        configRichTextEnabled = Config.Bind<bool>(
                                "Display",
                                "ChatRichText",
                                true,
                                "Whether rich text tags get parsed in messages (e.g. <b> for bold text)"
                            );

        configFadeDelay = Config.Bind<float>(
                                "Display",
                                "ChatFadeDelay",
                                15f,
                                "How long before the chat fades out (a negative number means never)"
                            );


        configHideDelay = Config.Bind<float>(
                                "Display",
                                "ChatHideDelay",
                                40f,
                                "How long before the chat hides completely (a negative number means never)"
                            );

        configMessageFadeDelay = Config.Bind<float>(
                                    "Display",
                                    "ChatMessageHideDelay",
                                    40f,
                                    "How long before a chat message disappears (a negative number means never)"
                                );

        harmony = new Harmony("com.lightjunction.peakchatops");
        harmony.PatchAll(typeof(GameUtilsPatch));
        harmony.PatchAll(typeof(StaminaBarPatch));
        harmony.PatchAll(typeof(GUIManagerPatch));
        harmony.PatchAll(typeof(InputBlockingPatches));

        // 初始化聊天系统
        InitializeChatSystem();
    }

    private void OnDestroy()
    {
        if (TextChatDisplay.instance != null)
            GameObject.Destroy(TextChatDisplay.instance.gameObject);
        if (GUIManagerPatch.textChatCanvas != null)
            GameObject.Destroy(GUIManagerPatch.textChatCanvas);

        StaminaBarPatch.CleanupObjects();

        // 清理聊天系统
        ChatSystem.Instance.Clear();

        harmony.UnpatchSelf();
    }

    private void InitializeChatSystem()
    {
        Logger.LogInfo("Initializing Chat System...");

        // 注册基础消息处理器
        ChatSystem.Instance.RegisterHandler(new BasicMessageHandler());

        Logger.LogInfo("Chat System initialized successfully!");
    }
}

public static class ProceduralImageExtensions {
    public static T SetModifierType<T>(this ProceduralImage image) where T : ProceduralImageModifier {
        image.ModifierType = typeof(T);
        return image.gameObject.GetComponent<T>() ?? image.gameObject.AddComponent<T>();
    }
}