
using UnityEngine;

using PeakChatOps.UI.LoopScrollRectLib;
using PeakChatOps.Core; // 如有需要请根据实际命名空间调整
namespace PeakChatOps.UI;
public class PeakChatOpsLoopVerticalScrollRect : LoopVerticalScrollRect
{
    public RectTransform LoopRectTransform;
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsLoopVerticalScrollRect));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsLoopVerticalScrollRect>();
        return go;
    }

    private new void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsLoopVerticalScrollRect)}");

        LoopRectTransform = GetComponent<RectTransform>();
        // 设置锚点、轴心、位置、大小、缩放
        LoopRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        LoopRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        LoopRectTransform.pivot = new Vector2(0.5f, 0.5f);
        LoopRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        LoopRectTransform.sizeDelta = new Vector2(750, 500);
        LoopRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        LoopRectTransform.localScale = Vector3.one;

        // ScrollRect 组件
        var scrollRect = gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.4f;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        scrollRect.scrollSensitivity = 1f;

        // Viewport 子节点
        var viewportGO = new GameObject("Viewport", typeof(RectTransform));
        viewportGO.transform.SetParent(this.transform, false);
        var viewportComp = viewportGO.AddComponent<PeakChatOpsViewport>();
        scrollRect.viewport = viewportGO.GetComponent<RectTransform>();


        // TODO: LoopScrollRect 相关参数可后续扩展
    }
}