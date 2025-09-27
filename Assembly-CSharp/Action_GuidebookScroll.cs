// Decompiled with JetBrains decompiler
// Type: Action_GuidebookScroll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class Action_GuidebookScroll : ItemActionBase
{
  private Guidebook guidebook;

  private void Awake() => this.guidebook = this.GetComponent<Guidebook>();

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
    if (!(bool) (UnityEngine.Object) this.guidebook || !this.guidebook.isOpen)
      return;
    if ((double) value < 0.0)
    {
      this.guidebook.FlipPageLeft();
    }
    else
    {
      if ((double) value <= 0.0)
        return;
      this.guidebook.FlipPageRight();
    }
  }
}
