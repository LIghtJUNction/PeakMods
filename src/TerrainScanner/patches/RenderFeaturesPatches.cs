
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.Rendering.Universal;

namespace TerrainScanner.patches;

[HarmonyPatch(typeof(ScriptableRenderer), "rendererFeatures", MethodType.Getter)]
class RendererFeaturesPatch
{
    static void Postfix(ScriptableRenderer __instance, ref List<ScriptableRendererFeature> __result)
    {
        // 在这里可以访问 rendererFeatures
        foreach (var feature in __result)
        {
            TerrainScannerPlugin.Logger.LogInfo($"[DEBUG] Found feature of type: {feature.GetType()}");
            if (feature is ScanFeature scanFeature)
            {
                TerrainScannerPlugin.Logger.LogInfo($"[DEBUG] rendererFeatures count: {__result.Count}");
                // 在这里可以配置 ScanFeature
                TerrainScannerPlugin.Logger.LogInfo("[DEBUG] Found ScanFeature in renderer features.");
                TerrainScannerPlugin.Instance.ConfigureScanFeature(scanFeature);
            }
        }
    }
}