// Decompiled with JetBrains decompiler
// Type: OnItemStateChangedAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class OnItemStateChangedAction : ItemActionBase
{
  protected override void Subscribe()
  {
    this.item.OnStateChange += new Action<ItemState>(this.RunAction);
  }

  protected override void Unsubscribe()
  {
    this.item.OnStateChange -= new Action<ItemState>(this.RunAction);
  }

  public virtual void RunAction(ItemState state)
  {
  }
}
