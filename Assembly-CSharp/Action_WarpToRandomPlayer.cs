// Decompiled with JetBrains decompiler
// Type: Action_WarpToRandomPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Action_WarpToRandomPlayer : ItemAction
{
  public float minimumDistance = 12f;
  public bool restoreUsesOnFailure = true;
  public SFX_Instance[] warpSFX;

  public override void RunAction()
  {
    for (int index = 0; index < this.warpSFX.Length; ++index)
      this.warpSFX[index].Play();
    List<Character> enumerable = new List<Character>();
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (!((UnityEngine.Object) allCharacter == (UnityEngine.Object) this.character) && !allCharacter.data.dead && (double) Vector3.Distance(this.character.Center, allCharacter.Center) > (double) this.minimumDistance)
        enumerable.Add(allCharacter);
    }
    if (enumerable.Count == 0 && this.restoreUsesOnFailure)
      this.item.photonView.RPC("IncreaseUsesRPC", RpcTarget.All);
    else
      this.character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, (object) enumerable.RandomSelection<Character>((Func<Character, int>) (c => 1)).Center, (object) true);
  }

  [PunRPC]
  public void IncreaseUsesRPC()
  {
    OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
    if (!data.HasData || data.Value == -1)
      return;
    ++data.Value;
    if (this.item.totalUses <= 0)
      return;
    this.item.SetUseRemainingPercentage((float) data.Value / (float) this.item.totalUses);
  }
}
