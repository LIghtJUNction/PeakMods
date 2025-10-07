using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using BepInEx;
using BepInEx.Configuration;

namespace TerrainScanner;

    // Runtime configuration container backed by BepInEx plugin config values.
    // This mirrors the former ScanFeature.Settings fields but lives in its own file
    // and is populated by TerrainScannerPlugin during initialization.
    public class ScanConfig
    {
        // Render timing
        public RenderPassEvent renderEvent = RenderPassEvent.BeforeRenderingTransparents;

        // Materials / particles are provided at runtime by the plugin
        public Material scanMaterial;

        // Static Settings (defaults kept from original Settings)
        public Color scanColorHead = new Color(0.054901965f, 0.5686275f, 0.85098046f, 1f);
        public Color scanColor = new Color(0.38823533f, 0.7372549f, 0.8705883f, 1f);
        public float outlineWidth = 2.48f;
        public float scanLineWidth = 1f;
        public float scanLineInterval = 1f;
        public float headScanLineWidth = 1f;

        // Dynamics (controlled by code / config)
        public float scanLineBrightness = 2.5f;
        public float scanRange = 5f;
        public float outlineBrightness = 1.32f;
        public float headScanLineDistance = 13.2f;
        public Vector3 scanCenterWS = new Vector3(123.05f, 36.3f, 147.86f);
        public float outlineStarDistance = 30f;

        // Render mark resources
        public Material markMaterial;
        public GameObject markParticle3;
        public GameObject markParticle2;
        public GameObject markParticle1;

        // Particle probabilities
        public float steepSpawnProb = 0.1f;
        public float midSpawnProb = 0.3f;
        public float flatSpawnProb = 0.0002f;

        // BepInEx config entries (populated by Bind)
        public ConfigEntry<string> cfgScanColorHead;
        public ConfigEntry<string> cfgScanColor;
        public ConfigEntry<float> cfgOutlineWidth;
        public ConfigEntry<float> cfgScanLineWidth;
        public ConfigEntry<float> cfgScanLineInterval;
        public ConfigEntry<float> cfgHeadScanLineWidth;
        public ConfigEntry<float> cfgScanLineBrightness;
        public ConfigEntry<float> cfgScanRange;
        public ConfigEntry<float> cfgOutlineBrightness;
        public ConfigEntry<float> cfgHeadScanLineDistance;
        public ConfigEntry<string> cfgScanCenterWS;
        public ConfigEntry<float> cfgOutlineStarDistance;
        public ConfigEntry<float> cfgSteepSpawnProb;
        public ConfigEntry<float> cfgMidSpawnProb;
        public ConfigEntry<float> cfgFlatSpawnProb;

        bool _bound = false;

        // Bind this ScanConfig to a BepInEx plugin's Config. onChanged is invoked after initial population and on any setting change.
        public void Bind(BaseUnityPlugin plugin, Action<ScanConfig> onChanged = null)
        {
            if (plugin == null) return;
            if (_bound) return;
            _bound = true;
            var cfg = plugin.Config;
            cfgScanColorHead = cfg.Bind("Style", "ScanColorHead", "0.054901965,0.5686275,0.85098046,1",
                "Scan head color as r,g,b,a");
            cfgScanColor = cfg.Bind("Style", "ScanColor", "0.38823533,0.7372549,0.8705883,1", "Scan color as r,g,b,a");
            cfgOutlineWidth = cfg.Bind("Style", "OutlineWidth", 2.48f, "Outline width");
            cfgScanLineWidth = cfg.Bind("Style", "ScanLineWidth", 1f, "Scan line width");
            cfgScanLineInterval = cfg.Bind("Style", "ScanLineInterval", 1f, "Scan line interval");
            cfgHeadScanLineWidth = cfg.Bind("Style", "HeadScanLineWidth", 1f, "Head scan line width");
            cfgScanLineBrightness = cfg.Bind("Style", "ScanLineBrightness", 2.5f, "Scan line brightness");
            cfgScanRange = cfg.Bind("Style", "ScanRange", 5f, "Scan range");
            cfgOutlineBrightness = cfg.Bind("Style", "OutlineBrightness", 1.32f, "Outline brightness");
            cfgHeadScanLineDistance = cfg.Bind("Style", "HeadScanLineDistance", 13.2f, "Head scan line distance");
            cfgScanCenterWS = cfg.Bind("Style", "ScanCenterWS", "123.05,36.3,147.86",
                "Scan center world-space as x,y,z");
            cfgOutlineStarDistance = cfg.Bind("Style", "OutlineStarDistance", 30f, "Outline star distance");
            cfgSteepSpawnProb = cfg.Bind("Style", "SteepSpawnProb", 0.1f,
                "Probability to spawn particle on steep slopes (category 3)");
            cfgMidSpawnProb = cfg.Bind("Style", "MidSpawnProb", 0.3f,
                "Probability to spawn particle on mid slopes (category 2)");
            cfgFlatSpawnProb = cfg.Bind("Style", "FlatSpawnProb", 0.0002f,
                "Probability to spawn particle on flat slopes (category 1)");

            // parse helpers
            Color ParseColor(string s)
            {
                try
                {
                    var parts = s.Split(',');
                    if (parts.Length < 3) return Color.blue;
                    float r = float.Parse(parts[0]);
                    float g = float.Parse(parts[1]);
                    float b = float.Parse(parts[2]);
                    float a = parts.Length >= 4 ? float.Parse(parts[3]) : 1f;
                    return new Color(r, g, b, a);
                }
                catch
                {
                    return Color.blue;
                }
            }

            Vector3 ParseVec3(string s)
            {
                try
                {
                    var parts = s.Split(',');
                    if (parts.Length < 3) return Vector3.zero;
                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);
                    float z = float.Parse(parts[2]);
                    return new Vector3(x, y, z);
                }
                catch
                {
                    return Vector3.zero;
                }
            }

            void UpdateFromConfig()
            {
                try
                {
                    scanColorHead = ParseColor(cfgScanColorHead.Value);
                    scanColor = ParseColor(cfgScanColor.Value);
                    outlineWidth = cfgOutlineWidth.Value;
                    scanLineWidth = cfgScanLineWidth.Value;
                    scanLineInterval = cfgScanLineInterval.Value;
                    headScanLineWidth = cfgHeadScanLineWidth.Value;
                    scanLineBrightness = cfgScanLineBrightness.Value;
                    scanRange = cfgScanRange.Value;
                    outlineBrightness = cfgOutlineBrightness.Value;
                    headScanLineDistance = cfgHeadScanLineDistance.Value;
                    scanCenterWS = ParseVec3(cfgScanCenterWS.Value);
                    outlineStarDistance = cfgOutlineStarDistance.Value;
                    steepSpawnProb = cfgSteepSpawnProb.Value;
                    midSpawnProb = cfgMidSpawnProb.Value;
                    flatSpawnProb = cfgFlatSpawnProb.Value;
                }
                catch
                {
                }

                try
                {
                    onChanged?.Invoke(this);
                }
                catch
                {
                }
            }

            // initial population
            UpdateFromConfig();

            // subscribe to changes
            try
            {
                cfgScanColorHead.SettingChanged += (s, e) => UpdateFromConfig();
                cfgScanColor.SettingChanged += (s, e) => UpdateFromConfig();
                cfgOutlineWidth.SettingChanged += (s, e) => UpdateFromConfig();
                cfgScanLineWidth.SettingChanged += (s, e) => UpdateFromConfig();
                cfgScanLineInterval.SettingChanged += (s, e) => UpdateFromConfig();
                cfgHeadScanLineWidth.SettingChanged += (s, e) => UpdateFromConfig();
                cfgScanLineBrightness.SettingChanged += (s, e) => UpdateFromConfig();
                cfgScanRange.SettingChanged += (s, e) => UpdateFromConfig();
                cfgOutlineBrightness.SettingChanged += (s, e) => UpdateFromConfig();
                cfgHeadScanLineDistance.SettingChanged += (s, e) => UpdateFromConfig();
                cfgScanCenterWS.SettingChanged += (s, e) => UpdateFromConfig();
                cfgOutlineStarDistance.SettingChanged += (s, e) => UpdateFromConfig();
                cfgSteepSpawnProb.SettingChanged += (s, e) => UpdateFromConfig();
                cfgMidSpawnProb.SettingChanged += (s, e) => UpdateFromConfig();
                cfgFlatSpawnProb.SettingChanged += (s, e) => UpdateFromConfig();
            }
            catch
            {
            }
        }

        // Helper: copy simple values from another ScanConfig (used when plugin updates values)
        public void CopyFrom(ScanConfig other)
        {
            if (other == null) return;
            scanColorHead = other.scanColorHead;
            scanColor = other.scanColor;
            outlineWidth = other.outlineWidth;
            scanLineWidth = other.scanLineWidth;
            scanLineInterval = other.scanLineInterval;
            headScanLineWidth = other.headScanLineWidth;
            scanLineBrightness = other.scanLineBrightness;
            scanRange = other.scanRange;
            outlineBrightness = other.outlineBrightness;
            headScanLineDistance = other.headScanLineDistance;
            scanCenterWS = other.scanCenterWS;
            outlineStarDistance = other.outlineStarDistance;
            steepSpawnProb = other.steepSpawnProb;
            midSpawnProb = other.midSpawnProb;
            flatSpawnProb = other.flatSpawnProb;
        }
    }

    // Central manager to own BepInEx binding and push config updates to registered features
    public static class ScanConfigManager
    {
        static ScanConfig _current;
        public static ScanConfig Current => _current;

        static event Action<ScanConfig> _onChanged;

        static readonly System.Collections.Generic.List<ScanFeature> _registered =
            new System.Collections.Generic.List<ScanFeature>();

        // Initialize and bind to plugin config. Safe to call multiple times.
        public static void Initialize(BaseUnityPlugin plugin)
        {
            if (plugin == null) return;
            if (_current != null) return;
            _current = new ScanConfig();
            try
            {
                _current.Bind(plugin, (cfg) =>
                {
                    // notify manager and push to registered features
                    try
                    {
                        _onChanged?.Invoke(_current);
                    }
                    catch
                    {
                    }
                });
            }
            catch
            {
            }

            // ensure initial push
            try
            {
                _onChanged?.Invoke(_current);
            }
            catch
            {
            }
        }

        // Register a ScanFeature to receive config updates. Copies current config immediately.
        public static void RegisterFeature(ScanFeature feature)
        {
            if (feature == null) return;
            if (_current == null) return; // not initialized yet
            if (!_registered.Contains(feature)) _registered.Add(feature);
            try
            {
                feature.config.CopyFrom(_current);
            }
            catch
            {
            }

            // inject runtime assets (materials/particles) from the central config
            try {
                if (_current.scanMaterial != null) feature.config.scanMaterial = _current.scanMaterial;
                if (_current.markMaterial != null) {
                    try { _current.markMaterial.enableInstancing = true; } catch { }
                    feature.config.markMaterial = _current.markMaterial;
                }
                feature.config.markParticle1 = _current.markParticle1;
                feature.config.markParticle2 = _current.markParticle2;
                feature.config.markParticle3 = _current.markParticle3;
            } catch {
            }

            // subscribe to future updates
            _onChanged -= feature_OnConfigChanged;
            _onChanged += feature_OnConfigChanged;
        }

        // Unregister feature
        public static void UnregisterFeature(ScanFeature feature)
        {
            if (feature == null) return;
            _registered.Remove(feature);
            _onChanged -= feature_OnConfigChanged;
        }

        static void feature_OnConfigChanged(ScanConfig cfg)
        {
            // push to all registered features
            for (int i = 0; i < _registered.Count; i++)
            {
                var f = _registered[i];
                if (f == null) continue;
                try
                {
                    f.config.CopyFrom(cfg);
                }
                catch
                {
                }
            }
        }
    }
