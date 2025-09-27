// Decompiled with JetBrains decompiler
// Type: SingleItemSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class SingleItemSpawner : MonoBehaviour, ISpawner
{
  public bool isKinematic;
  public GameObject prefab;
  public int playersInRoomRequirement;
  public int belowAscentRequirement = -1;

  public List<PhotonView> TrySpawnItems()
  {
    if (this.playersInRoomRequirement > PhotonNetwork.PlayerList.Length)
    {
      Debug.LogError((object) $"Not spawning: {this.prefab} because of players in room req: {this.playersInRoomRequirement}");
      return new List<PhotonView>();
    }
    if (this.belowAscentRequirement != -1 && Ascents.currentAscent >= this.belowAscentRequirement)
    {
      Debug.LogError((object) $"Not spawning: {this.prefab} because ascent is too high: {Ascents.currentAscent}");
      return new List<PhotonView>();
    }
    PhotonView component = PhotonNetwork.InstantiateItemRoom(this.prefab.name, this.transform.position + Vector3.up * 0.1f, this.transform.rotation).GetComponent<PhotonView>();
    if (this.isKinematic)
      component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) true, (object) component.transform.position, (object) component.transform.rotation);
    return new List<PhotonView>() { component };
  }

  private void OnDrawGizmos()
  {
    if (!((Object) this.prefab != (Object) null))
      return;
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(this.transform.position, 0.25f);
  }
}
