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
    private class CharacterStateInfo
    {
        public bool died { get; set; }
        public bool revived { get; set; }
        public bool passedOut { get; set; }
    }

    private static Dictionary<string, object> CreateExtraForEvent(CharacterStateInfo info)
    {
        return new Dictionary<string, object>
        {
            ["CharacterState"] = new Dictionary<string, bool>
            {
                ["died"] = info?.died ?? false,
                ["revived"] = info?.revived ?? false,
                ["passedOut"] = info?.passedOut ?? false
            }
        };
    }
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

        // 记录所有玩家的状态变化到AI上下文（system身份）
        string playerName = __instance.name;
        string userId = PhotonNetwork.LocalPlayer.UserId;
        // 只处理本地玩家的聊天推送，AI上下文记录所有玩家
        bool isLocal = __instance.name.Contains(PhotonNetwork.LocalPlayer.NickName);

        var timeline = __instance.timelineInfo;
        if (timeline == null || timeline.Count == 0)
            return;

        var last = timeline[timeline.Count - 1];
        bool prevDied = false, prevRevived = false, prevPassedOut = false;
        lastStates.TryGetValue(__instance, out var prev);
        prevDied = prev.died;
        prevRevived = prev.revived;
        prevPassedOut = prev.passedOut;

        // 仅在状态发生变化时推送/记录
        if (last.died && !prevDied)
        {
            DevLog.UI($"[CharacterStatsPatches] {__instance.name} justDied");
            if (isLocal)
            {
                EventBusRegistry.ChatMessageBus.Publish(
                    "sander://self",
                    new ChatMessageEvent(
                        PhotonNetwork.LocalPlayer.NickName,
                        PeakChatOpsPlugin.config.DeathMessage.Value,
                        PhotonNetwork.LocalPlayer.UserId,
                        isDead: true,
                        extra: CreateExtraForEvent(new CharacterStateInfo { died = true })
                    )
                );
            }
            // 记录到AI上下文
            AIChatContextLogger.Instance?.LogSystem($"玩家[{playerName}]死亡", playerName, userId);
        }
        if (last.revived && !prevRevived)
        {
            DevLog.UI($"[CharacterStatsPatches] {__instance.name} justRevived");
            if (isLocal)
            {
                EventBusRegistry.ChatMessageBus.Publish(
                    "sander://self",
                    new ChatMessageEvent(
                        PhotonNetwork.LocalPlayer.NickName,
                        PeakChatOpsPlugin.config.ReviveMessage.Value,
                        PhotonNetwork.LocalPlayer.UserId,
                        isDead: false,
                        extra: CreateExtraForEvent(new CharacterStateInfo { revived = true })
                    )
                );
            }
            AIChatContextLogger.Instance?.LogSystem($"玩家[{playerName}]复活", playerName, userId);
        }
        if (last.justPassedOut && !prevPassedOut)
        {
            DevLog.UI($"[CharacterStatsPatches] {__instance.name} justPassedOut");
            if (isLocal)
            {
                EventBusRegistry.ChatMessageBus.Publish(
                    "sander://self",
                    new ChatMessageEvent(
                        PhotonNetwork.LocalPlayer.NickName,
                        PeakChatOpsPlugin.config.PassOutMessage.Value,
                        PhotonNetwork.LocalPlayer.UserId,
                        isDead: false,
                        extra: CreateExtraForEvent(new CharacterStateInfo { passedOut = true })
                    )
                );
            }
            AIChatContextLogger.Instance?.LogSystem($"玩家[{playerName}]晕厥", playerName, userId);
        }

        // 更新缓存
        lastStates[__instance] = (last.died, last.revived, last.justPassedOut);
    }

}
