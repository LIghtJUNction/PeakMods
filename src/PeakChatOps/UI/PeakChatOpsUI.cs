using System;
using PeakChatOps.core;
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
            Destroy(this);
            return;
        }

        Instance = this;
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

        // ⚡ 性能优化：配置 message-list
        var messageList = root.Q<ListView>("message-list");
        if (messageList != null)
        {
            messageList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            messageList.selectionType = SelectionType.None;
            messageList.style.flexGrow = 1;
            
            // 初始化消息列表
            messages.Clear();
            messages.Add(PLocalizedText.GetText("PEAKCHATOPSWELCOME"));
            
            messageList.itemsSource = messages;
            messageList.makeItem = MakeItem();
            messageList.bindItem = BindItem();
        }

        // 清除默认位置样式类
        var chatPanel = root.Q("chat-panel");
        if (chatPanel != null)
        {
            chatPanel.RemoveFromClassList("pos-topleft");
            chatPanel.RemoveFromClassList("pos-topright");
            chatPanel.RemoveFromClassList("pos-center");
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
        if (isBlockingInput) return;

        isBlockingInput = true;
        ShowNow();
        
        if (messageInputField != null)
        {
            messageInputField.Focus();
        }
    }

    /// <summary>
    /// 退出输入模式：失焦输入框、取消选择、恢复游戏输入、锁定鼠标
    /// </summary>
    void ExitInputMode()
    {
        if (!isBlockingInput) return;
        
        if (messageInputField != null)
        {
            messageInputField.Blur();
        }

        // 清除 ListView 选择
        if (uIDocument != null)
        {
            var root = uIDocument.rootVisualElement;
            var messageListView = root.Q<ListView>("message-list");
            messageListView?.ClearSelection();
        }

        isBlockingInput = false;
    }

    // ⚡ 性能优化：异步发送消息
    async void OnSendMessage()
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        var messageInput = root.Q<TextField>("message-input");
        
        if (messageInput != null && !string.IsNullOrWhiteSpace(messageInput.value))
        {
            string msg = messageInput.value;
            messageInput.value = "";
            ExitInputMode();
            
            // 通过 ChatSystem 发送消息
            if (ChatSystem.Instance != null)
            {
                try
                {
                    await ChatSystem.Instance.SendChatMessageAsync(msg, null);
                }
                catch (Exception ex)
                {
                    DevLog.File($"[PeakChatOpsUI] Error sending message: {ex.Message}");
                }
            }
        }
    }

    public void OnTopLeft()
    {
        var chatPanel = uIDocument?.rootVisualElement?.Q("chat-panel");
        if (chatPanel == null) return;
        
        chatPanel.RemoveFromClassList("pos-topright");
        chatPanel.RemoveFromClassList("pos-center");
        chatPanel.AddToClassList("pos-topleft");
    }

    public void OnTopRight()
    {
        var chatPanel = uIDocument?.rootVisualElement?.Q("chat-panel");
        if (chatPanel == null) return;
        
        chatPanel.RemoveFromClassList("pos-topleft");
        chatPanel.RemoveFromClassList("pos-center");
        chatPanel.AddToClassList("pos-topright");
    }
    
    public void OnCenter()
    {
        var chatPanel = uIDocument?.rootVisualElement?.Q("chat-panel");
        if (chatPanel == null) return;
        
        chatPanel.RemoveFromClassList("pos-topright");
        chatPanel.RemoveFromClassList("pos-topleft");
        chatPanel.AddToClassList("pos-center");
    }


    #region API

    public void AddMessage(string sender, string content)
    {
        if (uIDocument == null) return;

        var root = uIDocument.rootVisualElement;
        var messageListView = root.Q<ListView>("message-list");

        if (messageListView == null) return;

        // ⚡ 性能优化：减少字符串拼接和日志
        var message = string.IsNullOrEmpty(sender) 
            ? content 
            : $"[{sender}] {content}";
        
        messages.Add(message);
        
        // ⚡ 性能优化：使用 Rebuild() 而不是 RefreshItems()
        // Rebuild() 只更新数据源变化，RefreshItems() 会刷新所有可见项
        messageListView.Rebuild();
        
        // ⚡ 性能优化：减少延迟时间到 50ms，提升响应速度
        if (messages.Count > 0)
        {
            messageListView.schedule.Execute(() =>
            {
                messageListView.ScrollToItem(messages.Count - 1);
            }).StartingIn(50);
        }
        
        // ⚡ 性能优化：移除高亮效果（减少 DOM 操作）
        // 高亮动画会触发额外的样式计算和重绘
    }


    public void RefreshUI()
    {
        uIDocument?.rootVisualElement?.Clear();
    }

    public void HideNow()
    {
        if (uIDocument != null)
        {
            uIDocument.rootVisualElement.style.display = DisplayStyle.None;
            ExitInputMode();
        }
    }
    
    public void ShowNow()
    {
        if (uIDocument != null)
        {
            uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            EnterInputMode();
        }
    }
    
    void MinimizeUI()
    {
        var chatPanel = uIDocument?.rootVisualElement?.Q<VisualElement>("chat-panel");
        if (chatPanel == null) return;

        bool wasMinimized = chatPanel.ClassListContains("minimized");
        chatPanel.RemoveFromClassList("minimized");
        chatPanel.RemoveFromClassList("maximized");

        if (!wasMinimized)
        {
            chatPanel.AddToClassList("minimized");
        }
        
        ExitInputMode();
    }

    void MaximizeUI()
    {
        var chatPanel = uIDocument?.rootVisualElement?.Q<VisualElement>("chat-panel");
        if (chatPanel == null) return;

        bool wasMaximized = chatPanel.ClassListContains("maximized");
        chatPanel.RemoveFromClassList("minimized");
        chatPanel.RemoveFromClassList("maximized");

        if (!wasMaximized)
        {
            chatPanel.AddToClassList("maximized");
        }
        
        EnterInputMode();
    }

    public static void Help()
    {
        // 国际化组件测试
        PeakChatOpsPlugin.Logger.LogInfo($"Localization test: '{PLocalizedText.GetText("PEAKCHATOPSWELCOME")}'");
        
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
        
        // ✅ 文本换行设置（最重要！）
        style.whiteSpace = WhiteSpace.Normal;        // 允许文本自动换行
        
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