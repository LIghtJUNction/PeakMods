// Decompiled with JetBrains decompiler
// Type: PSC_SameTypeDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_SameTypeDistance : PropSpawnerConstraint
{
  public float minDistance = 5f;
  public Vector3 axisMultipliers = Vector3.one;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    int childCount = spawnData.spawnerTransform.childCount;
    for (int index = 0; index < childCount; ++index)
    {
      Vector3 vector3 = spawnData.pos - spawnData.spawnerTransform.GetChild(index).position;
      vector3.x /= this.axisMultipliers.x;
      vector3.y /= this.axisMultipliers.y;
      vector3.z /= this.axisMultipliers.z;
      if ((double) vector3.magnitude < (double) this.minDistance)
        return false;
    }
    return true;
  }
}
