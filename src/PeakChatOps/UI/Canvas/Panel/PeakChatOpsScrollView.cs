
using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsScrollView : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsScrollView));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsScrollView>();
        return go;
    }

    public RectTransform ScrollViewRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsScrollView)}");

        ScrollViewRectTransform = GetComponent<RectTransform>();
        // RectTransform 属性
        ScrollViewRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        ScrollViewRectTransform.sizeDelta = new Vector2(750, 550);
        ScrollViewRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        ScrollViewRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        ScrollViewRectTransform.pivot = new Vector2(0.5f, 0.5f);
        ScrollViewRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        ScrollViewRectTransform.localScale = Vector3.one;

        // CanvasRenderer
        var renderer = GetComponent<CanvasRenderer>();
        if (renderer == null)
            renderer = gameObject.AddComponent<CanvasRenderer>();
        renderer.cullTransparentMesh = true;

        // 子节点 PeakChatOpsLoopVerticalScrollRect（自定义组件）
        var loopScrollGO = new GameObject("PeakChatOpsLoopVerticalScrollRect", typeof(RectTransform));
        loopScrollGO.transform.SetParent(this.transform, false);
        loopScrollGO.AddComponent<PeakChatOpsLoopVerticalScrollRect>();
    }
}