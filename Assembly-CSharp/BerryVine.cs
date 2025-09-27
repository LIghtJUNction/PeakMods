// Decompiled with JetBrains decompiler
// Type: BerryVine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BerryVine : Spawner
{
  public Vector2 possibleBerries;
  public float randomPow = 1f;

  protected override List<Transform> GetSpawnSpots()
  {
    Collider[] componentsInChildren = this.GetComponentsInChildren<Collider>();
    List<Transform> spawnSpots = new List<Transform>();
    for (int index = 1; index < componentsInChildren.Length - 1; ++index)
      spawnSpots.Add(componentsInChildren[index].transform);
    return spawnSpots;
  }

  public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
  {
    List<PhotonView> photonViewList = new List<PhotonView>();
    if (!PhotonNetwork.IsMasterClient)
      return photonViewList;
    List<Transform> transformList = new List<Transform>((IEnumerable<Transform>) spawnSpots);
    GameObject spawn = this.spawns.GetSpawns(1)[0];
    int num = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, Mathf.Pow(Random.value, this.randomPow)));
    for (int index1 = 0; index1 < spawnSpots.Count && index1 < num; ++index1)
    {
      int index2 = Random.Range(0, transformList.Count);
      Item component = PhotonNetwork.InstantiateItemRoom(spawn.name, transformList[index2].position, Quaternion.identity).GetComponent<Item>();
      photonViewList.Add(component.GetComponent<PhotonView>());
      if ((bool) (Object) this.spawnUpTowardsTarget)
        component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
      component.transform.rotation = Quaternion.Euler(0.0f, (float) Random.Range(0, 360), 0.0f);
      if ((Object) component != (Object) null)
        component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) true, (object) component.transform.position, (object) component.transform.rotation);
      transformList.RemoveAt(index2);
    }
    return photonViewList;
  }
}
