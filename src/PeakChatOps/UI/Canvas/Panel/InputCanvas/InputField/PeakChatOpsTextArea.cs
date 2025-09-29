using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsTextArea : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsTextArea));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsTextArea>();
        return go;
    }

    public RectTransform TextAreaRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsTextArea)}");

        TextAreaRectTransform = GetComponent<RectTransform>();
        // 设置锚点、轴心、位置、大小、缩放
        TextAreaRectTransform.anchoredPosition3D = new Vector3(10f, 7f, 0f);
        TextAreaRectTransform.sizeDelta = new Vector2(10f, 6f);
        TextAreaRectTransform.anchorMin = new Vector2(0f, 0f);
        TextAreaRectTransform.anchorMax = new Vector2(1f, 1f);
        TextAreaRectTransform.pivot = new Vector2(0.5f, 0.5f);
        TextAreaRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        TextAreaRectTransform.localScale = Vector3.one;

        // RectMask2D
        if (GetComponent<UnityEngine.UI.RectMask2D>() == null)
            gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        // 填充参数暂不支持代码设置（如需自定义可用 Mask Shader 实现）

        // 子节点 Placeholder
        var placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
        placeholderGO.transform.SetParent(this.transform, false);
        placeholderGO.AddComponent<PeakChatOpsPlaceholder>();
    }
}