

using PeakChatOps.Core;
using UnityEngine;

namespace PeakChatOps.UI;

public class PeakChatOpsCanvas : MonoBehaviour
{
    public static PeakChatOpsCanvas Instance;

    public RectTransform CanvasRectTransform;
    public PeakChatOpsPanel MainPanel;

    private void Awake()
    {
        DevLog.File("Awake: " + nameof(PeakChatOpsCanvas));
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        if (this.transform.parent == null)
        {
            DevLog.File("设置 Canvas 为根节点");
            DontDestroyOnLoad(this.gameObject);
        }

        // 检查是否被禁用
        DevLog.File($"Canvas 是否被禁用: {!this.gameObject.activeSelf}");
    }

    private void Start()
    {
        DevLog.File("初始化: " + nameof(PeakChatOpsCanvas));
        // 动态创建 Canvas 及相关组件
        var canvasGO = this.gameObject;
        // RectTransform（不可配置，自动挂载）
        CanvasRectTransform = canvasGO.GetComponent<RectTransform>();
        if (CanvasRectTransform == null)
            CanvasRectTransform = canvasGO.AddComponent<RectTransform>();

        // Canvas
        var canvas = canvasGO.GetComponent<Canvas>();
        if (canvas == null)
            canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvas.pixelPerfect = false; // 可根据实际需求设置

        // CanvasScaler
        var scaler = canvasGO.GetComponent<UnityEngine.UI.CanvasScaler>();
        if (scaler == null)
            scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; // 0=宽，1=高，0.5=居中

        // GraphicRaycaster
        var raycaster = canvasGO.GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster == null)
            raycaster = canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        // 默认配置即可

        // 创建主面板子节点
        MainPanel = PeakChatOpsPanel.Create(canvasGO.transform);
    }
    }
    
