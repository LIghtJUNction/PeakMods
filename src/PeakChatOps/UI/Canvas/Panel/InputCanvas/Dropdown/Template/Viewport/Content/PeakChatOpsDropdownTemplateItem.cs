using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownTemplateItem : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownTemplateItem));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownTemplateItem>();
        return go;
    }

    public RectTransform ItemRectTransform;
    public Text ItemText;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownTemplateItem)}");

        ItemRectTransform = GetComponent<RectTransform>();
        ItemRectTransform.anchorMin = new Vector2(0f, 0f);
        ItemRectTransform.anchorMax = new Vector2(1f, 1f);
        ItemRectTransform.pivot = new Vector2(0.5f, 0.5f);
        ItemRectTransform.sizeDelta = new Vector2(0, 30);
        ItemText = gameObject.AddComponent<Text>();
        ItemText.text = "下拉项";
        ItemText.fontSize = 16;
        ItemText.color = new Color32(80, 80, 80, 255);
        ItemText.alignment = TextAnchor.MiddleLeft;
    }
}
