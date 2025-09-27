// Decompiled with JetBrains decompiler
// Type: PSM_LocalOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_LocalOffset : PropSpawnerMod
{
  public Vector3 offset;
  [Range(0.0f, 1f)]
  public float minEffect;
  [Range(0.0f, 1f)]
  public float maxEffect = 1f;
  public float randomPow = 1f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    Vector3 vector3 = Vector3.zero + spawned.transform.right * Mathf.Lerp(-this.offset.x, this.offset.x, Random.value) * Mathf.Pow(Random.value, this.randomPow) + spawned.transform.up * Mathf.Lerp(-this.offset.y, this.offset.y, Random.value) * Mathf.Pow(Random.value, this.randomPow) + spawned.transform.forward * Mathf.Lerp(-this.offset.z, this.offset.z, Random.value) * Mathf.Pow(Random.value, this.randomPow);
    spawned.transform.position += vector3;
  }
}
