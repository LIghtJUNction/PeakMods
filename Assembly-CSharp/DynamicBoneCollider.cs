// Decompiled with JetBrains decompiler
// Type: DynamicBoneCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Dynamic Bone/Dynamic Bone Collider")]
public class DynamicBoneCollider : DynamicBoneColliderBase
{
  [Tooltip("The radius of the sphere or capsule.")]
  public float m_Radius = 0.5f;
  [Tooltip("The height of the capsule.")]
  public float m_Height;

  private void OnValidate()
  {
    this.m_Radius = Mathf.Max(this.m_Radius, 0.0f);
    this.m_Height = Mathf.Max(this.m_Height, 0.0f);
  }

  public override bool Collide(ref Vector3 particlePosition, float particleRadius)
  {
    float num1 = this.m_Radius * Mathf.Abs(this.transform.lossyScale.x);
    float num2 = this.m_Height * 0.5f - this.m_Radius;
    if ((double) num2 <= 0.0)
      return this.m_Bound == DynamicBoneColliderBase.Bound.Outside ? DynamicBoneCollider.OutsideSphere(ref particlePosition, particleRadius, this.transform.TransformPoint(this.m_Center), num1) : DynamicBoneCollider.InsideSphere(ref particlePosition, particleRadius, this.transform.TransformPoint(this.m_Center), num1);
    Vector3 center1 = this.m_Center;
    Vector3 center2 = this.m_Center;
    switch (this.m_Direction)
    {
      case DynamicBoneColliderBase.Direction.X:
        center1.x -= num2;
        center2.x += num2;
        break;
      case DynamicBoneColliderBase.Direction.Y:
        center1.y -= num2;
        center2.y += num2;
        break;
      case DynamicBoneColliderBase.Direction.Z:
        center1.z -= num2;
        center2.z += num2;
        break;
    }
    return this.m_Bound == DynamicBoneColliderBase.Bound.Outside ? DynamicBoneCollider.OutsideCapsule(ref particlePosition, particleRadius, this.transform.TransformPoint(center1), this.transform.TransformPoint(center2), num1) : DynamicBoneCollider.InsideCapsule(ref particlePosition, particleRadius, this.transform.TransformPoint(center1), this.transform.TransformPoint(center2), num1);
  }

  private static bool OutsideSphere(
    ref Vector3 particlePosition,
    float particleRadius,
    Vector3 sphereCenter,
    float sphereRadius)
  {
    float num1 = sphereRadius + particleRadius;
    float num2 = num1 * num1;
    Vector3 vector3 = particlePosition - sphereCenter;
    float sqrMagnitude = vector3.sqrMagnitude;
    if ((double) sqrMagnitude <= 0.0 || (double) sqrMagnitude >= (double) num2)
      return false;
    float num3 = Mathf.Sqrt(sqrMagnitude);
    particlePosition = sphereCenter + vector3 * (num1 / num3);
    return true;
  }

  private static bool InsideSphere(
    ref Vector3 particlePosition,
    float particleRadius,
    Vector3 sphereCenter,
    float sphereRadius)
  {
    float num1 = sphereRadius - particleRadius;
    float num2 = num1 * num1;
    Vector3 vector3 = particlePosition - sphereCenter;
    float sqrMagnitude = vector3.sqrMagnitude;
    if ((double) sqrMagnitude <= (double) num2)
      return false;
    float num3 = Mathf.Sqrt(sqrMagnitude);
    particlePosition = sphereCenter + vector3 * (num1 / num3);
    return true;
  }

