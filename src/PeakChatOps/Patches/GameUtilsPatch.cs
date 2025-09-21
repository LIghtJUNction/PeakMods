using HarmonyLib;
using UnityEngine;
using PeakChatOps.Core;

namespace PeakChatOps;

public static class GameUtilsPatch {
    [HarmonyPatch(typeof(GameUtils),"Awake")]
    [HarmonyPostfix]
    public static void AwakePatch(GameUtils __instance) {
        // ChatSystem 会自动创建单例，不需要手动创建
        // 确保 ChatSystem 实例被初始化
        var chatSystem = ChatSystem.Instance;
        PeakChatOpsPlugin.Logger.LogInfo("ChatSystem instance created via GameUtils patch");
    }
}