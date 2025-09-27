// Decompiled with JetBrains decompiler
// Type: ScoutEffigy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class ScoutEffigy : Constructable
{
  protected override void Update()
  {
    if ((bool) (UnityEngine.Object) this.item.holderCharacter)
    {
      if (!Character.PlayerIsDeadOrDown())
        this.item.overrideUsability = Optionable<bool>.Some(false);
      else
        this.item.overrideUsability = Optionable<bool>.None;
    }
    base.Update();
  }

  public override void FinishConstruction()
  {
    if (!this.constructing || (UnityEngine.Object) this.currentPreview == (UnityEngine.Object) null)
      return;
    List<Character> enumerable = new List<Character>();
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (allCharacter.data.dead || allCharacter.data.fullyPassedOut)
        enumerable.Add(allCharacter);
    }
    if (enumerable.Count == 0)
      return;
    enumerable.RandomSelection<Character>((Func<Character, int>) (c => 1)).photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, (object) (this.currentConstructHit.point + Vector3.up * 1f), (object) false);
    if (!(bool) (UnityEngine.Object) Singleton<AchievementManager>.Instance)
      return;
    Singleton<AchievementManager>.Instance.AddToRunBasedInt(RUNBASEDVALUETYPE.ScoutsResurrected, 1);
  }
}
