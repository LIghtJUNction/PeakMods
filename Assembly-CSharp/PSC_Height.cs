// Decompiled with JetBrains decompiler
// Type: PSC_Height
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class PSC_Height : PropSpawnerConstraint
{
  public float maxHeight = 10000f;
  public float minHeight = -10000f;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    return (double) spawnData.pos.y > (double) this.minHeight && (double) spawnData.pos.y < (double) this.maxHeight;
  }
}
