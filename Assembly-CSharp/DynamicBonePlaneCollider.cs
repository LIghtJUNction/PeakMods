// Decompiled with JetBrains decompiler
// Type: DynamicBonePlaneCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Dynamic Bone/Dynamic Bone Plane Collider")]
public class DynamicBonePlaneCollider : DynamicBoneColliderBase
{
  private void OnValidate()
  {
  }

  public override bool Collide(ref Vector3 particlePosition, float particleRadius)
  {
    Vector3 inNormal = Vector3.up;
    switch (this.m_Direction)
    {
      case DynamicBoneColliderBase.Direction.X:
        inNormal = this.transform.right;
        break;
      case DynamicBoneColliderBase.Direction.Y:
        inNormal = this.transform.up;
        break;
      case DynamicBoneColliderBase.Direction.Z:
        inNormal = this.transform.forward;
        break;
    }
    Vector3 inPoint = this.transform.TransformPoint(this.m_Center);
    float distanceToPoint = new Plane(inNormal, inPoint).GetDistanceToPoint(particlePosition);
    if (this.m_Bound == DynamicBoneColliderBase.Bound.Outside)
    {
      if ((double) distanceToPoint < 0.0)
      {
        particlePosition -= inNormal * distanceToPoint;
        return true;
      }
    }
    else if ((double) distanceToPoint > 0.0)
    {
      particlePosition -= inNormal * distanceToPoint;
      return true;
    }
    return false;
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.enabled)
      return;
    Gizmos.color = this.m_Bound != DynamicBoneColliderBase.Bound.Outside ? Color.magenta : Color.yellow;
    Vector3 vector3 = Vector3.up;
    switch (this.m_Direction)
    {
      case DynamicBoneColliderBase.Direction.X:
        vector3 = this.transform.right;
        break;
      case DynamicBoneColliderBase.Direction.Y:
        vector3 = this.transform.up;
        break;
      case DynamicBoneColliderBase.Direction.Z:
        vector3 = this.transform.forward;
        break;
    }
    Vector3 from = this.transform.TransformPoint(this.m_Center);
    Gizmos.DrawLine(from, from + vector3);
  }
}
