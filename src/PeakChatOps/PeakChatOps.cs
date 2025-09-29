using BepInEx;
using System;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using PeakChatOps.Core;
using PeakChatOps.Core.MsgChain;
using UnityEngine.UI.ProceduralImage;

using PeakChatOps.Patches;
using PeakChatOps.UI;

namespace PeakChatOps;

[BepInAutoPlugin]
[BepInDependency("com.snosz.photoncustompropsutils", BepInDependency.DependencyFlags.HardDependency)]
partial class PeakChatOpsPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger = null!;
    public static Harmony harmony = null!;
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Logger.LogInfo($"PeakChatOps is loaded!");

        PConfig.InitConfig(
            Config, // 这里的 Config 是 BaseUnityPlugin 的属性，类型为 ConfigFile
            out Key, out Pos, out ChatSize, out FontSize, out BgOpacity, out FrameVisible,
            out FadeDelay, out HideDelay, out CmdPrefix,
            out DeathMessage, out ReviveMessage, out PassOutMessage,
            out aiModel, out aiApiKey, out aiEndpoint, out aiContextMaxCount,
            out aiAutoTranslate, out promptTranslate, out promptSend,
            out aiShowResponse
        );

        harmony = new Harmony("com.lightjunction.peakchatops");

        harmony.PatchAll(typeof(GUIManagerPatch));

        harmony.PatchAll(typeof(InputBlockingPatches));
        harmony.PatchAll(typeof(CharacterStatsPatches));

        // 加载自定义本地化文本
        harmony.PatchAll(typeof(LocalizationPatches));


        // 聊天系统初始化：自动挂载 ChatSystem
        if (GameObject.Find("ChatSystem") == null)
        {
            var chatSystemObj = new GameObject("ChatSystem");
            chatSystemObj.AddComponent<ChatSystem>();
            GameObject.DontDestroyOnLoad(chatSystemObj);
            Logger.LogInfo("[PeakChatOps] ChatSystem GameObject created and initialized.");
        }


        // 初始化AI参数配置
        InitAIConfig();
        // AI上下文记录器初始化
        AIChatContextLogger.CreateGlobalInstance(aiContextMaxCount.Value);

        // Ensure message handler chain is initialized (subscribe buses, start runners)
        try
        {
            CentralCmdRouter.EnsureInitialized();
            Logger.LogDebug("[PeakChatOps] CentralCmdRouter.EnsureInitialized called");
        }
        catch (Exception ex)
        {
            Logger.LogError($"[PeakChatOps] Error ensuring MsgHandlerChain initialization: {ex.Message}");
        }
    }

        // 在Awake或配置初始化处添加如下配置项绑定（如已存在则合并）
        private void InitAIConfig()
        {
            aiMaxTokens = Config.Bind("AI", "MaxTokens", 1024, "AI回复最大token数，越大回复越长，消耗也越大");
            aiTemperature = Config.Bind("AI", "Temperature", 0.7f, "AI采样温度，越高越随机，越低越保守");
            aiTopP = Config.Bind("AI", "TopP", 1.0f, "AI采样top_p，1.0为全概率，越低越保守");
            aiN = Config.Bind("AI", "N", 1, "每次生成的回复数量，通常为1");
        }
    private void OnDestroy()
    {
        if (PeakChatOpsUI.Instance != null)
            GameObject.Destroy(PeakChatOpsUI.Instance.gameObject);
        if (GUIManagerPatch.ChatOpsCanvas != null)
            GameObject.Destroy(GUIManagerPatch.ChatOpsCanvas);

        // 销毁 ChatSystem GameObject
        var chatSystemObj = GameObject.Find("ChatSystem");
        if (chatSystemObj != null)
            GameObject.Destroy(chatSystemObj);


        harmony.UnpatchSelf();
    }
}

public static class ProceduralImageExtensions {
    public static T SetModifierType<T>(this ProceduralImage image) where T : ProceduralImageModifier {
        image.ModifierType = typeof(T);
        return image.gameObject.GetComponent<T>() ?? image.gameObject.AddComponent<T>();
    }
}