// Decompiled with JetBrains decompiler
// Type: EmoteWheelSlice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
public class EmoteWheelSlice : 
  UIWheelSlice,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  private EmoteWheel emoteWheel;
  private EmoteWheelData emoteData;
  public Image image;

  public void Init(EmoteWheelData data, EmoteWheel wheel)
  {
    this.emoteWheel = wheel;
    this.emoteData = data;
    if ((Object) data == (Object) null)
    {
      this.image.enabled = false;
      this.button.interactable = false;
    }
    else
    {
      this.image.enabled = true;
      this.image.sprite = data.emoteSprite;
      this.button.interactable = true;
    }
  }

  public void Hover() => this.emoteWheel.Hover(this.emoteData);

  public void Dehover() => this.emoteWheel.Dehover(this.emoteData);

  public void OnPointerEnter(PointerEventData eventData) => this.Hover();

  public void OnPointerExit(PointerEventData eventData) => this.Dehover();
}
