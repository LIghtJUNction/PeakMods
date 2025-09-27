// Decompiled with JetBrains decompiler
// Type: Action_Consume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Action_Consume : ItemAction
{
  public override void RunAction()
  {
    if (!(bool) (Object) this.character)
      return;
    this.item.StartCoroutine(this.item.ConsumeDelayed());
  }
}
