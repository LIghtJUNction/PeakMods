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

namespace PeakChatOps;

public static class ChatFontUtil
{
    private static TMP_FontAsset _darumaFontAsset;
    public static TMP_FontAsset DarumaDropOneFont
    {
        get
        {
            if (_darumaFontAsset == null)
            {
                var assets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                _darumaFontAsset = assets.FirstOrDefault(fontAsset =>
                    fontAsset.faceInfo.familyName == "Daruma Drop One"
                );
            }
            return _darumaFontAsset;
        }
    }
}

public class TextChatDisplay : MonoBehaviour {
    int maxMessages = 30;
    Vector2 boxSize = new Vector2(500,300);
    float fadeInTime = 0.03f;
    float fadeOutTime = 5;
    float hideTime = 5;
    float fadeOutDelay = 15;
    float hideDelay = 40;
    float messageHideDelay = 40;
    float fontSize = 25;
    bool usingIMGUI = false;

    TMP_InputField inputField = null!;
    RectTransform chatLogViewportTransform = null!;
    RectTransform baseTransform = null!;
    CanvasGroup canvasGroup = null!;
    ScrollRect scrollRect = null!;

    float fade = 1;
    float fadeTimer = -1;

    float hide = 0;
    float hideTimer = -1;

    KeyCode chatKey;
    string chatKeyText;

    Color offWhite = new Color(0.87f,0.85f,0.76f);
    Color red = new Color(0.99f,0.33f,0);

    List<ChatMessage> messages = new List<ChatMessage>();

    public bool isBlockingInput = false;
    public int framesSinceInputBlocked = 0;

    public static TextChatDisplay instance = null!;

    string imguiFieldString = "";
    bool imguiTyping = false;

    void Awake() {
        instance = this;
    }

    void Start() {
        chatKey = PeakChatOpsPlugin.configKey.Value;
        chatKeyText = chatKey.ToString();
        fontSize = PeakChatOpsPlugin.configFontSize.Value < 0 ? 1000000000 : PeakChatOpsPlugin.configFontSize.Value;
        hideDelay = PeakChatOpsPlugin.configHideDelay.Value < 0 ? Mathf.Infinity : PeakChatOpsPlugin.configHideDelay.Value;
        fadeOutDelay = Mathf.Min(PeakChatOpsPlugin.configFadeDelay.Value < 0 ? Mathf.Infinity : PeakChatOpsPlugin.configFadeDelay.Value,hideDelay);
        messageHideDelay = PeakChatOpsPlugin.configMessageFadeDelay.Value < 0 ? Mathf.Infinity : PeakChatOpsPlugin.configMessageFadeDelay.Value;
        usingIMGUI = PeakChatOpsPlugin.configIMGUI.Value;
        
        var chatSizeConfigSplit = PeakChatOpsPlugin.configChatSize.Value.Split(':');
        if (chatSizeConfigSplit.Length >= 2) {
            var xString = chatSizeConfigSplit[0].Replace(" ","");
            var yString = chatSizeConfigSplit[1].Replace(" ","");

            if (float.TryParse(xString,out float chatSizeX) && float.TryParse(yString,out float chatSizeY)) {
                boxSize = new Vector2(Mathf.Max(chatSizeX,10),Mathf.Max(chatSizeY,10));
            } else {
                boxSize = new Vector2(500,300);
            }
        }

        ResetTimers();
        SetupChatGUI();
    }

    GameObject currentSelection = null!;

