using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using PeakChatOps.Core;

namespace PeakChatOps.Patches;

[HarmonyPatch(typeof(StaminaBar), "Start")]
public static class StaminaBarPatch
{
    public static BarGroupChildWatcher barGroupChildWatcher = null!;

    [HarmonyPostfix]
    public static void Postfix(StaminaBar __instance)
    {
        if (GUIManager.instance != null)
        {
            var ChatOpsDummyObj = new GameObject("ChatOpsPos");
            var parent = (RectTransform)__instance.transform.parent;
            parent.offsetMax = new Vector2(parent.offsetMax.x, 1000);
            ChatOpsDummyObj.transform.SetParent(parent);
            var transform = ChatOpsDummyObj.AddComponent<RectTransform>();
            transform.SetAsFirstSibling();
            transform.sizeDelta = Vector2.zero;
            barGroupChildWatcher = parent.gameObject.AddComponent<BarGroupChildWatcher>();
            barGroupChildWatcher.ChatOpsDummyTransform = ChatOpsDummyObj.transform;
        }
    }
    public static void CleanupObjects()
    {
        GameObject.Destroy(barGroupChildWatcher.ChatOpsDummyTransform);
        GameObject.Destroy(barGroupChildWatcher);
    }
}

[HarmonyPatch(typeof(GUIManager))]
public static class GUIManagerPatch
{
    public static Canvas ChatOpsCanvas = null!;

    static bool isHUDActive = true;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(GUIManager __instance)
    {
        DevLog.UI("GUIManager.Start patch executed - creating chat interface");

        var transform = __instance.transform;
        var ChatOpsCanvasObj = new GameObject("ChatOpsCanvas");
        ChatOpsCanvasObj.transform.SetParent(transform, false);
        ChatOpsCanvas = ChatOpsCanvasObj.AddComponent<Canvas>();
        ChatOpsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;  // 改为 Overlay 模式，不需要相机
        ChatOpsCanvas.sortingOrder = 100;  // 确保在最上层显示

        var ChatOpsCanvasScaler = ChatOpsCanvas.gameObject.GetComponent<CanvasScaler>() ?? ChatOpsCanvas.gameObject.AddComponent<CanvasScaler>();
        ChatOpsCanvasScaler.referencePixelsPerUnit = 100;
        ChatOpsCanvasScaler.matchWidthOrHeight = 1;
        ChatOpsCanvasScaler.referenceResolution = new Vector2(1920, 1080);
        ChatOpsCanvasScaler.scaleFactor = 1;
        ChatOpsCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        ChatOpsCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // 添加 GraphicRaycaster 以支持输入交互
        if (ChatOpsCanvasObj.GetComponent<GraphicRaycaster>() == null)
        {
            ChatOpsCanvasObj.AddComponent<GraphicRaycaster>();
        }

        var ChatOpsObj = new GameObject("ChatOps");
        ChatOpsObj.transform.SetParent(ChatOpsCanvas.transform, false);
        ChatOpsObj.AddComponent<PeakOpsUI>();

        DevLog.UI($"Chat interface created successfully - Canvas mode: {ChatOpsCanvas.renderMode}, sorting order: {ChatOpsCanvas.sortingOrder}");

        

    }

    [HarmonyPatch("LateUpdate")]
    [HarmonyPostfix]
    public static void LateUpdatePostfix(GUIManager __instance)
    {
        if (isHUDActive != __instance.hudCanvas.gameObject.activeInHierarchy)
        {
            isHUDActive = __instance.hudCanvas.gameObject.activeInHierarchy;
            ChatOpsCanvas.gameObject.SetActive(isHUDActive);
        }
    }

    [HarmonyPatch("UpdatePaused")]
    [HarmonyPrefix]
    public static bool UpdatePausedPrefix(GUIManager __instance)
    {
        if (PeakOpsUI.instance?.framesSinceInputBlocked <= 1 && !__instance.pauseMenu.activeSelf)
        {
            return false;
        }
        return true;
    }
}

public class BarGroupChildWatcher : MonoBehaviour
{
    public Transform ChatOpsDummyTransform = null!;

    void OnTransformChildrenChanged()
    {
        if (ChatOpsDummyTransform != null && ChatOpsDummyTransform.GetSiblingIndex() > 0)
        {
            ChatOpsDummyTransform.SetAsFirstSibling();
        }
    }
}