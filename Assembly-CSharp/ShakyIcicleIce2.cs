// Decompiled with JetBrains decompiler
// Type: ShakyIcicleIce2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class ShakyIcicleIce2 : MonoBehaviour
{
  public float fallChance = 0.5f;
  public float contactExplosionRadius = 0.2f;
  public float maxFracturePercent = 0.5f;
  public float fracturedMass = 1f;
  public float collisionDamp;
  public float shakeScale = 30f;
  public float fallTime = 5f;
  public float amount = 1f;
  public float startShakeDistance = 10f;
  public float startShakeAmount = 400f;
  public float climbingScreenShake = 240f;
  public float screenShakeTickTime = 0.2f;
  public bool isFalling;
  public bool isShaking;
  public bool fallOnStart;
  public Transform fullMesh;
  public Transform fracturedRoot;
  public SFX_Instance popSound;
  private readonly List<GameObject> shards = new List<GameObject>();
  private Vector3 lastAngularVelocity;
  private Vector3 lastLinearVelocity;
  private MeshCollider meshCollider;
  private PhotonView photonView;
  private Rigidbody rig;
  private GameObject shardsRoot;
  private AudioSource source;
  private int startPeicesCount;
  private List<Collider> stuckies = new List<Collider>();
  private GameObject stuckiesRoot;
  private float timeUntilShake;
  public bool drawGizmos;

  private bool IsLocalPlayerClimbing
  {
    get
    {
      return Character.localCharacter.data.isClimbing && (UnityEngine.Object) Character.localCharacter.data.climbHit.collider == (UnityEngine.Object) this.meshCollider;
    }
  }

  private float DistanceToLocalPlayer
  {
    get => Vector3.Distance(Character.localCharacter.Center, this.transform.position);
  }

  private void Awake()
  {
    this.source = this.GetComponent<AudioSource>();
    this.photonView = this.GetComponent<PhotonView>();
    this.meshCollider = this.GetComponent<MeshCollider>();
    this.startPeicesCount = this.fracturedRoot.transform.childCount;
    this.fracturedRoot.gameObject.SetActive(false);
    this.source.volume = 0.0f;
    this.source.Stop();
    if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.fallChance)
      return;
    this.enabled = false;
  }

  private void Start()
  {
    this.fracturedRoot.gameObject.SetActive(true);
    this.stuckies = this.GetStuckPieces();
    this.fracturedRoot.gameObject.SetActive(false);
    this.fullMesh.gameObject.SetActive(true);
    if (!this.fallOnStart)
      return;
    this.Fall_Rpc();
  }

  private void Update()
  {
    if (!this.photonView.IsMine)
      return;
    if (!this.isShaking && !this.isFalling && PlayerHandler.GetAllPlayerCharacters().Where<Character>((Func<Character, bool>) (p => p.data.isClimbing)).ToList<Character>().Where<Character>((Func<Character, bool>) (p => (UnityEngine.Object) p.data.climbHit.collider == (UnityEngine.Object) this.meshCollider)).ToList<Character>().Count > 0)
      this.photonView.RPC("ShakeRock_Rpc", RpcTarget.All);
    this.timeUntilShake -= Time.deltaTime;
    if (!this.isShaking || !this.IsLocalPlayerClimbing || (double) this.timeUntilShake > 0.0)
      return;
    GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake);
    Debug.Log((object) "Clime shake");
    this.timeUntilShake = this.screenShakeTickTime;
  }

  private void FixedUpdate()
  {
    if ((UnityEngine.Object) this.rig == (UnityEngine.Object) null)
      return;
    this.lastLinearVelocity = this.rig.linearVelocity;
    this.lastAngularVelocity = this.rig.angularVelocity;
  }

  public void OnDestroy()
  {
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.stuckiesRoot);
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.shardsRoot);
  }

  private void OnCollisionEnter(Collision other)
  {
    if (!this.isFalling || (double) this.fracturedRoot.transform.childCount < (double) this.startPeicesCount * (double) this.maxFracturePercent)
      return;
    bool flag = false;
    HashSet<Collider> hashSet = new HashSet<Collider>();
    foreach (ContactPoint contact in other.contacts)
    {
      Collider[] range = Physics.OverlapSphere(contact.point, this.contactExplosionRadius);
      hashSet.AddRange<Collider>((IEnumerable<Collider>) range);
    }
    foreach (Collider collider in hashSet)
    {
      if ((UnityEngine.Object) collider.transform.parent != (UnityEngine.Object) this.fracturedRoot)
      {
        if (this.shards.Contains(collider.gameObject))
        {
          this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
          this.rig.angularVelocity = this.lastAngularVelocity;
        }
      }
      else
      {
        flag = true;
        if ((UnityEngine.Object) this.shardsRoot == (UnityEngine.Object) null)
        {
          this.shardsRoot = new GameObject("ShardsRoot");
          this.shardsRoot.transform.position = collider.transform.position;
        }
        collider.gameObject.AddComponent<Rigidbody>().mass = this.fracturedMass;
        collider.transform.parent = this.shardsRoot.transform;
        this.shards.Add(collider.gameObject);
      }
    }
    if (!flag)
      return;
    this.rig.linearVelocity = this.lastLinearVelocity * this.collisionDamp;
    this.rig.angularVelocity = this.lastAngularVelocity;
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.drawGizmos)
      return;
    this.meshCollider = this.GetComponent<MeshCollider>();
    this.rig = this.GetComponent<Rigidbody>();
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.startShakeDistance);
    if (this.isFalling)
      return;
    foreach (Collider stuckPiece in this.GetStuckPieces())
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireMesh(stuckPiece.GetComponent<MeshCollider>().sharedMesh, stuckPiece.transform.position, stuckPiece.transform.rotation);
    }
  }

  [PunRPC]
  private void ShakeRock_Rpc()
  {
    Debug.Log((object) "start shake rock");
    this.isShaking = true;
    this.source.Play();
    this.source.volume = 0.7f;
    if ((double) this.DistanceToLocalPlayer < (double) this.startShakeDistance)
    {
      Debug.Log((object) $"start shake {this.startShakeAmount}");
      GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount);
    }
    this.StartCoroutine(RockShake());

    IEnumerator RockShake()
    {
      Debug.Log((object) "Start shaking");
      float duration = 0.0f;
      while ((double) duration < (double) this.fallTime)
      {
        Vector3 zero = (Vector3) Vector2.zero;
        zero.x += Perlin.Noise(Time.time * this.shakeScale, 0.0f, 0.0f) - 0.5f;
        zero.y += Perlin.Noise(0.0f, Time.time * this.shakeScale, 0.0f) - 0.5f;
        zero.z += Perlin.Noise(0.0f, 0.0f, Time.time * this.shakeScale) - 0.5f;
        Vector3 vector3 = zero * (this.amount * Time.deltaTime);
        duration += Time.deltaTime;
        this.fullMesh.localPosition = vector3;
        yield return (object) null;
      }
      Debug.Log((object) "Done shaking");
      this.isShaking = false;
      this.fullMesh.localPosition = 0.ToVec();
      this.source.volume = 0.0f;
      this.source.Stop();
      if (this.photonView.IsMine)
        this.photonView.RPC("Fall_Rpc", RpcTarget.All);
    }
  }

  [PunRPC]
  private void Fall_Rpc()
  {
    if (Character.localCharacter.data.isClimbing && (UnityEngine.Object) Character.localCharacter.data.climbHit.collider == (UnityEngine.Object) this.meshCollider)
      Character.localCharacter.refs.climbing.StopClimbing();
    this.popSound.Play(this.transform.position);
    if ((double) this.DistanceToLocalPlayer < (double) this.startShakeDistance)
    {
      Debug.Log((object) $"fall shake {this.startShakeAmount}");
      GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount);
    }
    this.fracturedRoot.gameObject.SetActive(true);
    this.fullMesh.gameObject.SetActive(false);
    this.rig = this.gameObject.AddComponent<Rigidbody>();
    this.rig.mass = 1000f;
    this.rig.useGravity = true;
    this.rig.isKinematic = false;
    this.meshCollider.enabled = false;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.meshCollider);
    foreach (Collider stucky in this.stuckies)
    {
      if ((UnityEngine.Object) this.stuckiesRoot == (UnityEngine.Object) null)
      {
        this.stuckiesRoot = new GameObject("StuckiesRoot");
        this.stuckiesRoot.transform.position = stucky.transform.position;
      }
      stucky.transform.parent = this.stuckiesRoot.transform;
      stucky.enabled = true;
    }
    this.startPeicesCount = this.fracturedRoot.transform.childCount;
    Debug.Log((object) "Falling");
    this.isFalling = true;
  }

  private List<Collider> GetStuckPieces()
  {
    List<MeshCollider> piecsColliders = ((IEnumerable<MeshCollider>) this.fracturedRoot.GetComponentsInChildren<MeshCollider>()).ToList<MeshCollider>();
    Bounds bounds = this.meshCollider.bounds;
    Vector3 center = bounds.center;
    bounds = this.meshCollider.bounds;
    Vector3 extents = bounds.extents;
    Quaternion rotation = this.transform.rotation;
    List<Collider> list = ((IEnumerable<Collider>) Physics.OverlapBox(center, extents, rotation)).Where<Collider>((Func<Collider, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this.meshCollider)).ToList<Collider>().Where<Collider>((Func<Collider, bool>) (c => c.gameObject.IsInLayer(HelperFunctions.LayerType.TerrainMap.ToLayerMask()))).ToList<Collider>().Where<Collider>((Func<Collider, bool>) (c => !((IEnumerable<Collider>) piecsColliders).Contains<Collider>(c))).ToList<Collider>();
    HashSet<Collider> colliderSet = new HashSet<Collider>();
    foreach (Collider colliderB in list)
    {
      foreach (MeshCollider colliderA in piecsColliders)
      {
        if (Physics.ComputePenetration((Collider) colliderA, colliderA.transform.position, colliderA.transform.rotation, colliderB, colliderB.transform.position, colliderB.transform.rotation, out Vector3 _, out float _))
          colliderSet.Add((Collider) colliderA);
      }
    }
    HashSet<Collider> range = new HashSet<Collider>();
    foreach (MeshCollider meshCollider in piecsColliders)
    {
      foreach (Component component in colliderSet)
      {
        if ((double) component.transform.position.y < (double) meshCollider.transform.position.y)
          range.Add((Collider) meshCollider);
      }
    }
    colliderSet.AddRange<Collider>((IEnumerable<Collider>) range);
    return colliderSet.ToList<Collider>();
  }
}
