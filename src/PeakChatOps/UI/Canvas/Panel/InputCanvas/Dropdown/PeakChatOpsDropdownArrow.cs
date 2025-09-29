using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;

public class PeakChatOpsDropdownArrow : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownArrow));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownArrow>();
        return go;
    }

    public RectTransform ArrowRectTransform;
    public Image ArrowImage;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownArrow)}");

        ArrowRectTransform = GetComponent<RectTransform>();
        ArrowRectTransform.anchorMin = new Vector2(1f, 0.5f);
        ArrowRectTransform.anchorMax = new Vector2(1f, 0.5f);
        ArrowRectTransform.pivot = new Vector2(0.5f, 0.5f);
        ArrowRectTransform.sizeDelta = new Vector2(16, 16);
        ArrowRectTransform.anchoredPosition = new Vector2(-8, 0);
        ArrowImage = gameObject.AddComponent<Image>();
        // TODO: ArrowImage.sprite = ... // 挂载下拉箭头UISprite
        ArrowImage.color = new Color32(180, 180, 180, 255);
        ArrowImage.raycastTarget = false;
    }
}