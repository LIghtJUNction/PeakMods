using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsSendButton : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsSendButton));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsSendButton>();
        return go;
    }

    public RectTransform SendButtonRectTransform;
    public UnityEngine.UI.Button SendButton;
    public UnityEngine.UI.Text ButtonText;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsSendButton)}");

        SendButtonRectTransform = GetComponent<RectTransform>();
        // RectTransform 属性
        SendButtonRectTransform.anchoredPosition3D = new Vector3(-96.7f, 25f, 0f);
        SendButtonRectTransform.sizeDelta = new Vector2(70f, 46f);
        SendButtonRectTransform.anchorMin = new Vector2(1f, 0f);
        SendButtonRectTransform.anchorMax = new Vector2(1f, 0f);
        SendButtonRectTransform.pivot = new Vector2(0f, 0f);
        SendButtonRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        SendButtonRectTransform.localScale = Vector3.one;

        // CanvasRenderer
        var renderer = GetComponent<CanvasRenderer>();
        if (renderer == null)
            renderer = gameObject.AddComponent<CanvasRenderer>();
        renderer.cullTransparentMesh = true;

        // Image
        var img = GetComponent<UnityEngine.UI.Image>();
        if (img == null)
            img = gameObject.AddComponent<UnityEngine.UI.Image>();
        // TODO: img.sprite = Resources.Load<Sprite>("KeyboardSproteSheet_68"); // 需确保资源已导入
        img.color = new Color(1f, 0.85f, 0.4f, 1f); // 柯基黄
        img.type = UnityEngine.UI.Image.Type.Simple;
        img.preserveAspect = false;

        // 按钮组件
        SendButton = gameObject.AddComponent<UnityEngine.UI.Button>();
        // 过渡色彩（可自定义或保持默认）
        var colors = SendButton.colors;
        colors.normalColor = img.color;
        colors.highlightedColor = new Color(1f, 0.95f, 0.6f, 1f);
        colors.pressedColor = new Color(0.9f, 0.8f, 0.3f, 1f);
        colors.selectedColor = img.color;
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        SendButton.colors = colors;

        // 按钮文本
        var textGO = new GameObject("ButtonText", typeof(RectTransform));
        textGO.transform.SetParent(this.transform, false);
        ButtonText = textGO.AddComponent<UnityEngine.UI.Text>();
        ButtonText.text = "发送";
        ButtonText.alignment = TextAnchor.MiddleCenter;
        ButtonText.color = Color.black;
        ButtonText.fontSize = 24;
        ButtonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 按钮点击事件示例
        SendButton.onClick.AddListener(OnSendClicked);
    }

    private void OnSendClicked()
    {
        Debug.Log("发送按钮被点击");
        // 可扩展：发送消息逻辑
    }
}