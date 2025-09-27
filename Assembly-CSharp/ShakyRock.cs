// Decompiled with JetBrains decompiler
// Type: ShakyRock
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
public class ShakyRock : MonoBehaviour
{
  public float fallTime = 5f;
  public float amount = 1f;
  public float shakeScale = 15f;
  public Transform mesh;
  public float shakeAmount = 10f;
  public bool drawGizmos;
  public float pushOutForce = 10f;
  private bool isFalling;
  private bool isFinished;
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

  private void Awake()
  {
    this.meshCollider = this.GetComponent<MeshCollider>();
    this.rig = this.GetComponent<Rigidbody>();
    this.view = this.GetComponent<PhotonView>();
    this.rig.useGravity = false;
    this.rig.isKinematic = true;
  }

  private void Start()
  {
  }

  private void Update()
  {
  }

  private void FixedUpdate()
  {
    if (this.isFinished || !this.isFalling)
      return;
    if (!this.once)
    {
      this.rig.AddForce(Vector3.back * this.pushOutForce, ForceMode.VelocityChange);
      this.once = true;
    }
    Vector3 center1 = this.meshCollider.bounds.center;
    Bounds bounds = this.meshCollider.bounds;
    double radius = (double) bounds.extents.magnitude / 2.0;
    if (((IEnumerable<Collider>) Physics.OverlapSphere(center1, (float) radius)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>().Count > 0)
      return;
    bounds = this.meshCollider.bounds;
    Vector3 center2 = bounds.center;
    bounds = this.meshCollider.bounds;
    Vector3 extents = bounds.extents;
    Quaternion rotation = this.transform.rotation;
    List<Collider> list = ((IEnumerable<Collider>) Physics.OverlapBox(center2, extents, rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>();
    Debug.Log((object) $"Count: {list.Count}");
    foreach (Collider colliderB in list)
    {
      if (Physics.ComputePenetration((Collider) this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, colliderB, colliderB.transform.position, colliderB.transform.rotation, out Vector3 _, out float _))
      {
        Debug.Log((object) ("colliding with " + colliderB.name));
        return;
      }
      Debug.Log((object) ("Not colliding with " + colliderB.name));
    }
    this.scaleOnChange = this.meshCollider.transform.lossyScale;
    this.positionOnChange = this.meshCollider.transform.position;
    this.rotationOnChange = this.meshCollider.transform.rotation;
    this.isFinished = true;
    this.rig.excludeLayers = (LayerMask) 0;
  }

  private void OnCollisionEnter(Collision other)
  {
    if (this.isShaking || this.isFalling || this.isFinished)
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

  private void OnDrawGizmosSelected()
  {
    if (!this.drawGizmos)
      return;
    if (this.isFinished)
      Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.positionOnChange, this.rotationOnChange, this.scaleOnChange);
    this.meshCollider = this.GetComponent<MeshCollider>();
    Bounds bounds = this.meshCollider.bounds;
    Vector3 center = bounds.center;
    bounds = this.meshCollider.bounds;
    double radius = (double) bounds.extents.magnitude / 2.0;
    List<Collider> list1 = ((IEnumerable<Collider>) Physics.OverlapSphere(center, (float) radius)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>();
    Gizmos.color = list1.Count > 0 ? Color.red : Color.green;
    Gizmos.DrawWireSphere(this.meshCollider.bounds.center, this.meshCollider.bounds.extents.magnitude / 2f);
    if (list1.Count > 0)
      return;
    List<Collider> list2 = ((IEnumerable<Collider>) Physics.OverlapBox(this.meshCollider.bounds.center, this.meshCollider.bounds.extents, this.transform.rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>();
    Gizmos.color = list2.Count > 0 ? Color.red : Color.green;
    Gizmos.DrawWireCube(this.meshCollider.bounds.center, this.meshCollider.bounds.size);
    foreach (Collider colliderB in list2)
    {
      if (Physics.ComputePenetration((Collider) this.meshCollider, this.meshCollider.transform.position, this.meshCollider.transform.rotation, colliderB, colliderB.transform.position, colliderB.transform.rotation, out Vector3 _, out float _))
      {
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
        return;
      }
      Debug.Log((object) ("Not colliding with " + colliderB.name));
    }
    Gizmos.color = Color.green;
    Gizmos.DrawWireMesh(this.meshCollider.sharedMesh, this.meshCollider.transform.position, this.meshCollider.transform.rotation, this.meshCollider.transform.lossyScale);
  }

  private void Go2() => GamefeelHandler.instance.AddPerlinShake(this.shakeAmount);

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
      this.rig.AddForce(Vector3.back * this.pushOutForce, ForceMode.VelocityChange);
    }
  }

  private void Go()
  {
    this.isFalling = true;
    this.rig.useGravity = true;
    this.rig.isKinematic = false;
  }
}
