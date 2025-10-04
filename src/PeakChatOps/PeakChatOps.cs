using BepInEx;
using System;
using BepInEx.Logging;
using HarmonyLib;

using PeakChatOps.Patches;
using UnityEngine;

using PeakChatOps.Core;
using PeakChatOps.Core.MsgChain;
using PEAKLib.Core;

using PeakChatOps.UI;
using System.Linq;
using TMPro;
using PeakChatOps.core;
using System.IO;
#nullable enable

namespace PeakChatOps;

[BepInAutoPlugin]
[BepInDependency("com.snosz.photoncustompropsutils")]
[BepInDependency(CorePlugin.Id)]
partial class PeakChatOpsPlugin : BaseUnityPlugin
{
    public static PeakChatOpsPlugin Instance { get; private set; } = null!;

    public static readonly string PluginPath = Path.Combine(Paths.PluginPath, "PeakChatOps");
    internal static new ManualLogSource Logger = null!;
    private Harmony _harmony = null!;
    public static PConfig config = null!;

    // Unity 中的 Prefab 资源 在代码里是以 GameObject 表示
    public static GameObject PeakChatOpsUIPrefab = null!;

    // 获取游戏字体
    private static TMP_FontAsset? _darumaFontAsset;
    public static TMP_FontAsset? DarumaDropOneFont
    {
        get
        {
            if (_darumaFontAsset == null)
            {
                var assets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                _darumaFontAsset = assets.FirstOrDefault(fontAsset =>
                    fontAsset.faceInfo.familyName == "Daruma Drop One"
                );
            }
            return _darumaFontAsset;
        }
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Instance = this;
        // 其他步骤
        Logger.LogInfo($"PeakChatOps is loaded!");
        // 加载 UXML/Prefab 资源（优先尝试 UIDocument prefab）
        this.LoadBundleWithName(
            "PeakChatOpsUI.peakbundle",
            peakBundle => 
            { 
                #if DEBUG
                peakBundle.GetAllAssetNames().ToList().ForEach(name => 
                {
                    Logger.LogInfo($"[DEBUG] Found asset: {name}");
                });
                #endif

                PeakChatOpsUIPrefab = peakBundle.LoadAsset<GameObject>("Assets/MOD/PeakChatOpsUI.prefab");

            }
        );
        _harmony = new Harmony("com.lightjunction.peakchatops");

        _harmony.PatchAll(typeof(GUIManagerPatch));
        _harmony.PatchAll(typeof(CharacterStatsPatches));
        _harmony.PatchAll(typeof(InputBlockingPatches));

        // 初始化本地化类
        PLocalizedText.Init();

        // 初始化UI类，测试本地化
        PeakChatOpsUI.Help();

        config = new PConfig(Config);
        // 聊天系统初始化：自动挂载 ChatSystem
        if (GameObject.Find("ChatSystem") == null)
        {
            var chatSystemObj = new GameObject("ChatSystem");
            chatSystemObj.AddComponent<ChatSystem>();
            GameObject.DontDestroyOnLoad(chatSystemObj);
            Logger.LogInfo("[PeakChatOps] ChatSystem GameObject created and initialized.");
        }

        // AI上下文记录器初始化
        AIChatContextLogger.CreateGlobalInstance(config.AiContextMaxCount.Value);

        // AI世界观设置
        AIChatContextLogger.Instance?.LogSystem(
            PLocalizedText.GetText("AI_CONTEXT_PEAK_WORLD_PROMPT_SYSTEM"),
            sender: "System",
            userId: "system",
            pinned: true
        );

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

    private void OnDestroy()
    {
        // 卸载补丁
        _harmony.UnpatchSelf();

        Logger.LogInfo("PeakChatOps is unloaded!");
    }
}
