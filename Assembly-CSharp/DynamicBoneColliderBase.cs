// Decompiled with JetBrains decompiler
// Type: DynamicBoneColliderBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DynamicBoneColliderBase : MonoBehaviour
{
  [Tooltip("The axis of the capsule's height.")]
  public DynamicBoneColliderBase.Direction m_Direction = DynamicBoneColliderBase.Direction.Y;
  [Tooltip("The center of the sphere or capsule, in the object's local space.")]
  public Vector3 m_Center = Vector3.zero;
  [Tooltip("Constrain bones to outside bound or inside bound.")]
  public DynamicBoneColliderBase.Bound m_Bound;

  public virtual bool Collide(ref Vector3 particlePosition, float particleRadius) => false;

  public enum Direction
  {
    X,
    Y,
    Z,
  }

  public enum Bound
  {
    Outside,
    Inside,
  }
}
