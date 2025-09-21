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

    public static ConfigEntry<float> FontSize = null!;
    public static ConfigEntry<string> ChatSize = null!;
    public static ConfigEntry<float> MessageFadeDelay = null!;
    public static ConfigEntry<float> FadeDelay = null!;
    public static ConfigEntry<float> HideDelay = null!;
    public static ConfigEntry<KeyCode> Key = null!;
    public static ConfigEntry<UIAlignment> Pos = null!;
    public static ConfigEntry<float> BgOpacity = null!;
    public static ConfigEntry<bool> FrameVisible = null!;
    public static ConfigEntry<bool> HideInputField = null!;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        MonoDetourManager.InvokeHookInitializers(typeof(PeakChatOpsPlugin).Assembly);
        Logger.LogInfo($"PeakChatOps is loaded!");

        Key = Config.Bind<KeyCode>(
                                "Display",
                                "ChatKey",
                                KeyCode.Y,
                                "The key that activates typing in chat"
                            );

        Pos = Config.Bind<UIAlignment>(
                                "Display",
                                "ChatPosition",
                                UIAlignment.TopLeft,
                                "The position of the text chat"
                            );

        ChatSize = Config.Bind<string>(
                                "Display",
                                "ChatSize",
                                "500:300",
                                "The size of the text chat (formatted X:Y)"
                            );

        FontSize = Config.Bind<float>(
                                "Display",
                                "ChatFontSize",
                                20f,
                                "Size of the chat's text"
                            );

        BgOpacity = Config.Bind<float>(
                              "Display",
                              "ChatBackgroundOpacity",
                              0.3f,
                              "Opacity of the chat's background/shadow"
                          );

        FrameVisible = Config.Bind<bool>(
                              "Display",
                              "ChatFrameVisible",
                              true,
                              "Whether the frame of the chat box is visible"
                          );


        FadeDelay = Config.Bind<float>(
                              "Display",
                              "ChatFadeDelay",
                              15f,
                              "How long before the chat fades out (a negative number means never)"
                          );

        HideDelay = Config.Bind<float>(
                              "Display",
                              "ChatHideDelay",
                              40f,
                              "How long before the chat hides completely (a negative number means never)"
                          );


        harmony = new Harmony("com.lightjunction.peakchatops");

        harmony.PatchAll(typeof(StaminaBarPatch));
        harmony.PatchAll(typeof(GUIManagerPatch));
        harmony.PatchAll(typeof(InputBlockingPatches));

        // 初始化聊天系统
        InitializeChatSystem();
    }

    private void OnDestroy()
    {
        if (PeakOpsUI.instance != null)
            GameObject.Destroy(PeakOpsUI.instance.gameObject);
        if (GUIManagerPatch.ChatOpsCanvas != null)
            GameObject.Destroy(GUIManagerPatch.ChatOpsCanvas);

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