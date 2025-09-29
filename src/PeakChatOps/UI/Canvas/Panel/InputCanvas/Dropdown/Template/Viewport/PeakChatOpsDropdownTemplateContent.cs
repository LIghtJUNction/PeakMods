using PeakChatOps.Core;
using UnityEngine;
using UnityEngine.UI;
namespace PeakChatOps.UI;
public class PeakChatOpsDropdownTemplateContent : MonoBehaviour{
    public static GameObject Create(Transform parent)
    {
        var go = new GameObject(nameof(PeakChatOpsDropdownTemplateContent));
        go.transform.SetParent(parent, false);
        go.AddComponent<PeakChatOpsDropdownTemplateContent>();
        return go;
    }

    public RectTransform ContentRectTransform;


    private void Awake()
    {
        DevLog.File($"启动: {nameof(PeakChatOpsDropdownTemplateContent)}");

        ContentRectTransform = GetComponent<RectTransform>();
        ContentRectTransform.anchorMin = new Vector2(0f, 0f);
        ContentRectTransform.anchorMax = new Vector2(1f, 1f);
        ContentRectTransform.pivot = new Vector2(0.5f, 0.5f);
        ContentRectTransform.sizeDelta = new Vector2(0, 0);

        // 自动创建并挂载Item子节点
        var itemGO = new GameObject("Item", typeof(RectTransform));
        itemGO.transform.SetParent(this.transform, false);
        itemGO.AddComponent<PeakChatOpsDropdownTemplateItem>();
    }
}
