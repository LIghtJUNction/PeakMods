using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using PEAKLib.UI;
using PEAKLib.UI.Elements;
using PeakChatOps.Patches;
using System.Linq;

namespace PeakChatOps.Core;

public class PeakOpsUI : MonoBehaviour
{
    // ...existing code...
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

    public static PeakOpsUI instance = null!; // 聊天显示的单例引用

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

        baseTransform = this.gameObject.GetComponent<RectTransform>() ?? this.gameObject.AddComponent<RectTransform>();
        baseTransform.SetParent(this.transform, false);
        UpdatePosition();
        baseTransform.sizeDelta = boxSize;

        canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.ignoreParentGroups = false;

        // 阴影背景
        var shadow = CreateUIGameObject("Shadow", baseTransform);
        var shadowTransform = shadow.AddComponent<RectTransform>();
        Utilities.ExpandToParent(shadowTransform);
        var shadowImg = shadow.AddComponent<ProceduralImage>();
        shadowImg.color = new Color(0, 0, 0, PeakChatOpsPlugin.BgOpacity.Value);
        shadowImg.FalloffDistance = 10;
        shadowImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 10;



        var chatLogHolderObj = CreateUIGameObject("ChatLog", baseTransform);
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


        // 聊天输入框
        var peakInputGo = GameObject.Instantiate(
            typeof(PeakTextInput).GetProperty("TextInputPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null) as GameObject,
            baseTransform, false);

        peakInput = peakInputGo.GetComponent<PeakTextInput>() ?? peakInputGo.AddComponent<PeakTextInput>();
        peakInput.SetPlaceholder($"Press {chatKeyText} to chat | 按 {chatKeyText}");


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
        chatLogLayout.childControlWidth = true;
        chatLogLayout.childControlHeight = false;
        chatLogLayout.childForceExpandWidth = true;
        chatLogLayout.childForceExpandHeight = false;
        chatLogLayout.childScaleWidth = false;
        chatLogLayout.childScaleHeight = false;
        chatLogLayout.childAlignment = TextAnchor.LowerCenter;
        chatLogLayout.padding = new RectOffset((int)(fontSize * 0.6f), (int)(fontSize * 0.6f), (int)(fontSize / 20), (int)(fontSize / 20));
        chatLogLayout.spacing = -fontSize / 8;


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
                baseTransform.pivot = Vector2.one;
                baseTransform.anchorMax = Vector2.one;
                baseTransform.anchorMin = Vector2.one;
                baseTransform.anchoredPosition = new Vector2(-64, 40);
                break;
        }
    }

    GameObject currentSelection = null!;
    void Update()
    {
        if (isBlockingInput)
            framesSinceInputBlocked = -1;
            // 输入时监听鼠标滚轮，上下滚动一格
            if (inputField.isFocused)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll > 0.01f)
                    ScrollUpOne();
                else if (scroll < -0.01f)
                    ScrollDownOne();
            }

        else
            framesSinceInputBlocked = Mathf.Min(framesSinceInputBlocked + 1, 50);

        if (!this.gameObject.activeInHierarchy)
        {
            isBlockingInput = false;
            return;
        }

        if (currentSelection != EventSystem.current.currentSelectedGameObject)
        {
            currentSelection = EventSystem.current.currentSelectedGameObject;
            if (currentSelection != inputField.gameObject)
            {
                isBlockingInput = false;
            }
        }

        if (!GUIManager.instance.windowBlockingInput && Input.GetKeyDown(chatKey) && EventSystem.current != null)
        {
            // 只要输入框不可见就显示输入框
            if (!inputField.gameObject.activeInHierarchy)
            {
                ResetTimers();
                inputField.gameObject.SetActive(true);
            }
            EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
            inputField.ActivateInputField();
            isBlockingInput = true;
        }

        if (isBlockingInput)
            ResetTimers();


        UpdatePosition();

        fade = Mathf.Clamp(fadeTimer <= 0 ? fade - (Time.deltaTime / fadeOutTime) : fade + (Time.deltaTime / fadeInTime), 0, 1);
        hide = Mathf.Clamp(hideTimer <= 0 ? hide + (Time.deltaTime / hideTime) : hide - (Time.deltaTime / fadeInTime), 0, 1);
        fadeTimer -= Time.deltaTime;
        hideTimer -= Time.deltaTime;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = fade * 0.5f + (1 - hide) * 0.5f;
        }

    }

    public void AddMessage(string message)
    {
        if (chatLogViewportTransform != null)
        {
            // 使用PEAKLib.UI的MenuAPI.CreateText(string)创建文本对象
            var peakText = MenuAPI.CreateText(message);
            // 设置父物体为聊天内容容器
            peakText.transform.SetParent(chatLogViewportTransform, false);
            // 设置样式
            peakText.SetFontSize(fontSize);
            peakText.SetColor(offWhite);

            // 最大消息数控制
            while (chatLogViewportTransform.childCount > maxMessages)
            {
                var first = chatLogViewportTransform.GetChild(0);
                GameObject.Destroy(first.gameObject);
            }

            ResetTimers();
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