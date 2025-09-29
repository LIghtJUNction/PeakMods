using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownTemplateSlidingArea : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownTemplateSlidingArea));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownTemplateSlidingArea>();
        return go;
    }

    public RectTransform SlidingAreaRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownTemplateSlidingArea)}");

        SlidingAreaRectTransform = GetComponent<RectTransform>();
        SlidingAreaRectTransform.anchorMin = new Vector2(0f, 0f);
        SlidingAreaRectTransform.anchorMax = new Vector2(1f, 1f);
        SlidingAreaRectTransform.pivot = new Vector2(0.5f, 0.5f);
        SlidingAreaRectTransform.sizeDelta = new Vector2(0, 0);

        // 自动创建并挂载Handle子节点
        var handleGO = new GameObject("Handle", typeof(RectTransform));
        handleGO.transform.SetParent(this.transform, false);
    }
}
