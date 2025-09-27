// Decompiled with JetBrains decompiler
// Type: PSC_Perlin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_Perlin : PropSpawnerConstraint
{
  public float perlinSize = 10f;
  public Vector2 minMax = new Vector2(0.0f, 0.5f);

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    float num = Mathf.PerlinNoise((float) (((double) spawnData.pos.x + 500.0) * (double) this.perlinSize * 0.10000000149011612), (float) (((double) spawnData.pos.z + 500.0) * (double) this.perlinSize * 0.10000000149011612));
    return (double) num > (double) this.minMax.x && (double) num < (double) this.minMax.y;
  }
}
