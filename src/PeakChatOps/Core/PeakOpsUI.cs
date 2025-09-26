
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using PEAKLib.UI;
using PEAKLib.UI.Elements;
using PeakChatOps.Patches;

namespace PeakChatOps.Core;

public class PeakOpsUI : MonoBehaviour
{
    // 双缓冲消息渲染结构
    private readonly Queue<string> messageRenderQueue = new Queue<string>(); // 待渲染消息队列
    private volatile bool isRendering = false; // 渲染中标记
    private List<string> backgroundVisibleMessages = new List<string>(); // 背景线程计算的可见消息
    private object renderLock = new object();

    // 虚拟化消息渲染核心数据结构
    private readonly List<string> allMessages = new List<string>(); // 全部消息历史
    private readonly Dictionary<int, PeakText> activeTextMap = new Dictionary<int, PeakText>(); // 当前激活的UI对象
    private readonly Stack<PeakText> textPool = new Stack<PeakText>(); // PeakText对象池
    private PeakTextInput peakInput; // 聊天输入框逻辑组件
    int maxMessages = 500; // 聊天记录最大条数
    Vector2 boxSize = new Vector2(500, 300); // 聊天框的宽高
    float fadeInTime = 0.03f; // 聊天框淡入动画时间
    float fadeOutTime = 5;    // 聊天框淡出动画时间
    float hideTime = 5;       // 聊天框完全隐藏所需时间
    float fadeOutDelay = 15;  // 多久后开始淡出
    float hideDelay = 40;     // 多久后完全隐藏
    float fontSize = 25;      // 聊天字体大小

    // 聊天输入框组件。用于接收玩家输入的聊天内容。
    // 这是 Unity TextMeshPro 提供的高级输入框，比原生 InputField 更美观和强大。
    TMP_InputField inputField = null!;

    // 聊天内容显示区域的 RectTransform。
    // RectTransform 是 Unity UI 元素的布局组件，负责控制位置、大小、锚点等。
    // 这个变量通常指向“聊天内容滚动区域”的内容容器（即所有聊天消息的父物体）。
    RectTransform chatLogViewportTransform = null!;

    // 聊天窗口的主容器 RectTransform。
    // 这是整个聊天框 UI 的根节点，决定了聊天框在屏幕上的整体位置和尺寸。
    // 你可以通过它设置聊天框的锚点、对齐方式、偏移等。
    RectTransform baseTransform = null!;

    // 控制整个聊天窗口 UI 的 CanvasGroup 组件。
    // CanvasGroup 是 Unity UI 的一个特殊组件，可以统一控制一组 UI 的透明度、交互性和可见性。
    // 例如：通过设置 alpha 实现淡入淡出，通过 blocksRaycasts/interactable 控制是否能被点击或输入。
    CanvasGroup canvasGroup = null!;


    float fade = 1;           // 聊天框当前透明度（用于动画）
    float fadeTimer = -1;     // 控制淡入淡出的计时器

    float hide = 0;           // 聊天框当前隐藏状态（用于动画）
    float hideTimer = -1;     // 控制隐藏的计时器

    KeyCode chatKey;          // 打开聊天框的快捷键
    string chatKeyText;       // 快捷键的文本描述

    Color offWhite = new Color(0.87f, 0.85f, 0.76f); // 聊天文本的主色
    Color red = new Color(0.99f, 0.33f, 0);          // 错误或警告文本色

    public bool isBlockingInput = false; // 聊天框是否正在阻止输入（如输入时屏蔽游戏操作）
    public int framesSinceInputBlocked = 0; // 距离上次输入被阻塞的帧数

    // 上一帧的输入阻塞状态缓存。注意：这是上一帧的快照，用于检测状态变化或作为优化用。
    // 不要把它当作当前状态的替代品，当前帧仍然以 `isBlockingInput` 为准。
    bool wasBlockingInput = false;

    // 位置更新的脏标记：当需要重新计算位置时设置为 true
    bool positionDirty = true;
    // 缓存上一次屏幕尺寸以检测分辨率/缩放变化
    int lastScreenWidth = 0;
    int lastScreenHeight = 0;

    public static PeakOpsUI instance = null!; // 聊天显示的单例引用

    GameObject currentSelection = null!; // 当前选中的 UI 元素
                                         // 当 UI 尚未创建时缓存要显示的消息，CreateChatUI 完成后会刷新
    List<string> pendingMessages = new List<string>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 读取配置
        getConfig();

