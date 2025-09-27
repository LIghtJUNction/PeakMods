// Decompiled with JetBrains decompiler
// Type: RopeClimbingAPI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#nullable disable
public class RopeClimbingAPI : MonoBehaviour
{
  private Rope rope;
  private PhotonView photonView;

  private void Awake()
  {
    this.rope = this.GetComponent<Rope>();
    this.photonView = this.GetComponentInParent<PhotonView>();
  }

  public float GetMove() => (float) (-1.0 * (1.0 / (double) this.rope.GetTotalLength()));

  public float GetPercentFromSegmentIndex(int segmentIndex)
  {
    return (float) segmentIndex / ((float) this.rope.SegmentCount - 1f);
  }

  public float GetAngleAtPercent(float percent)
  {
    Transform segmentFromPercent = this.GetSegmentFromPercent(percent);
    Debug.DrawLine(segmentFromPercent.transform.position, segmentFromPercent.transform.position + segmentFromPercent.up, Color.red);
    return segmentFromPercent.GetComponent<RopeSegment>().GetAngle();
  }

  public Matrix4x4 GetSegmentMatrixFromPercent(float percent)
  {
    int index = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float) (this.rope.SegmentCount - 1), percent));
    Transform ropeSegment = this.rope.GetRopeSegments()[index];
    return Matrix4x4.TRS(ropeSegment.position, ropeSegment.rotation, Vector3.one);
  }

  public Vector3 GetUp(float ropePercent)
  {
    Transform segmentFromPercent = this.GetSegmentFromPercent(ropePercent);
    Vector3 up = segmentFromPercent.up;
    if ((double) Vector3.Angle(Vector3.up, segmentFromPercent.up) > 90.0)
      up *= -1f;
    return up;
  }

  public float UpMult(float percent)
  {
    return (double) Vector3.Angle(Vector3.up, this.GetSegmentFromPercent(percent).up) < 90.0 ? -1f : 1f;
  }

  public Vector3 GetPosition(float percent)
  {
    percent = Mathf.Clamp01(percent);
    double f = (double) percent * (double) (this.rope.SegmentCount - 1);
    int valueToClamp1 = Mathf.FloorToInt((float) f);
    int valueToClamp2 = valueToClamp1;
    if (valueToClamp1 == 0)
      valueToClamp1 = 1;
    if ((double) percent < 1.0)
      valueToClamp2 = valueToClamp1 + 1;
    float t = (float) f - (float) valueToClamp1;
    List<Transform> ropeSegments = this.rope.GetRopeSegments();
    int num = math.clamp(valueToClamp1, 0, ropeSegments.Count - 1);
    int index = math.clamp(valueToClamp2, num, ropeSegments.Count - 1);
    return Vector3.Lerp(ropeSegments[num].position, ropeSegments[index].position, t);
  }

  public Transform GetSegmentFromPercent(float percent)
  {
    percent = Mathf.Clamp01(percent);
    double f = (double) percent * (double) (this.rope.SegmentCount - 1);
    int valueToClamp1 = Mathf.FloorToInt((float) f);
    int valueToClamp2 = valueToClamp1;
    if (valueToClamp1 == 0)
      valueToClamp1 = 1;
    if ((double) percent < 1.0)
      valueToClamp2 = valueToClamp1 + 1;
    float num1 = (float) f - (float) valueToClamp1;
    List<Transform> ropeSegments = this.rope.GetRopeSegments();
    int lowerBound = math.clamp(valueToClamp1, 0, ropeSegments.Count - 1);
    int num2 = math.clamp(valueToClamp2, lowerBound, ropeSegments.Count - 1);
    return ropeSegments[(double) num1 > 0.5 ? num2 : lowerBound];
  }
}
