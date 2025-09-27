// Decompiled with JetBrains decompiler
// Type: BakedVolumeLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class BakedVolumeLight : MonoBehaviour
{
  public BakedVolumeLight.LightModes mode;
  public Color color = Color.white;
  public float intensity = 1f;
  public float radius = 10f;
  [Range(0.0f, 1f)]
  public float falloff = 0.5f;
  [Range(0.0f, 1f)]
  [Tooltip("Percentage width at which the light should be full brightness. 1.0 means the entire cone is full bright, 0.0 means that the fade lerp starts immediately in the center")]
  public float coneFalloff = 0.9f;
  [Range(0.0f, 90f)]
  public float coneSize = 30f;
  [Range(0.0f, 1f)]
  public float scaleWithLossyScale;

  public float GetRadius()
  {
    if ((double) this.scaleWithLossyScale <= 0.0)
      return this.radius;
    return Mathf.Lerp(this.radius, this.radius * Mathf.Max(this.transform.lossyScale.x, this.transform.lossyScale.y, this.transform.lossyScale.z), this.scaleWithLossyScale);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = this.color;
    float radius = this.GetRadius();
    switch (this.mode)
    {
      case BakedVolumeLight.LightModes.Point:
        Gizmos.DrawWireSphere(this.transform.position, radius);
        break;
      case BakedVolumeLight.LightModes.Spot:
        Vector3 to1 = this.transform.position + this.transform.forward * radius;
        Gizmos.DrawLine(this.transform.position, to1);
        float num = this.coneSize * ((float) Math.PI / 90f);
        Vector3[] vector3Array = new Vector3[4]
        {
          to1 + this.transform.up * num * radius,
          to1 + this.transform.right * num * radius,
          to1 + -this.transform.up * num * radius,
          to1 + -this.transform.right * num * radius
        };
        foreach (Vector3 to2 in vector3Array)
          Gizmos.DrawLine(this.transform.position, to2);
        Gizmos.DrawLineStrip(ReadOnlySpan<Vector3>.op_Implicit(vector3Array), true);
        break;
    }
  }

  public void Rebake() => UnityEngine.Object.FindAnyObjectByType<LightVolume>().Bake();

  public enum LightModes
  {
    Point,
    Spot,
  }
}
