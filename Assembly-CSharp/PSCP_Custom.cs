// Decompiled with JetBrains decompiler
// Type: PSCP_Custom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSCP_Custom : PropSpawnerConstraintPost
{
  public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    foreach (CustomSpawnCondition component in spawned.GetComponents<CustomSpawnCondition>())
    {
      if (!component.CheckCondition(spawnData))
        return false;
    }
    return true;
  }
}
