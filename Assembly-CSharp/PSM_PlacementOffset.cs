// Decompiled with JetBrains decompiler
// Type: PSM_PlacementOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSM_PlacementOffset : PropSpawnerMod
{
  public float xMult;
  public float yMult = 1f;
  public float zMult;
  public Vector2 minHeight;
  public Vector2 maxHeight;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    float num1 = Mathf.Lerp(this.minHeight.x, this.maxHeight.x, spawnData.placement.x);
    float num2 = Mathf.Lerp(this.minHeight.y, this.maxHeight.y, spawnData.placement.y);
    spawned.transform.position += Vector3.right * (num1 + num2) * this.xMult;
    spawned.transform.position += Vector3.up * (num1 + num2) * this.yMult;
    spawned.transform.position += Vector3.forward * (num1 + num2) * this.zMult;
  }
}
