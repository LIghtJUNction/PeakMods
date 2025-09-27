// Decompiled with JetBrains decompiler
// Type: ButtonHoverFeedback
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
public class ButtonHoverFeedback : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  private float scale = 1f;
  private float vel;
  private float targetScale = 1f;

  private void Start()
  {
    this.GetComponent<Button>()?.onClick.AddListener(new UnityAction(this.OnClick));
  }

  private void OnClick() => this.vel += 15f;

  public void OnPointerEnter(PointerEventData eventData) => this.targetScale = 1.15f;

  public void OnPointerExit(PointerEventData eventData) => this.targetScale = 1f;

  private void OnEnable()
  {
    this.transform.localScale = Vector3.one;
    this.scale = 1f;
    this.vel = 0.0f;
    this.targetScale = 1f;
  }

  private void Update()
  {
    this.vel = FRILerp.Lerp(this.vel, (float) (((double) this.targetScale - (double) this.scale) * 25.0), 20f);
    this.scale += this.vel * Time.deltaTime;
    this.transform.localScale = Vector3.one * this.scale;
  }
}
