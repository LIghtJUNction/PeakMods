// Decompiled with JetBrains decompiler
// Type: PSC_VolumeLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_VolumeLight : PropSpawnerConstraint
{
  public Vector2 minMax = new Vector2(0.0f, 0.5f);

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    Color color = LightVolume.Instance().SamplePosition(spawnData.pos);
    return (double) color.a > (double) this.minMax.x && (double) color.a < (double) this.minMax.y;
  }
}