    void Update() {
        if (isBlockingInput)
            framesSinceInputBlocked = -1;
        else
            framesSinceInputBlocked = Mathf.Min(framesSinceInputBlocked + 1,50);

        if (!this.gameObject.activeInHierarchy) {
            isBlockingInput = false;
            return;
        }

        if (currentSelection != EventSystem.current.currentSelectedGameObject) {
            currentSelection = EventSystem.current.currentSelectedGameObject;
            if (currentSelection != inputField.gameObject) {
                isBlockingInput = false;
            }
        }

        if (!GUIManager.instance.windowBlockingInput && Input.GetKeyDown(chatKey) && EventSystem.current != null) {
            // 如果启用了输入框隐藏功能且输入框不可见，先显示聊天框和输入框
            if (PeakChatOpsPlugin.configHideInputField.Value && !inputField.gameObject.activeInHierarchy) {
                ResetTimers(); // 重置计时器，让聊天框显示
                inputField.gameObject.SetActive(true); // 显示输入框
            }
            
            if (usingIMGUI) {
                imguiTyping = true;
                GUI.FocusControl("ptctf");
            } else {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject,null);
                inputField.ActivateInputField();
            }
            isBlockingInput = true;
        }

        if (isBlockingInput)
            ResetTimers();

        // 添加键盘滚动控制
        HandleScrollInput();

        if (PeakChatOpsPlugin.configPos.Value == UIAlignment.BottomLeft)
            UpdatePosition(true);

        fade = Mathf.Clamp(fadeTimer <= 0 ? fade - (Time.deltaTime / fadeOutTime) : fade + (Time.deltaTime / fadeInTime),0,1);
        hide = Mathf.Clamp(hideTimer <= 0 ? hide + (Time.deltaTime / hideTime) : hide - (Time.deltaTime / fadeInTime),0,1);
        fadeTimer -= Time.deltaTime;
        hideTimer -= Time.deltaTime;
        if (canvasGroup != null) {
            canvasGroup.alpha = fade * 0.5f + (1 - hide) * 0.5f;
        }

        // 控制输入框的可见性 - 当聊天框完全隐藏时，输入框也隐藏
        UpdateInputFieldVisibility();

