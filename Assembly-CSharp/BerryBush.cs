// Decompiled with JetBrains decompiler
// Type: BerryBush
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BerryBush : Spawner
{
  public Vector2 possibleBerries;
  public float randomPow = 1f;

  public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
  {
    List<PhotonView> photonViewList = new List<PhotonView>();
    if (!PhotonNetwork.IsMasterClient)
      return photonViewList;
    List<Transform> transformList = new List<Transform>((IEnumerable<Transform>) spawnSpots);
    GameObject gameObject = this.spawnMode != Spawner.SpawnMode.SingleItem ? LootData.GetRandomItem(this.spawnPool) : this.spawnedObjectPrefab;
    int num = Mathf.RoundToInt(Mathf.Lerp(this.possibleBerries.x, this.possibleBerries.y, Mathf.Pow(Random.value, this.randomPow)));
    for (int index1 = 0; index1 < spawnSpots.Count && index1 < num; ++index1)
    {
      int index2 = Random.Range(0, transformList.Count);
      if (!((Object) gameObject == (Object) null))
      {
        Item component = PhotonNetwork.InstantiateItemRoom(gameObject.name, transformList[index2].position, Quaternion.identity).GetComponent<Item>();
        photonViewList.Add(component.GetComponent<PhotonView>());
        if ((bool) (Object) this.spawnUpTowardsTarget)
        {
          component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
          component.transform.Rotate(Vector3.up, Random.Range(0.0f, 360f), Space.Self);
        }
        if (this.spawnTransformIsSpawnerTransform)
        {
          component.transform.rotation = transformList[index2].rotation;
          component.transform.Rotate(Vector3.up, Random.Range(0.0f, 360f), Space.Self);
        }
        if ((Object) component != (Object) null)
          component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) true, (object) component.transform.position, (object) component.transform.rotation);
        transformList.RemoveAt(index2);
      }
    }
    return photonViewList;
  }

  public void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.magenta;
    for (int index = 0; index < this.spawnSpots.Count; ++index)
      Gizmos.DrawSphere(this.spawnSpots[index].position, 0.25f);
  }
}
