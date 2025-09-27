// Decompiled with JetBrains decompiler
// Type: BadgeUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
public class BadgeUI : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  ISelectHandler,
  IDeselectHandler
{
  public BadgeManager manager;
  public RawImage icon;
  public RawImage blank;
  public BadgeData data;
  public CanvasGroup canvasGroup;

  public void Init(BadgeData data)
  {
    this.data = data;
    if ((bool) (Object) data)
    {
      this.gameObject.SetActive(true);
      this.icon.texture = data.icon;
      this.icon.color = new Color(1f, 1f, 1f, data.IsLocked ? 0.0f : 1f);
      this.icon.enabled = true;
      this.blank.enabled = false;
    }
    else
      this.gameObject.SetActive(false);
  }

  public void Hover() => this.manager.selectedBadge = this;

  public void Dehover()
  {
    if (!((Object) this.manager.selectedBadge == (Object) this))
      return;
    this.manager.selectedBadge = (BadgeUI) null;
  }

  public void OnPointerEnter(PointerEventData eventData) => this.Hover();

  public void OnPointerExit(PointerEventData eventData) => this.Dehover();

  public void OnSelect(BaseEventData eventData) => this.Hover();

  public void OnDeselect(BaseEventData eventData) => this.Dehover();
}
