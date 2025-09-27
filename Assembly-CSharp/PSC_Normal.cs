// Decompiled with JetBrains decompiler
// Type: PSC_Normal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_Normal : PropSpawnerConstraint
{
  public float minAngle;
  public float maxAngle = 50f;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    float num = Vector3.Angle(Vector3.up, spawnData.normal);
    return (double) num < (double) this.maxAngle && (double) num > (double) this.minAngle;
  }
}
