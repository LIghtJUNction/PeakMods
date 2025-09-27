// Decompiled with JetBrains decompiler
// Type: Bodypart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[DefaultExecutionOrder(-1)]
public class Bodypart : MonoBehaviour
{
  private Character character;
  public BodypartType partType;
  public Bodypart.FrictionType frictionType;
  private Rigidbody rig;
  internal Bodypart jointParent;
  private Quaternion startLocal;
  private Vector3 localCenterOfMass;
  private ConfigurableJoint joint;
  private Quaternion targetRotation;
  private Quaternion lastTargetRotation;
  private Vector3 targetForward;
  private Vector3 targetUp;
  private Vector3 targetOffsetRelativeToHip;
  internal List<RigCreatorCollider> colliders = new List<RigCreatorCollider>();
  public Vector3 forcesToAdd;
  private Vector3 prevPos;
  private Quaternion prevRot;
  public Action<Collision> collisionEnterAction;
  public Action<Collision> collisionStayAction;

  public Rigidbody Rig
  {
    get
    {
      if ((UnityEngine.Object) this.rig == (UnityEngine.Object) null)
        this.rig = this.GetComponent<Rigidbody>();
      return this.rig;
    }
  }

  private void Awake()
  {
    this.startLocal = this.transform.localRotation;
    this.prevPos = this.transform.position;
    this.prevRot = this.transform.rotation;
    this.rig = this.GetComponent<Rigidbody>();
  }

  private void Start()
  {
    this.character = this.GetComponentInParent<Character>();
    this.joint = this.GetComponent<ConfigurableJoint>();
    if ((bool) (UnityEngine.Object) this.joint)
      this.jointParent = this.joint.connectedBody.GetComponent<Bodypart>();
    if ((bool) (UnityEngine.Object) this.rig)
      this.rig.maxAngularVelocity = 50f;
    this.localCenterOfMass = HelperFunctions.GetCenterOfMass(this.transform);
  }

  internal void RegisterCollider(RigCreatorCollider rigCreatorCollider)
  {
    this.colliders.Add(rigCreatorCollider);
  }

  internal void InitBodypart(BodypartType setPartType) => this.partType = setPartType;

  private Vector3 WorldCenterOfMass() => this.transform.position;

  public void SaveAnimationData()
  {
    if ((UnityEngine.Object) this != (UnityEngine.Object) this.character.refs.hip)
      this.targetOffsetRelativeToHip = this.WorldCenterOfMass() - this.character.refs.hip.transform.position;
    this.targetRotation = this.transform.localRotation;
    this.targetForward = this.transform.forward;
    this.targetUp = this.transform.up;
  }

  public void ResetTransform()
  {
    this.transform.rotation = this.rig.rotation;
    this.transform.position = this.rig.position;
  }

  internal void Animate(float force, float torque)
  {
    if (this.rig.isKinematic)
    {
      this.SnapToAnim();
    }
    else
    {
      this.FollowRotation_Joint();
      this.FollowRotation_Rotation(torque);
      this.FollowRotation_Position(force);
    }
  }

  public void SnapToAnim()
  {
    this.transform.position += this.WorldTargetPos() - this.WorldCenterOfMass();
    this.transform.rotation = Quaternion.LookRotation(this.targetForward, this.targetUp);
    if (this.rig.isKinematic)
      return;
    this.rig.linearVelocity *= 0.0f;
    this.rig.angularVelocity *= 0.0f;
  }

  private void DrawDebug()
  {
  }

  private void FollowRotation_Joint()
  {
    if (!(bool) (UnityEngine.Object) this.joint)
      return;
    this.joint.SetTargetRotationLocal(this.targetRotation, this.startLocal);
  }

  private void FollowRotation_Rotation(float torque)
  {
    if (this.rig.isKinematic)
      return;
    this.rig.AddTorque((Vector3.Cross(this.transform.forward, this.targetForward).normalized * Vector3.Angle(this.transform.forward, this.targetForward) + Vector3.Cross(this.transform.up, this.targetUp).normalized * Vector3.Angle(this.transform.up, this.targetUp)) * torque, ForceMode.Acceleration);
  }

