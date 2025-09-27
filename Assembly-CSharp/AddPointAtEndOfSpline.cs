// Decompiled with JetBrains decompiler
// Type: AddPointAtEndOfSpline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
public class AddPointAtEndOfSpline : MonoBehaviour
{
  public void SetAllZ(float v)
  {
    SplineContainer component = this.GetComponent<SplineContainer>();
    List<BezierKnot> list = component.Spline.Knots.ToList<BezierKnot>();
    for (int index = 0; index < list.Count; ++index)
    {
      BezierKnot bezierKnot = list[index];
      bezierKnot.Position = bezierKnot.Position.xyn(v);
      list[index] = bezierKnot;
    }
    component.Spline.Knots = (IEnumerable<BezierKnot>) list;
  }

  private void GO()
  {
    SplineContainer component = this.GetComponent<SplineContainer>();
    BezierKnot bezierKnot1 = component.Spline.Knots.Last<BezierKnot>();
    List<BezierKnot> list = component.Spline.Knots.ToList<BezierKnot>();
    BezierKnot bezierKnot2 = list[list.Count - 2];
    component.Spline.Add((float3) (bezierKnot1.Position.PToV3() + (bezierKnot1.Position.PToV3() - bezierKnot2.Position.PToV3()).normalized));
    PExt.SaveObj((Object) component);
  }

  private void Start()
  {
  }

  private void Update()
  {
  }
}
