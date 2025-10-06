using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using PEAKLib.Core;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

using Cysharp.Threading.Tasks;

namespace TerrainScanner;

[BepInAutoPlugin]
[BepInDependency(CorePlugin.Id)]
public partial class TerrainScannerPlugin : BaseUnityPlugin
{
    public static TerrainScannerPlugin Instance;
    internal static new ManualLogSource Logger;
    public ConfigEntry<KeyCode> configActivationKey;
    // Style/config entries for ScanFeature
    public ConfigEntry<string> configScanColorHead;
    public ConfigEntry<string> configScanColor;
    public ConfigEntry<float> configOutlineWidth;
    public ConfigEntry<float> configScanLineWidth;
    public ConfigEntry<float> configScanLineInterval;
    public ConfigEntry<float> configHeadScanLineWidth;
    // more style/config entries
    public ConfigEntry<float> configScanLineBrightness;
    public ConfigEntry<float> configScanRange;
    public ConfigEntry<float> configOutlineBrightness;
    public ConfigEntry<float> configHeadScanLineDistance;
    public ConfigEntry<string> configScanCenterWS;
    public ConfigEntry<float> configOutlineStarDistance;

    // track the active ScanFeature instance we created/configured
    ScanFeature activeScanFeature = null;

    // 声明加载的资源变量
    private Material scanMaterial;
    private Material markMaterial;
    
    private GameObject markParticle1;
    private GameObject markParticle2;
    private GameObject markParticle3;
    
    private bool assetsLoaded = false;
    private bool scanFeatureInitialized = false;

    void Awake()
    {
        Instance = this;
        Logger = base.Logger;

        // 初始化 UniTask PlayerLoop 系统
        try
        {
            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);
            Logger.LogInfo("[INFO] UniTask PlayerLoop initialized successfully.");
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"[WARN] UniTask PlayerLoop initialization failed: {ex.Message}");
        }

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
                peakBundle.GetAllAssetNames().ToList().ForEach(assetName =>
                {
                    Logger.LogInfo($"[DEBUG] Found asset: {assetName}");
                });
                #endif
                // 保存加载的资源到类变量
                scanMaterial = peakBundle.LoadAsset<Material>("Assets/Material/Scan.mat");
                // 不再从 bundle 加载 ParticleTerrainMark.mat — 我们直接使用 bundle 中的 TerrianMarks.mat（instanced shader material）

                // Bundle 必含 TerrianMarks.mat，直接加载并使用它作为 markMaterial
                markMaterial = peakBundle.LoadAsset<Material>("Assets/Shader/TerrianMarks.mat");
                markParticle1 = peakBundle.LoadAsset<GameObject>("Assets/Particles/LightParticle1.prefab");
                markParticle2 = peakBundle.LoadAsset<GameObject>("Assets/Particles/LightParticle2.prefab");
                markParticle3 = peakBundle.LoadAsset<GameObject>("Assets/Particles/LightParticle3.prefab");


                // 检查是否为空
                if (scanMaterial == null)
                {
                    Logger.LogError("[ERROR] Scan material failed to load from AssetBundle.");
                    return;
                }
                // Bundle 必含 TerrianMarks.mat，直接赋值并进行空检查
                if (markMaterial == null)
                {
                    Logger.LogError("[ERROR] TerrianMarks.mat failed to load from AssetBundle.");
                    return;
                }

                Logger.LogInfo("[INFO] Loaded TerrianMarks.mat from bundle and assigned as markMaterial.");
                if (markParticle1 == null || markParticle2 == null || markParticle3 == null)
                {
                    Logger.LogError("[ERROR] One or more mark particles failed to load from AssetBundle.");
                    return;
                }

                Logger.LogInfo("[INFO] All assets loaded successfully from AssetBundle.");
                assetsLoaded = true;
                
                // 如果已经在场景中，立即初始化 ScanFeature
                if (Camera.main != null)
                {
                    InitializeScanFeature();
                }
            }
        );

    }

