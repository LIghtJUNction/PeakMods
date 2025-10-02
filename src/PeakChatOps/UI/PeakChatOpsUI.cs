using System.IO;
using BepInEx;
using PeakChatOps.Core;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

namespace PeakChatOps.UI;

public class PeakChatOpsUI : MonoBehaviour
{
    public static PeakChatOpsUI Instance { get; private set; }
    public static UIDocument uIDocument;
    public bool isBlockingInput;
    private TextField messageInputField;

    void Start()
    {
        if (Instance != null)
        {
            DevLog.File("PeakChatOpsUI instance already exists, destroying duplicate.");
            Destroy(this);
            return;
        }

        Instance = this;
        DevLog.File("PeakChatOpsUI instance created.");
    }

    public void BindUIEvents(GameObject uiGO)
    {
        if (uiGO == null)
        {
            DevLog.File("BindUIEvents: uiGO is null, cannot bind events.");
            return;
        }

        uIDocument = uiGO.GetComponent<UIDocument>();
        if (uIDocument == null)
        {
            DevLog.File("BindUIEvents: UIDocument component not found on uiGO.");
            return;
        }

        var root = uIDocument.rootVisualElement;

        // 绑定发送按钮
        var sendButton = root.Q<Button>("send-button");
        if (sendButton != null)
        {
            sendButton.clicked += OnSendMessage;
            DevLog.File("Bound send-button event.");
        }

        // 绑定关闭按钮
        var closeButton = root.Q<Button>("close-button");
        if (closeButton != null)
        {
            closeButton.clicked += () => HideNow();
            DevLog.File("Bound close-button event.");
        }

        // 绑定最小化按钮
        var minimizeButton = root.Q<Button>("minimize-button");
        if (minimizeButton != null)
        {
            minimizeButton.clicked += MinimizeUI;
            DevLog.File("Bound minimize-button event.");
        }

        // 保存输入框引用（按键检测在 Update 中处理）
        messageInputField = root.Q<TextField>("message-input");
        if (messageInputField != null)
        {
            DevLog.File("Found message-input TextField.");
        }

        DevLog.File("All UI events bound successfully.");
    }

