// Decompiled with JetBrains decompiler
// Type: PSM_SetMaterialOnChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class PSM_SetMaterialOnChild : PropSpawnerMod
{
  public string childName;
  public Material mat;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    List<Renderer> rends = new List<Renderer>();
    spawned.transform.FindChildrenRecursive(this.childName).ForEach((Action<Transform>) (c => rends.AddRange((IEnumerable<Renderer>) c.GetComponentsInChildren<Renderer>())));
    for (int index = 0; index < rends.Count; ++index)
      rends[index].sharedMaterial = this.mat;
  }
}
