// Decompiled with JetBrains decompiler
// Type: PSM_SpecificRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_SpecificRotation : PropSpawnerMod
{
  public Vector3 eulerAngles;
  [Range(0.0f, 1f)]
  public float random;
  public Vector3 eulerAnglesRandom;
  [Range(0.0f, 1f)]
  public float blend = 1f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    Vector3 vector3 = this.eulerAngles;
    if ((double) this.random > 0.0)
      vector3 = Vector3.Lerp(vector3, this.eulerAnglesRandom, Random.value * this.random);
    spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, Quaternion.Euler(vector3), this.blend);
  }
}
