// Decompiled with JetBrains decompiler
// Type: PSC_BannedMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_BannedMaterial : PropSpawnerConstraint
{
  public Material[] bannedMaterial;
  private bool returnVal = true;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    this.returnVal = true;
    MeshRenderer[] componentsInChildren = spawnData.hit.transform.GetComponentsInChildren<MeshRenderer>();
    foreach (Material material in this.bannedMaterial)
    {
      foreach (MeshRenderer meshRenderer in componentsInChildren)
      {
        if ((Object) meshRenderer != (Object) null && (Object) meshRenderer.sharedMaterial == (Object) material)
        {
          this.returnVal = false;
          break;
        }
      }
    }
    return this.returnVal;
  }
}
