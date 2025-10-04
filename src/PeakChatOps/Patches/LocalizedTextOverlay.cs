using System.Collections.Generic;
using System.IO;

using HarmonyLib;
using BepInEx;

namespace PeakChatOps.Patches;

[HarmonyPatch(typeof(LocalizedText))]
public static class LocalizationPatches
{
    public static Dictionary<string, List<string>> MOD_TABLE;

    [HarmonyPatch("LoadMainTable")]
    [HarmonyPostfix]
    public static void LoadMainTablePostfix(bool forceSerialization)
    {

        string pluginDir = Paths.PluginPath;
        var csvPath = Path.Combine(pluginDir, PeakChatOpsPlugin.Name, "Localization.csv");

        if (!File.Exists(csvPath))
        {
            return;
        }

        LoadModTable(csvPath);

        
    }
    public static void LoadModTable(string csvPath)
    {

        MOD_TABLE = CSVReader.SplitCsvDict(File.ReadAllText(csvPath));
        PeakChatOpsPlugin.Logger.LogInfo($"MOD_TABLE loaded, count: {MOD_TABLE.Count}");
        foreach (var kvp in MOD_TABLE)
        {
            PeakChatOpsPlugin.Logger.LogInfo($"MOD_TABLE key: {kvp.Key}, list count: {kvp.Value.Count}");
        }
        
        // 合并到 mainTable
        var mainTable = LocalizedText.mainTable;
        foreach (var kvp in MOD_TABLE)
        {
            mainTable[kvp.Key] = kvp.Value;
        }
    }

}