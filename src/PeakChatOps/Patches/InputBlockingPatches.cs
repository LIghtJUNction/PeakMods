using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PeakChatOps.Patches;

public static class InputBlockingPatches {
    private static readonly MethodInfo windowBlockingInput = AccessTools.PropertySetter(typeof(GUIManager),"windowBlockingInput");

    /// <summary>
    /// 智能输入阻塞 - 只在聊天输入时阻塞游戏输入
    /// </summary>
    [HarmonyPatch(typeof(GUIManager),nameof(GUIManager.UpdateWindowStatus))]
    [HarmonyPostfix]
    public static void UpdateWindowStatusPatch() {
    bool shouldBlockInput = PeakChatOps.Core.PeakOpsUI.instance?.isBlockingInput == true;
        
        if (shouldBlockInput) {
            windowBlockingInput?.Invoke(GUIManager.instance,[ true ]);
        }
    }

    /// <summary>
    /// 相机控制阻塞 - 在聊天输入时禁用相机移动
    /// </summary>
    [HarmonyPatch(typeof(CinemaCamera),"Update")]
    [HarmonyPrefix]
    public static bool UpdateCinemaCamPatch(CinemaCamera __instance) {
        // 只有在窗口阻塞输入且相机未开启时才阻塞
        if (GUIManager.instance?.windowBlockingInput == true && !__instance.on) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 角色输入阻塞 - 智能阻塞角色动作，保持原有功能
    /// </summary>
    [HarmonyPatch(typeof(Character),"UpdateVariablesFixed")]
    [HarmonyPrefix]
    public static bool UpdateCharacterVariablesPatch(Character __instance) {
        if (GUIManager.instance?.windowBlockingInput == true) {
            // 只阻塞交互动作，保持其他功能正常
            __instance.input.interactIsPressed = false;
            
        }
        return true;
    }

    /// <summary>
    /// 阻塞键盘输入以防止与聊天输入冲突
    /// </summary>
    [HarmonyPatch(typeof(Input), "inputString", MethodType.Getter)]
    [HarmonyPrefix]
    public static bool InputStringPatch(ref string __result) {
        if (GUIManager.instance?.windowBlockingInput == true) {
            // 只有在聊天输入时，让聊天系统独占键盘输入
            // 但不完全阻塞，因为聊天需要输入
            return true; // 让原方法执行，但聊天系统会优先处理
        }
        return true;
    }
}