        foreach (var chatMessage in messages)
            chatMessage.Update();
    }

    /// <summary>
    /// 创建UI游戏对象的辅助方法
    /// </summary>
    private GameObject CreateUIGameObject(string name, Transform parent)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj;
    }

    void SetupChatGUI() {
        var guiManager = GUIManager.instance;
        
        baseTransform = this.gameObject.GetComponent<RectTransform>() ?? this.gameObject.AddComponent<RectTransform>();
        baseTransform.SetParent(this.transform,false);
        UpdatePosition();
        baseTransform.sizeDelta = boxSize;

        canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.ignoreParentGroups = false;

        var shadow = CreateUIGameObject("Shadow", baseTransform);
        var shadowTransform = shadow.AddComponent<RectTransform>();
        Utilities.ExpandToParent(shadowTransform);
        var shadowImg = shadow.AddComponent<ProceduralImage>();
        shadowImg.color = new Color(0,0,0,PeakChatOpsPlugin.configBgOpacity.Value);
        shadowImg.FalloffDistance = 10;
        shadowImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 10;

        // var bgImage = this.gameObject.AddComponent<ProceduralImage>();
        // bgImage.color = new Color(0,0,0,0.6f);
        // bgImage.BorderWidth = 3;
        // bgImage.SetModifierType<UniformModifier>().Radius = 5;

        var chatLogHolderObj = CreateUIGameObject("ChatLog", baseTransform);
        var chatLogHolderTransform = chatLogHolderObj.AddComponent<RectTransform>();
        chatLogHolderTransform.anchorMin = Vector2.zero;
        chatLogHolderTransform.anchorMax = Vector2.one;
        chatLogHolderTransform.offsetMin = new Vector2(0,fontSize * 2);
        chatLogHolderTransform.offsetMax = Vector2.zero;
        
        // 添加ScrollRect组件来支持滚动
        scrollRect = chatLogHolderObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = fontSize * 2;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        
        // 设置遮罩 - 在ScrollRect所在的GameObject上
        chatLogHolderObj.AddComponent<RectMask2D>();

        // 创建内容容器
        var chatLogContentObj = CreateUIGameObject("Content", chatLogHolderTransform);
        chatLogViewportTransform = chatLogContentObj.AddComponent<RectTransform>();
        
        // 内容区域设置 - 从底部开始，向上增长
        chatLogViewportTransform.pivot = new Vector2(0.5f, 0);  // 底部中心为锚点
        chatLogViewportTransform.anchorMin = new Vector2(0, 0);
        chatLogViewportTransform.anchorMax = new Vector2(1, 0);
        chatLogViewportTransform.offsetMin = Vector2.zero;
        chatLogViewportTransform.offsetMax = Vector2.zero;
        chatLogViewportTransform.anchoredPosition = Vector2.zero;
        
        // 添加ContentSizeFitter以自动调整内容大小
        var contentSizeFitter = chatLogContentObj.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        
        // 正确设置ScrollRect的引用
        scrollRect.content = chatLogViewportTransform;      // content指向可滚动的内容
        scrollRect.viewport = chatLogHolderTransform;       // viewport指向可见区域

        inputField = CreateInputField();
        var inputFieldTransform = (RectTransform)inputField.transform;
        inputFieldTransform.pivot = new Vector2(0.5f,0);
        inputFieldTransform.anchorMin = new Vector2(0,0);
        inputFieldTransform.anchorMax = new Vector2(1,0);
        inputFieldTransform.offsetMin = new Vector2(5,5);
        inputFieldTransform.offsetMax = new Vector2(-5,fontSize * 1.75f);
        inputFieldTransform.SetParent(baseTransform,false);

        var chatLogLayout = chatLogContentObj.AddComponent<VerticalLayoutGroup>();
        chatLogLayout.childControlWidth = true;
        chatLogLayout.childControlHeight = false;
        chatLogLayout.childForceExpandWidth = true;
        chatLogLayout.childForceExpandHeight = false;
        chatLogLayout.childScaleWidth = false;
        chatLogLayout.childScaleHeight = false;
        chatLogLayout.childAlignment = TextAnchor.LowerCenter;
        chatLogLayout.padding = new RectOffset((int)(fontSize * 0.6f),(int)(fontSize * 0.6f),(int)(fontSize / 20),(int)(fontSize / 20));
        chatLogLayout.spacing = -fontSize / 8;

        inputField.onSubmit.AddListener((e) => {
            inputField.text = "";
            inputField.textComponent.text = "";
            // 使用ChatSystem处理消息而不是直接发送
            PeakChatOps.Core.ChatSystem.Instance.ProcessMessage(e);
        });

        inputField.onEndEdit.AddListener((e) => {
            EventSystem.current.SetSelectedGameObject(null);
            isBlockingInput = false;
        });

        if (PeakChatOpsPlugin.configFrameVisible.Value) {
            var border = CreateUIGameObject("Border", baseTransform);
            var borderTransform = border.AddComponent<RectTransform>();
            Utilities.ExpandToParent(borderTransform);
            var borderImg = border.AddComponent<ProceduralImage>();
            borderImg.color = offWhite;
            borderImg.BorderWidth = 2;
            borderImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 5;
        }
    }

    TMP_InputField CreateInputField() {
        var inputFieldObj = CreateUIGameObject("InputField", transform);
        TMP_InputField inputField = inputFieldObj.AddComponent<TMP_InputField>();
        inputField.enabled = false;

        var inputFieldImg = inputField.gameObject.AddComponent<ProceduralImage>();
        inputField.targetGraphic = inputFieldImg;
        // inputField.transition = Selectable.Transition.None;
        inputFieldImg.color = offWhite;
        inputFieldImg.SetModifierType<UniformModifier>().Radius = fontSize / 4;

        var textAreaObj = CreateUIGameObject("Text Area", inputField.transform);
        var textAreaTransform = textAreaObj.AddComponent<RectTransform>();
        textAreaTransform.anchorMax = Vector2.one;
        textAreaTransform.anchorMin = Vector2.zero;
        textAreaTransform.offsetMax = new Vector2(-fontSize / 2,-fontSize * 0.3f);
        textAreaTransform.offsetMin = new Vector2(fontSize / 2,fontSize * 0.3f);
        textAreaTransform.SetParent(inputField.transform,false);
        var textAreaMask = textAreaObj.AddComponent<RectMask2D>();
        textAreaMask.padding = new Vector4(-fontSize * 0.4f,-fontSize / 4,-fontSize * 0.4f,-fontSize / 4);
        
        var placeholderText = CreateText(textAreaTransform);
        placeholderText.color = Utilities.GetContrastingColor(offWhite, 0.4f);
        placeholderText.text = $"Press {chatKeyText} to chat";
        placeholderText.name = "Placeholder";

        var mainText = CreateText(textAreaTransform);
        mainText.color = Utilities.GetContrastingColor(offWhite, 0.8f);

        inputField.textViewport = textAreaTransform;
        inputField.textComponent = mainText;
        inputField.placeholder = placeholderText;
        inputField.richText = false;
        inputField.restoreOriginalTextOnEscape = false;
        inputField.enabled = true;

        return inputField;
    }

    TMP_Text CreateText(Transform parent) {
        var textObj = CreateUIGameObject("Text", parent);
        var textTransform = textObj.AddComponent<RectTransform>();
        textTransform.SetParent(parent,false);
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.one;
        textTransform.offsetMin = new Vector2(2,4);
        textTransform.offsetMax = new Vector2(-2,0);
        
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "New Text";
        if (ChatFontUtil.DarumaDropOneFont != null)
            text.font = ChatFontUtil.DarumaDropOneFont;
        text.fontSize = fontSize;
        text.horizontalAlignment = HorizontalAlignmentOptions.Left;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;

        return text;
    }

    public void AddMessage(string message) {
        AddMessage(message, "System", offWhite, ChatMessageType.System);
    }
    
    /// <summary>
    /// 添加带发送者信息的消息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="senderName">发送者名称</param>
    /// <param name="senderColor">发送者颜色</param>
    /// <param name="messageType">消息类型</param>
    public void AddMessage(string message, string senderName, Color senderColor, ChatMessageType messageType) {
        if (chatLogViewportTransform != null) {
            var tmpText = CreateText(chatLogViewportTransform);
            
            // 根据消息类型设置不同的格式
            string formattedMessage = FormatMessageByType(message, senderName, senderColor, messageType);
            
            tmpText.text = formattedMessage;
            tmpText.color = offWhite;
            tmpText.lineSpacing = -fontSize * 0.75f;
            var prefValues = tmpText.GetPreferredValues(formattedMessage, boxSize.x - 24, 1000);

            ((RectTransform)tmpText.transform).sizeDelta = new Vector2(0, prefValues.y);
            var chatMessage = new ChatMessage(message, senderName, senderColor, messageType, tmpText.gameObject, messageHideDelay);
            messages.Add(chatMessage);
            
            if (messages.Count > maxMessages) {
                var firstMessage = messages[0];
                if (firstMessage != null && firstMessage.textObj != null) {
                    GameObject.Destroy(firstMessage.textObj);
                }
                messages.RemoveAt(0);
            }
            
            // 自动滚动到底部显示最新消息
            AutoScrollToBottom();
            
            ResetTimers();
        }
    }
    
    /// <summary>
    /// 根据消息类型格式化消息
    /// </summary>
    private string FormatMessageByType(string message, string senderName, Color senderColor, ChatMessageType messageType) {
        switch (messageType) {
            case ChatMessageType.System:
                return $"<color=#87CEEB>[SYSTEM]</color> {message}";
                
            case ChatMessageType.SystemGlobal:
                return $"<color=#FFD700>[SYSTEM]</color> {message}";
                
            case ChatMessageType.Command:
                return $"<color=#98FB98>[CMD]</color> {message}";
                
            case ChatMessageType.Error:
                return $"<color=#FF6B6B>[ERROR]</color> {message}";
                
            case ChatMessageType.Admin:
                var adminLabel = $"<color=#FF69B4>[ADMIN]</color>";
                var adminSenderLabel = $"<color=#{ColorUtility.ToHtmlStringRGB(senderColor)}>[{senderName}]</color>";
                return $"{adminLabel} {adminSenderLabel}: {message}";
                
            case ChatMessageType.Normal:
            default:
                if (!string.IsNullOrEmpty(senderName) && senderName != "System") {
                    var normalSenderLabel = $"<color=#{ColorUtility.ToHtmlStringRGB(senderColor)}>[{senderName}]</color>";
                    return $"{normalSenderLabel}: {message}";
                }
                return message;
        }
    }

    /// <summary>
    /// 添加系统消息（特殊格式）
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="isGlobal">是否为全局消息（所有人可见）</param>
    public void AddSystemMessage(string message, bool isGlobal = false) {
        var messageType = isGlobal ? ChatMessageType.SystemGlobal : ChatMessageType.System;
        AddMessage(message, "System", offWhite, messageType);
    }

    void ResetTimers() {
        fadeTimer = fadeOutDelay;
        hideTimer = hideDelay;
    }

    void HandleScrollInput() {
        // 只有在scrollRect存在时才处理滚动，允许在输入时滚动
        if (scrollRect == null) return;

        // 使用PageUp/PageDown滚动
        if (Input.GetKeyDown(KeyCode.PageUp)) {
            PeakChatOpsPlugin.Logger.LogDebug("PageUp pressed - scrolling up");
            ScrollByLines(-5); // 向上滚动5行
        }
        else if (Input.GetKeyDown(KeyCode.PageDown)) {
            PeakChatOpsPlugin.Logger.LogDebug("PageDown pressed - scrolling down");
            ScrollByLines(5); // 向下滚动5行
        }
        // 使用方向键滚动（只在不处于输入模式或按住Ctrl时）
        else if (Input.GetKey(KeyCode.UpArrow) && (Input.GetKey(KeyCode.LeftControl) || !isBlockingInput)) {
            ScrollByLines(-1); // Ctrl+↑ 或非输入模式时向上滚动1行
        }
        else if (Input.GetKey(KeyCode.DownArrow) && (Input.GetKey(KeyCode.LeftControl) || !isBlockingInput)) {
            ScrollByLines(1); // Ctrl+↓ 或非输入模式时向下滚动1行
        }
        // Home/End键快速跳转
        else if (Input.GetKeyDown(KeyCode.Home) && Input.GetKey(KeyCode.LeftControl)) {
            PeakChatOpsPlugin.Logger.LogDebug("Ctrl+Home pressed - jumping to top");
            scrollRect.verticalNormalizedPosition = 1f; // 跳转到顶部
        }
        else if (Input.GetKeyDown(KeyCode.End) && Input.GetKey(KeyCode.LeftControl)) {
            PeakChatOpsPlugin.Logger.LogDebug("Ctrl+End pressed - jumping to bottom");
            scrollRect.verticalNormalizedPosition = 0f; // 跳转到底部
        }
        // 鼠标滚轮滚动（始终可用）
        else if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            PeakChatOpsPlugin.Logger.LogDebug($"Mouse wheel: {scrollDelta}");
            ScrollByLines(scrollDelta > 0 ? -2 : 2); // 滚轮向上时向上滚动，向下时向下滚动
        }
    }

    void ScrollByLines(int lines) {
        if (scrollRect == null) {
            PeakChatOpsPlugin.Logger.LogWarning("ScrollRect is null, cannot scroll");
            return;
        }
        
        float currentPos = scrollRect.verticalNormalizedPosition;
        
        // 使用标准的ScrollRect滚动方法
        float scrollAmount = lines * 0.1f; // 每行滚动10%
        float newPosition = currentPos + scrollAmount;
        
        // 限制在0-1范围内
        newPosition = Mathf.Clamp01(newPosition);
        scrollRect.verticalNormalizedPosition = newPosition;
        
        PeakChatOpsPlugin.Logger.LogDebug($"Scroll: lines={lines}, from {currentPos:F2} to {newPosition:F2}");
    }

    void AutoScrollToBottom() {
        if (scrollRect != null) {
            // 延迟一帧后滚动，确保布局更新完成
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    void UpdateInputFieldVisibility() {
        if (inputField != null && PeakChatOpsPlugin.configHideInputField.Value) {
            // 当聊天框完全隐藏时，输入框也隐藏
            bool shouldShowInput = isBlockingInput || hide < 0.9f || fade > 0.1f;
            
            // 控制输入框的可见性和交互性
            inputField.gameObject.SetActive(shouldShowInput);
            
            // 如果输入框被隐藏且当前正在输入，则停止输入
            if (!shouldShowInput && isBlockingInput) {
                isBlockingInput = false;
                if (EventSystem.current != null) {
                    EventSystem.current.SetSelectedGameObject(null);
                }
                if (usingIMGUI) {
                    imguiTyping = false;
                    GUI.FocusControl("");
                }
            }
        }
    }

    System.Collections.IEnumerator ScrollToBottomNextFrame() {
        yield return null; // 等待一帧
        if (scrollRect != null) {
            scrollRect.verticalNormalizedPosition = 0f; // 滚动到底部
        }
    }

    void UpdatePosition(bool isBottomLeft = false) {
        var alignment = isBottomLeft ? UIAlignment.BottomLeft : PeakChatOpsPlugin.configPos.Value;
        
        switch (alignment) {
            case UIAlignment.BottomLeft:
                if (baseTransform != null && StaminaBarPatch.barGroupChildWatcher.textChatDummyTransform != null) {
                    baseTransform.pivot = Vector2.zero;
                    baseTransform.anchorMax = Vector2.zero;
                    baseTransform.anchorMin = Vector2.zero;
                    baseTransform.position = StaminaBarPatch.barGroupChildWatcher.textChatDummyTransform.position;
                    baseTransform.anchoredPosition += new Vector2(0,40);
                }
                break;
                
            case UIAlignment.TopLeft:
                baseTransform.pivot = new Vector2(0,1);
                baseTransform.anchorMax = new Vector2(0,1);
                baseTransform.anchorMin = new Vector2(0,1);
                baseTransform.anchoredPosition = new Vector2(64,-62);
                break;
                
            case UIAlignment.TopRight:
                baseTransform.pivot = Vector2.one;
                baseTransform.anchorMax = Vector2.one;
                baseTransform.anchorMin = Vector2.one;
                baseTransform.anchoredPosition = new Vector2(-64,-62);
                break;
                
            // 现在我们可以轻松添加新的位置选项
            case UIAlignment.TopCenter:
                baseTransform.pivot = new Vector2(0.5f,1);
                baseTransform.anchorMax = new Vector2(0.5f,1);
                baseTransform.anchorMin = new Vector2(0.5f,1);
                baseTransform.anchoredPosition = new Vector2(0,-62);
                break;
                
            case UIAlignment.BottomCenter:
                baseTransform.pivot = new Vector2(0.5f,0);
                baseTransform.anchorMax = new Vector2(0.5f,0);
                baseTransform.anchorMin = new Vector2(0.5f,0);
                baseTransform.anchoredPosition = new Vector2(0,40);
                break;
                
            case UIAlignment.BottomRight:
                baseTransform.pivot = Vector2.one;
                baseTransform.anchorMax = Vector2.one;
                baseTransform.anchorMin = Vector2.one;
                baseTransform.anchoredPosition = new Vector2(-64,40);
                break;
        }
    }

    void OnGUI() {
        if (!imguiTyping)
            return;

        GUI.SetNextControlName("ptctf");
        var guiStyle = new GUIStyle();
        guiStyle.fontSize = (int)(fontSize * 0.8f);
        // var bg = new Texture2D(1,1,TextureFormat.RGBAFloat,false);
        // bg.SetPixel(0,0,new Color(0,0,0,0.4f));
        // bg.Apply();
        // guiStyle.normal.background = bg;
        guiStyle.clipping = TextClipping.Clip;
        guiStyle.padding = new RectOffset((int)(fontSize * 0.7f),(int)(fontSize * 0.7f),(int)(fontSize * 0.24f),0);
        var rect = RectTransformToScreenSpace((RectTransform)inputField.transform);
        PeakChatOpsPlugin.Logger.LogInfo(rect.xMin + " " + rect.yMin);
        imguiFieldString = GUI.TextArea(rect,imguiFieldString,guiStyle);

        inputField.text = " ";

        if (GUI.GetNameOfFocusedControl() != "ptctf")
            GUI.FocusControl("ptctf");

        if (imguiFieldString.Contains("\n")) {
            // 使用ChatSystem处理消息而不是直接发送
            PeakChatOps.Core.ChatSystem.Instance.ProcessMessage(imguiFieldString.Replace("\n",""));
            imguiFieldString = "";
            GUI.FocusControl("");
            Event.current.Use();
            imguiTyping = false;
            isBlockingInput = false;
            inputField.text = "";
        } else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape) {
            GUI.FocusControl("");
            imguiTyping = false;
            inputField.text = imguiFieldString;
            isBlockingInput = false;
            Event.current.Use();
        }
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        // return RectTransformUtility.PixelAdjustRect(transform,GUIManager.instance.hudCanvas);
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        var pos = new Vector2(transform.position.x,Screen.height - transform.position.y);
        return new Rect(pos - (size * new Vector2(0.5f,1)), size);
    }

    public class ChatMessage {
        public string message;
        public string senderName;
        public Color senderColor;
        public ChatMessageType messageType;
        public GameObject textObj;
        public TMP_Text text;
        Color textColor;

        float hideDelay = 40;
        // 移除未使用的变量，因为文本永不消失
        // float hideTime = 10;
        // float hideTimer = -1;
        // float hide = 0;

        public void Update() {
            // 文本永不消失 - 注释掉淡出逻辑
            /*
            hideTimer -= Time.deltaTime;

            if (hideTimer <= 0) {
                if (hide < 1) {
                    hide += Time.deltaTime / hideTime;
                    text.color = new Color(textColor.r,textColor.g,textColor.b,1 - hide);
                }
            }
            */
        }
        
        // 原有构造函数 - 用于普通消息（向后兼容）
        public ChatMessage(string message, GameObject textObject, float hideDelay) 
            : this(message, "Unknown", Color.white, ChatMessageType.Normal, textObject, hideDelay) {
        }
        
        // 扩展构造函数 - 包含发送者信息
        public ChatMessage(string message, string senderName, Color senderColor, ChatMessageType messageType, GameObject textObject, float hideDelay) {
            this.message = message;
            this.senderName = senderName;
            this.senderColor = senderColor;
            this.messageType = messageType;
            this.textObj = textObject;
            this.text = textObject.GetComponent<TMP_Text>();
            this.textColor = this.text.color;
            this.hideDelay = hideDelay;
            // hideTimer = hideDelay; // 不再需要
        }
        
        /// <summary>
        /// 获取格式化的发送者标签
        /// </summary>
        /// <returns>带颜色的发送者标签</returns>
        public string GetSenderLabel() {
            if (string.IsNullOrEmpty(senderName) || senderName == "Unknown") {
                return "";
            }
            
            return $"<color=#{ColorUtility.ToHtmlStringRGB(senderColor)}>[{senderName}]</color>";
        }
        
        /// <summary>
        /// 获取完整的格式化消息
        /// </summary>
        /// <returns>包含发送者标签的完整消息</returns>
        public string GetFormattedMessage() {
            var senderLabel = GetSenderLabel();
            if (string.IsNullOrEmpty(senderLabel)) {
                return message;
            }
            
            return $"{senderLabel}: {message}";
        }
    }
    
    /// <summary>
    /// 聊天消息类型枚举
    /// </summary>
    public enum ChatMessageType {
        Normal,         // 普通玩家消息
        System,         // 系统消息
        SystemGlobal,   // 全局系统消息
        Command,        // 命令响应
        Error,          // 错误消息
        Admin           // 管理员消息
    }
}

public class BarGroupChildWatcher : MonoBehaviour {
    public Transform textChatDummyTransform = null!;

    void OnTransformChildrenChanged() {
        if (textChatDummyTransform != null && textChatDummyTransform.GetSiblingIndex() > 0) {
            textChatDummyTransform.SetAsFirstSibling();
        }
    }
}