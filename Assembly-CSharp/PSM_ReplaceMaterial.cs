// Decompiled with JetBrains decompiler
// Type: PSM_ReplaceMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_ReplaceMaterial : PropSpawnerMod
{
  public Material replaceThis;
  public Material withThis;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    foreach (Renderer componentsInChild in spawned.GetComponentsInChildren<Renderer>())
    {
      Material[] sharedMaterials = componentsInChild.sharedMaterials;
      for (int index = 0; index < sharedMaterials.Length; ++index)
      {
        if ((Object) sharedMaterials[index] == (Object) this.replaceThis)
          sharedMaterials[index] = this.withThis;
      }
      componentsInChild.sharedMaterials = sharedMaterials;
    }
  }
}
