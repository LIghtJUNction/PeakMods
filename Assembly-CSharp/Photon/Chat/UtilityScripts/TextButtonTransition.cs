// Decompiled with JetBrains decompiler
// Type: Photon.Chat.UtilityScripts.TextButtonTransition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
namespace Photon.Chat.UtilityScripts;

[RequireComponent(typeof (Text))]
public class TextButtonTransition : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  private Text _text;
  public Selectable Selectable;
  public Color NormalColor = Color.white;
  public Color HoverColor = Color.black;

  public void Awake() => this._text = this.GetComponent<Text>();

  public void OnEnable() => this._text.color = this.NormalColor;

  public void OnDisable() => this._text.color = this.NormalColor;

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!((Object) this.Selectable == (Object) null) && !this.Selectable.IsInteractable())
      return;
    this._text.color = this.HoverColor;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (!((Object) this.Selectable == (Object) null) && !this.Selectable.IsInteractable())
      return;
    this._text.color = this.NormalColor;
  }
}
