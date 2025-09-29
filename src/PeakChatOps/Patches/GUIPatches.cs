using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using PeakChatOps.Core;
using PeakChatOps.UI;

namespace PeakChatOps.Patches;


[HarmonyPatch(typeof(GUIManager))]
public static class GUIManagerPatch
{
    public static Canvas ChatOpsCanvas = null!;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]

    public static void StartPostfix(GUIManager __instance)
    {

        DevLog.UI("GUIManager.Start patch executed - creating chat interface");

        // 获取 GUIManager 的 transform
        var parentTransform = __instance.transform;

        // 直接初始化主 Canvas（PeakChatOpsCanvas 已自动完成所有 UI 子组件挂载）
        var canvasGO = new GameObject("PeakChatOpsCanvas", typeof(PeakChatOpsCanvas));
        canvasGO.transform.SetParent(parentTransform, false);
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        ChatOpsCanvas = canvas;

        DevLog.UI("Chat interface created successfully by PeakChatOpsUI.InitUI()");
    }
    }

