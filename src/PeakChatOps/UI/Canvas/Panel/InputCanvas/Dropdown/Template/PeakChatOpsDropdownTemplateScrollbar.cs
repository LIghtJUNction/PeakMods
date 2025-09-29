using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownTemplateScrollbar : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownTemplateScrollbar));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownTemplateScrollbar>();
        return go;
    }

    public RectTransform ScrollbarRectTransform;
    public Scrollbar ScrollbarComponent;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownTemplateScrollbar)}");

        ScrollbarRectTransform = GetComponent<RectTransform>();
        ScrollbarRectTransform.anchorMin = new Vector2(1f, 0f);
        ScrollbarRectTransform.anchorMax = new Vector2(1f, 1f);
        ScrollbarRectTransform.pivot = new Vector2(1f, 0.5f);
        ScrollbarRectTransform.sizeDelta = new Vector2(20, 0);
        ScrollbarComponent = gameObject.AddComponent<Scrollbar>();
        ScrollbarComponent.direction = Scrollbar.Direction.BottomToTop;
        ScrollbarComponent.value = 1f;
        ScrollbarComponent.size = 0.2f;
        ScrollbarComponent.interactable = true;

        // 自动创建并挂载Sliding Area子节点
        var slidingAreaGO = new GameObject("Sliding Area", typeof(RectTransform));
        slidingAreaGO.transform.SetParent(this.transform, false);
    }
}