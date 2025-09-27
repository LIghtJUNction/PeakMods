// Decompiled with JetBrains decompiler
// Type: PSC_LineCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_LineCheck : PropSpawnerConstraint
{
  public Vector3 localStart = new Vector3(0.0f, 0.0f, 0.0f);
  public Vector3 localEnd = new Vector3(0.0f, 5f, 0.0f);

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    Vector3 vector3_1 = spawnData.hit.point + Vector3.Scale(spawnData.spawnerTransform.lossyScale, this.localStart);
    Vector3 vector3_2 = vector3_1 + Vector3.Scale(spawnData.spawnerTransform.localScale, this.localEnd);
    bool flag = !(bool) (Object) HelperFunctions.LineCheck(vector3_1, vector3_2, HelperFunctions.LayerType.TerrainMap).transform;
    Debug.DrawLine(vector3_1, vector3_2, flag ? Color.green : Color.red, 10f);
    return flag;
  }
}
