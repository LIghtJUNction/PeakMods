// Decompiled with JetBrains decompiler
// Type: Knot.KnotTemplate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
namespace Knot;

[ExecuteInEditMode]
public class KnotTemplate : MonoBehaviour, ISerializationCallbackReceiver
{
  public float widthMul = 0.1f;
  public float minWidth = 1f / 1000f;
  public LineRenderer lr;
  public SplineContainer splineContainer;
  public Transform colliderRoot;
  private Mesh lineMesh;
  private float counter;
  [NonSerialized]
  private bool registered;
  public bool editorRefresh;
  private float timeToRefresh;

  private void Awake()
  {
    if (!Application.isPlaying)
      return;
    this.SplineToLineRenderer();
    this.LineRendererToMeshColliders();
  }

  private void Update()
  {
  }

  public void OnBeforeSerialize()
  {
  }

  public void OnAfterDeserialize() => this.Register();

  public void SplineToLineRenderer()
  {
    if ((UnityEngine.Object) this.splineContainer == (UnityEngine.Object) null)
      return;
    List<Vector3> vector3List = new List<Vector3>();
    float num1 = 1f / (float) this.lr.positionCount;
    List<Keyframe> keyframeList = new List<Keyframe>();
    this.lr.transform.localPosition = Vector3.zero;
    this.lr.transform.localRotation = Quaternion.identity;
    for (int index = 0; index < this.lr.positionCount; ++index)
    {
      float num2 = num1 * (float) index;
      float3 position = this.splineContainer.Spline.EvaluatePosition<Spline>(num2);
      float num3 = position.z + this.splineContainer.transform.localPosition.z;
      float num4 = num3 * num3 * this.splineContainer.transform.TransformVector(Vector3.one).magnitude;
      float3 me = (float3) this.lr.transform.InverseTransformPoint((Vector3) (float3) this.splineContainer.transform.TransformPoint(position.PToV3()));
      vector3List.Add(me.PToV3().xyn((float) (-(double) num2 * 0.10000000149011612)));
      keyframeList.Add(new Keyframe(num2, Mathf.Max(this.minWidth, num4 * this.widthMul)));
    }
    this.lr.widthCurve = new AnimationCurve()
    {
      keys = keyframeList.ToArray()
    };
    this.lr.SetPositions(vector3List.ToArray());
  }

  private void LineRendererToMeshColliders()
  {
    Debug.Log((object) nameof (LineRendererToMeshColliders));
    this.lineMesh = new Mesh();
    this.lr.BakeMesh(this.lineMesh, Camera.main, true);
    this.colliderRoot.KillAllChildren(true);
    for (int index1 = 0; index1 < Mathf.FloorToInt((float) this.lineMesh.triangles.Length / 3f) / 2 && index1 <= this.lineMesh.triangles.Length; ++index1)
    {
      GameObject gameObject = new GameObject($"{index1}");
      List<int> intList = new List<int>();
      List<Vector3> vector3List = new List<Vector3>();
      gameObject.transform.parent = this.colliderRoot;
      gameObject.transform.localPosition = 0.ToVec();
      for (int index2 = 0; index2 < 2; ++index2)
      {
        int num = index1 * 2 + index2;
        for (int index3 = 0; index3 < 3; ++index3)
        {
          Vector3 vertex = this.lineMesh.vertices[this.lineMesh.triangles[num * 3 + index3]];
          vector3List.Add(vertex);
          intList.Add(vector3List.Count - 1);
        }
      }
      Mesh me = new Mesh();
      me.vertices = vector3List.ToArray();
      me.triangles = intList.ToArray();
      me.RecalculateAll();
      gameObject.AddComponent<MeshCollider>().sharedMesh = me;
    }
  }

  private void Register()
  {
    if (this.registered)
      return;
    this.registered = true;
  }
}
