using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsOutLine : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsOutLine));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsOutLine>();
        return go;
    }

    public RectTransform OutLineRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsOutLine)}");

        OutLineRectTransform = GetComponent<RectTransform>();
        // RectTransform 属性
        OutLineRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        OutLineRectTransform.sizeDelta = new Vector2(750, 550);
        OutLineRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        OutLineRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        OutLineRectTransform.pivot = new Vector2(0.5f, 0.5f);
        OutLineRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        OutLineRectTransform.localScale = Vector3.one;

        // CanvasRenderer
        if (GetComponent<CanvasRenderer>() == null)
            gameObject.AddComponent<CanvasRenderer>();

        // Image
        var img = GetComponent<UnityEngine.UI.Image>();
        if (img == null)
            img = gameObject.AddComponent<UnityEngine.UI.Image>();
        // TODO: img.sprite = Resources.Load<Sprite>("UI_Blur_Outlne_Thick"); // 需确保资源路径正确
        img.color = Color.white;
        img.raycastTarget = true;
        img.type = UnityEngine.UI.Image.Type.Sliced;
        img.fillCenter = true;
        img.pixelsPerUnitMultiplier = 1f;
        img.maskable = true;
        // 其他属性默认即可
    }
}