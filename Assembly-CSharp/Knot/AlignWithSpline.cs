// Decompiled with JetBrains decompiler
// Type: Knot.AlignWithSpline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
namespace Knot;

public class AlignWithSpline : MonoBehaviour
{
  public SplineContainer splineContainer;
  public float knotProgress;
  public float minKnotPointDistance = 1f / 1000f;
  public float lastKnotPointProgress;
  private AlignWithSpline.TiedKnot tiedKnot = new AlignWithSpline.TiedKnot();
  public TiedKnotVisualizer tiedKnotVisualizer;
  public float knotProgressRange = 0.025f;
  public Vector2 knotProgressRangeRelation = new Vector2(-2f, 1f);
  public float test = -0.3f;

  public float KnotStepSize => this.knotProgressRange * 2f;

  public void DistanceToSpline(Vector3 position, out float closest, out float atSplineProgress)
  {
    position = position.xyo();
    int num1 = 200;
    float num2 = 1f / (float) num1;
    closest = float.MaxValue;
    atSplineProgress = 0.0f;
    for (int index = 0; index < num1; ++index)
    {
      float t = num2 * (float) index;
      Vector3 vector3 = this.splineContainer.Spline.EvaluatePosition<Spline>(t).PToV3().xyo() - position;
      if ((double) vector3.magnitude < (double) closest)
      {
        closest = vector3.magnitude;
        atSplineProgress = t;
      }
    }
  }

  public Vector2 KnotProgressRangeRelation
  {
    get => this.knotProgressRangeRelation * this.knotProgressRange;
  }

  private void EvaluateKnot(AlignWithSpline.TiedKnot tiedKnot)
  {
    float templateProgress = tiedKnot.knotPoints[0].templateProgress;
    Vector2 progressRangeRelation = this.KnotProgressRangeRelation;
    progressRangeRelation.x += this.knotProgress;
    progressRangeRelation.y += this.knotProgress;
    Vector2 vector2 = progressRangeRelation;
    vector2.x += this.KnotStepSize;
    vector2.y += this.KnotStepSize;
    if ((double) templateProgress > (double) progressRangeRelation.y && (double) templateProgress > (double) vector2.y)
      Debug.LogError((object) "");
    double x = (double) progressRangeRelation.x;
  }

  private void TieRope2()
  {
    Plane plane = new Plane(Camera.main.transform.forward, this.splineContainer.transform.position);
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    float enter;
    if (!plane.Raycast(ray, out enter))
      return;
    Vector3 me = ray.direction * enter + ray.origin;
    this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint()
    {
      position = me.xyo(),
      templateProgress = this.lastKnotPointProgress,
      inside = false
    });
  }

  private void TieRope()
  {
    Plane plane = new Plane(Camera.main.transform.forward, this.splineContainer.transform.position);
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit[] source1 = Physics.RaycastAll(ray);
    if (source1.Length != 0)
    {
      IOrderedEnumerable<RaycastHit> source2 = ((IEnumerable<RaycastHit>) source1).OrderBy<RaycastHit, float>((Func<RaycastHit, float>) (h => Mathf.Abs(h.textureCoord.x - this.lastKnotPointProgress)));
      foreach (RaycastHit raycastHit in (IEnumerable<RaycastHit>) source2)
        Debug.Log((object) $"{Time.frameCount} Hit: {raycastHit.textureCoord.x}");
      RaycastHit raycastHit1 = source2.First<RaycastHit>();
      if (this.tiedKnot.knotPoints.Count > 0)
      {
        List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = this.tiedKnot.knotPoints;
        if ((double) Vector3.Distance(knotPoints[knotPoints.Count - 1].position, raycastHit1.point) < (double) this.minKnotPointDistance)
          return;
      }
      this.lastKnotPointProgress = raycastHit1.textureCoord.x;
      this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint()
      {
        position = raycastHit1.point.xyo(),
        templateProgress = this.lastKnotPointProgress,
        inside = true
      });
      Debug.Log((object) $"Added: {raycastHit1.textureCoord.x}");
    }
    else
    {
      float enter;
      if (!plane.Raycast(ray, out enter))
        return;
      Vector3 me = ray.direction * enter + ray.origin;
      this.tiedKnot.knotPoints.Add(new AlignWithSpline.TiedKnot.KnotPoint()
      {
        position = me.xyo(),
        templateProgress = this.lastKnotPointProgress,
        inside = false
      });
    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Mouse0))
      this.tiedKnot = new AlignWithSpline.TiedKnot();
    else if (Input.GetKey(KeyCode.Mouse0))
      this.TieRope();
    Input.GetKeyUp(KeyCode.Mouse0);
  }

  private void FixedUpdate()
  {
  }

  public class TiedKnot
  {
    public List<AlignWithSpline.TiedKnot.KnotPoint> knotPoints = new List<AlignWithSpline.TiedKnot.KnotPoint>();

    public class KnotPoint
    {
      public Vector3 position;
      public float templateProgress;
      public bool inside;
    }
  }
}
