// Decompiled with JetBrains decompiler
// Type: Actions_Binoculars
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class Actions_Binoculars : ItemActionBase
{
  public Action_ShowBinocularOverlay binocOverlay;
  public CameraOverride_Binoculars cameraOverride;
  public float scrollSpeed = 2f;
  public float scrollSpeedButton = 2f;

  protected override void Subscribe()
  {
    this.item.OnScrolledMouseOnly += new Action<float>(this.Scrolled);
    this.item.OnScrollForwardHeld += new Action(this.ScrollForwardHeld);
    this.item.OnScrollBackwardHeld += new Action(this.ScrollBackwardHeld);
  }

  protected override void Unsubscribe()
  {
    this.item.OnScrolledMouseOnly -= new Action<float>(this.Scrolled);
    this.item.OnScrollForwardHeld -= new Action(this.ScrollForwardHeld);
    this.item.OnScrollBackwardHeld -= new Action(this.ScrollBackwardHeld);
  }

  private void ScrollForwardHeld()
  {
    if (!this.binocOverlay.binocularsActive)
      return;
    this.cameraOverride.AdjustFOV(-this.scrollSpeedButton * Time.deltaTime);
  }

  private void ScrollBackwardHeld()
  {
    if (!this.binocOverlay.binocularsActive)
      return;
    this.cameraOverride.AdjustFOV(this.scrollSpeedButton * Time.deltaTime);
  }

  private void Scrolled(float value)
  {
    if (!this.binocOverlay.binocularsActive)
      return;
    this.cameraOverride.AdjustFOV(-value * this.scrollSpeed);
  }
}
