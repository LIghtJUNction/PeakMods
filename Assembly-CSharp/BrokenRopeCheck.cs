// Decompiled with JetBrains decompiler
// Type: BrokenRopeCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using UnityEngine;

#nullable disable
public class BrokenRopeCheck : CustomSpawnCondition
{
  [SerializeField]
  private RopeAnchor anchor;
  [SerializeField]
  private RopeAnchorWithRope ropeAnchorWithRope;
  private PropSpawner.SpawnData lastData;
  public float estimatedMaxRopeLength = 17f;

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    this.lastData = data;
    float num1 = this.estimatedMaxRopeLength / 40f;
    if (data == null)
      data = this.lastData;
    this.lastData = data;
    this.transform.rotation = ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, data.hit.normal);
    Debug.Log((object) $"Anchor {this.anchor}");
    if ((Object) this.anchor.anchorPoint == (Object) null)
      this.anchor.anchorPoint = this.anchor.transform.Find("AnchorPoint");
    Debug.Log((object) $"anchorPoint {this.anchor.anchorPoint}");
    RaycastHit hit;
    bool flag = new Ray(this.anchor.anchorPoint.transform.position, Vector3.down).Raycast(out hit, HelperFunctions.LayerType.TerrainMap.ToLayerMask(), 10f);
    float num2 = Vector3.Distance(hit.point, this.transform.position);
    this.ropeAnchorWithRope.ropeSegmentLength = (double) num2 > (double) num1 * 39.0 || !flag ? 39f : (float) ((double) num2 / (double) num1 - 1.0);
    return true;
  }
}
