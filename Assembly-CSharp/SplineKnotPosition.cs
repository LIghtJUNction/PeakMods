// Decompiled with JetBrains decompiler
// Type: SplineKnotPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
public class SplineKnotPosition : MonoBehaviour
{
  public SplineContainer splineContainer;
  public int knotIndex;
  public float f;

  private void Start()
  {
    if ((UnityEngine.Object) this.splineContainer == (UnityEngine.Object) null || this.splineContainer.Splines.Count == 0)
    {
      Debug.LogError((object) "SplineContainer is missing or empty.");
    }
    else
    {
      Spline spline = this.splineContainer.Splines[0];
      Debug.Log((object) $"Knot {this.knotIndex} is at {(ValueType) (float) ((double) SplineUtility.GetNormalizedInterpolation<Spline>(this.splineContainer.Spline, this.f, PathIndexUnit.Knot) * 100.0)}% along the spline.");
    }
  }
}
