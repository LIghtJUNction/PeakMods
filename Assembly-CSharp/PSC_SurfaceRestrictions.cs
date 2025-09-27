// Decompiled with JetBrains decompiler
// Type: PSC_SurfaceRestrictions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PSC_SurfaceRestrictions : PropSpawnerConstraint
{
  public LayerMask effectedLayers;
  public List<string> whitelistedTagWords = new List<string>();

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    if ((this.effectedLayers.value & 1 << spawnData.hit.transform.gameObject.layer) == 0)
      return true;
    for (int index = 0; index < this.whitelistedTagWords.Count; ++index)
    {
      if (spawnData.hit.transform.tag.ToUpper().Contains(this.whitelistedTagWords[index].ToUpper()))
        return true;
    }
    return false;
  }
}
