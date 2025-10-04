using HarmonyLib;
using UnityEngine;
using PeakChatOps.Core;
using PeakChatOps.UI;

namespace PeakChatOps.Patches;

[HarmonyPatch(typeof(GUIManager))]
public static class GUIManagerPatch
{
    public static GameObject PeakChatOpsUIGO = null!;
    public static PeakChatOpsUI PeakChatOpsUIInstance = null!;

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(GUIManager __instance)
    {
        DevLog.UI("GUIManager.Start patch executed - creating UI Toolkit interface");

        try
        {
            // 获取 GUIManager 的 transform 作为父对象
            var parentTransform = __instance.transform;

            
            // 实例化 prefab 并挂载
            if (PeakChatOpsPlugin.PeakChatOpsUIPrefab != null)
            {
                PeakChatOpsUIGO = Object.Instantiate(PeakChatOpsPlugin.PeakChatOpsUIPrefab, parentTransform);
                PeakChatOpsUIGO.name = "PeakChatOpsUI";
                DevLog.File("✅ PeakChatOpsUI prefab instantiated.");
                
                // 获取或添加 PeakChatOpsUI 组件
                PeakChatOpsUIInstance = PeakChatOpsUIGO.GetComponent<PeakChatOpsUI>();
                if (PeakChatOpsUIInstance == null)
                {
                    PeakChatOpsUIInstance = PeakChatOpsUIGO.AddComponent<PeakChatOpsUI>();
                    DevLog.File("Added PeakChatOpsUI component to prefab.");
                }
                
                // 初始化 UI 事件绑定
                PeakChatOpsUIInstance.BindUIEvents(PeakChatOpsUIGO);
                DevLog.File("✅ UI events bound successfully.");
            }
            else
            {
                DevLog.File("⚠️ PeakChatOpsUIPrefab is null, skipping UI creation.");
            }

        }
        catch (System.Exception ex)
        {
            DevLog.File($"Error creating PeakChatOps UI: {ex.Message}");
            DevLog.File($"Stack trace: {ex.StackTrace}");
        }
    }
}

