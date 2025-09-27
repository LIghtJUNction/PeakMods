// Decompiled with JetBrains decompiler
// Type: RopeRendering
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

#nullable disable
public class RopeRendering : MonoBehaviour
{
  public Transform startPos;
  public LineRenderer lineRenderer;
  private Rope rope;
  private PhotonView view;
  private List<float3> remoteRenderingPoints = new List<float3>();
  private List<float3> targetPoints = new List<float3>();
  private float sinceLastPackage;
  public bool hide = true;

  private void Awake()
  {
    this.view = this.GetComponentInParent<PhotonView>();
    this.rope = this.GetComponent<Rope>();
    this.lineRenderer = this.GetComponentInChildren<LineRenderer>();
  }

  private void LateUpdate()
  {
    if (this.hide)
      return;
    if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
    {
      Debug.LogError((object) "Target points count mismatch");
    }
    else
    {
      float num1 = 1f / (float) PhotonNetwork.SerializationRate;
      this.sinceLastPackage += Time.deltaTime;
      float num2 = this.sinceLastPackage / num1;
      for (int index = 0; index < this.remoteRenderingPoints.Count; ++index)
      {
        Vector3 vector3 = Vector3.Lerp((Vector3) this.remoteRenderingPoints[index], (Vector3) this.targetPoints[index], num2 * 0.5f);
        this.remoteRenderingPoints[index] = (float3) vector3;
      }
      List<float3> float3List = this.view.IsMine ? this.rope.GetRopeSegments().Select<Transform, float3>((Func<Transform, float3>) (transform1 => (float3) transform1.position)).ToList<float3>() : this.remoteRenderingPoints;
      List<Vector3> vector3List = new List<Vector3>();
      if ((UnityEngine.Object) this.startPos != (UnityEngine.Object) null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
        vector3List.Add(this.startPos.position);
      if (float3List.Count > 0)
        vector3List.Add((Vector3) float3List[0]);
      for (int index = float3List.Count - 1; index >= 1; --index)
        vector3List.Add((Vector3) float3List[index]);
      this.lineRenderer.positionCount = vector3List.Count;
      this.lineRenderer.SetPositions(vector3List.ToArray());
      if (this.rope.IsActive() && vector3List.Count > 1)
        this.lineRenderer.enabled = true;
      else
        this.lineRenderer.enabled = false;
    }
  }

  public void SetData(RopeSyncData data)
  {
    this.sinceLastPackage = 0.0f;
    this.targetPoints = ((IEnumerable<RopeSyncData.SegmentData>) data.segments).Select<RopeSyncData.SegmentData, float3>((Func<RopeSyncData.SegmentData, float3>) (segmentData => segmentData.position)).ToList<float3>();
    int length = data.segments.Length;
    int count = this.remoteRenderingPoints.Count;
    if (length < count)
    {
      int num = count - length;
      for (int index = 0; index < num; ++index)
        this.remoteRenderingPoints.RemoveLast<float3>();
    }
    else
    {
      if (length <= count)
        return;
      int num = length - count;
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = count + index1;
        this.remoteRenderingPoints.Add(data.segments[index2].position);
      }
    }
  }
}
