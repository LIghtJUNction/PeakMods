// Decompiled with JetBrains decompiler
// Type: PSC_RequiredMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_RequiredMaterial : PropSpawnerConstraint
{
  public Material[] RequiredMaterial;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    MeshRenderer componentInChildren = spawnData.hit.transform.GetComponentInChildren<MeshRenderer>();
    foreach (Material material in this.RequiredMaterial)
    {
      if ((Object) componentInChildren != (Object) null)
        return (Object) componentInChildren.sharedMaterial == (Object) material;
    }
    return true;
  }
}
