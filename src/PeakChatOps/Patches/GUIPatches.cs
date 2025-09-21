using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PeakChatOps.Patches;

[HarmonyPatch(typeof(StaminaBar),"Start")]
public static class StaminaBarPatch {
    public static BarGroupChildWatcher barGroupChildWatcher = null!;

    [HarmonyPostfix]
    public static void Postfix(StaminaBar __instance) {
        if (GUIManager.instance != null) {
            var textChatDummyObj = new GameObject("TextChatPos");
            var parent = (RectTransform)__instance.transform.parent;
            parent.offsetMax = new Vector2(parent.offsetMax.x,1000);
            textChatDummyObj.transform.SetParent(parent);
            var transform = textChatDummyObj.AddComponent<RectTransform>();
            transform.SetAsFirstSibling();
            transform.sizeDelta = Vector2.zero;
            barGroupChildWatcher = parent.gameObject.AddComponent<BarGroupChildWatcher>();
            barGroupChildWatcher.textChatDummyTransform = textChatDummyObj.transform;
        }
    }

    public static void CleanupObjects() {
        GameObject.Destroy(barGroupChildWatcher.textChatDummyTransform);
        GameObject.Destroy(barGroupChildWatcher);
    }
}

[HarmonyPatch(typeof(GUIManager))]
public static class GUIManagerPatch {
    public static Canvas textChatCanvas = null!;
    
    static bool isHUDActive = true;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(GUIManager __instance) {
        PeakChatOpsPlugin.Logger.LogInfo("GUIManager.Start patch executed - creating chat interface");
        
        var transform = __instance.transform;
        var textChatCanvasObj = new GameObject("TextChatCanvas");
        textChatCanvasObj.transform.SetParent(transform,false);
        textChatCanvas = textChatCanvasObj.AddComponent<Canvas>();
        textChatCanvas.renderMode = RenderMode.ScreenSpaceOverlay;  // 改为 Overlay 模式，不需要相机
        textChatCanvas.sortingOrder = 100;  // 确保在最上层显示
        
        var textChatCanvasScaler = textChatCanvas.gameObject.GetComponent<CanvasScaler>() ?? textChatCanvas.gameObject.AddComponent<CanvasScaler>();
        textChatCanvasScaler.referencePixelsPerUnit = 100;
        textChatCanvasScaler.matchWidthOrHeight = 1;
        textChatCanvasScaler.referenceResolution = new Vector2(1920,1080);
        textChatCanvasScaler.scaleFactor = 1;
        textChatCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        textChatCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // 添加 GraphicRaycaster 以支持输入交互
        var graphicRaycaster = textChatCanvasObj.GetComponent<GraphicRaycaster>() ?? textChatCanvasObj.AddComponent<GraphicRaycaster>();

        var textChatObj = new GameObject("TextChat");
        textChatObj.transform.SetParent(textChatCanvas.transform,false);
        textChatObj.AddComponent<TextChatDisplay>();
        
        PeakChatOpsPlugin.Logger.LogInfo($"Chat interface created successfully - Canvas mode: {textChatCanvas.renderMode}, sorting order: {textChatCanvas.sortingOrder}");
    }

    [HarmonyPatch("LateUpdate")]
    [HarmonyPostfix]
    public static void LateUpdatePostfix(GUIManager __instance) {
        if (isHUDActive != __instance.hudCanvas.gameObject.activeInHierarchy) {
            isHUDActive = __instance.hudCanvas.gameObject.activeInHierarchy;
            textChatCanvas.gameObject.SetActive(isHUDActive);
        }
    }

    [HarmonyPatch("UpdatePaused")]
    [HarmonyPrefix]
    public static bool UpdatePausedPrefix(GUIManager __instance) {
        if (TextChatDisplay.instance?.framesSinceInputBlocked <= 1 && !__instance.pauseMenu.activeSelf) {
            return false;
        }
        return true;
    }
}