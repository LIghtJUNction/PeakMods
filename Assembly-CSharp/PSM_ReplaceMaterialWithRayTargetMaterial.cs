// Decompiled with JetBrains decompiler
// Type: PSM_ReplaceMaterialWithRayTargetMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_ReplaceMaterialWithRayTargetMaterial : PropSpawnerMod
{
  public Material replaceThis;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    if ((Object) spawnData.hit.transform == (Object) null)
      return;
    MeshRenderer[] componentsInChildren = spawnData.hit.transform.GetComponentsInChildren<MeshRenderer>();
    MeshRenderer meshRenderer1 = (MeshRenderer) null;
    foreach (MeshRenderer meshRenderer2 in componentsInChildren)
    {
      if (meshRenderer2.enabled)
      {
        meshRenderer1 = meshRenderer2;
        break;
      }
    }
    if ((Object) meshRenderer1 == (Object) null)
      return;
    foreach (Renderer componentsInChild in spawned.GetComponentsInChildren<Renderer>())
    {
      Material[] sharedMaterials = componentsInChild.sharedMaterials;
      for (int index = 0; index < sharedMaterials.Length; ++index)
      {
        if ((Object) sharedMaterials[index] == (Object) this.replaceThis)
          sharedMaterials[index] = meshRenderer1.sharedMaterial;
      }
      componentsInChild.sharedMaterials = sharedMaterials;
    }
  }
}
