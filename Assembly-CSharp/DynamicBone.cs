﻿// Decompiled with JetBrains decompiler
// Type: DynamicBone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
  [Tooltip("The root of the transform hierarchy to apply physics.")]
  public Transform m_Root;
  [Tooltip("Internal physics simulation rate.")]
  public float m_UpdateRate = 60f;
  public DynamicBone.UpdateMode m_UpdateMode = DynamicBone.UpdateMode.Default;
  [Tooltip("How much the bones slowed down.")]
  [Range(0.0f, 1f)]
  public float m_Damping = 0.1f;
  public AnimationCurve m_DampingDistrib;
  [Tooltip("How much the force applied to return each bone to original orientation.")]
  [Range(0.0f, 1f)]
  public float m_Elasticity = 0.1f;
  public AnimationCurve m_ElasticityDistrib;
  [Tooltip("How much bone's original orientation are preserved.")]
  [Range(0.0f, 1f)]
  public float m_Stiffness = 0.1f;
  public AnimationCurve m_StiffnessDistrib;
  [Tooltip("How much character's position change is ignored in physics simulation.")]
  [Range(0.0f, 1f)]
  public float m_Inert;
  public AnimationCurve m_InertDistrib;
  [Tooltip("How much the bones slowed down when collide.")]
  public float m_Friction;
  public AnimationCurve m_FrictionDistrib;
  [Tooltip("Each bone can be a sphere to collide with colliders. Radius describe sphere's size.")]
  public float m_Radius;
  public AnimationCurve m_RadiusDistrib;
  [Tooltip("If End Length is not zero, an extra bone is generated at the end of transform hierarchy.")]
  public float m_EndLength;
  [Tooltip("If End Offset is not zero, an extra bone is generated at the end of transform hierarchy.")]
  public Vector3 m_EndOffset = Vector3.zero;
  [Tooltip("The force apply to bones. Partial force apply to character's initial pose is cancelled out.")]
  public Vector3 m_Gravity = Vector3.zero;
  [Tooltip("The force apply to bones.")]
  public Vector3 m_Force = Vector3.zero;
  [Tooltip("Collider objects interact with the bones.")]
  public List<DynamicBoneColliderBase> m_Colliders;
  [Tooltip("Bones exclude from physics simulation.")]
  public List<Transform> m_Exclusions;
  [Tooltip("Constrain bones to move on specified plane.")]
  public DynamicBone.FreezeAxis m_FreezeAxis;
  [Tooltip("Disable physics simulation automatically if character is far from camera or player.")]
  public bool m_DistantDisable;
  public Transform m_ReferenceObject;
  public float m_DistanceToObject = 20f;
  private Vector3 m_LocalGravity = Vector3.zero;
  private Vector3 m_ObjectMove = Vector3.zero;
  private Vector3 m_ObjectPrevPosition = Vector3.zero;
  private float m_BoneTotalLength;
  private float m_ObjectScale = 1f;
  private float m_Time;
  private float m_Weight = 1f;
  private bool m_DistantDisabled;
  private List<DynamicBone.Particle> m_Particles = new List<DynamicBone.Particle>();

  private void Start()
  {
    if (!(bool) (Object) this.m_Root)
      this.m_Root = this.transform;
    this.SetupParticles();
  }

  private void FixedUpdate()
  {
    if (this.m_UpdateMode != DynamicBone.UpdateMode.AnimatePhysics)
      return;
    this.PreUpdate();
  }

  private void Update()
  {
    if (this.m_UpdateMode == DynamicBone.UpdateMode.AnimatePhysics)
      return;
    this.PreUpdate();
  }

  private void LateUpdate()
  {
    if (this.m_DistantDisable)
      this.CheckDistance();
    if ((double) this.m_Weight <= 0.0 || this.m_DistantDisable && this.m_DistantDisabled)
      return;
    this.UpdateDynamicBones(this.m_UpdateMode == DynamicBone.UpdateMode.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
  }

  private void PreUpdate()
  {
    if ((double) this.m_Weight <= 0.0 || this.m_DistantDisable && this.m_DistantDisabled)
      return;
    this.InitTransforms();
  }

  private void CheckDistance()
  {
    Transform transform = this.m_ReferenceObject;
    if ((Object) transform == (Object) null && (Object) Camera.main != (Object) null)
      transform = Camera.main.transform;
    if (!((Object) transform != (Object) null))
      return;
    bool flag = (double) (transform.position - this.transform.position).sqrMagnitude > (double) this.m_DistanceToObject * (double) this.m_DistanceToObject;
    if (flag == this.m_DistantDisabled)
      return;
    if (!flag)
      this.ResetParticlesPosition();
    this.m_DistantDisabled = flag;
  }

  private void OnEnable() => this.ResetParticlesPosition();

  private void OnDisable() => this.InitTransforms();

  private void OnValidate()
  {
    this.m_UpdateRate = Mathf.Max(this.m_UpdateRate, 0.0f);
    this.m_Damping = Mathf.Clamp01(this.m_Damping);
    this.m_Elasticity = Mathf.Clamp01(this.m_Elasticity);
    this.m_Stiffness = Mathf.Clamp01(this.m_Stiffness);
    this.m_Inert = Mathf.Clamp01(this.m_Inert);
    this.m_Friction = Mathf.Clamp01(this.m_Friction);
    this.m_Radius = Mathf.Max(this.m_Radius, 0.0f);
    if (!Application.isEditor || !Application.isPlaying)
      return;
    this.InitTransforms();
    this.SetupParticles();
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.enabled || (Object) this.m_Root == (Object) null)
      return;
    if (Application.isEditor && !Application.isPlaying && this.transform.hasChanged)
    {
      this.InitTransforms();
      this.SetupParticles();
    }
    Gizmos.color = Color.white;
    for (int index = 0; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle1 = this.m_Particles[index];
      if (particle1.m_ParentIndex >= 0)
      {
        DynamicBone.Particle particle2 = this.m_Particles[particle1.m_ParentIndex];
        Gizmos.DrawLine(particle1.m_Position, particle2.m_Position);
      }
      if ((double) particle1.m_Radius > 0.0)
        Gizmos.DrawWireSphere(particle1.m_Position, particle1.m_Radius * this.m_ObjectScale);
    }
  }

  public void SetWeight(float w)
  {
    if ((double) this.m_Weight == (double) w)
      return;
    if ((double) w == 0.0)
      this.InitTransforms();
    else if ((double) this.m_Weight == 0.0)
      this.ResetParticlesPosition();
    this.m_Weight = w;
  }

  public float GetWeight() => this.m_Weight;

  private void UpdateDynamicBones(float t)
  {
    if ((Object) this.m_Root == (Object) null)
      return;
    this.m_ObjectScale = Mathf.Abs(this.transform.lossyScale.x);
    this.m_ObjectMove = this.transform.position - this.m_ObjectPrevPosition;
    this.m_ObjectPrevPosition = this.transform.position;
    int num1 = 1;
    float timeVar = 1f;
    if (this.m_UpdateMode == DynamicBone.UpdateMode.Default)
      timeVar = (double) this.m_UpdateRate <= 0.0 ? Time.deltaTime : Time.deltaTime * this.m_UpdateRate;
    else if ((double) this.m_UpdateRate > 0.0)
    {
      float num2 = 1f / this.m_UpdateRate;
      this.m_Time += t;
      num1 = 0;
      while ((double) this.m_Time >= (double) num2)
      {
        this.m_Time -= num2;
        if (++num1 >= 3)
        {
          this.m_Time = 0.0f;
          break;
        }
      }
    }
    if (num1 > 0)
    {
      for (int index = 0; index < num1; ++index)
      {
        this.UpdateParticles1(timeVar);
        this.UpdateParticles2(timeVar);
        this.m_ObjectMove = Vector3.zero;
      }
    }
    else
      this.SkipUpdateParticles();
    this.ApplyParticlesToTransforms();
  }

  public void SetupParticles()
  {
    this.m_Particles.Clear();
    if ((Object) this.m_Root == (Object) null)
      return;
    this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
    this.m_ObjectScale = Mathf.Abs(this.transform.lossyScale.x);
    this.m_ObjectPrevPosition = this.transform.position;
    this.m_ObjectMove = Vector3.zero;
    this.m_BoneTotalLength = 0.0f;
    this.AppendParticles(this.m_Root, -1, 0.0f);
    this.UpdateParameters();
  }

  private void AppendParticles(Transform b, int parentIndex, float boneLength)
  {
    DynamicBone.Particle particle = new DynamicBone.Particle();
    particle.m_Transform = b;
    particle.m_ParentIndex = parentIndex;
    if ((Object) b != (Object) null)
    {
      particle.m_Position = particle.m_PrevPosition = b.position;
      particle.m_InitLocalPosition = b.localPosition;
      particle.m_InitLocalRotation = b.localRotation;
    }
    else
    {
      Transform transform = this.m_Particles[parentIndex].m_Transform;
      if ((double) this.m_EndLength > 0.0)
      {
        Transform parent = transform.parent;
        particle.m_EndOffset = !((Object) parent != (Object) null) ? new Vector3(this.m_EndLength, 0.0f, 0.0f) : transform.InverseTransformPoint(transform.position * 2f - parent.position) * this.m_EndLength;
      }
      else
        particle.m_EndOffset = transform.InverseTransformPoint(this.transform.TransformDirection(this.m_EndOffset) + transform.position);
      particle.m_Position = particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset);
    }
    if (parentIndex >= 0)
    {
      boneLength += (this.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
      particle.m_BoneLength = boneLength;
      this.m_BoneTotalLength = Mathf.Max(this.m_BoneTotalLength, boneLength);
    }
    int count = this.m_Particles.Count;
    this.m_Particles.Add(particle);
    if (!((Object) b != (Object) null))
      return;
    for (int index = 0; index < b.childCount; ++index)
    {
      Transform child = b.GetChild(index);
      bool flag = false;
      if (this.m_Exclusions != null)
        flag = this.m_Exclusions.Contains(child);
      if (!flag)
        this.AppendParticles(child, count, boneLength);
      else if ((double) this.m_EndLength > 0.0 || this.m_EndOffset != Vector3.zero)
        this.AppendParticles((Transform) null, count, boneLength);
    }
    if (b.childCount != 0 || (double) this.m_EndLength <= 0.0 && !(this.m_EndOffset != Vector3.zero))
      return;
    this.AppendParticles((Transform) null, count, boneLength);
  }

  public void UpdateParameters()
  {
    if ((Object) this.m_Root == (Object) null)
      return;
    this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
    for (int index = 0; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle = this.m_Particles[index];
      particle.m_Damping = this.m_Damping;
      particle.m_Elasticity = this.m_Elasticity;
      particle.m_Stiffness = this.m_Stiffness;
      particle.m_Inert = this.m_Inert;
      particle.m_Friction = this.m_Friction;
      particle.m_Radius = this.m_Radius;
      if ((double) this.m_BoneTotalLength > 0.0)
      {
        float time = particle.m_BoneLength / this.m_BoneTotalLength;
        if (this.m_DampingDistrib != null && this.m_DampingDistrib.keys.Length != 0)
          particle.m_Damping *= this.m_DampingDistrib.Evaluate(time);
        if (this.m_ElasticityDistrib != null && this.m_ElasticityDistrib.keys.Length != 0)
          particle.m_Elasticity *= this.m_ElasticityDistrib.Evaluate(time);
        if (this.m_StiffnessDistrib != null && this.m_StiffnessDistrib.keys.Length != 0)
          particle.m_Stiffness *= this.m_StiffnessDistrib.Evaluate(time);
        if (this.m_InertDistrib != null && this.m_InertDistrib.keys.Length != 0)
          particle.m_Inert *= this.m_InertDistrib.Evaluate(time);
        if (this.m_FrictionDistrib != null && this.m_FrictionDistrib.keys.Length != 0)
          particle.m_Friction *= this.m_FrictionDistrib.Evaluate(time);
        if (this.m_RadiusDistrib != null && this.m_RadiusDistrib.keys.Length != 0)
          particle.m_Radius *= this.m_RadiusDistrib.Evaluate(time);
      }
      particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
      particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
      particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
      particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
      particle.m_Friction = Mathf.Clamp01(particle.m_Friction);
      particle.m_Radius = Mathf.Max(particle.m_Radius, 0.0f);
    }
  }

  private void InitTransforms()
  {
    for (int index = 0; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle = this.m_Particles[index];
      if ((Object) particle.m_Transform != (Object) null)
      {
        particle.m_Transform.localPosition = particle.m_InitLocalPosition;
        particle.m_Transform.localRotation = particle.m_InitLocalRotation;
      }
    }
  }

  private void ResetParticlesPosition()
  {
    for (int index = 0; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle = this.m_Particles[index];
      if ((Object) particle.m_Transform != (Object) null)
      {
        particle.m_Position = particle.m_PrevPosition = particle.m_Transform.position;
      }
      else
      {
        Transform transform = this.m_Particles[particle.m_ParentIndex].m_Transform;
        particle.m_Position = particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset);
      }
      particle.m_isCollide = false;
    }
    this.m_ObjectPrevPosition = this.transform.position;
  }

  private void UpdateParticles1(float timeVar)
  {
    Vector3 gravity = this.m_Gravity;
    Vector3 normalized = this.m_Gravity.normalized;
    Vector3 lhs = this.m_Root.TransformDirection(this.m_LocalGravity);
    Vector3 vector3_1 = normalized * Mathf.Max(Vector3.Dot(lhs, normalized), 0.0f);
    Vector3 vector3_2 = (gravity - vector3_1 + this.m_Force) * (this.m_ObjectScale * timeVar);
    for (int index = 0; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle = this.m_Particles[index];
      if (particle.m_ParentIndex >= 0)
      {
        Vector3 vector3_3 = particle.m_Position - particle.m_PrevPosition;
        Vector3 vector3_4 = this.m_ObjectMove * particle.m_Inert;
        particle.m_PrevPosition = particle.m_Position + vector3_4;
        float num = particle.m_Damping;
        if (particle.m_isCollide)
        {
          num += particle.m_Friction;
          if ((double) num > 1.0)
            num = 1f;
          particle.m_isCollide = false;
        }
        particle.m_Position += vector3_3 * (1f - num) + vector3_2 + vector3_4;
      }
      else
      {
        particle.m_PrevPosition = particle.m_Position;
        particle.m_Position = particle.m_Transform.position;
      }
    }
  }

  private void UpdateParticles2(float timeVar)
  {
    Plane plane = new Plane();
    for (int index1 = 1; index1 < this.m_Particles.Count; ++index1)
    {
      DynamicBone.Particle particle1 = this.m_Particles[index1];
      DynamicBone.Particle particle2 = this.m_Particles[particle1.m_ParentIndex];
      Vector3 vector3_1;
      float magnitude1;
      if ((Object) particle1.m_Transform != (Object) null)
      {
        vector3_1 = particle2.m_Transform.position - particle1.m_Transform.position;
        magnitude1 = vector3_1.magnitude;
      }
      else
      {
        vector3_1 = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle1.m_EndOffset);
        magnitude1 = vector3_1.magnitude;
      }
      float num1 = Mathf.Lerp(1f, particle1.m_Stiffness, this.m_Weight);
      if ((double) num1 > 0.0 || (double) particle1.m_Elasticity > 0.0)
      {
        Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
        localToWorldMatrix.SetColumn(3, (Vector4) particle2.m_Position);
        Vector3 vector3_2 = !((Object) particle1.m_Transform != (Object) null) ? localToWorldMatrix.MultiplyPoint3x4(particle1.m_EndOffset) : localToWorldMatrix.MultiplyPoint3x4(particle1.m_Transform.localPosition);
        Vector3 vector3_3 = vector3_2 - particle1.m_Position;
        particle1.m_Position += vector3_3 * (particle1.m_Elasticity * timeVar);
        if ((double) num1 > 0.0)
        {
          Vector3 vector3_4 = vector3_2 - particle1.m_Position;
          float magnitude2 = vector3_4.magnitude;
          float num2 = (float) ((double) magnitude1 * (1.0 - (double) num1) * 2.0);
          if ((double) magnitude2 > (double) num2)
            particle1.m_Position += vector3_4 * ((magnitude2 - num2) / magnitude2);
        }
      }
      if (this.m_Colliders != null)
      {
        float particleRadius = particle1.m_Radius * this.m_ObjectScale;
        for (int index2 = 0; index2 < this.m_Colliders.Count; ++index2)
        {
          DynamicBoneColliderBase collider = this.m_Colliders[index2];
          if ((Object) collider != (Object) null && collider.enabled)
            particle1.m_isCollide |= collider.Collide(ref particle1.m_Position, particleRadius);
        }
      }
      if (this.m_FreezeAxis != DynamicBone.FreezeAxis.None)
      {
        switch (this.m_FreezeAxis)
        {
          case DynamicBone.FreezeAxis.X:
            plane.SetNormalAndPosition(particle2.m_Transform.right, particle2.m_Position);
            break;
          case DynamicBone.FreezeAxis.Y:
            plane.SetNormalAndPosition(particle2.m_Transform.up, particle2.m_Position);
            break;
          case DynamicBone.FreezeAxis.Z:
            plane.SetNormalAndPosition(particle2.m_Transform.forward, particle2.m_Position);
            break;
        }
        particle1.m_Position -= plane.normal * plane.GetDistanceToPoint(particle1.m_Position);
      }
      Vector3 vector3_5 = particle2.m_Position - particle1.m_Position;
      float magnitude3 = vector3_5.magnitude;
      if ((double) magnitude3 > 0.0)
        particle1.m_Position += vector3_5 * ((magnitude3 - magnitude1) / magnitude3);
    }
  }

  private void SkipUpdateParticles()
  {
    for (int index = 0; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle1 = this.m_Particles[index];
      if (particle1.m_ParentIndex >= 0)
      {
        particle1.m_PrevPosition += this.m_ObjectMove;
        particle1.m_Position += this.m_ObjectMove;
        DynamicBone.Particle particle2 = this.m_Particles[particle1.m_ParentIndex];
        float num1 = !((Object) particle1.m_Transform != (Object) null) ? particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle1.m_EndOffset).magnitude : (particle2.m_Transform.position - particle1.m_Transform.position).magnitude;
        float num2 = Mathf.Lerp(1f, particle1.m_Stiffness, this.m_Weight);
        if ((double) num2 > 0.0)
        {
          Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
          localToWorldMatrix.SetColumn(3, (Vector4) particle2.m_Position);
          Vector3 vector3 = (!((Object) particle1.m_Transform != (Object) null) ? localToWorldMatrix.MultiplyPoint3x4(particle1.m_EndOffset) : localToWorldMatrix.MultiplyPoint3x4(particle1.m_Transform.localPosition)) - particle1.m_Position;
          float magnitude = vector3.magnitude;
          float num3 = (float) ((double) num1 * (1.0 - (double) num2) * 2.0);
          if ((double) magnitude > (double) num3)
            particle1.m_Position += vector3 * ((magnitude - num3) / magnitude);
        }
        Vector3 vector3_1 = particle2.m_Position - particle1.m_Position;
        float magnitude1 = vector3_1.magnitude;
        if ((double) magnitude1 > 0.0)
          particle1.m_Position += vector3_1 * ((magnitude1 - num1) / magnitude1);
      }
      else
      {
        particle1.m_PrevPosition = particle1.m_Position;
        particle1.m_Position = particle1.m_Transform.position;
      }
    }
  }

  private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
  {
    return v - axis * (Vector3.Dot(v, axis) * 2f);
  }

  private void ApplyParticlesToTransforms()
  {
    for (int index = 1; index < this.m_Particles.Count; ++index)
    {
      DynamicBone.Particle particle1 = this.m_Particles[index];
      DynamicBone.Particle particle2 = this.m_Particles[particle1.m_ParentIndex];
      if (particle2.m_Transform.childCount <= 1)
      {
        Vector3 direction = !((Object) particle1.m_Transform != (Object) null) ? particle1.m_EndOffset : particle1.m_Transform.localPosition;
        Vector3 toDirection = particle1.m_Position - particle2.m_Position;
        Quaternion rotation = Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(direction), toDirection);
        particle2.m_Transform.rotation = rotation * particle2.m_Transform.rotation;
      }
      if ((Object) particle1.m_Transform != (Object) null)
        particle1.m_Transform.position = particle1.m_Position;
    }
  }

  public enum UpdateMode
  {
    Normal,
    AnimatePhysics,
    UnscaledTime,
    Default,
  }

  public enum FreezeAxis
  {
    None,
    X,
    Y,
    Z,
  }

  private class Particle
  {
    public Transform m_Transform;
    public int m_ParentIndex = -1;
    public float m_Damping;
    public float m_Elasticity;
    public float m_Stiffness;
    public float m_Inert;
    public float m_Friction;
    public float m_Radius;
    public float m_BoneLength;
    public bool m_isCollide;
    public Vector3 m_Position = Vector3.zero;
    public Vector3 m_PrevPosition = Vector3.zero;
    public Vector3 m_EndOffset = Vector3.zero;
    public Vector3 m_InitLocalPosition = Vector3.zero;
    public Quaternion m_InitLocalRotation = Quaternion.identity;
  }
}
