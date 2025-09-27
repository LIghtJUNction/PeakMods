// Decompiled with JetBrains decompiler
// Type: ItemAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class ItemAction : ItemActionBase
{
  [SerializeField]
  public bool OnPressed;
  [SerializeField]
  public bool OnHeld;
  [SerializeField]
  public bool OnReleased;
  [SerializeField]
  public bool OnCastFinished;
  [SerializeField]
  public bool OnCancelled;
  public bool OnConsumed;

  protected override void Subscribe()
  {
    if (this.OnPressed)
      this.item.OnPrimaryStarted += new Action(((ItemActionBase) this).RunAction);
    if (this.OnHeld)
      this.item.OnPrimaryHeld += new Action(((ItemActionBase) this).RunAction);
    if (this.OnCastFinished)
      this.item.OnPrimaryFinishedCast += new Action(((ItemActionBase) this).RunAction);
    if (this.OnCancelled)
      this.item.OnPrimaryCancelled += new Action(((ItemActionBase) this).RunAction);
    if (!this.OnConsumed)
      return;
    this.item.OnConsumed += new Action(((ItemActionBase) this).RunAction);
  }

  protected override void Unsubscribe()
  {
    if (this.OnPressed)
      this.item.OnPrimaryStarted -= new Action(((ItemActionBase) this).RunAction);
    if (this.OnHeld)
      this.item.OnPrimaryHeld -= new Action(((ItemActionBase) this).RunAction);
    if (this.OnCastFinished)
      this.item.OnPrimaryFinishedCast -= new Action(((ItemActionBase) this).RunAction);
    if (this.OnCancelled)
      this.item.OnPrimaryCancelled -= new Action(((ItemActionBase) this).RunAction);
    if (!this.OnConsumed)
      return;
    this.item.OnConsumed -= new Action(((ItemActionBase) this).RunAction);
  }
}
