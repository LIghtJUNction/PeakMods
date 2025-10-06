using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.SceneManagement;
using PEAKLib.Core;
using HarmonyLib;
using TerrainScanner.patches;

namespace TerrainScanner;

[BepInAutoPlugin]
[BepInDependency(CorePlugin.Id)]
public partial class TerrainScannerPlugin : BaseUnityPlugin
{
    public static TerrainScannerPlugin Instance;
    internal static new ManualLogSource Logger;
    private ConfigEntry<KeyCode> configActivationKey;

    // 声明加载的资源变量
    private Material scanMaterial;
    private Material markMaterial;
    private GameObject markParticle1;
    private GameObject markParticle2;
    private GameObject markParticle3;

    void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        var harmony = new Harmony("terrainscanner");
        harmony.PatchAll(typeof(RendererFeaturesPatch));

        // load config
        setupConfig();

        LoadPeakBundle();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadPeakBundle()
    {

        // 加载 AssetBundle 并加载扫描材质
        this.LoadBundleWithName(
            "TerrainScanner.peakbundle",
            peakBundle =>
            {
#if DEBUG
                peakBundle.GetAllAssetNames().ToList().ForEach(name =>
                {
                    Logger.LogInfo($"[DEBUG] Found asset: {name}");
                });
#endif

                // 保存加载的资源到类变量
                scanMaterial = peakBundle.LoadAsset<Material>("Assets/Material/Scan.mat");
                markMaterial = peakBundle.LoadAsset<Material>("Assets/Material/ParticleTerrainMark.mat");
                markParticle1 = peakBundle.LoadAsset<GameObject>("Assets/Particles/LightParticle1.prefab");
                markParticle2 = peakBundle.LoadAsset<GameObject>("Assets/Particles/LightParticle2.prefab");
                markParticle3 = peakBundle.LoadAsset<GameObject>("Assets/Particles/LightParticle3.prefab");

                Logger.LogInfo("[INFO] All assets loaded successfully from AssetBundle.");

            }
        );

    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera = mainCamera;
            ActiveScan activeScan = mainCamera.gameObject.AddComponent<ActiveScan>();

        }
    }

    private void setupConfig()
    {
        configActivationKey = Config.Bind("General", "ActivationKey", KeyCode.T, "The key to toggle the terrain scanner.");
    }

    public void ConfigureScanFeature(ScanFeature scanFeature)
    {
        // 配置 ScanFeature 的设置
        scanFeature.settings.scanMaterial = scanMaterial;
        scanFeature.settings.markMaterial = markMaterial;
        scanFeature.settings.markParticle1 = markParticle1;
        scanFeature.settings.markParticle2 = markParticle2;
        scanFeature.settings.markParticle3 = markParticle3;
        scanFeature.settings.scanColor = Color.green;
        scanFeature.settings.scanRange = 15f;

        Debug.Log("ScanFeature 配置完成！");
    }


}