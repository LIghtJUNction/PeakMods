using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownTemplateViewport : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownTemplateViewport));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownTemplateViewport>();
        return go;
    }

    public RectTransform ViewportRectTransform;
    public Image ViewportImage;
    public Mask ViewportMask;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownTemplateViewport)}");

        ViewportRectTransform = GetComponent<RectTransform>();
        ViewportRectTransform.anchorMin = new Vector2(0f, 0f);
        ViewportRectTransform.anchorMax = new Vector2(1f, 1f);
        ViewportRectTransform.pivot = new Vector2(0.5f, 0.5f);
        ViewportRectTransform.sizeDelta = new Vector2(0, 0);
        ViewportImage = gameObject.AddComponent<Image>();
        ViewportImage.color = new Color32(255, 255, 255, 255);
        ViewportMask = gameObject.AddComponent<Mask>();
        ViewportMask.showMaskGraphic = false;

        // 自动创建并挂载Content子节点
        var contentGO = new GameObject("Content", typeof(RectTransform));
        contentGO.transform.SetParent(this.transform, false);
        contentGO.AddComponent<PeakChatOpsDropdownTemplateContent>();
    }
}