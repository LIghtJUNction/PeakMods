// Decompiled with JetBrains decompiler
// Type: PSM_UpLerp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_UpLerp : PropSpawnerMod
{
  [Range(0.0f, 1f)]
  public float minUpLerp;
  [Range(0.0f, 1f)]
  public float maxUpLerp = 1f;
  public float randomPow = 1f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    float t = Mathf.Lerp(this.minUpLerp, this.maxUpLerp, Mathf.Pow(Random.value, this.randomPow));
    Vector3 up = Vector3.Lerp(spawned.transform.up, Vector3.up, t);
    spawned.transform.rotation = HelperFunctions.GetRotationWithUp(spawned.transform.forward, up);
  }
}
