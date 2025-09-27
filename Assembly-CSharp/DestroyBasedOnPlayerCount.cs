// Decompiled with JetBrains decompiler
// Type: DestroyBasedOnPlayerCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;

#nullable disable
public class DestroyBasedOnPlayerCount : MonoBehaviourPun
{
  public int destroyIfPlayerCountIsLessThan;

  private IEnumerator Start()
  {
    DestroyBasedOnPlayerCount basedOnPlayerCount = this;
    while (!PhotonNetwork.InRoom)
      yield return (object) null;
    if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length < basedOnPlayerCount.destroyIfPlayerCountIsLessThan)
    {
      Debug.Log((object) $"Item was told to destroy if player count <{basedOnPlayerCount.destroyIfPlayerCountIsLessThan} and it is {PhotonNetwork.PlayerList.Length}");
      PhotonNetwork.Destroy(basedOnPlayerCount.photonView);
    }
  }
}
