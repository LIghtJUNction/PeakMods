// Decompiled with JetBrains decompiler
// Type: ScoutmasterSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class ScoutmasterSpawner : MonoBehaviourPunCallbacks
{
  private void Awake()
  {
    if (!PhotonNetwork.InRoom)
      return;
    this.SpawnScoutmaster();
  }

  public override void OnJoinedRoom() => this.SpawnScoutmaster();

  private void SpawnScoutmaster()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    Debug.Log((object) "SPAWN SCOUTMASTER");
    PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", this.transform.position, this.transform.rotation).GetComponent<Character>().data.spawnPoint = this.transform;
  }
}