public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    Camera mainCamera = Camera.main;
    if (mainCamera != null)
    {
        mainCamera.gameObject.AddComponent<ActiveScan>();
        
        // 只有在资源已加载且 ScanFeature 未初始化时才进行初始化
        if (assetsLoaded && !scanFeatureInitialized)
        {
            InitializeScanFeature();
        }
    }
}

private void InitializeScanFeature()
{
    // 确保资源已加载
    if (!assetsLoaded)
    {
        Logger.LogWarning("[WARN] Assets not loaded yet. Waiting for AssetBundle to finish loading...");
        return;
    }
    
    // 防止重复初始化
    if (scanFeatureInitialized)
    {
        Logger.LogInfo("[DEBUG] ScanFeature already initialized. Skipping...");
        return;
    }
    
    var pipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
    if (pipelineAsset == null)
    {
        Logger.LogError("[ERROR] UniversalRenderPipelineAsset is null!");
        return;
    }

    // 获取 UniversalRendererData
    var rendererDataList = typeof(UniversalRenderPipelineAsset)
        .GetField("m_RendererDataList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
    if (rendererDataList == null)
    {
        Logger.LogError("[ERROR] Unable to access m_RendererDataList via reflection.");
        return;
    }

    var rendererDataArray = rendererDataList.GetValue(pipelineAsset) as ScriptableRendererData[];
    if (rendererDataArray == null || rendererDataArray.Length == 0)
    {
        Logger.LogError("[ERROR] RendererData array is null or empty.");
        return;
    }

    var rendererData = rendererDataArray[0] as UniversalRendererData;
    if (rendererData == null)
    {
        Logger.LogError("[ERROR] First renderer is not UniversalRendererData.");
        return;
    }

    // Diagnostic: list renderer features (helpful to detect same-named but different-assembly types)
    try {
        var features = rendererData.rendererFeatures;
        Logger.LogInfo($"[DEBUG] rendererFeatures count={features?.Count ?? 0}");
        if (features != null) {
            for (int i = 0; i < features.Count; i++) {
                var f = features[i];
                Logger.LogInfo($"[DEBUG] rendererFeature[{i}] name={(f!=null?f.name:"null")} type={(f!=null?f.GetType().FullName:"null")}");
            }
        }
    } catch (Exception ex) { Logger.LogWarning($"[WARN] Failed to enumerate rendererFeatures: {ex.Message}"); }

    // 检查是否已经存在 ScanFeature
    foreach (var feature in rendererData.rendererFeatures)
    {
        if (feature is ScanFeature existingScanFeature)
        {
            Logger.LogInfo("[DEBUG] ScanFeature already exists. Configuring...");
            ConfigureScanFeature(existingScanFeature);
            existingScanFeature.Create();
            activeScanFeature = existingScanFeature;
            ApplyStyleConfigNow();
            scanFeatureInitialized = true;
            return;
        }
    }

    // 创建新的 ScanFeature
    var scanFeature = ScriptableObject.CreateInstance<ScanFeature>();
    scanFeature.name = "TerrainScanner_ScanFeature";
    
    // 配置 ScanFeature（在 Create 之前）
    ConfigureScanFeature(scanFeature);
    
    // 添加到渲染器
    rendererData.rendererFeatures.Add(scanFeature);
    Logger.LogInfo("[SUCCESS] ScanFeature created and added to renderer!");
    
    // 标记为脏数据以触发更新
    #if UNITY_EDITOR
    UnityEditor.EditorUtility.SetDirty(rendererData);
    #endif
    
    // 手动调用 Create 方法
    scanFeature.Create();
    activeScanFeature = scanFeature;
    ApplyStyleConfigNow();
    scanFeatureInitialized = true;
    Logger.LogInfo("[SUCCESS] ScanFeature initialized!");
}


#region 配置
    private void setupConfig()
    {
        configActivationKey = Config.Bind("Controls", "ActivationKey", KeyCode.Q, "The key to toggle the terrain scanner.");
        configActivationKey.SettingChanged += OnActivationKeyChanged;
        // 样式配置（颜色用 "r,g,b,a" 字符串表示，scanCenterWS 用 "x,y,z"）
        configScanColorHead = Config.Bind("Style", "ScanColorHead", "0,0,1,1", "Scan head color as r,g,b,a");
        configScanColor = Config.Bind("Style", "ScanColor", "0,0,1,1", "Scan color as r,g,b,a");
        configOutlineWidth = Config.Bind("Style", "OutlineWidth", 0.1f, "Outline width");
        configScanLineWidth = Config.Bind("Style", "ScanLineWidth", 1f, "Scan line width");
        configScanLineInterval = Config.Bind("Style", "ScanLineInterval", 1f, "Scan line interval");
        configHeadScanLineWidth = Config.Bind("Style", "HeadScanLineWidth", 1f, "Head scan line width");

        configScanLineBrightness = Config.Bind("Style", "ScanLineBrightness", 1f, "Scan line brightness");
        configScanRange = Config.Bind("Style", "ScanRange", 1f, "Scan range");
        configOutlineBrightness = Config.Bind("Style", "OutlineBrightness", 1f, "Outline brightness");
        configHeadScanLineDistance = Config.Bind("Style", "HeadScanLineDistance", 8f, "Head scan line distance");
        configScanCenterWS = Config.Bind("Style", "ScanCenterWS", "123.05,36.3,147.86", "Scan center world-space as x,y,z");
        configOutlineStarDistance = Config.Bind("Style", "OutlineStarDistance", 30f, "Outline star distance");

    // subscribe to changes
    configScanColorHead.SettingChanged += ApplyStyleConfig;
    configScanColor.SettingChanged += ApplyStyleConfig;
    configOutlineWidth.SettingChanged += ApplyStyleConfig;
    configScanLineWidth.SettingChanged += ApplyStyleConfig;
    configScanLineInterval.SettingChanged += ApplyStyleConfig;
    configHeadScanLineWidth.SettingChanged += ApplyStyleConfig;
    configScanLineBrightness.SettingChanged += ApplyStyleConfig;
    configScanRange.SettingChanged += ApplyStyleConfig;
    configOutlineBrightness.SettingChanged += ApplyStyleConfig;
    configHeadScanLineDistance.SettingChanged += ApplyStyleConfig;
    configScanCenterWS.SettingChanged += ApplyStyleConfig;
    configOutlineStarDistance.SettingChanged += ApplyStyleConfig;

    }

    void ApplyStyleConfig(object sender, EventArgs e)
    {
        if (activeScanFeature == null) return;
        try {
            // parse colors and vector
            Color ParseColor(string s) {
                var parts = s.Split(',');
                if (parts.Length < 3) return Color.blue;
                float r = float.Parse(parts[0]); float g = float.Parse(parts[1]); float b = float.Parse(parts[2]); float a = parts.Length>=4 ? float.Parse(parts[3]) : 1f;
                return new Color(r,g,b,a);
            }
            Vector3 ParseVec3(string s) {
                var parts = s.Split(',');
                if (parts.Length < 3) return new Vector3(0,0,0);
                float x = float.Parse(parts[0]); float y = float.Parse(parts[1]); float z = float.Parse(parts[2]);
                return new Vector3(x,y,z);
            }

            activeScanFeature.settings.scanColorHead = ParseColor(configScanColorHead.Value);
            activeScanFeature.settings.scanColor = ParseColor(configScanColor.Value);
            activeScanFeature.settings.outlineWidth = configOutlineWidth.Value;
            activeScanFeature.settings.scanLineWidth = configScanLineWidth.Value;
            activeScanFeature.settings.scanLineInterval = configScanLineInterval.Value;
            activeScanFeature.settings.headScanLineWidth = configHeadScanLineWidth.Value;
            activeScanFeature.settings.scanLineBrightness = configScanLineBrightness.Value;
            activeScanFeature.settings.scanRange = configScanRange.Value;
            activeScanFeature.settings.outlineBrightness = configOutlineBrightness.Value;
            activeScanFeature.settings.headScanLineDistance = configHeadScanLineDistance.Value;
            activeScanFeature.settings.scanCenterWS = ParseVec3(configScanCenterWS.Value);
            activeScanFeature.settings.outlineStarDistance = configOutlineStarDistance.Value;
        } catch { }
    }

    void ApplyStyleConfigNow() {
        ApplyStyleConfig(this, EventArgs.Empty);
    }

    private void OnActivationKeyChanged(object sender, EventArgs e)
    {
        Logger.LogInfo($"[INFO] Activation key changed to: {configActivationKey.Value}");
    }

    void OnDestroy()
    {
    if (configActivationKey != null) configActivationKey.SettingChanged -= OnActivationKeyChanged;
    if (configScanColorHead != null) configScanColorHead.SettingChanged -= ApplyStyleConfig;
    if (configScanColor != null) configScanColor.SettingChanged -= ApplyStyleConfig;
    if (configOutlineWidth != null) configOutlineWidth.SettingChanged -= ApplyStyleConfig;
    if (configScanLineWidth != null) configScanLineWidth.SettingChanged -= ApplyStyleConfig;
    if (configScanLineInterval != null) configScanLineInterval.SettingChanged -= ApplyStyleConfig;
    if (configHeadScanLineWidth != null) configHeadScanLineWidth.SettingChanged -= ApplyStyleConfig;
    if (configScanLineBrightness != null) configScanLineBrightness.SettingChanged -= ApplyStyleConfig;
    if (configScanRange != null) configScanRange.SettingChanged -= ApplyStyleConfig;
    if (configOutlineBrightness != null) configOutlineBrightness.SettingChanged -= ApplyStyleConfig;
    if (configHeadScanLineDistance != null) configHeadScanLineDistance.SettingChanged -= ApplyStyleConfig;
    if (configScanCenterWS != null) configScanCenterWS.SettingChanged -= ApplyStyleConfig;
    if (configOutlineStarDistance != null) configOutlineStarDistance.SettingChanged -= ApplyStyleConfig;
    }


#endregion

    public void ConfigureScanFeature(ScanFeature scanFeature)
    {
        // 配置 ScanFeature 的设置
        scanFeature.settings.scanMaterial = scanMaterial;
        // 选择 markMaterial：优先使用 bundle 中的 TerrianMarks.mat（如果 shader 匹配），否则尝试本地回退
        Material assignedMark = markMaterial;
        bool bundleOk = false;
        if (assignedMark != null && assignedMark.shader != null && assignedMark.shader.name == "TerrianMarks") bundleOk = true;
        if (!bundleOk) {
            // try local shader fallback and copy a few properties from bundle material if present
            var localShader = Shader.Find("TerrianMarks");
            if (localShader != null) {
                var preferMat = new Material(localShader);
                if (assignedMark != null) {
                    if (assignedMark.HasProperty("_SafeColor")) try { preferMat.SetColor("_SafeColor", assignedMark.GetColor("_SafeColor")); } catch { }
                    if (assignedMark.HasProperty("_WarningColor")) try { preferMat.SetColor("_WarningColor", assignedMark.GetColor("_WarningColor")); } catch { }
                    if (assignedMark.HasProperty("_DangerColor")) try { preferMat.SetColor("_DangerColor", assignedMark.GetColor("_DangerColor")); } catch { }
                    if (assignedMark.HasProperty("_IconSize")) try { preferMat.SetFloat("_IconSize", assignedMark.GetFloat("_IconSize")); } catch { }
                }
                preferMat.enableInstancing = true;
                assignedMark = preferMat;
            }
        } else {
            if (assignedMark != null) assignedMark.enableInstancing = true;
        }
        scanFeature.settings.markMaterial = assignedMark;
        scanFeature.settings.markParticle1 = markParticle1;
        scanFeature.settings.markParticle2 = markParticle2;
        scanFeature.settings.markParticle3 = markParticle3;
        scanFeature.settings.scanColor = Color.green;
        scanFeature.settings.scanRange = 15f;

        Debug.Log("ScanFeature 配置完成！");
    }


}