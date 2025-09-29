using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsViewport : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsViewport));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsViewport>();
        return go;
    }

    public RectTransform ViewportRectTransform;
    public RectTransform ContentRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsViewport)}");

        ViewportRectTransform = GetComponent<RectTransform>();
        // 可扩展：设置锚点、轴心、大小等属性

        // Content 子节点
        var contentGO = new GameObject("Content", typeof(RectTransform));
        contentGO.transform.SetParent(this.transform, false);
        var contentComp = contentGO.AddComponent<PeakChatOpsContent>();
        ContentRectTransform = contentGO.GetComponent<RectTransform>();
    }
}