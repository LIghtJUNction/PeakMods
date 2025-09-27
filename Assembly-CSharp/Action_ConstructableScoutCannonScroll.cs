// Decompiled with JetBrains decompiler
// Type: Action_ConstructableScoutCannonScroll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class Action_ConstructableScoutCannonScroll : ItemActionBase
{
  public Constructable constructable;
  public float angleAmount = 5f;
  public float maxAngle;

  protected override void Subscribe()
  {
    this.item.OnScrolledMouseOnly += new Action<float>(this.Scrolled);
    this.item.OnScrollBackwardPressed += new Action(this.ScrollLeft);
    this.item.OnScrollForwardPressed += new Action(this.ScrollRight);
  }

  protected override void Unsubscribe()
  {
    this.item.OnScrolledMouseOnly -= new Action<float>(this.Scrolled);
    this.item.OnScrollBackwardPressed -= new Action(this.ScrollLeft);
    this.item.OnScrollForwardPressed -= new Action(this.ScrollRight);
  }

  private void ScrollLeft() => this.Scrolled(-1f);

  private void ScrollRight() => this.Scrolled(1f);

  private void Scrolled(float value)
  {
    if (!((UnityEngine.Object) this.constructable != (UnityEngine.Object) null) || !((UnityEngine.Object) this.constructable.currentPreview != (UnityEngine.Object) null))
      return;
    this.constructable.angleOffset += value * this.angleAmount;
    this.constructable.angleOffset = Mathf.Clamp(this.constructable.angleOffset, -this.maxAngle, this.maxAngle);
    this.constructable.UpdateAngle();
  }
}
