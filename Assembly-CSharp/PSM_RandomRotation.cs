// Decompiled with JetBrains decompiler
// Type: PSM_RandomRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_RandomRotation : PropSpawnerMod
{
  [Range(0.0f, 1f)]
  public float minRotation;
  [Range(0.0f, 1f)]
  public float maxRotation = 0.5f;
  public float randomPow = 1f;
  [Range(0.0f, 1f)]
  public float snapToIncrement;
  public float increment = 90f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Random.rotation, Mathf.Lerp(this.minRotation, this.maxRotation, Mathf.Pow(Random.value, this.randomPow)));
    float x = Mathf.Round(spawned.transform.eulerAngles.x / this.increment) * this.increment;
    float y = Mathf.Round(spawned.transform.eulerAngles.y / this.increment) * this.increment;
    float z = Mathf.Round(spawned.transform.eulerAngles.z / this.increment) * this.increment;
    spawned.transform.eulerAngles = Vector3.Lerp(spawned.transform.eulerAngles, new Vector3(x, y, z), this.snapToIncrement);
  }
}
