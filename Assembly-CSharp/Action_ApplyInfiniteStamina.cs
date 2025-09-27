// Decompiled with JetBrains decompiler
// Type: Action_ApplyInfiniteStamina
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using UnityEngine;

#nullable disable
public class Action_ApplyInfiniteStamina : ItemAction
{
  public float buffTime;
  public float drowsyAmount = 0.25f;

  public override void RunAction()
  {
    Debug.Log((object) "Adding infinite stamina buff");
    this.character.refs.afflictions.AddAffliction((Affliction) new Affliction_InfiniteStamina(this.buffTime));
  }
}