  private static bool OutsideCapsule(
    ref Vector3 particlePosition,
    float particleRadius,
    Vector3 capsuleP0,
    Vector3 capsuleP1,
    float capsuleRadius)
  {
    float num1 = capsuleRadius + particleRadius;
    float num2 = num1 * num1;
    Vector3 rhs = capsuleP1 - capsuleP0;
    Vector3 lhs = particlePosition - capsuleP0;
    float num3 = Vector3.Dot(lhs, rhs);
    if ((double) num3 <= 0.0)
    {
      float sqrMagnitude = lhs.sqrMagnitude;
      if ((double) sqrMagnitude > 0.0 && (double) sqrMagnitude < (double) num2)
      {
        float num4 = Mathf.Sqrt(sqrMagnitude);
        particlePosition = capsuleP0 + lhs * (num1 / num4);
        return true;
      }
    }
    else
    {
      float sqrMagnitude1 = rhs.sqrMagnitude;
      if ((double) num3 >= (double) sqrMagnitude1)
      {
        Vector3 vector3 = particlePosition - capsuleP1;
        float sqrMagnitude2 = vector3.sqrMagnitude;
        if ((double) sqrMagnitude2 > 0.0 && (double) sqrMagnitude2 < (double) num2)
        {
          float num5 = Mathf.Sqrt(sqrMagnitude2);
          particlePosition = capsuleP1 + vector3 * (num1 / num5);
          return true;
        }
      }
      else if ((double) sqrMagnitude1 > 0.0)
      {
        float num6 = num3 / sqrMagnitude1;
        Vector3 vector3 = lhs - rhs * num6;
        float sqrMagnitude3 = vector3.sqrMagnitude;
        if ((double) sqrMagnitude3 > 0.0 && (double) sqrMagnitude3 < (double) num2)
        {
          float num7 = Mathf.Sqrt(sqrMagnitude3);
          particlePosition += vector3 * ((num1 - num7) / num7);
          return true;
        }
      }
    }
    return false;
  }

  private static bool InsideCapsule(
    ref Vector3 particlePosition,
    float particleRadius,
    Vector3 capsuleP0,
    Vector3 capsuleP1,
    float capsuleRadius)
  {
    float num1 = capsuleRadius - particleRadius;
    float num2 = num1 * num1;
    Vector3 rhs = capsuleP1 - capsuleP0;
    Vector3 lhs = particlePosition - capsuleP0;
    float num3 = Vector3.Dot(lhs, rhs);
    if ((double) num3 <= 0.0)
    {
      float sqrMagnitude = lhs.sqrMagnitude;
      if ((double) sqrMagnitude > (double) num2)
      {
        float num4 = Mathf.Sqrt(sqrMagnitude);
        particlePosition = capsuleP0 + lhs * (num1 / num4);
        return true;
      }
    }
    else
    {
      float sqrMagnitude1 = rhs.sqrMagnitude;
      if ((double) num3 >= (double) sqrMagnitude1)
      {
        Vector3 vector3 = particlePosition - capsuleP1;
        float sqrMagnitude2 = vector3.sqrMagnitude;
        if ((double) sqrMagnitude2 > (double) num2)
        {
          float num5 = Mathf.Sqrt(sqrMagnitude2);
          particlePosition = capsuleP1 + vector3 * (num1 / num5);
          return true;
        }
      }
      else if ((double) sqrMagnitude1 > 0.0)
      {
        float num6 = num3 / sqrMagnitude1;
        Vector3 vector3 = lhs - rhs * num6;
        float sqrMagnitude3 = vector3.sqrMagnitude;
        if ((double) sqrMagnitude3 > (double) num2)
        {
          float num7 = Mathf.Sqrt(sqrMagnitude3);
          particlePosition += vector3 * ((num1 - num7) / num7);
          return true;
        }
      }
    }
    return false;
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.enabled)
      return;
    Gizmos.color = this.m_Bound != DynamicBoneColliderBase.Bound.Outside ? Color.magenta : Color.yellow;
    float radius = this.m_Radius * Mathf.Abs(this.transform.lossyScale.x);
    float num = this.m_Height * 0.5f - this.m_Radius;
    if ((double) num <= 0.0)
    {
      Gizmos.DrawWireSphere(this.transform.TransformPoint(this.m_Center), radius);
    }
    else
    {
      Vector3 center1 = this.m_Center;
      Vector3 center2 = this.m_Center;
      switch (this.m_Direction)
      {
        case DynamicBoneColliderBase.Direction.X:
          center1.x -= num;
          center2.x += num;
          break;
        case DynamicBoneColliderBase.Direction.Y:
          center1.y -= num;
          center2.y += num;
          break;
        case DynamicBoneColliderBase.Direction.Z:
          center1.z -= num;
          center2.z += num;
          break;
      }
      Gizmos.DrawWireSphere(this.transform.TransformPoint(center1), radius);
      Gizmos.DrawWireSphere(this.transform.TransformPoint(center2), radius);
    }
  }
}
