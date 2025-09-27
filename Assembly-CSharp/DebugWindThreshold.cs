// Decompiled with JetBrains decompiler
// Type: DebugWindThreshold
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class DebugWindThreshold : MonoBehaviour
{
  [Range(0.0f, 1f)]
  public float lowerThreshold;
  [Range(0.0f, 1f)]
  public float thresholdMargin;
  public Collider zone;
  public float nodeSpacing = 5f;
  public const float MIN_NODE_SPACING = 2f;
  public List<DebugWindThreshold.WindNode> nodes = new List<DebugWindThreshold.WindNode>();
  public Vector3 min;
  public Vector3 max;

  public void GenerateMap()
  {
    this.ClearMap();
    this.min = this.zone.bounds.min;
    this.max = this.zone.bounds.max;
    Vector3 min = this.min;
    while ((double) min.z < (double) this.max.z)
    {
      while ((double) min.y < (double) this.max.y)
      {
        for (; (double) min.x < (double) this.max.x; min.x += this.nodeSpacing)
          this.nodes.Add(new DebugWindThreshold.WindNode(min));
        min.y += this.nodeSpacing;
        min.x = this.min.x;
      }
      min.z += this.nodeSpacing;
      min.y = this.min.y;
      min.x = this.min.x;
    }
  }

  public void ClearMap() => this.nodes.Clear();

  private void OnDrawGizmosSelected()
  {
    for (int index = 0; index < this.nodes.Count; ++index)
    {
      float amt = (double) this.nodes[index].wind <= (double) this.lowerThreshold + (double) this.thresholdMargin ? ((double) this.nodes[index].wind >= (double) this.lowerThreshold ? Util.RangeLerp(0.0f, 1f, this.lowerThreshold, this.lowerThreshold + this.thresholdMargin, this.nodes[index].wind) : 0.0f) : 1f;
      this.nodes[index].DrawGizmo_HeatMap(amt);
    }
  }

  [Serializable]
  public class WindNode
  {
    public float wind;
    public Vector3 position;

    public WindNode(Vector3 position)
    {
      this.position = position;
      this.wind = LightVolume.Instance().SamplePositionAlpha(position);
    }

    public void DrawGizmo_HeatMap(float amt)
    {
      Gizmos.color = ((double) amt != 1.0 ? ((double) amt != 0.0 ? Color.Lerp(Color.yellow, Color.red, amt) : Color.green) : Color.red) with
      {
        a = 0.5f
      };
      Gizmos.DrawSphere(this.position, 1f);
    }
  }
}
