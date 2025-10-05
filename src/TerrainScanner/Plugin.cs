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
namespace TerrainScanner;

[BepInAutoPlugin]
[BepInDependency(CorePlugin.Id)]
public partial class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger = null!;
    internal static Scene currentScene;
    internal static MainCamera? mainCamera;
    internal static bool isScanning;
    // 扫描参数
    internal static float scanStartTime;
    internal static Vector3 scanCenter = Vector3.zero;

    internal static float scanSpeed = 20f; // 扫描波扩散速度
    internal static float scanWidth = 5f;  // 扫描波宽度
    
    // 后处理材质
    internal static Material? scanMaterial;
    
    private ConfigEntry<KeyCode> configActivationKey = null!;
    
    private void Awake()
    {
        Logger = base.Logger;
        
        configActivationKey = Config.Bind("General", "ActivationKey", KeyCode.Q);

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

                // 直接从 AssetBundle 加载材质
                scanMaterial = peakBundle.LoadAsset<Material>("Assets/MOD/TerrainScanMaterial.mat");
                
                if (scanMaterial != null)
                {
                    Logger.LogInfo($"Successfully loaded material: {scanMaterial.name} with shader: {scanMaterial.shader.name}");
                }
                else
                {
                    Logger.LogError("Failed to load TerrainScanMaterial from AssetBundle");
                }
            }
        );
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Logger.LogInfo($"Loaded TerrainScanner version {Version}!");
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        currentScene = scene;
        
        if (!currentScene.name.StartsWith("Level_"))
        {
            mainCamera = null;
            isScanning = false;
        }
        else
        {
            mainCamera = null;
        }
    }
    
    private void Update()
    {
        if (currentScene.name.StartsWith("Level_"))
        {
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<MainCamera>();
                if (mainCamera != null)
                {
                    // 启用相机深度图和法线图
                    Camera cam = mainCamera.GetComponent<Camera>();
                    if (cam != null)
                    {
                        cam.depthTextureMode |= DepthTextureMode.DepthNormals;
                        
                        // 添加扫描效果组件到相机
                        var scanEffect = cam.gameObject.GetComponent<TerrainScanEffect>();
                        if (scanEffect == null)
                        {
                            scanEffect = cam.gameObject.AddComponent<TerrainScanEffect>();
                            Logger.LogInfo("Added TerrainScanEffect component to camera");
                        }
                    }
                }
                return;
            }
            
            CheckHotkeys();
        }
    }
    
    private void CheckHotkeys()
    {
        if (Input.GetKeyDown(configActivationKey.Value))
        {
            StartScan();
        }
    }
    
    private void StartScan()
    {
        if (mainCamera == null) return;
        
        // 从相机位置开始扫描
        scanCenter = mainCamera.transform.position;
        scanStartTime = Time.time;
        isScanning = true;
        
        Logger.LogInfo($"Scan started at {scanCenter}");
    }
}

// URP Scriptable Render Pass 用于扫描后处理效果 (Unity 6 RenderGraph API)
public class TerrainScanRenderPass : ScriptableRenderPass
{
    private Material scanMaterial;
    private static bool loggedOnce = false;
    
    // RenderGraph 需要的数据类
    private class PassData
    {
        internal Material material;
        internal Vector3 scanCenter;
        internal float scanRange;
        internal float scanWidth;
        internal Matrix4x4 camToWorld;
        internal Vector3 camPos;
        internal float camFar;
        internal TextureHandle source;
    }
    
    public TerrainScanRenderPass(Material material)
    {
        scanMaterial = material;
        // 在后处理之后渲染
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        Plugin.Logger.LogInfo("[DEBUG] TerrainScanRenderPass created");
    }
    
    // Unity 6 RenderGraph API - 替代 Execute 方法
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (!Plugin.isScanning || scanMaterial == null)
        {
            return;
        }
        
        // 调试:记录第一次调用
        if (!loggedOnce)
        {
            Plugin.Logger.LogInfo("[DEBUG] RecordRenderGraph called!");
            loggedOnce = true;
        }
        
        // 计算当前扫描半径
        float scanRange = (Time.time - Plugin.scanStartTime) * Plugin.scanSpeed;
        
        // 扫描超过一定范围后停止
        if (scanRange > 100f)
        {
            Plugin.isScanning = false;
            Plugin.Logger.LogInfo("[DEBUG] Scan finished (range > 100)");
            return;
        }
        
        // 获取 Universal Resource Data (包含 color texture)
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        
        // 使用双 Pass 方式：source -> temp (用材质处理) -> source (拷贝回去)
        TextureHandle source = resourceData.activeColorTexture;
        
        // 创建临时纹理
        RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;
        TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_TempScanTexture", false);
        
        // Pass 1: 从 source 渲染到 temp，应用扫描效果
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("TerrainScanEffect", out var passData))
        {
            passData.material = scanMaterial;
            passData.scanCenter = Plugin.scanCenter;
            passData.scanRange = scanRange;
            passData.scanWidth = Plugin.scanWidth;
            passData.camToWorld = cameraData.camera.cameraToWorldMatrix;
            passData.camPos = cameraData.camera.transform.position;
            passData.camFar = cameraData.camera.farClipPlane;
            passData.source = source;
            
            // 读取 source，写入 temp
            builder.UseTexture(source, AccessFlags.Read);
            builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);
            
            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                // 更新 shader 参数
                data.material.SetVector("_ScanCenter", data.scanCenter);
                data.material.SetFloat("_ScanRange", data.scanRange);
                data.material.SetFloat("_ScanWidth", data.scanWidth);
                data.material.SetMatrix("_CamToWorld", data.camToWorld);
                data.material.SetVector("_CamWorldPos", data.camPos);
                data.material.SetFloat("_CamFar", data.camFar);
                
                // 使用 Blitter.BlitTexture 自动绑定 _BlitTexture
                Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
            });
        }
        
        // Pass 2: 从 temp 拷贝回 source
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("CopyBack", out var passData))
        {
            passData.source = tempTexture;
            
            builder.UseTexture(tempTexture, AccessFlags.Read);
            builder.SetRenderAttachment(source, 0, AccessFlags.Write);
            
            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                // 简单拷贝：从 temp 到 source
                Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0f, false);
            });
        }
    }
}

// MonoBehaviour 组件,用于将 RenderPass 注入到相机
public class TerrainScanEffect : MonoBehaviour
{
    private Camera _targetCamera;
    private TerrainScanRenderPass _renderPass;
    
    private void Start()
    {
        _targetCamera = GetComponent<Camera>();
        if (_targetCamera == null)
        {
            Plugin.Logger.LogError("[TerrainScanEffect] No camera found!");
            return;
        }
        
        // 创建 RenderPass
        if (Plugin.scanMaterial != null)
        {
            _renderPass = new TerrainScanRenderPass(Plugin.scanMaterial);
            Plugin.Logger.LogInfo("[TerrainScanEffect] Created ScriptableRenderPass");
        }
        
        // 订阅相机渲染事件
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        Plugin.Logger.LogInfo("[TerrainScanEffect] Subscribed to RenderPipelineManager.beginCameraRendering");
    }
    
    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    
    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        // 只对主相机应用
        if (camera != _targetCamera)
            return;
        if (_renderPass == null)
            return;
        // 将 RenderPass 加入渲染队列
        if (camera.TryGetComponent<UniversalAdditionalCameraData>(out var cameraData))
        {
            _renderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            cameraData.scriptableRenderer.EnqueuePass(_renderPass);
        }
    }
}