        // 重置计时器
        ResetTimers();

        // 创建聊天框 UI
        CreateChatUI();

    }

    public void getConfig()
    {
        // 读取配置
        chatKey = PeakChatOpsPlugin.Key.Value;
        chatKeyText = chatKey.ToString();
        fontSize = PeakChatOpsPlugin.FontSize.Value < 0 ? 1000000000 : PeakChatOpsPlugin.FontSize.Value;
        hideDelay = PeakChatOpsPlugin.HideDelay.Value < 0 ? Mathf.Infinity : PeakChatOpsPlugin.HideDelay.Value;
        fadeOutDelay = Mathf.Min(PeakChatOpsPlugin.FadeDelay.Value < 0 ? Mathf.Infinity : PeakChatOpsPlugin.FadeDelay.Value, hideDelay);

        var chatSizeConfigSplit = PeakChatOpsPlugin.ChatSize.Value.Split(':');
        if (chatSizeConfigSplit.Length >= 2)
        {
            var xString = chatSizeConfigSplit[0].Replace(" ", "");
            var yString = chatSizeConfigSplit[1].Replace(" ", "");

            if (float.TryParse(xString, out float chatSizeX) && float.TryParse(yString, out float chatSizeY))
            {
                boxSize = new Vector2(Mathf.Max(chatSizeX, 10), Mathf.Max(chatSizeY, 10));
            }
            else
            {
                boxSize = new Vector2(500, 300);
            }
        }
    }

    /// <summary>
    /// 刷新聊天框UI，使配置变更即时生效（如字体、大小、透明度等）
    /// </summary>
    public void RefreshUI()
    {
        getConfig();
        // 聊天框尺寸
        if (baseTransform != null)
        {
            baseTransform.sizeDelta = boxSize;
        }
        // 聊天内容字体
        if (chatLogViewportTransform != null)
        {
            foreach (Transform child in chatLogViewportTransform)
            {
                var text = child.GetComponent<PeakText>();
                if (text != null)
                {
                    text.SetFontSize(fontSize);
                    text.SetColor(offWhite);
                }
            }
        }
        // 输入框字体
        if (inputField != null && inputField.textComponent != null)
        {
            inputField.textComponent.fontSize = fontSize;
            inputField.textComponent.color = Utilities.GetContrastingColor(offWhite, 0.8f);
        }
        // 背景透明度
        var shadow = baseTransform.Find("Shadow");
        if (shadow != null)
        {
            var img = shadow.GetComponent<ProceduralImage>();
            if (img != null)
                img.color = new Color(0, 0, 0, PeakChatOpsPlugin.BgOpacity.Value);
        }
        // 边框可见性
        var border = baseTransform.Find("Border");
        if (border != null)
        {
            border.gameObject.SetActive(PeakChatOpsPlugin.FrameVisible.Value);
        }
        // 位置刷新
        UpdatePosition();

    }

    void ResetTimers()
    {
        fadeTimer = fadeOutDelay;
        hideTimer = hideDelay;
    }

    /// <summary>
    /// 立即隐藏聊天框：将隐藏计时器归零并强制进入隐藏状态。
    /// 其它代码可在需要时调用此方法（例如 /hide 命令）。
    /// </summary>
    public void HideNow()
    {
        // 将计时器设置为立即开始隐藏，并将 fade 值置为最小以加速隐藏表现
        hideTimer = 0f;
        // 也将 fadeTimer 设为非常小以确保 alpha 下降
        fadeTimer = -Mathf.Abs(fadeOutTime);
        // 直接应用隐藏状态值
        hide = 1f;
        fade = 0f;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = fade * 0.5f + (1 - hide) * 0.5f;
        }

        // 取消输入模式：取消选中项、停用输入框并隐藏，解除阻止输入的状态
        try
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        catch
        {
            // 防御性：EventSystem 可能在某些载入阶段为 null
        }

        try
        {
            if (inputField != null)
            {
                inputField.DeactivateInputField();
                if (inputField.gameObject != null)
                    inputField.gameObject.SetActive(false);
            }
        }
        catch
        {
            // 防御性：如果 inputField 尚未初始化或被销毁，忽略错误
        }

        isBlockingInput = false;
    }

    private GameObject CreateUIGameObject(string name, Transform parent)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj;
    }

    /// <summary>
    /// 创建一个新的 UI GameObject，并设置其父对象。
    /// 用于动态生成聊天框、输入框、内容容器等 UI 结构节点。
    /// </summary>
    /// <param name="name">新建 GameObject 的名称，便于在层级视图中识别。</param>
    /// <param name="parent">要挂载到的父 Transform，决定新对象在 UI 层级中的归属。</param>
    /// <returns>返回新创建的 GameObject 实例。</returns>
    void CreateChatUI()
    {
        // 创建聊天框 UI 的代码
        PeakChatOpsPlugin.Logger.LogDebug("[DebugUI] CreateChatUI start");

        baseTransform = this.gameObject.GetComponent<RectTransform>() ?? this.gameObject.AddComponent<RectTransform>();
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] baseTransform assigned: {(baseTransform != null)}");
        baseTransform.SetParent(this.transform, false);
        UpdatePosition();
        baseTransform.sizeDelta = boxSize;

        canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] canvasGroup created: {(canvasGroup != null)}");
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.ignoreParentGroups = false;

        // 阴影背景
        var shadow = CreateUIGameObject("Shadow", baseTransform);
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] Shadow created: {(shadow != null ? shadow.name : "<null>")}");
        var shadowTransform = shadow.AddComponent<RectTransform>();
        Utilities.ExpandToParent(shadowTransform);
        var shadowImg = shadow.AddComponent<ProceduralImage>();
        shadowImg.color = new Color(0, 0, 0, PeakChatOpsPlugin.BgOpacity.Value);
        shadowImg.FalloffDistance = 10;
        shadowImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 10;

        var chatLogHolderObj = CreateUIGameObject("ChatLog", baseTransform);
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] ChatLog holder created: {(chatLogHolderObj != null ? chatLogHolderObj.name : "<null>")}");
        var chatLogHolderTransform = chatLogHolderObj.AddComponent<RectTransform>();
        chatLogHolderTransform.anchorMin = Vector2.zero;
        chatLogHolderTransform.anchorMax = Vector2.one;
        chatLogHolderTransform.offsetMin = new Vector2(0, fontSize * 2);
        chatLogHolderTransform.offsetMax = Vector2.zero;
        chatLogHolderObj.AddComponent<RectMask2D>();

        // 添加 ScrollRect 组件
        var scrollRect = chatLogHolderObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = fontSize * 1.2f;

        // 聊天内容容器
        var chatLogContentObj = CreateUIGameObject("Content", chatLogHolderTransform);
        chatLogViewportTransform = chatLogContentObj.AddComponent<RectTransform>();
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] chatLogViewportTransform created: {(chatLogViewportTransform != null)}");
        chatLogViewportTransform.pivot = new Vector2(0.5f, 0);
        chatLogViewportTransform.anchorMin = new Vector2(0, 0);
        chatLogViewportTransform.anchorMax = new Vector2(1, 0);
        chatLogViewportTransform.offsetMin = Vector2.zero;
        chatLogViewportTransform.offsetMax = Vector2.zero;
        chatLogViewportTransform.anchoredPosition = Vector2.zero;
        var contentSizeFitter = chatLogContentObj.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // 设置 ScrollRect 的 content 和 viewport
        scrollRect.content = chatLogViewportTransform;
        scrollRect.viewport = chatLogHolderTransform;
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] ScrollRect setup: content={(scrollRect.content != null)} viewport={(scrollRect.viewport != null)}");


        // 聊天输入框
        var peakInputGo = GameObject.Instantiate(
            typeof(PeakTextInput).GetProperty("TextInputPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null) as GameObject,
            baseTransform, false);
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] peakInputGo instantiated: {(peakInputGo != null ? peakInputGo.name : "<null>")}");

        peakInput = peakInputGo.GetComponent<PeakTextInput>() ?? peakInputGo.AddComponent<PeakTextInput>();
        peakInput.SetPlaceholder($"Press {chatKeyText} to chat , Prefix: {PeakChatOpsPlugin.CmdPrefix.Value} .");

        var textComp = peakInput.InputField.textComponent;
        textComp.fontSize = fontSize;
        textComp.color = Utilities.GetContrastingColor(offWhite, 0.8f);
        var darumaFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
            .FirstOrDefault(f => f.faceInfo.familyName == "Daruma Drop One");
        if (darumaFont != null) textComp.font = darumaFont;
        inputField = peakInput.InputField;
        var inputFieldTransform = (RectTransform)inputField.transform;
        inputFieldTransform.SetParent(baseTransform, false);
        inputFieldTransform.anchorMin = new Vector2(0, 0);
        inputFieldTransform.anchorMax = new Vector2(1, 0);
        inputFieldTransform.pivot = new Vector2(0.5f, 0);
        inputFieldTransform.offsetMin = new Vector2(5, 5);
        inputFieldTransform.offsetMax = new Vector2(-5, fontSize * 1.75f);

        var chatLogLayout = chatLogContentObj.AddComponent<VerticalLayoutGroup>();
        // Let the layout control child heights so they layout reliably
        chatLogLayout.childControlWidth = true;
        chatLogLayout.childControlHeight = true; // was false - enable to avoid overlap when children don't size properly
        chatLogLayout.childForceExpandWidth = true;
        chatLogLayout.childForceExpandHeight = false;
        chatLogLayout.childScaleWidth = false;
        chatLogLayout.childScaleHeight = false;
        chatLogLayout.childAlignment = TextAnchor.LowerCenter;
        chatLogLayout.padding = new RectOffset((int)(fontSize * 0.6f), (int)(fontSize * 0.6f), (int)(fontSize / 20), (int)(fontSize / 20));
        // Original spacing was negative which can cause overlap when child heights are inconsistent.
        // Clamp spacing to a safe range relative to fontSize.
        float desiredSpacing = -fontSize / 8f;
        float minSpacing = -Mathf.Max(1f, fontSize / 16f); // allow small negative overlap but not large
        float maxSpacing = Mathf.Max(0f, fontSize / 4f);
        chatLogLayout.spacing = Mathf.Clamp(desiredSpacing, minSpacing, maxSpacing);
        PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] chatLogLayout configured: spacing={chatLogLayout.spacing}");

        inputField.onSubmit.AddListener((e) =>
        {
            inputField.text = "";
            inputField.textComponent.text = "";
            ChatSystem.Instance?.SendChatMessage(e);
        });

        inputField.onEndEdit.AddListener((e) =>
        {
            EventSystem.current.SetSelectedGameObject(null);
            isBlockingInput = false;
        });

        if (PeakChatOpsPlugin.FrameVisible.Value)
        {
            var border = CreateUIGameObject("Border", baseTransform);
            var borderTransform = border.AddComponent<RectTransform>();
            Utilities.ExpandToParent(borderTransform);
            var borderImg = border.AddComponent<ProceduralImage>();
            borderImg.color = offWhite;
            borderImg.BorderWidth = 2;
            borderImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 5;
            PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] Border created: {(border != null ? border.name : "<null>")}");
        }
        // 如果在 UI 创建前有缓存的消息，刷新它们
        if (pendingMessages != null && pendingMessages.Count > 0)
        {
            PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] Flushing {pendingMessages.Count} pending messages");
            try
            {
                foreach (var m in pendingMessages.ToArray())
                {
                    AddMessage(m);
                }
            }
            catch (Exception ex)
            {
                PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] Exception while flushing pending messages: {ex.Message}");
            }
            pendingMessages.Clear();
            PeakChatOpsPlugin.Logger.LogDebug("[DebugUI] Pending messages flushed");
        }
        PeakChatOpsPlugin.Logger.LogDebug("[DebugUI] CreateChatUI end");

        // 初始化时添加一条消息以确保内容容器被创建和展示
        try
        {
            // 使用 TextMeshPro 富文本美化启动提示：把标签和链接高亮为蓝色
            AddMessage("<b><color=#DED9C2>Github:</color></b> <color=#59A6FF>https://github.com/LIghtJUNction/PeakMods</color>"); // 初始化时添加一条消息以创建内容容器
                                                                                                                                  // 额外的启动提示（版本号高亮）
            AddMessage($"<i><color=#C0C0C0>Version:</color></i> <color=#59A6FF>{typeof(PeakOpsUI).Assembly.GetName().Version}</color>");

        }
        catch (Exception ex)
        {
            PeakChatOpsPlugin.Logger.LogDebug($"[DebugUI] Failed to add init message: {ex.Message}");
        }
    }

    /// <summary>
    /// 动态设置输入框的占位符文本
    /// </summary>
    public void SetInputPlaceholder(string text)
    {
        if (peakInput != null)
            peakInput.SetPlaceholder(text);
    }

    void UpdatePosition()
    {
        // 直接读取配置判断
        var alignment = PeakChatOpsPlugin.Pos.Value;
        // 根据对齐方式设置聊天框的锚点、pivot和位置
        switch (alignment)
        {
            case UIAlignment.TopLeft:
                baseTransform.pivot = new Vector2(0, 1);
                baseTransform.anchorMax = new Vector2(0, 1);
                baseTransform.anchorMin = new Vector2(0, 1);
                baseTransform.anchoredPosition = new Vector2(64, -62);
                break;

            case UIAlignment.TopCenter:
                baseTransform.pivot = new Vector2(0.5f, 1);
                baseTransform.anchorMax = new Vector2(0.5f, 1);
                baseTransform.anchorMin = new Vector2(0.5f, 1);
                baseTransform.anchoredPosition = new Vector2(0, -62);
                break;

            case UIAlignment.TopRight:
                baseTransform.pivot = Vector2.one;
                baseTransform.anchorMax = Vector2.one;
                baseTransform.anchorMin = Vector2.one;
                baseTransform.anchoredPosition = new Vector2(-64, -62);
                break;

            case UIAlignment.MiddleLeft:
                baseTransform.pivot = new Vector2(0, 0.5f);
                baseTransform.anchorMax = new Vector2(0, 0.5f);
                baseTransform.anchorMin = new Vector2(0, 0.5f);
                baseTransform.anchoredPosition = new Vector2(64, 0);
                break;

            case UIAlignment.MiddleCenter:
                baseTransform.pivot = new Vector2(0.5f, 0.5f);
                baseTransform.anchorMax = new Vector2(0.5f, 0.5f);
                baseTransform.anchorMin = new Vector2(0.5f, 0.5f);
                baseTransform.anchoredPosition = Vector2.zero;
                break;

            case UIAlignment.MiddleRight:
                baseTransform.pivot = new Vector2(1, 0.5f);
                baseTransform.anchorMax = new Vector2(1, 0.5f);
                baseTransform.anchorMin = new Vector2(1, 0.5f);
                baseTransform.anchoredPosition = new Vector2(-64, 0);
                break;

            case UIAlignment.BottomLeft:
                // 左下角特殊：跟随耐力条的虚拟锚点
                if (baseTransform != null && StaminaBarPatch.barGroupChildWatcher.ChatOpsDummyTransform != null)
                {
                    baseTransform.pivot = Vector2.zero;
                    baseTransform.anchorMax = Vector2.zero;
                    baseTransform.anchorMin = Vector2.zero;
                    baseTransform.position = StaminaBarPatch.barGroupChildWatcher.ChatOpsDummyTransform.position;
                    baseTransform.anchoredPosition += new Vector2(0, 40);
                }
                break;

            case UIAlignment.BottomCenter:
                baseTransform.pivot = new Vector2(0.5f, 0);
                baseTransform.anchorMax = new Vector2(0.5f, 0);
                baseTransform.anchorMin = new Vector2(0.5f, 0);
                baseTransform.anchoredPosition = new Vector2(0, 40);
                break;

            case UIAlignment.BottomRight:
                // 右下角：始终使用屏幕右下（pivot/anchors = (1,0)），
                baseTransform.pivot = new Vector2(1, 0);
                baseTransform.anchorMax = new Vector2(1, 0);
                baseTransform.anchorMin = new Vector2(1, 0);
                baseTransform.anchoredPosition = new Vector2(-64, 40);
                break;
        }
    }

    /// <summary>
    /// 将位置标记为需要重新计算（可由外部或配置修改时调用）。
    /// </summary>
    public void MarkPositionDirty()
    {
        positionDirty = true;
    }

    /// <summary>
    /// Unity 在 RectTransform 尺寸变化时调用此方法——我们把它视为需要重新计算位置。
    /// </summary>
    void OnRectTransformDimensionsChange()
    {
        positionDirty = true;
    }

    void Update()
    {
        // 缓存常用引用以减少每帧的查找
        var evt = EventSystem.current;
        var gui = GUIManager.instance;
        var input = inputField; // 可能为 null，部分访问放在 try/catch 或条件判断中

        if (isBlockingInput)
        {
            framesSinceInputBlocked = -1;
            // 输入时监听鼠标滚轮，上下滚动一格
            if (input != null && input.isFocused)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll > 0.01f)
                    ScrollUpOne();
                else if (scroll < -0.01f)
                    ScrollDownOne();
            }
        }
        else
        {
            framesSinceInputBlocked = Mathf.Min(framesSinceInputBlocked + 1, 50);
        }

        if (!this.gameObject.activeInHierarchy)
        {
            isBlockingInput = false;
            wasBlockingInput = isBlockingInput;
            return;
        }

        // 仅在 EventSystem 实例存在时访问
        if (evt != null)
        {
            var current = evt.currentSelectedGameObject;
            if (currentSelection != current)
            {
                currentSelection = current;
                if (input == null || currentSelection != input.gameObject)
                {
                    isBlockingInput = false;
                }
            }
        }

        // 仅在 GUIManager 可用且未被其它窗口阻塞时响应聊天键或命令前缀键
        bool prefixPressed = false;
        // 支持可配置的命令前缀（默认 '/'），按下该字符时也自动打开输入框并预填前缀
        try
        {
            var prefix = PeakChatOpsPlugin.CmdPrefix.Value ?? "/";
            if (!string.IsNullOrEmpty(prefix))
            {
                // 只处理第一个字符作为触发键
                var p = prefix[0];
                // 使用 Input.GetKeyDown 对字符键的检测（支持多数键盘布局）
                if (Input.GetKeyDown(p.ToString()))
                    prefixPressed = true;
            }
        }
        catch
        {
            // 防御性：如果读取配置失败，忽略
        }

        if (gui != null && !gui.windowBlockingInput && (Input.GetKeyDown(chatKey) || prefixPressed) && evt != null)
        {
            // 只要输入框不可见就显示输入框
            if (input == null || !input.gameObject.activeInHierarchy)
            {
                ResetTimers();
                if (input != null)
                    input.gameObject.SetActive(true);
            }
            if (evt != null && input != null)
            {
                evt.SetSelectedGameObject(input.gameObject, null);
                input.ActivateInputField();
                // 如果是通过前缀触发，预填前缀字符并移动光标到末尾
                if (prefixPressed)
                {
                    try
                    {
                        // 只在输入框为空或尚未包含前缀时预填
                        if (string.IsNullOrEmpty(input.text) || !input.text.StartsWith(PeakChatOpsPlugin.CmdPrefix.Value))
                        {
                            input.text = PeakChatOpsPlugin.CmdPrefix.Value;
                            input.caretPosition = input.text.Length;
                        }
                    }
                    catch
                    {
                        // 忽略任何异常以避免阻塞 UI
                    }
                }
            }
            isBlockingInput = true;
        }

        if (isBlockingInput)
            ResetTimers();

        // 仅在需要时更新位置：当被标记为脏或屏幕尺寸发生变化时
        int sw = Screen.width;
        int sh = Screen.height;
        if (positionDirty || sw != lastScreenWidth || sh != lastScreenHeight)
        {
            UpdatePosition();
            positionDirty = false;
            lastScreenWidth = sw;
            lastScreenHeight = sh;
        }

        fade = Mathf.Clamp(fadeTimer <= 0 ? fade - (Time.deltaTime / fadeOutTime) : fade + (Time.deltaTime / fadeInTime), 0, 1);
        hide = Mathf.Clamp(hideTimer <= 0 ? hide + (Time.deltaTime / hideTime) : hide - (Time.deltaTime / fadeInTime), 0, 1);
        fadeTimer -= Time.deltaTime;
        hideTimer -= Time.deltaTime;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = fade * 0.5f + (1 - hide) * 0.5f;
        }

        // 更新上一帧缓存
        wasBlockingInput = isBlockingInput;
    }

    // 新增：支持指定颜色的 AddMessage 重载


    /// <summary>
    /// 新增虚拟化渲染的AddMessage：只添加到数据，不直接创建UI
    /// </summary>
    public void AddMessage(string message)
    {
        allMessages.Add(message);
        if (allMessages.Count > maxMessages)
            allMessages.RemoveAt(0);
        lock (renderLock)
        {
            messageRenderQueue.Enqueue(message);
        }
        TryStartBackgroundRender();
    }


    /// <summary>
    /// 启动后台渲染任务（如未在渲染中）
    /// </summary>
    private void TryStartBackgroundRender()
    {
        if (isRendering) return;
        isRendering = true;
        System.Threading.Tasks.Task.Run(() => BackgroundRenderLoop());
    }


    /// <summary>
    /// 后台线程：计算可见消息、准备渲染状态
    /// </summary>
    private void BackgroundRenderLoop()
    {
        while (true)
        {
            string msg = null;
            lock (renderLock)
            {
                if (messageRenderQueue.Count > 0)
                    msg = messageRenderQueue.Dequeue();
            }
            if (msg == null) break;
            // 这里只做数据准备和可见区计算，不操作UI
            var visible = ComputeVisibleMessages();
            lock (renderLock)
            {
                backgroundVisibleMessages = visible;
            }
            // 通知主线程刷新
            MainThreadDispatcher.Run(() => ApplyBackgroundRender());
        }
        isRendering = false;
    }


    /// <summary>
    /// 计算当前可见消息（可扩展为分页、滚动等）
    /// </summary>
    private List<string> ComputeVisibleMessages()
    {
        // 这里只做简单的“显示最后N条”
        int visibleCount = GetVisibleMessageCount();
        int total = allMessages.Count;
        int end = total - 1;
        int start = Mathf.Max(0, end - visibleCount + 1);
        return allMessages.Skip(start).Take(visibleCount).ToList();
    }

    /// <summary>
    /// 主线程：应用后台渲染结果，切换UI
    /// </summary>
    private List<string> lastRenderedMessages = new List<string>();
    private void ApplyBackgroundRender()
    {
        lock (renderLock)
        {
            // 优化：如果只是新增一条消息，直接增量渲染，避免全量diff重排
            if (lastRenderedMessages.Count + 1 == backgroundVisibleMessages.Count
                && backgroundVisibleMessages.Take(lastRenderedMessages.Count).SequenceEqual(lastRenderedMessages))
            {
                AddMessageIncremental();
                lastRenderedMessages = backgroundVisibleMessages.ToList();
            }
            else
            {
                int baseIdx = allMessages.Count - backgroundVisibleMessages.Count;
                // diff渲染：只更新变动的消息
                for (int i = 0; i < backgroundVisibleMessages.Count; i++)
                {
                    int idx = baseIdx + i;
                    string newMsg = backgroundVisibleMessages[i];
                    string oldMsg = (lastRenderedMessages.Count > i) ? lastRenderedMessages[i] : null;
                    PeakText pt = null;
                    if (!activeTextMap.TryGetValue(idx, out pt))
                    {
                        if (textPool.Count > 0)
                        {
                            pt = textPool.Pop();
                            pt.gameObject.SetActive(true);
                        }
                        else
                        {
                            pt = MenuAPI.CreateText("");
                            pt.transform.SetParent(chatLogViewportTransform, false);
                        }
                        activeTextMap[idx] = pt;
                        // 新建UI对象必须设置内容
                        pt.SetText(newMsg);
                        pt.SetFontSize(fontSize);
                        pt.SetColor(offWhite);
                        pt.transform.SetSiblingIndex(i);
                    }
                    else
                    {
                        // 只有内容变化时才SetText
                        if (oldMsg != newMsg)
                        {
                            pt.SetText(newMsg);
                        }
                        // 始终设置字体和颜色（因无法diff，且SetFontSize/SetColor本身较轻）
                        pt.SetFontSize(fontSize);
                        pt.SetColor(offWhite);
                        // 只有顺序变化时才SetSiblingIndex
                        if (pt.transform.GetSiblingIndex() != i)
                            pt.transform.SetSiblingIndex(i);
                    }
                }
                // 隐藏多余UI
                int maxIdx = baseIdx;
                var toHide = activeTextMap.Keys.Where(k => k < maxIdx).ToList();
                foreach (var k in toHide)
                {
                    if (activeTextMap.TryGetValue(k, out var pt))
                    {
                        pt.gameObject.SetActive(false);
                        textPool.Push(pt);
                        activeTextMap.Remove(k);
                    }
                }
                // 更新渲染快照
                lastRenderedMessages = backgroundVisibleMessages.ToList();
            }
        }
        ResetTimers();
    }

    /// <summary>
    /// 虚拟化渲染核心：只为可见区间创建/激活PeakText对象
    /// </summary>
    /// <summary>
    /// 增量渲染：仅在AddMessage时插入一条UI对象，不刷新全区间
    /// </summary>
    /// <summary>
    /// 异步分帧插入消息，避免主线程阻塞
    /// </summary>
    private void AddMessageIncremental()
    {
        if (chatLogViewportTransform == null || allMessages.Count == 0)
            return;
        int idx = allMessages.Count - 1;
        PeakText pt = null;
        if (textPool.Count > 0)
        {
            pt = textPool.Pop();
            pt.gameObject.SetActive(true);
        }
        else
        {
            pt = MenuAPI.CreateText("");
            pt.transform.SetParent(chatLogViewportTransform, false);
        }
        var msg = allMessages[idx];
        pt.SetText(msg);
        pt.SetFontSize(fontSize);
        pt.SetColor(offWhite);
        activeTextMap[idx] = pt;
        pt.transform.SetSiblingIndex(chatLogViewportTransform.childCount - 1);
        ResetTimers();
    }

    /// <summary>
    /// 滚动时批量虚拟化：只在滚动或历史翻页时调用
    /// </summary>
    public void RefreshVirtualizedChat()
    {
        if (chatLogViewportTransform == null || allMessages.Count == 0)
            return;
        int visibleCount = GetVisibleMessageCount();
        int total = allMessages.Count;
        int end = total - 1;
        int start = Mathf.Max(0, end - visibleCount + 1);
        // 回收不在区间内的PeakText到池
        var toRemove = activeTextMap.Keys.Where(idx => idx < start || idx > end).ToList();
        foreach (var idx in toRemove)
        {
            var pt = activeTextMap[idx];
            pt.gameObject.SetActive(false);
            textPool.Push(pt);
            activeTextMap.Remove(idx);
        }
        // 激活/创建区间内PeakText
        for (int i = start; i <= end; i++)
        {
            if (!activeTextMap.ContainsKey(i))
            {
                PeakText pt = null;
                if (textPool.Count > 0)
                {
                    pt = textPool.Pop();
                    pt.gameObject.SetActive(true);
                }
                else
                {
                    pt = MenuAPI.CreateText("");
                    pt.transform.SetParent(chatLogViewportTransform, false);
                }
                var msg = allMessages[i];
                pt.SetText(msg);
                pt.SetFontSize(fontSize);
                pt.SetColor(offWhite);
                activeTextMap[i] = pt;
            }
            activeTextMap[i].transform.SetSiblingIndex(i - start);
        }
        // 多余的UI对象隐藏
        while (chatLogViewportTransform.childCount > (end - start + 1))
        {
            var extra = chatLogViewportTransform.GetChild(0);
            if (!activeTextMap.Values.Contains(extra.GetComponent<PeakText>()))
            {
                extra.gameObject.SetActive(false);
            }
        }
        ResetTimers();
    }

    /// <summary>
    /// 估算可见消息数（可根据chatLogViewportTransform高度和字体大小调整）
    /// </summary>
    private int GetVisibleMessageCount()
    {
        if (chatLogViewportTransform == null)
            return 10;
        float height = chatLogViewportTransform.rect.height;
        float line = Mathf.Max(1, fontSize * 1.2f);
        return Mathf.Max(3, Mathf.FloorToInt(height / line));
    }


    /// <summary>
    /// 聊天内容向上滚动一格（单条消息）
    /// </summary>
    public void ScrollUpOne()
    {
        if (chatLogViewportTransform != null)
        {
            var scrollRect = chatLogViewportTransform.GetComponentInParent<ScrollRect>();
            if (scrollRect != null && chatLogViewportTransform.childCount > 1)
            {
                float step = 1f / (chatLogViewportTransform.childCount - 1);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + step);
            }
        }
    }


    /// <summary>
    /// 聊天内容向下滚动一格（单条消息）
    /// </summary>
    public void ScrollDownOne()
    {
        if (chatLogViewportTransform != null)
        {
            var scrollRect = chatLogViewportTransform.GetComponentInParent<ScrollRect>();
            if (scrollRect != null && chatLogViewportTransform.childCount > 1)
            {
                float step = 1f / (chatLogViewportTransform.childCount - 1);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - step);
            }
        }
    }

}


