// Decompiled with JetBrains decompiler
// Type: PSM_SingleItemSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_SingleItemSpawner : PropSpawnerMod
{
  public GameObject objToSpawn;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    spawned.GetComponentInChildren<SingleItemSpawner>().prefab = this.objToSpawn;
    spawned.gameObject.name = this.objToSpawn.gameObject.name + " (spawner)";
  }
}
