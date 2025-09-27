// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.UiExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity.Demos.DemoVoiceUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos;

public static class UiExtensions
{
  public static void SetPosX(this RectTransform rectTransform, float x)
  {
    rectTransform.anchoredPosition3D = new Vector3(x, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
  }

  public static void SetHeight(this RectTransform rectTransform, float h)
  {
    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
  }

  public static void SetValue(this Toggle toggle, bool isOn) => toggle.SetIsOnWithoutNotify(isOn);

  public static void SetValue(this Slider slider, float v) => slider.SetValueWithoutNotify(v);

  public static void SetValue(this InputField inputField, string v)
  {
    inputField.SetTextWithoutNotify(v);
  }

  public static void DestroyChildren(this Transform transform)
  {
    if (!((Object) null != (Object) transform) || !(bool) (Object) transform)
      return;
    for (int index = transform.childCount - 1; index >= 0; --index)
    {
      Transform child = transform.GetChild(index);
      if ((bool) (Object) child && (bool) (Object) child.gameObject)
        Object.Destroy((Object) child.gameObject);
    }
    transform.DetachChildren();
  }

  public static void Hide(this CanvasGroup canvasGroup, bool blockRaycasts = false, bool interactable = false)
  {
    canvasGroup.alpha = 0.0f;
    canvasGroup.blocksRaycasts = blockRaycasts;
    canvasGroup.interactable = interactable;
  }

  public static void Show(this CanvasGroup canvasGroup, bool blockRaycasts = true, bool interactable = true)
  {
    canvasGroup.alpha = 1f;
    canvasGroup.blocksRaycasts = blockRaycasts;
    canvasGroup.interactable = interactable;
  }

  public static bool IsHidden(this CanvasGroup canvasGroup) => (double) canvasGroup.alpha <= 0.0;

  public static bool IsShown(this CanvasGroup canvasGroup) => (double) canvasGroup.alpha > 0.0;

  public static void SetSingleOnClickCallback(this Button button, UnityAction action)
  {
    button.onClick.RemoveAllListeners();
    button.onClick.AddListener(action);
  }

  public static void SetSingleOnValueChangedCallback(this Toggle toggle, UnityAction<bool> action)
  {
    toggle.onValueChanged.RemoveAllListeners();
    toggle.onValueChanged.AddListener(action);
  }

  public static void SetSingleOnValueChangedCallback(
    this InputField inputField,
    UnityAction<string> action)
  {
    inputField.onValueChanged.RemoveAllListeners();
    inputField.onValueChanged.AddListener(action);
  }

  public static void SetSingleOnEndEditCallback(
    this InputField inputField,
    UnityAction<string> action)
  {
    inputField.onEndEdit.RemoveAllListeners();
    inputField.onEndEdit.AddListener(action);
  }

  public static void SetSingleOnValueChangedCallback(
    this Dropdown inputField,
    UnityAction<int> action)
  {
    inputField.onValueChanged.RemoveAllListeners();
    inputField.onValueChanged.AddListener(action);
  }

  public static void SetSingleOnValueChangedCallback(this Slider slider, UnityAction<float> action)
  {
    slider.onValueChanged.RemoveAllListeners();
    slider.onValueChanged.AddListener(action);
  }

  public static void SetSingleOnValueChangedCallback(
    this MicrophoneSelector selector,
    UnityAction<Photon.Voice.Unity.Demos.DemoVoiceUI.MicType, DeviceInfo> action)
  {
    selector.onValueChanged.RemoveAllListeners();
    selector.onValueChanged.AddListener(action);
  }
}
