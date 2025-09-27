// Decompiled with JetBrains decompiler
// Type: RopeBoneVisualizer
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
using Zorro.Core;

#nullable disable
public class RopeBoneVisualizer : MonoBehaviour
{
  public Material ghostMaterial;
  public Material ropeMaterial;
  public GameObject boneRoot;
  public List<Transform> bones;
  public float segmentMod = 1f;
  public bool withRotOfStartPos;
  private readonly List<RopeSyncData.SegmentData> remoteRenderingPoints = new List<RopeSyncData.SegmentData>();
  private SkinnedMeshRenderer meshRenderer;
  private Rope rope;
  private float sinceLastPackage;
  [NonSerialized]
  private List<RopeSyncData.SegmentData> targetPoints = new List<RopeSyncData.SegmentData>();
  private PhotonView view;

  public Transform StartTransform { get; set; }

  public Optionable<bool> ManuallyUpdateNextFrame { get; set; }

  private void Awake()
  {
    this.view = this.GetComponentInParent<PhotonView>();
    this.rope = this.GetComponent<Rope>();
    this.bones = this.boneRoot.PGetComponentsInChildrenButNotMe<Transform>().ToList<Transform>();
    this.bones.Reverse();
    this.meshRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
    this.CheckVisible();
    this.ghostMaterial = UnityEngine.Object.Instantiate<Material>(this.ghostMaterial);
    this.ropeMaterial = UnityEngine.Object.Instantiate<Material>(this.ropeMaterial);
  }

