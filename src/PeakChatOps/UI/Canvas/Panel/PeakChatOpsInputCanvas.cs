using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsInputCanvas : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsInputCanvas));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsInputCanvas>();
        return go;
    }

    public RectTransform InputCanvasRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsInputCanvas)}");

        InputCanvasRectTransform = GetComponent<RectTransform>();
        // RectTransform 配置
        InputCanvasRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        InputCanvasRectTransform.sizeDelta = new Vector2(0, 0);
        InputCanvasRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        InputCanvasRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        InputCanvasRectTransform.pivot = new Vector2(0.5f, 0.5f);
        InputCanvasRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        InputCanvasRectTransform.localScale = Vector3.one;

        // Canvas
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        canvas.pixelPerfect = false; // 继承父级

        // GraphicRaycaster
        var raycaster = gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        raycaster.ignoreReversedGraphics = true;
        // 其余默认

        // 子节点 SendButton
        var sendButtonGO = new GameObject("SendButton", typeof(RectTransform));
        sendButtonGO.transform.SetParent(this.transform, false);
        sendButtonGO.AddComponent<PeakChatOpsSendButton>();

        // 子节点 InputField
        var inputFieldGO = new GameObject("InputField", typeof(RectTransform));
        inputFieldGO.transform.SetParent(this.transform, false);
        inputFieldGO.AddComponent<PeakChatOpsInputField>();

        // 子节点 Dropdown
        var dropdownGO = new GameObject("Dropdown", typeof(RectTransform));
        dropdownGO.transform.SetParent(this.transform, false);
        dropdownGO.AddComponent<PeakChatOpsDropdown>();
    }
}