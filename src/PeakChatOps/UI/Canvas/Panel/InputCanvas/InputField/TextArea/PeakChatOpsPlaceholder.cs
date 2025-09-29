#nullable enable
using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsPlaceholder : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsPlaceholder));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsPlaceholder>();
        return go;
    }

    public RectTransform? PlaceholderRectTransform;
    private static TMPro.TMP_FontAsset? _darumaFontAsset;

    private void Awake()
    
    {
        DevLog.File($"启动: {nameof(PeakChatOpsPlaceholder)}");
        PlaceholderRectTransform = GetComponent<RectTransform>();
        // 基础 RectTransform 属性
        PlaceholderRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        PlaceholderRectTransform.sizeDelta = new Vector2(0, 0);
        PlaceholderRectTransform.anchorMin = new Vector2(0f, 0f);
        PlaceholderRectTransform.anchorMax = new Vector2(1f, 1f);
        PlaceholderRectTransform.pivot = new Vector2(0.5f, 0.5f);
        PlaceholderRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        PlaceholderRectTransform.localScale = Vector3.one;

        // CanvasRenderer（用于UI渲染）
        if (GetComponent<CanvasRenderer>() == null)
            gameObject.AddComponent<CanvasRenderer>();

        // 剔除（裁剪）功能，依赖父节点 RectMask2D
        // TMP_Text 组件
        var tmpText = gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        tmpText.text = "Enter...";
        tmpText.font = DarumaDropOneFont;
        tmpText.fontSizeMin = 3;
        tmpText.fontSizeMax = 72;
        tmpText.enableAutoSizing = true;
        tmpText.fontStyle = TMPro.FontStyles.Italic;
        tmpText.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
        tmpText.textWrappingMode = TMPro.TextWrappingModes.Normal;
        tmpText.overflowMode = TMPro.TextOverflowModes.Ellipsis;
        tmpText.color = new Color32(180, 180, 180, 255);
        // 材质 preset 可选，推荐使用默认或根据字体自动分配
        // tmpText.fontSharedMaterial = tmpText.font.material;

    }

    public static TMPro.TMP_FontAsset? DarumaDropOneFont
    {
        get
        {
            if (_darumaFontAsset == null)
            {
                var assets = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>();
                _darumaFontAsset = System.Linq.Enumerable.FirstOrDefault(assets, fontAsset =>
                    fontAsset.faceInfo.familyName == "Daruma Drop One"
                );
            }
            return _darumaFontAsset;
        }
    }
    
}
