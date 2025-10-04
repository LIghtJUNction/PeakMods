using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class PeakChatOpsUI : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private List<string> messages = new List<string>();
    private ListView messageListView;
    private TextField messageInputField;

    [MenuItem("Window/UI Toolkit/PeakChatOpsUI Debug")]
    public static void ShowExample()
    {
        PeakChatOpsUI wnd = GetWindow<PeakChatOpsUI>();
        wnd.titleContent = new GUIContent("PeakChatOps Debug");
        wnd.minSize = new Vector2(400, 500);
    }

    public void CreateGUI()
    {
        // 清空根元素
        VisualElement root = rootVisualElement;
        root.Clear();

        // 实例化 UXML
        if (m_VisualTreeAsset != null)
        {
            m_VisualTreeAsset.CloneTree(root);
            Debug.Log("[Editor] UXML instantiated");
        }
        else
        {
            Debug.LogError("[Editor] m_VisualTreeAsset is null! Please assign the UXML file in the Inspector.");
            return;
        }

        // 绑定 UI 事件
        BindUIEvents(root);
    }

    private void BindUIEvents(VisualElement root)
    {
        // 应用聊天面板样式
        var chatPanel = root.Q("chat-panel");
        if (chatPanel != null)
        {
            ApplyChatPanelStyle(chatPanel);
            Debug.Log("[Editor] Applied chat-panel styles");
        }

        // 绑定发送按钮
        var sendButton = root.Q<Button>("send-button");
        if (sendButton != null)
        {
            sendButton.clicked += OnSendMessage;
            ApplyButtonStyle(sendButton, new Color(0.24f, 0.47f, 0.71f), new Color(0.31f, 0.55f, 0.78f));
            Debug.Log("[Editor] Bound send-button");
        }
        else
        {
            Debug.LogWarning("[Editor] send-button not found");
        }

        // 绑定关闭按钮
        var closeButton = root.Q<Button>("close-button");
        if (closeButton != null)
        {
            closeButton.clicked += () => {
                Debug.Log("[Editor] Close button clicked");
                Close();
            };
            ApplyButtonStyle(closeButton, new Color(0.8f, 0.2f, 0.2f), new Color(0.9f, 0.3f, 0.3f));
            Debug.Log("[Editor] Bound close-button");
        }
        else
        {
            Debug.LogWarning("[Editor] close-button not found");
        }

        // 绑定最小化按钮
        var minimizeButton = root.Q<Button>("minimize-button");
        if (minimizeButton != null)
        {
            minimizeButton.clicked += MinimizeUI;
            ApplyButtonStyle(minimizeButton, new Color(0.3f, 0.3f, 0.3f), new Color(0.4f, 0.4f, 0.4f));
            Debug.Log("[Editor] Bound minimize-button");
        }
        else
        {
            Debug.LogWarning("[Editor] minimize-button not found");
        }

        // 绑定最大化按钮
        var maximizeButton = root.Q<Button>("maximize-button");
        if (maximizeButton != null)
        {
            maximizeButton.clicked += MaximizeUI;
            ApplyButtonStyle(maximizeButton, new Color(0.3f, 0.3f, 0.3f), new Color(0.4f, 0.4f, 0.4f));
            Debug.Log("[Editor] Bound maximize-button");
        }
        else
        {
            Debug.LogWarning("[Editor] maximize-button not found");
        }

        // 获取输入框
        messageInputField = root.Q<TextField>("message-input");
        if (messageInputField != null)
        {
            // 监听回车键
            messageInputField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    OnSendMessage();
                    evt.StopPropagation();
                }
            });
            Debug.Log("[Editor] Found message-input");
        }
        else
        {
            Debug.LogWarning("[Editor] message-input not found");
        }

        // 配置 ListView
        messageListView = root.Q<ListView>("message-list");
        if (messageListView != null)
        {
            messageListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            messageListView.selectionType = SelectionType.None;
            
            // 添加欢迎消息
            messages.Clear();
            messages.Add("Welcome to PeakChatOps Debug Window!");
            messages.Add("Click buttons to test functionality.");
            
            messageListView.itemsSource = messages;
            messageListView.makeItem = () => new Label();
            messageListView.bindItem = (element, i) =>
            {
                if (element is Label label && i >= 0 && i < messages.Count)
                {
                    label.text = messages[i];
                    label.style.paddingLeft = 8;
                    label.style.paddingRight = 8;
                    label.style.paddingTop = 4;
                    label.style.paddingBottom = 4;
                    label.style.whiteSpace = WhiteSpace.Normal;
                    label.style.color = new Color(0.9f, 0.9f, 0.9f);
                }
            };
            
            messageListView.RefreshItems();
            Debug.Log($"[Editor] ListView configured with {messages.Count} messages");
        }
        else
        {
            Debug.LogWarning("[Editor] message-list not found");
        }

        // 应用位置样式类（使用已有的 chatPanel 变量）
        if (chatPanel != null)
        {
            // 清除默认位置类
            chatPanel.RemoveFromClassList("pos-topleft");
            chatPanel.RemoveFromClassList("pos-topright");
            chatPanel.RemoveFromClassList("pos-center");
            
            // 添加默认位置
            chatPanel.AddToClassList("pos-topleft");
            Debug.Log("[Editor] chat-panel position class applied");
        }
    }

    private void OnSendMessage()
    {
        if (messageInputField == null || messageListView == null) return;

        string msg = messageInputField.value;
        if (!string.IsNullOrWhiteSpace(msg))
        {
            messages.Add($"[You] {msg}");
            messageInputField.value = "";
            messageListView.RefreshItems();
            
            // 滚动到最后
            messageListView.ScrollToItem(messages.Count - 1);
            
            Debug.Log($"[Editor] Message sent: {msg}");
        }
    }

    private void MinimizeUI()
    {
        var chatPanel = rootVisualElement.Q("chat-panel");
        if (chatPanel != null)
        {
            bool wasMinimized = chatPanel.ClassListContains("minimized");
            chatPanel.RemoveFromClassList("minimized");
            chatPanel.RemoveFromClassList("maximized");
            
            if (!wasMinimized)
            {
                chatPanel.AddToClassList("minimized");
                Debug.Log("[Editor] Panel minimized");
            }
            else
            {
                Debug.Log("[Editor] Panel restored to normal");
            }
        }
    }

    private void MaximizeUI()
    {
        var chatPanel = rootVisualElement.Q("chat-panel");
        if (chatPanel != null)
        {
            bool wasMaximized = chatPanel.ClassListContains("maximized");
            chatPanel.RemoveFromClassList("minimized");
            chatPanel.RemoveFromClassList("maximized");
            
            if (!wasMaximized)
            {
                chatPanel.AddToClassList("maximized");
                Debug.Log("[Editor] Panel maximized");
            }
            else
            {
                Debug.Log("[Editor] Panel restored to normal");
            }
        }
    }

    private void ApplyChatPanelStyle(VisualElement panel)
    {
        // 应用背景色和边框
        panel.style.backgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        panel.style.borderBottomColor = new Color(0.39f, 0.39f, 0.39f, 0.5f);
        panel.style.borderLeftColor = new Color(0.39f, 0.39f, 0.39f, 0.5f);
        panel.style.borderRightColor = new Color(0.39f, 0.39f, 0.39f, 0.5f);
        panel.style.borderTopColor = new Color(0.39f, 0.39f, 0.39f, 0.5f);
        panel.style.borderBottomWidth = new StyleFloat(1f);
        panel.style.borderLeftWidth = new StyleFloat(1f);
        panel.style.borderRightWidth = new StyleFloat(1f);
        panel.style.borderTopWidth = new StyleFloat(1f);
        panel.style.borderBottomLeftRadius = new StyleLength(new Length(8f, LengthUnit.Pixel));
        panel.style.borderBottomRightRadius = new StyleLength(new Length(8f, LengthUnit.Pixel));
        panel.style.borderTopLeftRadius = new StyleLength(new Length(8f, LengthUnit.Pixel));
        panel.style.borderTopRightRadius = new StyleLength(new Length(8f, LengthUnit.Pixel));
        panel.style.paddingBottom = new StyleLength(new Length(5f, LengthUnit.Pixel));
        panel.style.paddingLeft = new StyleLength(new Length(5f, LengthUnit.Pixel));
        panel.style.paddingRight = new StyleLength(new Length(5f, LengthUnit.Pixel));
        panel.style.paddingTop = new StyleLength(new Length(5f, LengthUnit.Pixel));
    }

    private void ApplyButtonStyle(Button button, Color bgColor, Color hoverColor)
    {
        button.style.backgroundColor = bgColor;
        button.style.color = Color.white;
        button.style.borderBottomLeftRadius = new StyleLength(new Length(4f, LengthUnit.Pixel));
        button.style.borderBottomRightRadius = new StyleLength(new Length(4f, LengthUnit.Pixel));
        button.style.borderTopLeftRadius = new StyleLength(new Length(4f, LengthUnit.Pixel));
        button.style.borderTopRightRadius = new StyleLength(new Length(4f, LengthUnit.Pixel));
        button.style.borderBottomWidth = new StyleFloat(1f);
        button.style.borderLeftWidth = new StyleFloat(1f);
        button.style.borderRightWidth = new StyleFloat(1f);
        button.style.borderTopWidth = new StyleFloat(1f);
        
        // 鼠标悬停效果
        button.RegisterCallback<MouseEnterEvent>(_ => {
            button.style.backgroundColor = hoverColor;
        });
        button.RegisterCallback<MouseLeaveEvent>(_ => {
            button.style.backgroundColor = bgColor;
        });
    }
}
