using UnityEngine;
using PeakChatOps.UI;
using PeakChatOps.Core;
namespace PeakChatOps.UI;
public class PeakChatOpsPanel : MonoBehaviour{
    public RectTransform PanelRectTransform;
    public static PeakChatOpsPanel Create(Transform parent)
    {
        // 创建时直接带 RectTransform
        var go = new GameObject(nameof(PeakChatOpsPanel), typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var panel = go.AddComponent<PeakChatOpsPanel>();
        return panel;
    }


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsPanel)}");
        // RectTransform
        PanelRectTransform = GetComponent<RectTransform>();
        if (PanelRectTransform == null)
            PanelRectTransform = gameObject.AddComponent<RectTransform>();
        PanelRectTransform.anchoredPosition3D = new Vector3(100, 500, 0);
        PanelRectTransform.sizeDelta = new Vector2(500, 300);
        PanelRectTransform.anchorMin = new Vector2(0, 0);
        PanelRectTransform.anchorMax = new Vector2(0, 0);
        PanelRectTransform.pivot = new Vector2(0, 0);
        PanelRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        PanelRectTransform.localScale = Vector3.one;

        // CanvasRenderer
        if (GetComponent<CanvasRenderer>() == null)
            gameObject.AddComponent<CanvasRenderer>();

        // Image
        var img = GetComponent<UnityEngine.UI.Image>();
        if (img == null)
            img = gameObject.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(1f, 1f, 0.8f, 1f); // 淡黄色
        img.raycastTarget = true;
        img.type = UnityEngine.UI.Image.Type.Sliced;
        img.fillCenter = true;
        img.pixelsPerUnitMultiplier = 1f;
        img.maskable = true;
        // TODO
        // 源图像、材质等可后续赋值

        // Shadow
        var shadow = GetComponent<UnityEngine.UI.Shadow>();
        if (shadow == null)
            shadow = gameObject.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(1, -15);

        // 子节点 OutLine
        var outlineGO = new GameObject("OutLine", typeof(RectTransform));
        outlineGO.transform.SetParent(this.transform, false);
        outlineGO.AddComponent<PeakChatOpsOutLine>();

        // 子节点 ScrollView
        var scrollViewGO = new GameObject("Scroll View", typeof(RectTransform));
        scrollViewGO.transform.SetParent(this.transform, false);
        scrollViewGO.AddComponent<PeakChatOpsScrollView>();

        // 子节点 Input Canvas
        var inputCanvasGO = new GameObject("Input Canvas", typeof(RectTransform));
        inputCanvasGO.transform.SetParent(this.transform, false);
        inputCanvasGO.AddComponent<PeakChatOpsInputCanvas>();

    }
}