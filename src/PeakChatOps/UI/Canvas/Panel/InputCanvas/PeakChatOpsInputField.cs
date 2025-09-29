using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsInputField : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsInputField));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsInputField>();
        return go;
    }

    public RectTransform InputFieldRectTransform;
    public TMPro.TMP_InputField TMPInputField;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsInputField)}");

        InputFieldRectTransform = GetComponent<RectTransform>();
        // RectTransform 属性
        InputFieldRectTransform.anchoredPosition3D = new Vector3(96.721f, 24.5f, 0f);
        InputFieldRectTransform.sizeDelta = new Vector2(505f, 30f);
        InputFieldRectTransform.anchorMin = new Vector2(0f, 0f);
        InputFieldRectTransform.anchorMax = new Vector2(0f, 0f);
        InputFieldRectTransform.pivot = new Vector2(0f, 0f);
        InputFieldRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        InputFieldRectTransform.localScale = Vector3.one;

        // CanvasRenderer
        var renderer = GetComponent<CanvasRenderer>();
        if (renderer == null)
            renderer = gameObject.AddComponent<CanvasRenderer>();

        // Image
        var img = GetComponent<UnityEngine.UI.Image>();
        if (img == null)
            img = gameObject.AddComponent<UnityEngine.UI.Image>();
        // TODO: img.sprite = Resources.Load<Sprite>("InputFielBackground"); // 需确保资源已导入
        img.color = new Color(0.0f, 0.7f, 1.0f, 1f); // 天青色
        img.raycastTarget = true;
        img.maskable = true;
        img.type = UnityEngine.UI.Image.Type.Sliced;
        img.fillCenter = true;
        img.pixelsPerUnitMultiplier = 1f;

        // TMP_InputField
        TMPInputField = gameObject.AddComponent<TMPro.TMP_InputField>();
        TMPInputField.interactable = true;
        TMPInputField.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
        TMPInputField.shouldActivateOnSelect = true;
        TMPInputField.richText = true;
        TMPInputField.isRichTextEditingAllowed = true;
        TMPInputField.restoreOriginalTextOnEscape = true;

        // 文本视口 TextArea 子节点
        var textAreaGO = new GameObject("TextArea", typeof(RectTransform));
        textAreaGO.transform.SetParent(this.transform, false);
        textAreaGO.AddComponent<PeakChatOpsTextArea>();
        TMPInputField.textViewport = textAreaGO.GetComponent<RectTransform>();
    }
}