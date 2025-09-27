using System.Linq;
using TMPro;
using UnityEngine;
namespace PeakChatOps.Core;


// Minimal replacement for PEAKLib.UI.Elements.PeakText to allow migration/testing.
// Keeps a compatible API surface used by PeakChatOps (SetText, SetFontSize, SetColor, TextMesh).
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class PeakChatOpsText : MonoBehaviour
{
    private static TMP_FontAsset? _darumaFontAsset;
    public static TMP_FontAsset? DarumaDropOneFont
    {
        get
        {
            if (_darumaFontAsset == null)
            {
                var assets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                _darumaFontAsset = assets.FirstOrDefault(fontAsset =>
                    fontAsset.faceInfo.familyName == "Daruma Drop One"
                );
            }
            return _darumaFontAsset;
        }
    }

    private TextMeshProUGUI? _textMesh;
    private RectTransform? _rectTransform;

    // Expose similar API as PeakText
    public TextMeshProUGUI TextMesh
    {
        get
        {
            if (_textMesh == null) _textMesh = GetComponent<TextMeshProUGUI>();
            return _textMesh!;
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform!;
        }
    }

    private void Awake()
    {
        RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0, 1);
        RectTransform.pivot = new Vector2(0, 1);

        TextMesh.font = DarumaDropOneFont;
        TextMesh.color = Color.white;
        RectTransform.sizeDelta = TextMesh.GetPreferredValues();
    }

    public PeakChatOpsText SetText(string text)
    {
        TextMesh.text = text;
        RectTransform.sizeDelta = TextMesh.GetPreferredValues();
        return this;
    }

    public PeakChatOpsText SetFontSize(float size)
    {
        TextMesh.fontSize = size;
        RectTransform.sizeDelta = TextMesh.GetPreferredValues();
        return this;
    }

    public PeakChatOpsText SetColor(Color color)
    {
        TextMesh.color = color;
        return this;
    }
}
