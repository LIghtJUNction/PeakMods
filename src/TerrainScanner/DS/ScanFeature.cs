using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TerrainScanner {
    public class ScanFeature : ScriptableRendererFeature {
        [Serializable]
        public class Settings {
            public RenderPassEvent renderEvent = RenderPassEvent.BeforeRenderingTransparents;
            [FormerlySerializedAs("scanShader")]
            public Material scanMaterial;

            [Header("Static Settings")]
            public Color scanColorHead = new Color(0.054901965f, 0.5686275f, 0.85098046f, 1f);
            public Color scanColor = new Color(0.38823533f, 0.7372549f, 0.8705883f, 1f);
            public float outlineWidth = 2.48f;
            public float scanLineWidth = 1f;
            public float scanLineInterval = 1f;
            public float headScanLineWidth = 1f;

            [Header("Dynamics Settings(control by code)")]
            public float scanLineBrightness = 2.5f;
            public float scanRange = 5f;
            public float outlineBrightness = 1.32f;
            public float headScanLineDistance = 13.2f;
            public Vector3 scanCenterWS = new Vector3(123.05f, 36.3f, 147.86f);
            public float outlineStarDistance = 30f;

            [Header("Render Mark")]
            public Material markMaterial;
            public GameObject markParticle3;
            public GameObject markParticle2;
            public GameObject markParticle1;
            [Header("Particle probabilities")]
            // 生成粒子的概率，可在 Inspector 中调整
            public float steepSpawnProb = 0.1f;   // 对应陡坡 (category 3)
            public float midSpawnProb = 0.3f;  // 对应中等坡 (category 2)
            public float flatSpawnProb = 0.0002f; // 对应平地 (category 1)
        }

        public Settings settings = new Settings();

        static ScanFeature _instance;
        CustomRenderPass _myPass;

        // diagnostics
        static bool s_reportedDepthHandle = false;
        static bool s_reportedMarkState = false;
        static int s_scanCounter = 0;
        static int s_lastReportedScanId = -1;

        // (DedupLog removed) only error logs are kept; lightweight diagnostics remain above.

        public static void ExecuteScan(Transform player) {
            StartScan(player).Forget();
        }

        static async UniTaskVoid StartScan(Transform player) {
            if (!Application.isPlaying) {
                TerrainScannerPlugin.Logger?.LogWarning("[WARN] Cannot scan: not playing");
                return;
            }
            if (!canScan) {
                TerrainScannerPlugin.Logger?.LogInfo("[INFO] Scan already in progress");
                return;
            }
            if (_instance == null) {
                TerrainScannerPlugin.Logger?.LogError("[ERROR] ScanFeature instance is null. Ensure it is properly initialized.");
                return;
            }
            if (_instance.settings == null) {
                TerrainScannerPlugin.Logger?.LogError("[ERROR] ScanFeature settings is null.");
                return;
            }
            if (_instance.settings.scanMaterial == null) {
                TerrainScannerPlugin.Logger?.LogError("[ERROR] ScanFeature scanMaterial is null. Assets may not be loaded yet.");
                return;
            }
            canScan = false;
            showMark = true;
            markTween?.Kill();

            var scanCenter = player.position - player.forward * 2;
            var material = _instance.settings.scanMaterial;
            var markMaterial = _instance.settings.markMaterial;
            if (material != null) {
                material.SetVector(ScanCenterWs, scanCenter);
                material.SetFloat(HeadScanLineDistance, 4);
                var tween1 = material.DOFloat(250, HeadScanLineDistance, 3.5f).SetEase(Ease.InSine);
                if (tween1 != null) tween1.onComplete += () => { canScan = true; };
                material.SetFloat(ScanRange, 1);
                material.DOFloat(5, ScanRange, 1.5f).SetEase(Ease.InSine).SetDelay(1);
                material.SetFloat(ScanLineBrightness, 0.3f);
                material.SetFloat(HeadScanLineBrightness, 0);
                material.DOFloat(1, ScanLineBrightness, 0.2f).SetDelay(0.25f);
                material.DOFloat(1, HeadScanLineBrightness, 0.1f).SetDelay(0.25f);
                material.DOFloat(0, ScanLineBrightness, 0.5f).SetDelay(2.25f).SetEase(Ease.Linear);
                material.DOFloat(0, HeadScanLineBrightness, 0.5f).SetDelay(2.25f).SetEase(Ease.Linear);
                material.SetFloat(OutlineBrightness, 1);
                material.SetFloat(OutlineStarDistance, 0);
                material.DOFloat(0, OutlineBrightness, 0.5f).SetDelay(2.25f).SetEase(Ease.Linear);
                material.DOFloat(30, OutlineStarDistance, 1f).SetEase(Ease.InCubic);
            }
            if (markMaterial != null) {
                markMaterial.SetFloat(ColorAlpha, 0);
                var tween9 = markMaterial.DOFloat(1, ColorAlpha, 1f);
                markTween = markMaterial.DOFloat(0, ColorAlpha, 1f).SetDelay(7);
                if (markTween != null) markTween.onComplete += () => { showMark = false; };
            }

            int scanId = ++s_scanCounter;
            await GenerateTerrainMarks(player, scanId);
        }

        static ProfilerMarker _generateTerrainMarks = new ProfilerMarker("GenerateTerrainMarks");
        struct Marks { public Vector3 markPosition; public int markCategory; }
        static Marks[] _marks;
        // double-buffered completed marks for rendering; swap in when a scan finishesza
        static Marks[] _marksForRender;
    // player-marked road positions (store as integer grid keys) - thread-safe
    static readonly System.Collections.Concurrent.ConcurrentDictionary<long, byte> _playerRoads = new System.Collections.Concurrent.ConcurrentDictionary<long, byte>();
    const float RoadGridSize = 0.5f; // same as gridStep

        // Public API: mark a world position as road (player walked here)
        public static void MarkRoadAt(Vector3 worldPos) {
            int gx = Mathf.FloorToInt(worldPos.x / RoadGridSize);
            int gz = Mathf.FloorToInt(worldPos.z / RoadGridSize);
            long key = ((long)gx << 32) ^ (uint)gz;
            _playerRoads.TryAdd(key, 1);
            // logging removed per user request
        }

        public static void ClearRoadAt(Vector3 worldPos) {
            int gx = Mathf.FloorToInt(worldPos.x / RoadGridSize);
            int gz = Mathf.FloorToInt(worldPos.z / RoadGridSize);
            long key = ((long)gx << 32) ^ (uint)gz;
            _playerRoads.TryRemove(key, out _);
        }

        public static void ClearAllRoads() {
            _playerRoads.Clear();
        }
        const int horizontalCount = 70;
        const int verticalCount = 50;
        const float gridStep = 0.5f;
    // angle thresholds (use cosine of angle for normal.y comparisons)
    static readonly float Cos30 = Mathf.Cos(30f * Mathf.Deg2Rad); // ~0.8660254
    static readonly float Cos50 = Mathf.Cos(50f * Mathf.Deg2Rad); // ~0.6427876

        static void ShootParticle(Vector3 position, Vector3 normal, int index = 3) {
            try {
                float distanceToCamera01 = 1.0f;
                if (Camera.main != null) distanceToCamera01 = Vector3.Distance(position, Camera.main.transform.position) / 20 + 0.5f;
                GameObject prefab = null;
                if (_instance != null && _instance.settings != null) {
                    prefab = index switch { 3 => _instance.settings.markParticle3, 2 => _instance.settings.markParticle2, _ => _instance.settings.markParticle1 };
                }

                if (prefab == null) {
                    TerrainScannerPlugin.Logger?.LogWarning($"[ScanFeature] ShootParticle: prefab for index {index} is null, skipping particle.");
                    return;
                }

                GameObject instance = null;
                try {
                    instance = GameObject.Instantiate(prefab);
                } catch (Exception ex) {
                    TerrainScannerPlugin.Logger?.LogWarning($"[ScanFeature] ShootParticle: Instantiate failed: {ex.Message}");
                }

                if (instance == null) {
                    TerrainScannerPlugin.Logger?.LogWarning("[ScanFeature] ShootParticle: instantiated instance is null, skipping.");
                    return;
                }

                instance.transform.position = position;
                instance.transform.localScale = Random.Range(0.5f, 1.5f) * Vector3.one * distanceToCamera01;
                if (instance.transform.childCount > 0) {
                    try { instance.transform.GetChild(0).localScale = Random.Range(2f, 5f) * Vector3.one * distanceToCamera01; } catch { }
                }

                var ps = instance.GetComponentInChildren<ParticleSystem>();
                if (ps == null) {
                    TerrainScannerPlugin.Logger?.LogWarning($"[ScanFeature] ShootParticle: ParticleSystem not found in prefab '{prefab.name}' (index {index}). Destroying instance.");
                    try { GameObject.Destroy(instance); } catch { }
                    return;
                }

                try {
                    ps.Play();
                } catch (Exception ex) {
                    TerrainScannerPlugin.Logger?.LogWarning($"[ScanFeature] ShootParticle: ParticleSystem.Play() failed: {ex.Message}");
                }
            } catch (Exception ex) {
                TerrainScannerPlugin.Logger?.LogError($"[ScanFeature] ShootParticle: unexpected exception: {ex}");
            }
        }

        static async UniTask GenerateTerrainMarks(Transform player, int scanId) {
            Array.Clear(_marks, 0, _marks.Length);
            var forward = player.forward; var right = player.right;
            int maskScanRoad = LayerMask.GetMask("Scan", "Road");
            int maskScan = LayerMask.GetMask("Scan");
            if (maskScanRoad == 0) {
                // LayerMask for Scan+Road missing; fall back to default raycast layers
                maskScanRoad = Physics.DefaultRaycastLayers;
            }
            if (maskScan == 0) {
                // LayerMask for Scan missing; fall back to default raycast layers
                maskScan = Physics.DefaultRaycastLayers;
            }

            int totalHits = 0;
            int perRowSampleLimit = 3; int logRowLimit = 5;
            Vector3 startPos = player.position - forward * 2 + Vector3.up * 100;
            var rayCastPos = startPos - right * horizontalCount / 2 * gridStep - forward * (3 * gridStep);

            for (int i = 0; i < verticalCount; i++) {
                _generateTerrainMarks.Begin();
                int rowHits = 0; int rowSampleLogged = 0;
                for (int j = 0; j < horizontalCount; j++) {
                    Physics.Raycast(rayCastPos, Vector3.down, out RaycastHit hit, 300, maskScanRoad);
                    if (hit.collider == null) { rayCastPos += right * gridStep; continue; }
                    rowHits++; totalHits++;
                    var normal = hit.normal;
                    int computedCat = hit.collider != null ? (normal.y < Cos50 ? 3 : (normal.y < Cos30 ? 2 : 1)) : 0;
                    if (i < logRowLimit && rowSampleLogged < perRowSampleLimit) { /* debug sample log removed */ rowSampleLogged++; }
                    // if this hit position was previously marked as player road, treat as flat (category 1)
                    long cellKey = 0;
                    var hitPoint = hit.point;
                    if (hit.collider != null) {
                        var gx = Mathf.FloorToInt(hitPoint.x / RoadGridSize);
                        var gz = Mathf.FloorToInt(hitPoint.z / RoadGridSize);
                        cellKey = ((long)gx << 32) ^ (uint)gz;
                        if (_playerRoads.ContainsKey(cellKey)) {
                            _marks[i * horizontalCount + j].markCategory = 1;
                            _marks[i * horizontalCount + j].markPosition = hitPoint;
                            rayCastPos += right * gridStep;
                            continue;
                        }
                    }

                    if (hit.collider.isTrigger)
                    {
                        // 命中的是 Trigger：把该格记为类别 0（特殊/触发器），
                        // 尝试用 maskScan 做二次射线（以查找非触发器的真实表面），
                        // 只有二次射线命中时才使用 hit.point，否则位置设为 Vector3.zero（无有效位置）。
                        Physics.Raycast(rayCastPos, Vector3.down, out hit, 300, maskScan);
                        _marks[i * horizontalCount + j].markCategory = 0;
                        _marks[i * horizontalCount + j].markPosition = hit.collider != null ? hit.point : Vector3.zero;
                    }
                    else if (normal.y < Cos50)
                    {
                        // 类别 3：陡坡（坡度大于 50°，即 normal.y < cos(50°) ≈ 0.6428）
                        // 行为：将 markCategory 设为 3；以 10% 概率记录 markPosition 并发射粒子（使用 prefab index = 3）。
                        _marks[i * horizontalCount + j].markCategory = 3;
                        // use configured steep spawn probability (fallback to existing default if settings missing)
                        if (Random.Range(0f, 1f) < (_instance?.settings?.steepSpawnProb ?? 0.1f)) { _marks[i * horizontalCount + j].markPosition = hit.point; ShootParticle(hit.point, normal, 3); }
                    }
                    else if (normal.y < Cos30)
                    {
                        // 类别 2：中等坡（坡度在 30°~50° 之间，即 cos(50°) ≤ normal.y < cos(30°））
                        // 行为：将 markCategory 设为 2，始终记录 markPosition；以较大概率（0.3）发射粒子（index = 1）。
                        _marks[i * horizontalCount + j].markCategory = 2;
                        _marks[i * horizontalCount + j].markPosition = hit.point;
                        // use configured mid spawn probability
                        if (Random.Range(0f, 1f) < (_instance?.settings?.midSpawnProb ?? 0.3f)) ShootParticle(hit.point, normal, 1);
                    }
                    else
                    {
                        // 类别 1：平地 / 缓坡（坡度 ≤ 30°，即 normal.y ≥ cos(30°) ≈ 0.8660）
                        // 行为：将 markCategory 设为 1，始终记录 markPosition；以极小概率（0.0002）发射稀有粒子（index = 1）。
                        _marks[i * horizontalCount + j].markCategory = 1;
                        _marks[i * horizontalCount + j].markPosition = hit.point;
                        // use configured flat spawn probability
                        if (Random.Range(0f, 1f) < (_instance?.settings?.flatSpawnProb ?? 0.0002f)) ShootParticle(hit.point, normal, 1);
                    }

                    rayCastPos += right * gridStep;
                }
                _generateTerrainMarks.End();
                rayCastPos -= right * horizontalCount * gridStep; rayCastPos += forward * gridStep;
                await UniTask.Yield();
                if (i < 3 && rowHits > 0) { /* debug row hits log removed */ }
            }

            if (s_lastReportedScanId != scanId) {
                s_lastReportedScanId = scanId;
                // debug summary removed
            }

            // compact full result and publish atomically for render to consume
            if (_marks != null) {
                int validCount = 0; for (int i = 0; i < _marks.Length; i++) if (_marks[i].markCategory != 0 || _marks[i].markPosition != Vector3.zero) validCount++;
                    if (validCount > 0) {
                    var compactAll = new Marks[validCount]; int dst = 0; for (int i = 0; i < _marks.Length; i++) if (_marks[i].markCategory != 0 || _marks[i].markPosition != Vector3.zero) compactAll[dst++] = _marks[i];
                    // publish completed marks for rendering
                    System.Threading.Thread.MemoryBarrier();
                    System.Threading.Interlocked.Exchange(ref _marksForRender, compactAll);
                } else {
                    System.Threading.Interlocked.Exchange(ref _marksForRender, null);
                }
            }
        }

        // constants and shader IDs
        readonly static int ScanColorHead = Shader.PropertyToID("scanColorHead");
        readonly static int ScanColor = Shader.PropertyToID("scanColor");
        readonly static int OutlineWidth = Shader.PropertyToID("outlineWidth");
        readonly static int OutlineBrightness = Shader.PropertyToID("outlineBrightness");
        readonly static int OutlineStarDistance = Shader.PropertyToID("outlineStarDistance");
        readonly static int ScanLineWidth = Shader.PropertyToID("scanLineWidth");
        readonly static int ScanLineInterval = Shader.PropertyToID("scanLineInterval");
        readonly static int ScanLineBrightness = Shader.PropertyToID("scanLineBrightness");
        readonly static int ScanRange = Shader.PropertyToID("scanRange");
        readonly static int HeadScanLineDistance = Shader.PropertyToID("headScanLineDistance");
        readonly static int HeadScanLineWidth = Shader.PropertyToID("headScanLineWidth");
        readonly static int HeadScanLineBrightness = Shader.PropertyToID("headScanLineBrightness");
        readonly static int ScanCenterWs = Shader.PropertyToID("scanCenterWS");
        readonly static int ColorAlpha = Shader.PropertyToID("colorAlpha");

        static bool canScan = true;
        static bool showMark = false;
        static Tween markTween;

        // --- Render pass (kept simple) ---
        class CustomRenderPass : ScriptableRenderPass {
            GraphicsBuffer _graphicsBuffer;
            GraphicsBuffer.IndirectDrawIndexedArgs[] _commandData;
            ComputeBuffer _computeBuffer;
            Mesh mesh;
            Settings settings;
            string _passName;
            public CustomRenderPass(Settings settings) {
                _graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
                _commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
                _computeBuffer = new ComputeBuffer(horizontalCount * verticalCount, sizeof(float) * 4);
                // create a simple quad mesh for instanced marks
                mesh = new Mesh();
                var verts = new Vector3[4] { new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(-0.5f, 0, 0.5f) };
                var uvs = new Vector2[4] { new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1) };
                var tris = new int[6] { 0,1,2, 0,2,3 };
                mesh.vertices = verts;
                mesh.uv = uvs;
                mesh.triangles = tris;
                // make bounds very large to avoid frustum culling of far-away instances
                mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000f);
                this.settings = settings;
                _passName = "ScanEffect";
                // ensure mark material supports GPU instancing
                try { if (this.settings?.markMaterial != null) this.settings.markMaterial.enableInstancing = true; } catch { }
                // initialize indirect args: indexCountPerInstance, instanceCount, startIndex, baseVertex, startInstance
                _commandData[0].indexCountPerInstance = (uint)mesh.triangles.Length;
                _commandData[0].instanceCount = 0u;
                _commandData[0].startIndex = 0u;
                _commandData[0].startInstance = 0u;
                _graphicsBuffer.SetData(_commandData);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
                var resourceData = frameData.Get<UniversalResourceData>();
                var colorTarget = resourceData.activeColorTexture;
                var depthTarget = resourceData.activeDepthTexture;
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(_passName, out var passData)) {
                    passData.scanMaterial = settings.scanMaterial;
                    passData.markMaterial = settings.markMaterial;
                    passData.localShowMark = showMark;
                    passData.marks = _marks;
                    passData.computeBuffer = _computeBuffer;
                    passData.graphicsBuffer = _graphicsBuffer;
                    passData.commandData = _commandData;
                    passData.mesh = mesh;
                    passData.localHorizontalCount = horizontalCount;
                    passData.localVerticalCount = verticalCount;
                    passData.depthTarget = depthTarget;
                    builder.SetRenderAttachment(colorTarget, 0, AccessFlags.ReadWrite);
                    builder.UseTexture(depthTarget);
                    builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => ExecutePass(data, ctx));
                }
            }

            class PassData {
                public Material scanMaterial;
                public Material markMaterial;
                public bool localShowMark;
                public Marks[] marks;
                public ComputeBuffer computeBuffer;
                public GraphicsBuffer graphicsBuffer;
                public GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;
                public Mesh mesh;
                public int localHorizontalCount;
                public int localVerticalCount;
                public TextureHandle depthTarget;
            }

            static void ExecutePass(PassData data, RasterGraphContext context) {
                if (data.scanMaterial == null) return;
                var cmd = context.cmd;
                RTHandle depthHdl = data.depthTarget;
                Vector2 viewportScale = Vector2.one;
                try { if (depthHdl != null && depthHdl.useScaling) viewportScale = new Vector2(depthHdl.rtHandleProperties.rtHandleScale.x, depthHdl.rtHandleProperties.rtHandleScale.y); } catch { }
                if (!s_reportedDepthHandle) { s_reportedDepthHandle = true; }
                if (depthHdl != null) { try { Blitter.BlitTexture(cmd, depthHdl, viewportScale, data.scanMaterial, 0); } catch (Exception ex) { TerrainScannerPlugin.Logger.LogError($"[ScanFeature] ExecutePass: BlitTexture failed: {ex}"); } }

                if (data.localShowMark && data.markMaterial != null) {
                    if (data.computeBuffer == null || data.graphicsBuffer == null || data.commandData == null || data.mesh == null) {
                        // buffers/mesh missing - skipping instanced draw
                        s_reportedMarkState = true;
                        return;
                    }
                    // diagnostics removed
                    // read the latest completed marks published by GenerateTerrainMarks
                    var renderMarks = _marksForRender;
                    // renderMarks diagnostics removed
                    var matProp = new MaterialPropertyBlock(); int validCount = 0; if (renderMarks != null) for (int i = 0; i < renderMarks.Length; i++) if (renderMarks[i].markCategory != 0 || renderMarks[i].markPosition != Vector3.zero) validCount++;
                    if (validCount <= 0) return;
                    var compact = new Marks[validCount]; int idx = 0; for (int i = 0; i < renderMarks.Length; i++) if (renderMarks[i].markCategory != 0 || renderMarks[i].markPosition != Vector3.zero) compact[idx++] = renderMarks[i];
                    // diagnostics: material shader and instancing support
                    try {
                        if (data.markMaterial != null) {
                            // mark material diagnostics removed
                        } else {
                            // mark material null diagnostic removed
                        }
                    } catch { }

                    // validate computeBuffer capacity vs data to upload
                    try {
                        if (data.computeBuffer == null) { return; }
                        if (data.computeBuffer.count < compact.Length) { /* computeBuffer capacity insufficient */ }
                    } catch { }

                    // diagnostics: print first few compact entries to verify positions (deduped)
                    // compact sample diagnostics removed
                    data.computeBuffer.SetData(compact);
                    matProp.SetBuffer("markBuffer", data.computeBuffer);
                    data.commandData[0].indexCountPerInstance = 6;
                    data.commandData[0].instanceCount = (uint)validCount;
                    data.graphicsBuffer.SetData(data.commandData);
                    // now log the indirect args we just wrote
                    // indirect args diagnostics removed
                    // basic validation: nothing to draw -> skip
                    if (data.commandData[0].instanceCount == 0) { return; }
                    // ensure material has instancing enabled before draw
                    try { if (data.markMaterial != null) data.markMaterial.enableInstancing = true; } catch { }
                    // --- TEMP DEBUG: force material render queue & depth state to test visibility ---
                    int origQueue = -1; bool changedQueue = false;
                    var origZWrite = -1; var origZTest = -1; bool changedZ = false;
                    try {
                        // force material state diagnostic removed
                        if (data.markMaterial != null) {
                            try {
                                origQueue = data.markMaterial.renderQueue; data.markMaterial.renderQueue = 5000; changedQueue = true;
                            } catch { }
                            try { origZWrite = data.markMaterial.HasProperty("_ZWrite") ? data.markMaterial.GetInt("_ZWrite") : -1; data.markMaterial.SetInt("_ZWrite", 0); changedZ = true; } catch { }
                            try { origZTest = data.markMaterial.HasProperty("_ZTest") ? data.markMaterial.GetInt("_ZTest") : -1; data.markMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); changedZ = true; } catch { }
                        }
                    } catch { }
                    try { cmd.DrawMeshInstancedIndirect(data.mesh, 0, data.markMaterial, 0, data.graphicsBuffer, 0, matProp); } catch (Exception ex) { TerrainScannerPlugin.Logger.LogError($"[ScanFeature] ExecutePass: DrawMeshInstancedIndirect failed: {ex}"); }
                    // restore material state
                    try {
                        if (data.markMaterial != null) {
                            try { if (changedQueue) data.markMaterial.renderQueue = origQueue; } catch { }
                            try { if (changedZ && origZWrite >= 0) data.markMaterial.SetInt("_ZWrite", origZWrite); } catch { }
                            try { if (changedZ && origZTest >= 0) data.markMaterial.SetInt("_ZTest", origZTest); } catch { }
                        }
                    } catch { }
                    // --- end TEMP DEBUG ---
                }
            }

            // Note: Do not dispose graphics/compute buffers in finalizer here.
            // Disposal should be handled explicitly when the feature is destroyed or on editor domain unload.
        }

        public override void Create() {
            TerrainScannerPlugin.Logger?.LogInfo("[ScanFeature] Create() called");
            
            if (settings.scanMaterial == null) { 
                TerrainScannerPlugin.Logger?.LogError("[ScanFeature] scanMaterial is not assigned!"); 
                return; 
            }
            if (settings.markMaterial == null) { 
                TerrainScannerPlugin.Logger?.LogError("[ScanFeature] markMaterial is not assigned!"); 
                return; 
            }
            if (settings.markParticle1 == null || settings.markParticle2 == null || settings.markParticle3 == null) { 
                TerrainScannerPlugin.Logger?.LogError("[ScanFeature] One or more mark particles are not assigned!"); 
                return; 
            }
            if (!Application.isPlaying) {
                TerrainScannerPlugin.Logger?.LogWarning("[ScanFeature] Not in play mode, skipping Create()");
                return;
            }
            
            _marks = new Marks[horizontalCount * verticalCount];
            _myPass = new CustomRenderPass(settings);
            _instance = this;
            
            TerrainScannerPlugin.Logger?.LogInfo("[ScanFeature] Create() completed successfully, _instance set");
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            if (settings.scanMaterial == null) return; if (!Application.isPlaying) return;
            if (renderingData.cameraData.cameraType == CameraType.Game) {
                _myPass.renderPassEvent = settings.renderEvent;
                _myPass.ConfigureInput(ScriptableRenderPassInput.Color);
                _myPass.ConfigureInput(ScriptableRenderPassInput.Normal);
                _myPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (settings.scanMaterial == null) return; if (!Application.isPlaying) return;
            renderer.EnqueuePass(_myPass);
        }
    }
}