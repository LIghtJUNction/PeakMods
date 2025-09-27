// Decompiled with JetBrains decompiler
// Type: WaterFallConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WaterFallConfig : CustomSpawnCondition
{
  public MeshRenderer mesh;
  public Transform endRock;
  public Transform rayStart;
  public Transform rayEnd;

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    RaycastHit raycastHit = HelperFunctions.LineCheck(this.rayStart.position, this.rayEnd.position, HelperFunctions.LayerType.TerrainMap);
    if ((bool) (Object) raycastHit.transform)
    {
      this.endRock.transform.position = raycastHit.point;
      MaterialPropertyBlock properties = new MaterialPropertyBlock();
      this.mesh.GetPropertyBlock(properties);
      properties.SetFloat("_WorldPositionY", raycastHit.point.y);
      this.mesh.SetPropertyBlock(properties);
    }
    return true;
  }
}
