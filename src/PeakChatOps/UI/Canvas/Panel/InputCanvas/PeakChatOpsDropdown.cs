using PeakChatOps.Core;
using UnityEngine;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdown : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdown));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdown>();
        return go;
    }

    public RectTransform DropdownRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdown)}");
        DropdownRectTransform = GetComponent<RectTransform>();
        // RectTransform 属性
        DropdownRectTransform.anchoredPosition3D = new Vector3(33f, 25f, 0f);
        DropdownRectTransform.sizeDelta = new Vector2(65f, 30f);
        DropdownRectTransform.anchorMin = new Vector2(0f, 0f);
        DropdownRectTransform.anchorMax = new Vector2(1f, 1f);
        DropdownRectTransform.pivot = new Vector2(0.5f, 0.5f);
        DropdownRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        DropdownRectTransform.localScale = Vector3.one;

        // 剔除（裁剪）功能，依赖父节点 RectMask2D

        // Image 组件（米黄色，UISprite，光线投射目标启用）
        var image = gameObject.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color32(255, 245, 200, 255); // 米黄色
        image.raycastTarget = true;
        // TODO: image.sprite = ... // 挂载UISprite资源

        // 挂载 Dropdown 组件
        var dropdown = gameObject.AddComponent<UnityEngine.UI.Dropdown>();
        dropdown.interactable = true;
        dropdown.options.Clear();
        dropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Option 1"));
        dropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Option 2"));
        dropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData("Option 3"));
        dropdown.value = 0;

        // 子组件 Label
        var labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(this.transform, false);
        labelGO.AddComponent<PeakChatOpsDropdownLabel>();
        dropdown.captionText = labelGO.GetComponent<UnityEngine.UI.Text>();

        // 子组件 Arrow
        var arrowGO = new GameObject("Arrow", typeof(RectTransform));
        arrowGO.transform.SetParent(this.transform, false);
        arrowGO.AddComponent<PeakChatOpsDropdownArrow>();
    }
}