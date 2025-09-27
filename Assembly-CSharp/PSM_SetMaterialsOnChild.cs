// Decompiled with JetBrains decompiler
// Type: PSM_SetMaterialsOnChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class PSM_SetMaterialsOnChild : PropSpawnerMod
{
  public string childName;
  public MatAndID[] edits;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    List<Renderer> rends = new List<Renderer>();
    spawned.transform.FindChildrenRecursive(this.childName).ForEach((Action<Transform>) (c => rends.AddRange((IEnumerable<Renderer>) c.GetComponentsInChildren<Renderer>())));
    for (int index1 = 0; index1 < rends.Count; ++index1)
    {
      Material[] sharedMaterials = rends[index1].sharedMaterials;
      for (int index2 = 0; index2 < sharedMaterials.Length; ++index2)
      {
        foreach (MatAndID edit in this.edits)
        {
          if (edit.id == index2)
            sharedMaterials[index2] = edit.mat;
        }
      }
      rends[index1].sharedMaterials = sharedMaterials;
    }
  }
}
