using System.Collections.Generic;
using HarmonyLib;
using PeakChatOps.API;
using PeakChatOps.Core;
using Photon.Pun;

namespace PeakChatOps.Patches;

// 明确声明参数类型，确保 patch 正确
[HarmonyPatch(typeof(CharacterStats))]
public class CharacterStatsPatches
{
    // 用于缓存每个角色的上一次状态，避免重复推送
    private static readonly Dictionary<CharacterStats, (bool died, bool revived, bool passedOut)> lastStates = new();
    static CharacterStatsPatches()
    {
        PeakChatOpsPlugin.Logger.LogWarning("[Harmony] CharacterStatsPatches static ctor loaded!");
    }

    [HarmonyPatch("Record", new[] { typeof(bool), typeof(float) })]
    [HarmonyPostfix]
    public static void RecordPostfix(CharacterStats __instance, bool useOverridePosition, float overrideHeight)
    {
        DevLog.UI("[Harmony] RecordPostfix called!");
        // 只处理本地玩家
        if (!__instance.name.Contains(PhotonNetwork.LocalPlayer.NickName))
            return;

        var timeline = __instance.timelineInfo;
        if (timeline == null || timeline.Count == 0)
            return;

        var last = timeline[timeline.Count - 1];
        bool prevDied = false, prevRevived = false, prevPassedOut = false;
        lastStates.TryGetValue(__instance, out var prev);
        prevDied = prev.died;
        prevRevived = prev.revived;
        prevPassedOut = prev.passedOut;

        // 仅在状态发生变化时推送
        if (last.died && !prevDied)
        {
            DevLog.UI($"[CharacterStatsPatches] {__instance.name} justDied");
            EventBusRegistry.ChatMessageBus.Publish(
                "sander://self",
                new ChatMessageEvent(
                    PhotonNetwork.LocalPlayer.NickName,
                    PeakChatOpsPlugin.DeathMessage.Value,
                    PhotonNetwork.LocalPlayer.UserId,
                    isDead: true,
                    extra: null
                )
            );
        }
        if (last.revived && !prevRevived)
        {
            DevLog.UI($"[CharacterStatsPatches] {__instance.name} justRevived");
            EventBusRegistry.ChatMessageBus.Publish(
                "sander://self",
                new ChatMessageEvent(
                    PhotonNetwork.LocalPlayer.NickName,
                    PeakChatOpsPlugin.ReviveMessage.Value,
                    PhotonNetwork.LocalPlayer.UserId,
                    isDead: false,
                    extra: null
                )
            );
        }
        if (last.justPassedOut && !prevPassedOut)
        {
            DevLog.UI($"[CharacterStatsPatches] {__instance.name} justPassedOut");
            EventBusRegistry.ChatMessageBus.Publish(
                "sander://self",
                new ChatMessageEvent(
                    PhotonNetwork.LocalPlayer.NickName,
                    PeakChatOpsPlugin.PassOutMessage.Value,
                    PhotonNetwork.LocalPlayer.UserId,
                    isDead: false,
                    extra: null
                )
            );
        }

        // 更新缓存
        lastStates[__instance] = (last.died, last.revived, last.justPassedOut);
    }

}
