// Decompiled with JetBrains decompiler
// Type: NetworkedObjectSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class NetworkedObjectSpawner : MonoBehaviour
{
  public GameObject objToSpawn;
  public float minRate = 3f;
  public float maxRate = 6f;
  public float randomPow = 2f;
  public Vector3 spawnOffset;
  private float untilNext;

  private void Start() => this.SetCounter();

  private void SetCounter()
  {
    this.untilNext = Mathf.Lerp(this.minRate, this.maxRate, Mathf.Pow(Random.value, this.randomPow));
  }

  private void Update()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.untilNext -= Time.deltaTime;
    if ((double) this.untilNext >= 0.0)
      return;
    this.SetCounter();
    this.SpawnObject();
  }

  private void SpawnObject()
  {
    PhotonNetwork.Instantiate(this.objToSpawn.name, this.transform.position + this.spawnOffset, this.transform.rotation);
  }
}
