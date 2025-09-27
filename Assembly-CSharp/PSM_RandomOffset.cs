// Decompiled with JetBrains decompiler
// Type: PSM_RandomOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_RandomOffset : PropSpawnerMod
{
  public float minOffset;
  public float maxOffset = 0.5f;
  public float randomPow = 1f;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    float num = Mathf.Lerp(this.minOffset, this.maxOffset, Mathf.Pow(Random.value, this.randomPow));
    spawned.transform.position += Random.onUnitSphere * num;
  }
}
