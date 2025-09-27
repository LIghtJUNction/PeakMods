// Decompiled with JetBrains decompiler
// Type: PSM_SetMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_SetMaterial : PropSpawnerMod
{
  public Material mat;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    foreach (Renderer componentsInChild in spawned.GetComponentsInChildren<Renderer>())
      componentsInChild.sharedMaterial = this.mat;
  }
}
