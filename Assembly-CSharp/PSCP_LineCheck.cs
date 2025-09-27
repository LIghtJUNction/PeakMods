// Decompiled with JetBrains decompiler
// Type: PSCP_LineCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSCP_LineCheck : PropSpawnerConstraintPost
{
  public Vector3 localStart = new Vector3(0.0f, 0.1f, 0.0f);
  public Vector3 localEnd = new Vector3(0.0f, 5f, 0.0f);

  public override bool CheckConstraint(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    return !(bool) (Object) HelperFunctions.LineCheck(spawned.transform.TransformPoint(this.localStart), spawned.transform.TransformPoint(this.localEnd), HelperFunctions.LayerType.TerrainMap).transform;
  }
}
