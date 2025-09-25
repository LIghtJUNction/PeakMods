using HarmonyLib;
using PeakChatOps.API;
using PeakChatOps.Core;
using Photon.Pun;

namespace PeakChatOps.Patches;

// 明确声明参数类型，确保 patch 正确
[HarmonyPatch(typeof(CharacterStats))]
public class CharacterStatsPatches
{
    static CharacterStatsPatches()
    {
        PeakChatOpsPlugin.Logger.LogWarning("[Harmony] CharacterStatsPatches static ctor loaded!");
    }

    [HarmonyPatch("Record", new[] { typeof(bool), typeof(float) })]
    [HarmonyPostfix]
    public static void RecordPostfix(CharacterStats __instance, bool useOverridePosition, float overrideHeight)
    {
        DevLog.UI("[Harmony] RecordPostfix called!");
        var timeline = __instance.timelineInfo;
        if (timeline == null || timeline.Count == 0)
            return;

        var last = timeline[timeline.Count - 1];

        if (last.died)
        {
            // __instance.name ：Character [LIghtJUNction : 1]
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
        if (last.revived)
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
        if (last.justPassedOut)
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
    }

}
