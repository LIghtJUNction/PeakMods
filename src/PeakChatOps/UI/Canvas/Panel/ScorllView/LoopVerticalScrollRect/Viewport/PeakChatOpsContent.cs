using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsContent : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsContent));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsContent>();
        return go;
    }

    public RectTransform ContentRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsContent)}");

        ContentRectTransform = GetComponent<RectTransform>();
        // 设置锚点、轴心、位置、大小、缩放
        ContentRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        ContentRectTransform.sizeDelta = new Vector2(650, 0); // 高度由布局自动适应
        ContentRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        ContentRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        ContentRectTransform.pivot = new Vector2(0.5f, 0.5f);
        ContentRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        ContentRectTransform.localScale = Vector3.one;

        // Vertical Layout Group
        var layout = gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        // 其余填充、间距默认

        // Content Size Fitter
        var fitter = gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
    }
}