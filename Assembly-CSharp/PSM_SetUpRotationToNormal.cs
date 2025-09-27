// Decompiled with JetBrains decompiler
// Type: PSM_SetUpRotationToNormal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_SetUpRotationToNormal : PropSpawnerMod
{
  [Range(0.0f, 1f)]
  public float minEffect;
  [Range(0.0f, 1f)]
  public float maxEffect = 1f;
  public float randomPow = 1f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    spawned.transform.rotation = Quaternion.Lerp(spawned.transform.rotation, HelperFunctions.GetRandomRotationWithUp(spawnData.normal), Mathf.Lerp(this.minEffect, this.maxEffect, Mathf.Pow(Random.value, this.randomPow)));
  }
}
