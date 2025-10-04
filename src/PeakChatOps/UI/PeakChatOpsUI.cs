using System;

using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace PeakChatOps.UI;

public class PeakChatOpsUI : MonoBehaviour
{
    public static PeakChatOpsUI Instance { get; private set; }
    public static UIDocument uIDocument;
    public bool isBlockingInput;
    private TextField messageInputField;

    public static System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();


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
            closeButton.clicked += HideNow;
            DevLog.File("Bound close-button event.");
        }

        // 绑定最小化按钮
        var minimizeButton = root.Q<Button>("minimize-button");
        if (minimizeButton != null)
        {
            minimizeButton.clicked += MinimizeUI;
            DevLog.File("Bound minimize-button event.");
        }

        // 绑定最大化按钮
        var maximizeButton = root.Q<Button>("maximize-button");
        if (maximizeButton != null)
        {
            maximizeButton.clicked += MaximizeUI;
            DevLog.File("Bound maximize-button event.");
        }

        // 保存输入框引用（按键检测在 Update 中处理）
        messageInputField = root.Q<TextField>("message-input");
        if (messageInputField != null)
        {
            DevLog.File("Found message-input TextField.");
        }

        DevLog.File("All UI events bound successfully.");

        // 处理message-list
        var messageList = root.Q<ListView>("message-list");
        if (messageList != null)
        {
            DevLog.File("Found message-list ListView.");
            
            // ✅ 关键修复：启用动态高度，避免消息重叠
            messageList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            
            
            // ✅ 设置选择模式（单选或不可选）
            messageList.selectionType = SelectionType.None;
            
            // ✅ 确保 ListView 本身的样式正确
            messageList.style.flexGrow = 1;
            
            // 先清空并添加欢迎消息
            messages.Clear();
            messages.Add(LocalizedText.GetText("PEAKCHATOPSWELCOME"));
            
            // 绑定到全局列表
            messageList.itemsSource = messages;
            messageList.makeItem = MakeItem();
            messageList.bindItem = BindItem();
            
            DevLog.File($"[ListView] Configured: DynamicHeight, Non-reorderable, {messages.Count} messages");
        }

        // 获取 chat-panel 并清除所有默认样式类
        var chatPanel = root.Q("chat-panel");
        if (chatPanel != null)
        {
            // 移除 UXML 中可能设置的所有位置相关类
            chatPanel.RemoveFromClassList("pos-topleft");
            chatPanel.RemoveFromClassList("pos-topright");
            chatPanel.RemoveFromClassList("pos-center");
            DevLog.File("[BindUIEvents] Cleared all position classes from chat-panel");
        }

        var pos = PeakChatOpsPlugin.config?.Pos.Value ?? UIAlignment.TopLeft;
        switch (pos)
        {
            case UIAlignment.TopLeft:
                OnTopLeft();
                break;
            case UIAlignment.TopRight:
                OnTopRight();
                break;
            case UIAlignment.Center:
                OnCenter();
                break;
            default:
                OnTopLeft();
                break;
        }
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
    /// 退出输入模式：失焦输入框、取消选择、恢复游戏输入、锁定鼠标
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

        // 2. 取消聊天框中所有选择（特别是滚动区域的选中）
        if (uIDocument != null)
        {
            var root = uIDocument.rootVisualElement;
            var messageListView = root.Q<ListView>("message-list");
            if (messageListView != null)
            {
                // 清除 ListView 的选择
                messageListView.ClearSelection();
                DevLog.File("ListView selection cleared.");

            }
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
                catch (Exception ex)
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

    public void OnTopLeft()
    {
        var root = uIDocument?.rootVisualElement;
        if (root == null)
        {
            DevLog.File("OnTopLeft: uIDocument.rootVisualElement is null");
            return;
        }
        var chatPanel = root.Q("chat-panel");
        if (chatPanel == null)
        {
            DevLog.File("OnTopLeft: chat-panel not found in UI document");
            return;
        }
        chatPanel.RemoveFromClassList("pos-topright");
        chatPanel.RemoveFromClassList("pos-center");
        chatPanel.AddToClassList("pos-topleft");

    }

    public void OnTopRight()
    {
        var root = uIDocument?.rootVisualElement;
        if (root == null)
        {
            DevLog.File("OnTopRight: uIDocument.rootVisualElement is null");
            return;
        }
        var chatPanel = root.Q("chat-panel");
        if (chatPanel == null)
        {
            DevLog.File("OnTopRight: chat-panel not found in UI document");
            return;
        }
        chatPanel.RemoveFromClassList("pos-topleft");
        chatPanel.RemoveFromClassList("pos-center");
        chatPanel.AddToClassList("pos-topright");
    }
    public void OnCenter()
    {
        var root = uIDocument?.rootVisualElement;
        if (root == null)
        {
            DevLog.File("OnCenter: uIDocument.rootVisualElement is null");
            return;
        }
        var chatPanel = root.Q("chat-panel");
        if (chatPanel == null)
        {
            DevLog.File("OnCenter: chat-panel not found in UI document");
            return;
        }
        chatPanel.RemoveFromClassList("pos-topright");
        chatPanel.RemoveFromClassList("pos-topleft");
        chatPanel.AddToClassList("pos-center");
    }


    #region API

    public void AddMessage(string sender, string content)
    {
        if (uIDocument == null)
        {
            DevLog.File("[AddMessage] ERROR: uIDocument is null");
            return;
        }

        var root = uIDocument.rootVisualElement;
        var messageListView = root.Q<ListView>("message-list");

        if (messageListView == null)
        {
            DevLog.File("[AddMessage] ERROR: messageListView not found");
            return;
        }

        // ✅ 简单逻辑：1. 添加到列表
        var message = $"[{sender}] {content}";
        messages.Add(message);
        DevLog.File($"[AddMessage] Added '{message}', total: {messages.Count}");
        
        // ✅ 简单逻辑：2. 刷新 ListView
        messageListView.RefreshItems();
        DevLog.File($"[AddMessage] RefreshItems() called");
        
        // ✅ 简单逻辑：3. 延迟滚动到最新（等待布局更新）
        messageListView.schedule.Execute(() =>
        {
            if (messages.Count > 0)
            {
                var lastIndex = messages.Count - 1;
                messageListView.ScrollToItem(lastIndex);
                DevLog.File($"[AddMessage] Scrolled to item {lastIndex}");
            }
        }).StartingIn(100); // 延迟100ms确保动态高度计算完成
        
        // 不在输入状态时添加高亮效果
        if (!isBlockingInput)
        {
            var chatPanel = root.Q("chat-panel");
            if (chatPanel != null)
            {
                chatPanel.AddToClassList("highlight");
                chatPanel.schedule.Execute(() => chatPanel.RemoveFromClassList("highlight")).StartingIn(500);
            }
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
        var root = uIDocument.rootVisualElement;
        var chatPanel = root.Q<VisualElement>("chat-panel");
        if (chatPanel != null)
        {
            // 先清除所有大小状态类
            bool wasMinimized = chatPanel.ClassListContains("minimized");
            chatPanel.RemoveFromClassList("minimized");
            chatPanel.RemoveFromClassList("maximized");
            
            if (!wasMinimized)
            {
                // 如果之前不是最小化状态，则最小化
                chatPanel.AddToClassList("minimized");
                DevLog.File("[MinimizeUI] Panel minimized");
            }
            else
            {
                // 如果之前是最小化状态，则恢复普通模式
                DevLog.File("[MinimizeUI] Panel restored to normal");
            }
        }
    }

    void MaximizeUI()
    {
        var root = uIDocument.rootVisualElement;
        var chatPanel = root.Q<VisualElement>("chat-panel");
        if (chatPanel != null)
        {
            // 先清除所有大小状态类
            bool wasMaximized = chatPanel.ClassListContains("maximized");
            chatPanel.RemoveFromClassList("minimized");
            chatPanel.RemoveFromClassList("maximized");
            
            if (!wasMaximized)
            {
                // 如果之前不是最大化状态，则最大化
                chatPanel.AddToClassList("maximized");
                DevLog.File("[MaximizeUI] Panel maximized");
            }
            else
            {
                // 如果之前是最大化状态，则恢复普通模式
                DevLog.File("[MaximizeUI] Panel restored to normal");
            }
        }
    }

    public static void Help()
    {
        DevLog.File("PeakChatOpsUI help called");
        // 国际化组件测试
        PeakChatOpsPlugin.Logger.LogInfo($"'{LocalizedText.GetText("PEAKCHATOPSWELCOME")}'");
        
    }

    #endregion

    #region 辅助方法

    // ListView 生成单个消息项
    Func<VisualElement> MakeItem()
    {
        return () => new MessageLabel();
    }
    // ListView 绑定数据到消息项
    Action<VisualElement, int> BindItem()
    {
        return (element, i) =>
        {

            if (element is Label label)
            {
                // 获取消息文本
                if (i >= 0 && i < messages.Count)
                {
                    label.text = messages[i];
                }
                else
                {
                    label.text = string.Empty;
                }
            }
        };
    }


    #endregion


}

/// <summary>
/// 自定义消息 Label，封装所有消息项的默认配置
/// </summary>
public class MessageLabel : Label
{
    public MessageLabel()
    {
        // 添加样式类
        AddToClassList("message-item");
        
        // ✅ 启用文本选择
        selection.isSelectable = true;
        
        
        // ✅ Flex 布局设置
        style.flexShrink = 0;                        // 不压缩
        style.flexGrow = 1;                          // 允许横向扩展填满容器
        
        // ✅ 设置内边距，避免内容贴边
        style.paddingLeft = 8;
        style.paddingRight = 8;
        style.paddingTop = 4;
        style.paddingBottom = 4;
        style.marginTop = 1;
        style.marginBottom = 1;                      // 消息之间的间距
        
        // ✅ 确保文本可见
        style.unityTextAlign = TextAnchor.UpperLeft;
        style.color = new Color(0.9f, 0.9f, 0.9f, 1f); // 浅灰色文本
    }
}