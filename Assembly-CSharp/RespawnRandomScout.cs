// Decompiled with JetBrains decompiler
// Type: RespawnRandomScout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RespawnRandomScout : MonoBehaviour
{
  private void Start()
  {
    if (PhotonNetwork.IsMasterClient)
    {
      List<Character> enumerable = new List<Character>();
      foreach (Character allCharacter in Character.AllCharacters)
      {
        if (allCharacter.data.dead || allCharacter.data.fullyPassedOut)
          enumerable.Add(allCharacter);
      }
      enumerable.RandomSelection<Character>((Func<Character, int>) (c => 1)).photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, (object) this.transform.position, (object) false);
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }
}