  private void FollowRotation_Position(float force)
  {
    if (!(bool) (UnityEngine.Object) this.character || (UnityEngine.Object) this == (UnityEngine.Object) this.character.refs.hip || this.targetOffsetRelativeToHip == Vector3.zero)
      return;
    Vector3 force1 = (this.WorldTargetPos() - this.WorldCenterOfMass()) * force;
    this.AddForce(force1, ForceMode.Acceleration);
    if (!(bool) (UnityEngine.Object) this.jointParent)
      return;
    Vector3 vector3 = force1 * this.rig.mass;
    this.jointParent.AddForce(-vector3 * 0.5f, ForceMode.Force);
    this.character.refs.hip.AddForce(-vector3 * 0.5f, ForceMode.Force);
  }

  private Vector3 WorldTargetPos()
  {
    return this.character.refs.hip.transform.position + this.targetOffsetRelativeToHip;
  }

  internal void Drag(float drag, bool ignoreRagdoll = false)
  {
    if (!ignoreRagdoll)
      drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
    if (this.rig.isKinematic)
      return;
    this.rig.linearVelocity *= drag;
    this.rig.angularVelocity *= drag;
  }

  internal void ParasolDrag(float drag, float xzDrag, bool ignoreRagdoll = false)
  {
    if (!ignoreRagdoll)
      drag = Mathf.Lerp(1f, drag, this.character.data.currentRagdollControll);
    if (this.rig.isKinematic || (double) this.rig.linearVelocity.y >= 0.0)
      return;
    this.rig.linearVelocity = new Vector3(this.rig.linearVelocity.x * xzDrag, this.rig.linearVelocity.y * drag, this.rig.linearVelocity.z * xzDrag);
  }

  private void OnCollisionEnter(Collision collision)
  {
    if ((UnityEngine.Object) this.character == (UnityEngine.Object) null || (UnityEngine.Object) collision.collider.transform.root == (UnityEngine.Object) this.transform.root)
      return;
    Action<Collision> collisionEnterAction = this.collisionEnterAction;
    if (collisionEnterAction != null)
      collisionEnterAction(collision);
    this.character.refs.movement.OnCollision(collision, true, this);
  }

  private void OnCollisionStay(Collision collision)
  {
    if (!(bool) (UnityEngine.Object) this.character || (UnityEngine.Object) collision.collider.transform.root == (UnityEngine.Object) this.transform.root)
      return;
    Action<Collision> collisionStayAction = this.collisionStayAction;
    if (collisionStayAction != null)
      collisionStayAction(collision);
    this.character.refs.movement.OnCollision(collision, false, this);
  }

  internal void Gravity(Vector3 gravity) => this.AddForce(gravity, ForceMode.Acceleration);

  public void AddForce(Vector3 force, ForceMode forceMode)
  {
    if (this.rig.isKinematic)
      return;
    if (forceMode == ForceMode.Acceleration)
      force *= this.rig.mass;
    this.forcesToAdd += force;
  }

  internal void ToggleUseGravity(bool useGrav)
  {
    if (this.rig.useGravity == useGrav)
      return;
    this.rig.useGravity = useGrav;
  }

  internal void ApplyForces()
  {
    this.rig.AddForce(this.forcesToAdd, ForceMode.Force);
    this.forcesToAdd *= 0.0f;
  }

  internal void AddMovementForce(float movementForce)
  {
    if (!(bool) (UnityEngine.Object) this.character)
      return;
    Vector3 movementInputLerp = this.character.data.worldMovementInput_Lerp;
    this.AddForce(movementForce * movementInputLerp, ForceMode.Acceleration);
  }

  internal void SetPhysicsMaterial(
    Bodypart.FrictionType setFrictionType,
    PhysicsMaterial slipperyMat,
    PhysicsMaterial normalMat)
  {
    foreach (RigCreatorCollider collider in this.colliders)
    {
      if (this.frictionType == Bodypart.FrictionType.Grippy)
        collider.col.sharedMaterial = normalMat;
      else if (this.frictionType == Bodypart.FrictionType.Slippery)
        collider.col.sharedMaterial = slipperyMat;
      else if (setFrictionType == Bodypart.FrictionType.Grippy)
        collider.col.sharedMaterial = normalMat;
      else
        collider.col.sharedMaterial = slipperyMat;
    }
  }

  public enum FrictionType
  {
    Unspecified,
    Grippy,
    Slippery,
  }
}
