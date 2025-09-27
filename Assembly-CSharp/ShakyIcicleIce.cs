// Decompiled with JetBrains decompiler
// Type: ShakyIcicleIce
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class ShakyIcicleIce : MonoBehaviour
{
  public float fallTime = 5f;
  public float amount = 1f;
  public float shakeScale = 15f;
  public Transform mesh;
  public float shakeAmount = 10f;
  public bool drawGizmos;
  public float pushOutForce = 10f;
  private bool isFalling;
  private bool isInTheClear;
  private bool isShaking;
  private MeshCollider meshCollider;
  private Transform model;
  private bool once;
  private Vector3 positionOnChange;
  private Rigidbody rig;
  private Quaternion rotationOnChange;
  private Vector3 scaleOnChange;
  private float tickTime;
  private Vector3 velocity = Vector3.zero;
  private PhotonView view;
  public Vector3 innerCheck;
  private HashSet<Collider> ignoreColliders = new HashSet<Collider>();
  public Transform fractureRoot;
  private bool isFractured;

  private void Awake()
  {
    this.meshCollider = this.GetComponent<MeshCollider>();
    this.rig = this.GetComponent<Rigidbody>();
    this.view = this.GetComponent<PhotonView>();
    this.fractureRoot.gameObject.SetActive(false);
    this.rig.useGravity = false;
    this.rig.isKinematic = true;
  }

  private void Start()
  {
  }

  private void Update()
  {
  }

  private void SetIgnoreColliders()
  {
    this.ignoreColliders = new HashSet<Collider>();
    Bounds bounds = this.meshCollider.bounds;
    Vector3 center1 = bounds.center;
    bounds = this.meshCollider.bounds;
    Vector3 extents = bounds.extents;
    Quaternion rotation = this.transform.rotation;
    HashSet<Collider> hashSet = ((IEnumerable<Collider>) Physics.OverlapBox(center1, extents, rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToHashSet<Collider>();
    Vector3 halfExtents = this.transform.TransformVector(this.innerCheck);
    Vector3 center2 = this.transform.position + -this.transform.up * halfExtents.y;
    Debug.Log((object) $"Count: {hashSet.Count}");
    foreach (Collider colliderB in hashSet)
    {
      if (Physics.ComputePenetration((Collider) this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, colliderB, colliderB.transform.position, colliderB.transform.rotation, out Vector3 _, out float _))
        this.ignoreColliders.Add(colliderB);
      else if (((IEnumerable<Collider>) Physics.OverlapBox(center2, halfExtents, this.transform.rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>().Count > 0)
        this.ignoreColliders.Add(colliderB);
    }
  }

  private bool CheckInTheClear()
  {
    Bounds bounds = this.meshCollider.bounds;
    Vector3 center = bounds.center;
    bounds = this.meshCollider.bounds;
    Vector3 extents = bounds.extents;
    Quaternion rotation = this.transform.rotation;
    HashSet<Collider> hashSet = ((IEnumerable<Collider>) Physics.OverlapBox(center, extents, rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToHashSet<Collider>();
    if (hashSet.Count != 0 && hashSet.Any<Collider>((Func<Collider, bool>) (c => this.ignoreColliders.Contains(c))))
      return false;
    this.scaleOnChange = this.meshCollider.transform.lossyScale;
    this.positionOnChange = this.meshCollider.transform.position;
    this.rotationOnChange = this.meshCollider.transform.rotation;
    return true;
  }

  private void FixedUpdate()
  {
    if (!this.isFalling || this.isFractured)
      return;
    if (!this.once)
      this.once = true;
    if (!this.isInTheClear)
    {
      this.isInTheClear = this.CheckInTheClear();
      if (this.isInTheClear)
      {
        this.ignoreColliders.Clear();
        this.rig.excludeLayers = (LayerMask) 0;
      }
    }
    if (!this.CheckBoundingBox(out Vector3 _, out Vector3 _, out List<Collider> _))
      return;
    this.isFractured = true;
    this.mesh.gameObject.SetActive(false);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.meshCollider);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.GetComponent<MeshRenderer>());
    this.fractureRoot.gameObject.SetActive(true);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.rig);
  }

  private void OnCollisionEnter(Collision other)
  {
    if (this.isShaking || this.isFalling)
      return;
    Character componentInParent = other.gameObject.GetComponentInParent<Character>();
    if (!(bool) (UnityEngine.Object) componentInParent || !componentInParent.IsLocal)
      return;
    Debug.Log((object) "Before Shake rock");
    this.view.RPC("ShakeRock", RpcTarget.All);
  }

  private void OnCollisionStay(Collision other)
  {
    if (!this.isShaking)
      return;
    Character componentInParent = other.gameObject.GetComponentInParent<Character>();
    if (!(bool) (UnityEngine.Object) componentInParent || !componentInParent.IsLocal)
      return;
    this.tickTime += Time.deltaTime;
    if ((double) this.tickTime <= 0.1)
      return;
    this.tickTime = 0.0f;
    GamefeelHandler.instance.AddPerlinShake(this.shakeAmount);
  }

  private bool CheckInnerBox(out Vector3 halfExtends, out Vector3 innerCheckPosition)
  {
    halfExtends = this.transform.TransformVector(this.innerCheck);
    innerCheckPosition = this.transform.position + -this.transform.up * halfExtends.y;
    return ((IEnumerable<Collider>) Physics.OverlapBox(innerCheckPosition, halfExtends, this.transform.rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>().Where<Collider>((Func<Collider, bool>) (c => !this.ignoreColliders.Contains(c))).ToList<Collider>().Count > 0;
  }

  public bool CheckBoundingBox(
    out Vector3 halfExtends,
    out Vector3 position,
    out List<Collider> colliders)
  {
    halfExtends = this.meshCollider.bounds.extents;
    position = this.meshCollider.bounds.center;
    colliders = ((IEnumerable<Collider>) Physics.OverlapBox(position, halfExtends, this.transform.rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>();
    colliders = colliders.Where<Collider>((Func<Collider, bool>) (c => !this.ignoreColliders.Contains(c))).ToList<Collider>();
    return colliders.Count > 0;
  }

  public bool ConvexMeshCollision(List<Collider> colliders)
  {
    foreach (Collider collider in colliders)
    {
      if (Physics.ComputePenetration((Collider) this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 _, out float _))
        return true;
    }
    return false;
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.drawGizmos || this.isFractured)
      return;
    this.meshCollider = this.GetComponent<MeshCollider>();
    if (this.isInTheClear)
      Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
    foreach (Collider ignoreCollider in this.ignoreColliders)
      Debug.DrawLine(this.transform.position, ignoreCollider.bounds.center);
    this.CheckInTheClear();
    Vector3 halfExtends1;
    Vector3 innerCheckPosition;
    Gizmos.color = this.CheckInnerBox(out halfExtends1, out innerCheckPosition) ? Color.red : Color.green;
    Gizmos.DrawCube(innerCheckPosition, halfExtends1 * 2f);
    Vector3 halfExtends2;
    Vector3 position;
    List<Collider> colliders;
    Gizmos.color = this.CheckBoundingBox(out halfExtends2, out position, out colliders) ? Color.red : Color.green;
    Gizmos.DrawWireCube(position, halfExtends2 * 2f);
    Gizmos.color = this.ConvexMeshCollision(colliders) ? Color.red : Color.green;
    Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
  }

  [PunRPC]
  private void ShakeRock()
  {
    Debug.Log((object) "start shake rock");
    this.isShaking = true;
    this.StartCoroutine(RockShake());

    IEnumerator RockShake()
    {
      Debug.Log((object) "Start shaking");
      float duration = 0.0f;
      Debug.Log((object) $"duration: {duration}, fallTime: {this.fallTime}");
      while ((double) duration < (double) this.fallTime)
      {
        Debug.Log((object) $"duration: {duration}, fallTime: {this.fallTime}");
        Vector3 zero = (Vector3) Vector2.zero;
        zero.x += Perlin.Noise(Time.time * this.shakeScale, 0.0f, 0.0f) - 0.5f;
        zero.y += Perlin.Noise(0.0f, Time.time * this.shakeScale, 0.0f) - 0.5f;
        zero.z += Perlin.Noise(0.0f, 0.0f, Time.time * this.shakeScale) - 0.5f;
        zero *= this.amount * Time.deltaTime;
        duration += Time.deltaTime;
        Debug.Log((object) $"offset: {zero}");
        this.mesh.localPosition = zero;
        yield return (object) null;
      }
      Debug.Log((object) "Done shaking");
      this.isShaking = false;
      this.mesh.localPosition = 0.ToVec();
      this.isFalling = true;
      this.rig.useGravity = true;
      this.rig.isKinematic = false;
    }
  }

  private void Go()
  {
    this.isFalling = true;
    this.rig.useGravity = true;
    this.rig.isKinematic = false;
  }
}
