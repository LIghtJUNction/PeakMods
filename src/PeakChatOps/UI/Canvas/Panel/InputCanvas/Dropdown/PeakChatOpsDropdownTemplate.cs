using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownTemplate : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownTemplate));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownTemplate>();
        return go;
    }

    public RectTransform TemplateRectTransform;
    public RectTransform ViewportRectTransform;
    public Scrollbar ScrollbarComponent;

    private void Awake()
    
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownTemplate)}");
        TemplateRectTransform = GetComponent<RectTransform>();
        TemplateRectTransform.anchorMin = new Vector2(0f, 0f);
        TemplateRectTransform.anchorMax = new Vector2(1f, 1f);
        TemplateRectTransform.pivot = new Vector2(0.5f, 1f);
        TemplateRectTransform.sizeDelta = new Vector2(0, 150);
        gameObject.SetActive(false); // Unity标准做法，初始隐藏

    // Viewport 子组件
    var viewportGO = new GameObject("Viewport", typeof(RectTransform));
    viewportGO.transform.SetParent(this.transform, false);
    viewportGO.AddComponent<PeakChatOpsDropdownTemplateViewport>();
    ViewportRectTransform = viewportGO.GetComponent<RectTransform>();

    // Scrollbar 子组件
    var scrollbarGO = new GameObject("Scrollbar", typeof(RectTransform));
    scrollbarGO.transform.SetParent(this.transform, false);
    scrollbarGO.AddComponent<PeakChatOpsDropdownTemplateScrollbar>();
    ScrollbarComponent = scrollbarGO.GetComponent<UnityEngine.UI.Scrollbar>();
    }
}
