using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using PEAKLib.UI;
using Cysharp.Threading.Tasks;
using PEAKLib.UI.Elements;
using PeakChatOps.Patches;
using System.Linq;
using System;

namespace PeakChatOps.Core;

public class PeakOpsUI : MonoBehaviour
{
    // 缓存命令前缀字符串，减少每帧读取配置和分配的开销
    private string cachedCmdPrefixString = string.Empty;
    private PeakTextInput peakInput; // 聊天输入框逻辑组件
    int maxMessages = 100; // 聊天记录最大条数
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

    // Template instance used to Instantiate chat lines to avoid repeated heavy initialization
    private PeakText peakTextTemplate = null!;
    // Optional pool for PeakText instances
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
        try
        {
            CreateChatUI();
        }
        catch (Exception ex)
        {
            try { PeakChatOpsPlugin.Logger.LogError($"[DEBUG] Exception during CreateChatUI in Start: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"); } catch { }
        }

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
        try
        {
            // 创建聊天框 UI 的代码
            DevLog.File("[DEBUG] CreateChatUI start");

            baseTransform = this.gameObject.GetComponent<RectTransform>() ?? this.gameObject.AddComponent<RectTransform>();
            DevLog.File($"[DEBUG] baseTransform assigned: {(baseTransform!=null)}");
            baseTransform.SetParent(this.transform, false);
            UpdatePosition();
            baseTransform.sizeDelta = boxSize;

            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
            DevLog.File($"[DEBUG] canvasGroup created: {(canvasGroup!=null)}");
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.ignoreParentGroups = false;

            // 阴影背景
            var shadow = CreateUIGameObject("Shadow", baseTransform);
            DevLog.File($"[DEBUG] Shadow created: {(shadow!=null ? shadow.name : "<null>")}");
            var shadowTransform = shadow.AddComponent<RectTransform>();
            Utilities.ExpandToParent(shadowTransform);
            // 尝试使用 ProceduralImage；若不存在（不同版本或未包含库），回退到 UnityEngine.UI.Image
            try
            {
                var shadowImg = shadow.AddComponent<ProceduralImage>();
                shadowImg.color = new Color(0, 0, 0, PeakChatOpsPlugin.BgOpacity.Value);
                shadowImg.FalloffDistance = 10;
                shadowImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 10;
            }
            catch (Exception ex)
            {
                try
                {
                    var img = shadow.AddComponent<Image>();
                    img.color = new Color(0, 0, 0, PeakChatOpsPlugin.BgOpacity.Value);
                    DevLog.File($"[DEBUG] ProceduralImage unavailable, used Image instead: {ex.GetType().Name}");
                }
                catch { }
            }

        var chatLogHolderObj = CreateUIGameObject("ChatLog", baseTransform);
        DevLog.File($"[DEBUG] ChatLog holder created: {(chatLogHolderObj!=null ? chatLogHolderObj.name : "<null>")}");
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
        DevLog.File($"[DEBUG] chatLogViewportTransform created: {(chatLogViewportTransform!=null)}");
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
        DevLog.File($"[DEBUG] ScrollRect setup: content={(scrollRect.content!=null)} viewport={(scrollRect.viewport!=null)}");

        // 聊天输入框 - 防御性创建：如果 prefab 不可用或初始化出错，则安全回退，不影响其它 UI 创建
        GameObject peakInputGo = null;
        try
        {
            var prop = typeof(PeakTextInput).GetProperty("TextInputPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (prop != null)
            {
                var prefab = prop.GetValue(null) as GameObject;
                if (prefab != null)
                {
                    peakInputGo = GameObject.Instantiate(prefab, baseTransform, false) as GameObject;
                    DevLog.File($"[DEBUG] peakInputGo instantiated: {(peakInputGo!=null ? peakInputGo.name : "<null>")}");
                }
                else
                {
                    DevLog.File("[DEBUG] PeakTextInput.TextInputPrefab is null; skipping input creation");
                }
            }
            else
            {
                DevLog.File("[DEBUG] PeakTextInput.TextInputPrefab property not found; skipping input creation");
            }
        }
        catch (Exception ex)
        {
            try { PeakChatOpsPlugin.Logger.LogError($"[DEBUG] Exception instantiating PeakTextInput prefab: {ex.GetType().Name}: {ex.Message}"); } catch { }
            peakInputGo = null;
        }

        if (peakInputGo != null)
        {
            try
            {
                peakInput = peakInputGo.GetComponent<PeakTextInput>() ?? peakInputGo.AddComponent<PeakTextInput>();
                peakInput.SetPlaceholder($"Press {chatKeyText} to chat , Prefix: {PeakChatOpsPlugin.CmdPrefix.Value} .");

                var textComp = peakInput.InputField.textComponent;
                textComp.fontSize = fontSize;
                textComp.color = Utilities.GetContrastingColor(offWhite, 0.8f);
                // Font selection is handled by PeakChatOpsText; do not override here.
                inputField = peakInput.InputField;
                var inputFieldTransform = (RectTransform)inputField.transform;
                inputFieldTransform.SetParent(baseTransform, false);
                inputFieldTransform.anchorMin = new Vector2(0, 0);
                inputFieldTransform.anchorMax = new Vector2(1, 0);
                inputFieldTransform.pivot = new Vector2(0.5f, 0);
                inputFieldTransform.offsetMin = new Vector2(5, 5);
                inputFieldTransform.offsetMax = new Vector2(-5, fontSize * 1.75f);
            }
            catch (Exception ex)
            {
                try { PeakChatOpsPlugin.Logger.LogError($"[DEBUG] Error initializing PeakTextInput: {ex.GetType().Name}: {ex.Message}"); } catch { }
                peakInput = null;
                inputField = null;
            }
        }
        else
        {
            DevLog.File("[DEBUG] Input prefab not created; continuing without input field.");
            peakInput = null;
            inputField = null;
        }

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
        DevLog.File($"[DEBUG] chatLogLayout configured: spacing={chatLogLayout.spacing}");

        #region 交互事件绑定
        // 绑定输入框提交事件
        inputField.onSubmit.AddListener((e) =>
        {
            inputField.text = "";
            inputField.textComponent.text = "";
            DevLog.File($"[DEBUG] onSubmit fired: '{e}'");
            if (ChatSystem.Instance != null)
            {
                ChatSystem.Instance.SendChatMessageAsync(e, new Dictionary<string, object>()).Forget();
            }
            else
            {
                DevLog.File($"[DEBUG] onSubmit: ChatSystem.Instance null, enqueueing pending send: '{e}'");
                ChatSystem.EnqueuePendingSend(e, new Dictionary<string, object>());
            }
        });
        // 绑定输入框选中事件
        inputField.onEndEdit.AddListener((e) =>
        {
            EventSystem.current.SetSelectedGameObject(null);
            isBlockingInput = false;
        });
        #endregion
        if (PeakChatOpsPlugin.FrameVisible.Value)
        {
            var border = CreateUIGameObject("Border", baseTransform);
            var borderTransform = border.AddComponent<RectTransform>();
            Utilities.ExpandToParent(borderTransform);
            var borderImg = border.AddComponent<ProceduralImage>();
            borderImg.color = offWhite;
            borderImg.BorderWidth = 2;
            borderImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 5;
            DevLog.File($"[DEBUG] Border created: {(border!=null ? border.name : "<null>")}");
        }
            // 初始化 PeakText 模板与对象池，减少运行时首次创建开销
            try
            {
                // Create a template PeakText (inactive) so clones inherit desired structure
                try
                {
                    peakTextTemplate = MenuAPI.CreateText("");
                    if (peakTextTemplate != null)
                    {
                        peakTextTemplate.name = "UI_PeakText_Template";
                        try { peakTextTemplate.transform.SetParent(baseTransform, false); } catch { }
                        try { peakTextTemplate.gameObject.SetActive(false); } catch { }
                    }
                }
                catch (Exception ex)
                {
                    DevLog.File($"[DEBUG] Failed to create peakTextTemplate: {ex.GetType().Name}: {ex.Message}");
                }

                // Create pool (use baseTransform as parent for the pool container). Warm up a few instances.
                try
                {
                    // peakTextPool = new PeakTextPool(baseTransform, peakTextTemplate, maxPoolSize: 100, poolContainerName: "PeakTextPool", warmupCount: 0);
                    DevLog.File("[DEBUG] PeakTextPool initialized");
                }
                catch (Exception ex)
                {
                    DevLog.File($"[DEBUG] Failed to initialize PeakTextPool: {ex.GetType().Name}: {ex.Message}");
                }
            }
            catch { }

            // 如果在 UI 创建前有缓存的消息，刷新它们
        if (pendingMessages != null && pendingMessages.Count > 0)
        {
            DevLog.File($"[DEBUG] Flushing {pendingMessages.Count} pending messages");
            try
            {
                foreach (var m in pendingMessages.ToArray())
                {
                    AddMessage(m);
                }
            }
            catch (Exception ex)
            {
                DevLog.File($"[DEBUG] Exception while flushing pending messages: {ex.Message}");
            }
            pendingMessages.Clear();
            DevLog.File("[DEBUG] Pending messages flushed");
        }
        
            DevLog.File("[DEBUG] CreateChatUI end");

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
                DevLog.File($"[DEBUG] Failed to add init message: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            try { PeakChatOpsPlugin.Logger.LogError($"[DEBUG] Exception in CreateChatUI: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"); } catch { }
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
        // 优化：缓存第一个字符和字符串，避免每帧分配（例如 p.ToString()）或多次读取配置
        try
        {
            var prefix = PeakChatOpsPlugin.CmdPrefix.Value ?? "/";
            if (!string.IsNullOrEmpty(prefix))
            {
                // 更新缓存（仅在配置变化时会改变）
                if (cachedCmdPrefixString != prefix)
                {
                    cachedCmdPrefixString = prefix;
                }

                // 使用缓存的字符串调用 Input.GetKeyDown，避免每帧分配
                if (!string.IsNullOrEmpty(cachedCmdPrefixString) && Input.GetKeyDown(cachedCmdPrefixString))
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
                        var cachedPrefix = cachedCmdPrefixString;
                        if (string.IsNullOrEmpty(input.text) || string.IsNullOrEmpty(cachedPrefix) || !input.text.StartsWith(cachedPrefix))
                        {
                            // 如果 cachedPrefix 为空，退回到配置读取作为最后手段
                            var toSet = !string.IsNullOrEmpty(cachedPrefix) ? cachedPrefix : (PeakChatOpsPlugin.CmdPrefix.Value ?? "/");
                            input.text = toSet;
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

    public void AddMessage(string message)
    {
        // 调试短路：当 DisableAddMessage 为 true 时，使用占位实现仅记录日志并立即返回。
        // if (true)
        // {
        //     try { DevLog.File($"[DEBUG] AddMessage skipped due to DisableAddMessage=true. message='{message}'"); } catch { }
        //     return;
        // }
        // 实验结果，还真TM的是字多了就卡
        DevLog.File($"[DEBUG] AddMessage called. message='{message}' chatLogViewportTransform={(chatLogViewportTransform!=null)} childCount={(chatLogViewportTransform!=null?chatLogViewportTransform.childCount:-1)}");

        // Timer for this AddMessage invocation
#if DEBUG
        var __timerKey = $"AddMessage::{Guid.NewGuid():N}";
#endif
        // DevLog.TimeStart(__timerKey);
        if (chatLogViewportTransform == null)
        {
            // UI 尚未创建，缓存消息以便稍后刷新
            pendingMessages ??= new List<string>();
            pendingMessages.Add(message);
            // DevLog.TimeLap(__timerKey, "queued");
            DevLog.File($"[DEBUG] AddMessage queued. pendingCount={pendingMessages.Count}");
            // DevLog.TimeStop(__timerKey, "queued_path");
            return;
        }

        try
        {
            // DevLog.TimeLap(__timerKey, "create_text_start");
            DevLog.File("[DEBUG] Creating PeakChatOpsText for message");

            // PeakChatOpsText peakText = peakTextPool.Get(message);

            //  DevLog.TimeLap(__timerKey, "before_setparent");
            // peakText.transform.SetParent(chatLogViewportTransform, false);
            //  DevLog.TimeLap(__timerKey, "after_setparent");

            // peakText.SetFontSize(fontSize);
            
            DevLog.File($"[DEBUG] After SetParent childCount={chatLogViewportTransform.childCount}");
            // DevLog.TimeLap(__timerKey, "after_setparent_childcount");

            // 优化：只移除一条，避免多次Destroy
            if (chatLogViewportTransform.childCount > maxMessages)
            {
                // DevLog.TimeLap(__timerKey, "remove_old_start");
                var first = chatLogViewportTransform.GetChild(0);
                if (first != null)
                {
                    var pt = first.GetComponent<PeakText>();

                }
                DevLog.TimeLap(__timerKey, "remove_old_end");
            }
            DevLog.File($"[DEBUG] Message added. new childCount={chatLogViewportTransform.childCount}");

            ResetTimers();

        }
        catch (Exception ex)
        {
            DevLog.File($"[DEBUG] Exception in AddMessage: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");

        }
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