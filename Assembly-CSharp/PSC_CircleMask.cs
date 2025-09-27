// Decompiled with JetBrains decompiler
// Type: PSC_CircleMask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_CircleMask : PropSpawnerConstraint
{
  public float circleSize = 10f;
  public bool inverted;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    float num = Vector2.Distance(new Vector2(spawnData.pos.x, spawnData.pos.z), new Vector2(spawnData.spawnerTransform.position.x, spawnData.spawnerTransform.position.z));
    return this.inverted ? (double) num > (double) this.circleSize : (double) num < (double) this.circleSize;
  }
}
