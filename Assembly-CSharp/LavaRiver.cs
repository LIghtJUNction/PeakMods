// Decompiled with JetBrains decompiler
// Type: LavaRiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
public class LavaRiver : CustomSpawnCondition
{
  public float spawnVel = 5f;
  public float gravity = 10f;
  public float wallStick;
  public float drag = 0.8f;
  public float timeStep = 0.02f;
  public int maxSteps = 1000;
  public float maxLength = 500f;
  public bool spawnLights = true;
  private int steps;
  public float prefDistancePerFrame = 0.3f;
  public GameObject endRock;
  public GameObject splash;
  public GameObject spawnAlongSpline;
  private Transform splineObjectParent;
  public List<LavaRiver.LavaRiverFrame> frames = new List<LavaRiver.LavaRiverFrame>();

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    this.Spawn();
    return true;
  }

  private void OnDrawGizmosSelected()
  {
    for (int index = 0; index < this.frames.Count; ++index)
    {
      Gizmos.color = Color.Lerp(Color.blue, Color.red, (float) index / (float) this.frames.Count);
      Gizmos.DrawSphere(this.frames[index].position, 0.1f);
      Gizmos.DrawLine(this.frames[index].position, this.frames[index].position + this.frames[index].up * 0.5f);
    }
  }

  public void Spawn()
  {
    this.GenerateData();
    this.Apply();
    this.AddLights();
    if (!((UnityEngine.Object) this.spawnAlongSpline != (UnityEngine.Object) null))
      return;
    this.SpawnItemsAlongSpline();
  }

  private void AddLights()
  {
    Transform transform = this.transform.Find("BakedLight");
    if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      return;
    GameObject gameObject = transform.gameObject;
    Transform parent = this.transform.Find("BakedLights");
    for (int index = parent.childCount - 1; index >= 0; --index)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) parent.GetChild(index).gameObject);
    if (!this.spawnLights)
      return;
    for (int index = 0; index < this.frames.Count; index += 3)
      UnityEngine.Object.Instantiate<GameObject>(gameObject, this.frames[index].position + this.frames[index].up * 4f, Quaternion.identity, parent).SetActive(true);
  }

  private void SpawnItemsAlongSpline()
  {
    this.splineObjectParent = this.transform.Find("SpawnedObjects");
    if ((UnityEngine.Object) this.splineObjectParent == (UnityEngine.Object) null)
    {
      this.splineObjectParent = new GameObject("SpawnedObjects").transform;
      this.splineObjectParent.SetParent(this.transform);
    }
    for (int index = this.splineObjectParent.childCount - 1; index >= 0; --index)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.splineObjectParent.GetChild(index).gameObject);
    for (int index = 0; index < this.frames.Count; ++index)
      UnityEngine.Object.Instantiate<GameObject>(this.spawnAlongSpline, this.frames[index].position, Quaternion.LookRotation(this.frames[index].forward), this.splineObjectParent).SetActive(true);
  }

  private void GenerateData()
  {
    this.Simulate();
    this.Simplify();
    this.SmoothUps();
    this.SmoothUps();
    this.SmoothUps();
    this.SmoothUps();
    this.SmoothUps();
  }

  public void Apply()
  {
    SplineContainer componentInChildren = this.GetComponentInChildren<SplineContainer>();
    componentInChildren.transform.position = Vector3.zero;
    componentInChildren.transform.rotation = Quaternion.identity;
    for (int splineIndex = componentInChildren.Splines.Count - 1; splineIndex >= 0; --splineIndex)
      componentInChildren.RemoveSplineAt<SplineContainer>(splineIndex);
    Spline spline = new Spline();
    foreach (LavaRiver.LavaRiverFrame frame in this.frames)
    {
      Quaternion rotationWithUp = HelperFunctions.GetRotationWithUp(frame.forward, frame.up);
      spline.Add(new BezierKnot((float3) frame.position, (float3) Vector3.zero, (float3) Vector3.zero, (quaternion) rotationWithUp), TangentMode.AutoSmooth);
    }
    for (int index = 0; index < spline.Count; ++index)
      spline.SetKnot(index, new BezierKnot(spline[index].Position, spline[index].TangentIn, spline[index].TangentOut, (quaternion) Quaternion.Euler(Vector3.left)));
    componentInChildren.AddSpline<SplineContainer>(spline);
    SplineExtrude component = componentInChildren.GetComponent<SplineExtrude>();
    component.GetComponent<MeshFilter>().mesh = new Mesh();
    component.Capped = true;
    component.Rebuild();
    if ((UnityEngine.Object) this.splash != (UnityEngine.Object) null)
      this.splash.transform.position = this.frames[this.frames.Count - 1].position;
    if (!((UnityEngine.Object) this.endRock != (UnityEngine.Object) null))
      return;
    this.endRock.transform.position = this.frames[this.frames.Count - 1].position;
    this.endRock.transform.rotation = UnityEngine.Random.rotation;
  }

  private void Simulate()
  {
    this.frames = new List<LavaRiver.LavaRiverFrame>();
    this.steps = this.maxSteps;
    Vector3 vel = this.transform.forward * this.spawnVel;
    Vector3 pos = this.transform.position + this.transform.up * 0.1f + this.transform.forward * 0.1f;
    Vector3 up = this.transform.up;
    Vector3 lastPos = pos;
    do
      ;
    while (SimulationStep());

    bool SimulationStep()
    {
      --this.steps;
      if (this.steps < 0 || (double) vel.magnitude < 0.0099999997764825821 || (double) Vector3.Distance(this.transform.position, pos) > (double) this.maxLength)
        return false;
      vel += Vector3.down * this.gravity * this.timeStep;
      vel += up * -this.wallStick * this.timeStep;
      vel *= this.drag;
      Vector3 to = pos + vel * this.timeStep;
      RaycastHit raycastHit = HelperFunctions.LineCheck(lastPos, to, HelperFunctions.LayerType.TerrainMap);
      if ((bool) (UnityEngine.Object) raycastHit.transform)
      {
        up = raycastHit.normal;
        to = raycastHit.point + raycastHit.normal * 0.05f;
        vel = Vector3.ProjectOnPlane(vel, raycastHit.normal);
      }
      pos = to;
      this.frames.Add(new LavaRiver.LavaRiverFrame()
      {
        position = pos,
        up = up,
        forward = vel.normalized
      });
      lastPos = pos;
      return true;
    }
  }

  public void SmoothUps()
  {
    for (int index = 1; index < this.frames.Count - 1; ++index)
    {
      LavaRiver.LavaRiverFrame frame1 = this.frames[index - 1];
      LavaRiver.LavaRiverFrame frame2 = this.frames[index];
      LavaRiver.LavaRiverFrame frame3 = this.frames[index + 1];
      Vector3 normalized = (frame1.up + frame2.up + frame3.up).normalized;
      frame2.up = normalized;
    }
  }

  public void Simplify()
  {
    for (int index = 1; index < this.frames.Count; ++index)
    {
      if ((double) Vector3.Distance(this.frames[index - 1].position, this.frames[index].position) < (double) this.prefDistancePerFrame)
      {
        this.frames.RemoveAt(index);
        --index;
      }
    }
  }

  public void Clear()
  {
    this.frames.Clear();
    SplineContainer componentInChildren = this.GetComponentInChildren<SplineContainer>();
    componentInChildren.transform.position = Vector3.zero;
    componentInChildren.transform.rotation = Quaternion.identity;
    for (int splineIndex = componentInChildren.Splines.Count - 1; splineIndex >= 0; --splineIndex)
      componentInChildren.RemoveSplineAt<SplineContainer>(splineIndex);
    componentInChildren.GetComponent<SplineExtrude>().Rebuild();
    this.endRock.transform.position = this.transform.position;
    if ((UnityEngine.Object) this.splash != (UnityEngine.Object) null)
      this.splash.transform.position = this.transform.position;
    if (!((UnityEngine.Object) this.splineObjectParent != (UnityEngine.Object) null))
      return;
    for (int index = this.splineObjectParent.childCount - 1; index >= 0; --index)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.splineObjectParent.GetChild(index).gameObject);
  }

  [Serializable]
  public class LavaRiverFrame
  {
    public Vector3 position;
    public Vector3 up;
    public Vector3 forward;
  }
}
