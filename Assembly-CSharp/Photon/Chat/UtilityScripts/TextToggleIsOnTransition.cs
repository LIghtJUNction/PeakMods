// Decompiled with JetBrains decompiler
// Type: Photon.Chat.UtilityScripts.TextToggleIsOnTransition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.UtilityScripts;

[RequireComponent(typeof (Text))]
public class TextToggleIsOnTransition : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  public Toggle toggle;
  private Text _text;
  public Color NormalOnColor = Color.white;
  public Color NormalOffColor = Color.black;
  public Color HoverOnColor = Color.black;
  public Color HoverOffColor = Color.black;
  private bool isHover;

  public void OnEnable()
  {
    this._text = this.GetComponent<Text>();
    this.OnValueChanged(this.toggle.isOn);
    this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
  }

  public void OnDisable()
  {
    this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
  }

  public void OnValueChanged(bool isOn)
  {
    this._text.color = isOn ? (this.isHover ? this.HoverOnColor : this.HoverOnColor) : (this.isHover ? this.NormalOffColor : this.NormalOffColor);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.isHover = true;
    this._text.color = this.toggle.isOn ? this.HoverOnColor : this.HoverOffColor;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.isHover = false;
    this._text.color = this.toggle.isOn ? this.NormalOnColor : this.NormalOffColor;
  }
}
