using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownLabel : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownLabel));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownLabel>();
        return go;
    }

    public RectTransform LabelRectTransform;
    public Text LabelText;

    private void Awake()
    
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownLabel)}");
        LabelRectTransform = GetComponent<RectTransform>();
        LabelRectTransform.anchorMin = new Vector2(0f, 0f);
        LabelRectTransform.anchorMax = new Vector2(1f, 1f);
        LabelRectTransform.pivot = new Vector2(0.5f, 0.5f);
        LabelRectTransform.sizeDelta = new Vector2(0, 0);
        LabelText = gameObject.AddComponent<Text>();
        LabelText.text = "Option 1";
        LabelText.fontSize = 18;
        LabelText.color = new Color32(80, 80, 80, 255);
        LabelText.alignment = TextAnchor.MiddleLeft;
    }
}