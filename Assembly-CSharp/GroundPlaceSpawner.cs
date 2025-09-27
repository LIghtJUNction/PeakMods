// Decompiled with JetBrains decompiler
// Type: GroundPlaceSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class GroundPlaceSpawner : Spawner
{
  [FormerlySerializedAs("possibleBerries")]
  public Vector2 possibleItems;

  public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
  {
    List<PhotonView> photonViewList = new List<PhotonView>();
    if (!PhotonNetwork.IsMasterClient)
      return photonViewList;
    List<Transform> transformList = new List<Transform>((IEnumerable<Transform>) spawnSpots);
    GameObject spawn = this.spawns.GetSpawns(1)[0];
    int num = Random.Range(Mathf.FloorToInt(this.possibleItems.x), Mathf.FloorToInt(this.possibleItems.y + 1f));
    for (int index1 = 0; index1 < spawnSpots.Count && index1 < num; ++index1)
    {
      int index2 = Random.Range(0, transformList.Count);
      RaycastHit hitInfo;
      if (Physics.Raycast(transformList[index2].position, -this.transform.up, out hitInfo, 100f, (int) HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
      {
        Item component = PhotonNetwork.InstantiateItemRoom(spawn.name, hitInfo.point, HelperFunctions.GetRandomRotationWithUp(hitInfo.normal)).GetComponent<Item>();
        photonViewList.Add(component.GetComponent<PhotonView>());
        component.SetKinematicNetworked(true, component.transform.position, component.transform.rotation);
      }
      transformList.RemoveAt(index2);
    }
    return photonViewList;
  }
}
