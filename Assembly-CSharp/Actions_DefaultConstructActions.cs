// Decompiled with JetBrains decompiler
// Type: Actions_DefaultConstructActions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (Constructable))]
public class Actions_DefaultConstructActions : ItemActionBase
{
  public Constructable constructable;

  private void Awake() => this.constructable = this.GetComponent<Constructable>();

  protected override void Subscribe()
  {
    this.item.OnPrimaryStarted += new Action(this.StartConstruction);
    this.item.OnPrimaryFinishedCast += new Action(((ItemActionBase) this).RunAction);
    this.item.OnPrimaryCancelled += new Action(this.CancelConstruction);
  }

  protected override void Unsubscribe()
  {
    this.item.OnPrimaryStarted -= new Action(this.StartConstruction);
    this.item.OnPrimaryFinishedCast -= new Action(((ItemActionBase) this).RunAction);
    this.item.OnPrimaryCancelled -= new Action(this.CancelConstruction);
  }

  public virtual void StartConstruction() => this.constructable.StartConstruction();

  public virtual void CancelConstruction() => this.constructable.DestroyPreview();

  public override void RunAction() => this.constructable.FinishConstruction();
}
