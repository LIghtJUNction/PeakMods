// Decompiled with JetBrains decompiler
// Type: ConfigurableJointExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class ConfigurableJointExtensions
{
  public static void SetTargetRotationLocal(
    this ConfigurableJoint joint,
    Quaternion targetLocalRotation,
    Quaternion startLocalRotation)
  {
    if (joint.configuredInWorldSpace)
      Debug.LogError((object) "SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", (Object) joint);
    ConfigurableJointExtensions.SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
  }

  public static void SetTargetRotation(
    this ConfigurableJoint joint,
    Quaternion targetWorldRotation,
    Quaternion startWorldRotation)
  {
    if (!joint.configuredInWorldSpace)
      Debug.LogError((object) "SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", (Object) joint);
    ConfigurableJointExtensions.SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
  }

  private static void SetTargetRotationInternal(
    ConfigurableJoint joint,
    Quaternion targetRotation,
    Quaternion startRotation,
    Space space)
  {
    Vector3 axis = joint.axis;
    Vector3 vector3 = Vector3.Cross(joint.axis, joint.secondaryAxis);
    Vector3 normalized = vector3.normalized;
    vector3 = Vector3.Cross(normalized, axis);
    Quaternion rotation = Quaternion.LookRotation(normalized, vector3.normalized);
    Quaternion quaternion1 = Quaternion.Inverse(rotation);
    Quaternion quaternion2 = (space != Space.World ? quaternion1 * (Quaternion.Inverse(targetRotation) * startRotation) : quaternion1 * (startRotation * Quaternion.Inverse(targetRotation))) * rotation;
    joint.targetRotation = quaternion2;
  }
}
