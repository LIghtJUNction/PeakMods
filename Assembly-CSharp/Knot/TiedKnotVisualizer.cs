// Decompiled with JetBrains decompiler
// Type: Knot.TiedKnotVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
namespace Knot;

public class TiedKnotVisualizer : MonoBehaviour
{
  private LineRenderer lr;
  public int count;
  public bool splineIt;
  public List<TiedKnotVisualizer.KnotPart> knot = new List<TiedKnotVisualizer.KnotPart>();

  private void Awake() => this.lr = this.GetComponent<LineRenderer>();

  public void Refresh() => this.Visualize(this.knot);

  public void Go()
  {
    foreach (TiedKnotVisualizer.KnotPart knotPart in this.knot)
      Debug.Log((object) $"Quality: {knotPart.quality}, Position: {knotPart.position}");
  }

  public void Visualize(List<TiedKnotVisualizer.KnotPart> knot)
  {
    this.knot = knot;
    List<Vector3> list = knot.Select<TiedKnotVisualizer.KnotPart, Vector3>((Func<TiedKnotVisualizer.KnotPart, Vector3>) (knotPoint => knotPoint.position)).ToList<Vector3>();
    if (!this.splineIt)
    {
      this.lr.positionCount = list.Count;
      this.lr.SetPositions(list.ToArray());
    }
    else
    {
      Spline spline = new Spline();
      spline.Knots = (IEnumerable<BezierKnot>) list.Select<Vector3, BezierKnot>((Func<Vector3, BezierKnot>) (knotPoint => new BezierKnot((float3) knotPoint))).ToArray<BezierKnot>();
      List<Vector3> vector3List = new List<Vector3>();
      float num = 1f / (float) this.count;
      for (int index = 0; index < this.count; ++index)
      {
        float t = num * (float) index;
        float3 position = spline.EvaluatePosition<Spline>(t);
        vector3List.Add(position.PToV3());
      }
      this.lr.positionCount = this.count;
      this.lr.SetPositions(vector3List.ToArray());
    }
  }

  private void Start()
  {
  }

  private void Update()
  {
  }

  public void Clear()
  {
    this.knot.Clear();
    this.Refresh();
  }

  public struct KnotPart(bool quality, Vector3 position, int part)
  {
    public bool quality = quality;
    public Vector3 position = position;
    public int part = part;
  }
}