    void Update()
    {
        // 情况1：未进入输入模式时，监听快捷键
        if (!isBlockingInput)
        {
            if (PeakChatOpsPlugin.config?.Key != null && 
                Input.GetKeyDown(PeakChatOpsPlugin.config.Key.Value))
            {
                EnterInputMode();
            }
        }
        // 情况2：已进入输入模式时，监听回车和ESC
        else
        {
            // 回车发送消息
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendMessage();
            }
            // ESC 退出输入模式
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitInputMode();
            }
        }
    }

    /// <summary>
    /// 进入输入模式：显示UI、聚焦输入框、禁用游戏输入、释放鼠标
    /// </summary>
    void EnterInputMode()
    {
        if (isBlockingInput)
        {
            DevLog.File("Already in input mode, ignoring EnterInputMode call.");
            return; // 已经在输入模式中
        }

        DevLog.File("Entering input mode...");
        
        // 1. 首先设置输入阻塞标志（InputBlockingPatches 会响应此标志）
        isBlockingInput = true;

        
        // 3. 显示UI
        ShowNow();
        
        // 4. 聚焦输入框
        if (messageInputField != null)
        {
            messageInputField.Focus();

            DevLog.File("TextField focused.");
        }
        
        DevLog.File("Input mode activated - game input blocked, cursor released.");
    }

    /// <summary>
    /// 退出输入模式：失焦输入框、恢复游戏输入、锁定鼠标
    /// </summary>
    void ExitInputMode()
    {
        if (!isBlockingInput)
        {
            DevLog.File("Not in input mode, ignoring ExitInputMode call.");
            return; // 没有在输入模式中
        }

        DevLog.File("Exiting input mode...");
        
        // 1. 失焦输入框
        if (messageInputField != null)
        {
            messageInputField.Blur();
            DevLog.File("TextField blurred.");
        }

        isBlockingInput = false;

    }

    // 转发至 ChatSystem
    async void OnSendMessage()
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        var messageInput = root.Q<TextField>("message-input");
        
        if (messageInput != null && !string.IsNullOrWhiteSpace(messageInput.value))
        {
            string msg = messageInput.value;
            messageInput.value = "";
            
            // 立即退出输入模式（在发送之前）
            ExitInputMode();
            
            DevLog.File($"[PeakChatOpsUI] Sending message via ChatSystem: {msg}");
            
            // 通过 ChatSystem 发送消息以接入命令系统
            if (ChatSystem.Instance != null)
            {
                try
                {
                    await ChatSystem.Instance.SendChatMessageAsync(msg, null);
                    DevLog.File($"[PeakChatOpsUI] Message sent successfully via ChatSystem");
                }
                catch (System.Exception ex)
                {
                    DevLog.File($"[PeakChatOpsUI] Error sending message: {ex.Message}");
                }
            }
            else
            {
                DevLog.File("[PeakChatOpsUI] ChatSystem.Instance is null, cannot send message");
            }
        }
    }

    #region API

    public void AddMessage(string sender, string content)
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        var messageList = root.Q<VisualElement>("message-list");

        if (messageList == null) return;

        // 创建消息卡片
        var messageItem = new VisualElement();
        messageItem.AddToClassList("message-item");
        messageItem.AddToClassList("slide-in");

        // 发送者
        var senderLabel = new Label(sender);
        senderLabel.AddToClassList("message-sender");
        messageItem.Add(senderLabel);

        // 内容
        var contentLabel = new Label(content);
        contentLabel.AddToClassList("message-content");
        messageItem.Add(contentLabel);

        // 时间
        var timeLabel = new Label(System.DateTime.Now.ToString("HH:mm"));
        timeLabel.AddToClassList("message-time");
        messageItem.Add(timeLabel);

        // 添加到列表
        messageList.Add(messageItem);

        // 自动滚动到底部
        var scrollView = root.Q<ScrollView>("message-area");
        if (scrollView != null)
        {
            scrollView.schedule.Execute(() => scrollView.scrollOffset = new Vector2(0, float.MaxValue)).StartingIn(50);
        }
    }


    public void RefreshUI()
    {
        DevLog.File("RefreshUI called");
        
        if (uIDocument != null)
        {
            var root = uIDocument.rootVisualElement;
            root.Clear();

        }
    }

    public void HideNow()
    {
        DevLog.File("HideNow called");
        
        if (uIDocument != null)
        {
            var root = uIDocument.rootVisualElement;
            root.style.display = DisplayStyle.None;
        }
    }
    
    public void ShowNow()
    {
        DevLog.File("ShowNow called");
        
        if (uIDocument != null)
        {
            var root = uIDocument.rootVisualElement;
            root.style.display = DisplayStyle.Flex;
        }
    }
    
    void MinimizeUI()
    {
        DevLog.File("MinimizeUI called");
        
        if (uIDocument != null)
        {
            var mainPanel = uIDocument.rootVisualElement.Q("main-panel");
            if (mainPanel != null)
            {
                // 切换最小化状态
                bool isMinimized = mainPanel.style.height.value.value < 100;
                mainPanel.style.height = isMinimized ? 500 : 40;
                
                var contentArea = mainPanel.Q("content-area");
                var actionBar = mainPanel.Q("action-bar");
                
                if (contentArea != null)
                    contentArea.style.display = isMinimized ? DisplayStyle.Flex : DisplayStyle.None;
                if (actionBar != null)
                    actionBar.style.display = isMinimized ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }

    public static void help()
    {
        DevLog.File("PeakChatOpsUI help called");
        // 国际化组件测试
        PeakChatOpsPlugin.Logger.LogInfo($"'{LocalizedText.GetText("PEAKCHATOPSWELCOME")}'");
        
    }


    #endregion
}