  private void OnDestroy()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.ropeMaterial);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.ghostMaterial);
  }

  private void LateUpdate()
  {
    this.CheckVisible();
    if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
    {
      Debug.LogError((object) "Target points count mismatch");
    }
    else
    {
      float num1 = 1f / (float) PhotonNetwork.SerializationRate;
      this.sinceLastPackage += Time.deltaTime;
      Optionable<bool> manuallyUpdateNextFrame = this.ManuallyUpdateNextFrame;
      if (manuallyUpdateNextFrame.IsSome)
      {
        manuallyUpdateNextFrame = this.ManuallyUpdateNextFrame;
        if (!manuallyUpdateNextFrame.Value)
          return;
        this.ManuallyUpdateNextFrame = Optionable<bool>.Some(false);
      }
      float num2 = this.sinceLastPackage / num1;
      RopeSyncData.SegmentData segmentData1;
      for (int index1 = 0; index1 < this.remoteRenderingPoints.Count; ++index1)
      {
        Vector3 vector3 = Vector3.Lerp((Vector3) this.remoteRenderingPoints[index1].position, (Vector3) this.targetPoints[index1].position, num2 * 0.5f);
        Quaternion quaternion = Quaternion.Lerp(this.remoteRenderingPoints[index1].rotation, this.targetPoints[index1].rotation, num2 * 0.5f);
        List<RopeSyncData.SegmentData> remoteRenderingPoints = this.remoteRenderingPoints;
        int index2 = index1;
        segmentData1 = new RopeSyncData.SegmentData();
        segmentData1.position = (float3) vector3;
        segmentData1.rotation = quaternion;
        RopeSyncData.SegmentData segmentData2 = segmentData1;
        remoteRenderingPoints[index2] = segmentData2;
      }
      List<RopeSyncData.SegmentData> segmentDataList1 = this.view.IsMine ? this.rope.GetRopeSegments().Select<Transform, RopeSyncData.SegmentData>((Func<Transform, RopeSyncData.SegmentData>) (transform1 => new RopeSyncData.SegmentData()
      {
        position = (float3) transform1.position,
        rotation = transform1.rotation
      })).ToList<RopeSyncData.SegmentData>() : this.remoteRenderingPoints;
      List<RopeSyncData.SegmentData> positions = new List<RopeSyncData.SegmentData>();
      if ((UnityEngine.Object) this.StartTransform != (UnityEngine.Object) null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
      {
        List<RopeSyncData.SegmentData> segmentDataList2 = positions;
        segmentData1 = new RopeSyncData.SegmentData();
        segmentData1.position = (float3) this.StartTransform.position;
        segmentData1.rotation = this.StartTransform.rotation;
        RopeSyncData.SegmentData segmentData3 = segmentData1;
        segmentDataList2.Add(segmentData3);
      }
      if (segmentDataList1.Count > 0)
        positions.Add(segmentDataList1[0]);
      for (int index = segmentDataList1.Count - 1; index >= 1; --index)
        positions.Add(segmentDataList1[index]);
      positions.Reverse();
      this.meshRenderer.sharedMaterial.SetFloat("_RopeCutoff", (float) (((double) Mathf.Floor((float) positions.Count) - (double) this.segmentMod) * (1.0 / (double) (this.bones.Count - 1))));
      if (positions.Count == 0)
        return;
      if (this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
        RenderInSpool();
      else
        RenderInNotSpool();
      RenderInSpool();

      void RenderInNotSpool()
      {
        int num = 0;
        for (int index = 0; index < this.bones.Count; ++index)
        {
          Transform bone = this.bones[index];
          if (index > positions.Count - 1 && index > 0)
          {
            bone.gameObject.name = num.ToString();
            if (num == 0 && (UnityEngine.Object) this.StartTransform != (UnityEngine.Object) null)
            {
              bone.position = this.StartTransform.position;
              Vector3 vector3 = bone.position - this.bones[index - 1].position;
              bone.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector3);
              ++num;
            }
            else
            {
              bone.position = this.bones[index - 1].position;
              bone.rotation = this.bones[index - 1].rotation;
              ++num;
              bone.localScale = Vector3.zero;
            }
          }
          else
          {
            bone.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -positions[index].rotation.GetUp());
            bone.localScale = 1f.xxx();
            bone.position = positions[index].position.PToV3();
            bone.gameObject.name = index.ToString();
          }
        }
      }

      void RenderInSpool()
      {
        int num = 0;
        for (int index = 0; index < this.bones.Count; ++index)
        {
          Transform bone = this.bones[index];
          if (index > positions.Count - 3 && index > 0)
          {
            bone.gameObject.name = num.ToString();
            if (num == 0 && (UnityEngine.Object) this.StartTransform != (UnityEngine.Object) null)
            {
              bone.position = this.StartTransform.position;
              if (this.withRotOfStartPos)
              {
                bone.rotation = this.StartTransform.rotation;
              }
              else
              {
                Vector3 vector3 = bone.position - this.bones[index - 1].position;
                bone.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector3);
              }
              ++num;
            }
            else
            {
              bone.position = this.bones[index - 1].position;
              bone.rotation = this.bones[index - 1].rotation;
              ++num;
              bone.localScale = Vector3.zero;
            }
          }
          else
          {
            bone.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -positions[index].rotation.GetUp());
            bone.localScale = 1f.xxx();
            bone.position = positions[index].position.PToV3();
            bone.gameObject.name = index.ToString();
          }
        }
      }
    }
  }

  public void OnDrawGizmosSelected()
  {
    foreach (Transform bone in this.bones)
    {
      DrawArrow.ForGizmo(bone.position, bone.up, Color.green, 0.25f);
      DrawArrow.ForGizmo(bone.position, bone.forward, Color.blue, 0.25f);
      DrawArrow.ForGizmo(bone.position, bone.right, Color.red, 0.25f);
    }
  }

  private void CheckVisible()
  {
    this.meshRenderer.sharedMaterial = this.rope.attachmenState == Rope.ATTACHMENT.inSpool ? this.ghostMaterial : this.ropeMaterial;
  }

  public void SetData(RopeSyncData data)
  {
    if (this.rope.creatorLeft)
      return;
    this.sinceLastPackage = 0.0f;
    this.targetPoints = ((IEnumerable<RopeSyncData.SegmentData>) data.segments).ToList<RopeSyncData.SegmentData>();
    int length = data.segments.Length;
    int count = this.remoteRenderingPoints.Count;
    if (length < count)
    {
      int num = count - length;
      for (int index = 0; index < num; ++index)
        this.remoteRenderingPoints.RemoveLast<RopeSyncData.SegmentData>();
    }
    else
    {
      if (length <= count)
        return;
      int num = length - count;
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = count + index1;
        this.remoteRenderingPoints.Add(data.segments[index2]);
      }
    }
  }
}
