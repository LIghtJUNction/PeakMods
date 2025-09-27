// Decompiled with JetBrains decompiler
// Type: PSM_SetRandomMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_SetRandomMaterial : PropSpawnerMod
{
  public Material[] mats;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    Renderer[] componentsInChildren = spawned.GetComponentsInChildren<Renderer>();
    Material mat = this.mats[Random.Range(0, this.mats.Length)];
    for (int index = 0; index < componentsInChildren.Length; ++index)
      componentsInChildren[index].sharedMaterial = mat;
  }
}
