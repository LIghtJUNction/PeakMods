// Decompiled with JetBrains decompiler
// Type: JungleVine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class JungleVine : CustomSpawnCondition, IInteractible
{
  public float minDist = 25f;
  public float maxDist = 50f;
  public float maxHeightDifference = 100f;
  public float normalYMult = 1f;
  public float minDown = 5f;
  public float maxDown = 30f;
  public float meshLength = 40f;
  public int vineType;
  public float colliderOffset;
  public JungleVine.ColliderType colliderType;
  public Transform colliderTransform;
  public Vector2 boxShape = Vector2.one;
  public PhotonView photonView;
  public Transform connectTo;
  private readonly int segments = 50;
  private float totalLength;
  public Transform hangCenter;
  public string displayName;
  public Transform colliderRoot;

  private void Awake()
  {
    this.totalLength = 0.0f;
    this.photonView = this.GetComponent<PhotonView>();
    if (!((Object) this.colliderRoot == (Object) null))
      return;
    this.colliderRoot = new GameObject("ColliderRoot").transform;
    this.colliderRoot.parent = this.transform;
    this.colliderRoot.localPosition = Vector3.zero;
    this.colliderRoot.localRotation = Quaternion.identity;
  }

  private void Start() => this.SetRendererBounds();

  public void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(this.transform.position + this.transform.forward * this.maxDist, 0.5f);
    MeshRenderer componentInChildren = this.GetComponentInChildren<MeshRenderer>();
    Bounds bounds = componentInChildren.bounds;
    Vector3 center = bounds.center;
    bounds = componentInChildren.bounds;
    Vector3 size = bounds.size;
    Gizmos.DrawWireCube(center, size);
  }

  public bool IsInteractible(Character interactor)
  {
    return this.colliderType == JungleVine.ColliderType.Capsule;
  }

  public void Interact(Character interactor)
  {
    interactor.refs.items.EquipSlot(Optionable<byte>.None);
    int closestChild = this.GetClosestChild(interactor.Center);
    Debug.Log((object) $"Grabbing Vine with index: {closestChild}");
    interactor.GetComponent<PhotonView>().RPC("GrabVineRpc", RpcTarget.All, (object) this.GetComponent<PhotonView>(), (object) closestChild);
  }

  public void HoverEnter()
  {
  }

  public void HoverExit()
  {
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("GRAB");

  public string GetName() => LocalizedText.GetText(this.displayName);

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    Vector3 vector3 = data.normal;
    vector3.y *= this.normalYMult;
    vector3 = vector3.normalized;
    RaycastHit raycastHit = HelperFunctions.LineCheck(this.transform.position, this.transform.position + vector3 * this.maxDist, HelperFunctions.LayerType.TerrainMap);
    if (!(bool) (Object) raycastHit.transform || (double) raycastHit.distance < (double) this.minDist || (double) Mathf.Abs(raycastHit.point.y - this.transform.position.y) > (double) this.maxHeightDifference)
      return false;
    int num = this.ConfigVine(raycastHit.point) ? 1 : 0;
    if (num == 0)
      return num != 0;
    BreakableBridge component;
    if (!this.TryGetComponent<BreakableBridge>(out component))
      return num != 0;
    component.AddCollisionModifiers();
    return num != 0;
  }

  public static bool CheckVinePath(Vector3 from, Vector3 to, float hang, out Vector3 mid)
  {
    mid = Vector3.Lerp(from, to, 0.5f);
    mid.y += hang;
    Vector3 from1 = from;
    for (int index = 0; index < 50; ++index)
    {
      float _t = (float) index / 49f;
      Vector3 to1 = BezierCurve.QuadraticBezier(from, mid, to, _t);
      if (index < 49 && (bool) (Object) HelperFunctions.LineCheck(from1, to1, HelperFunctions.LayerType.TerrainMap).transform)
        return false;
    }
    return true;
  }

  [PunRPC]
  public void ForceBuildVine_RPC(Vector3 from, Vector3 to, float hang, Vector3 mid)
  {
    this.ForceBuildVine(from, to, hang, mid);
  }

  public void ForceBuildVine(Vector3 from, Vector3 to, float hang, Vector3 mid)
  {
    if ((Object) this.colliderRoot == (Object) null)
    {
      Debug.Log((object) ("colliderRoot was null, creating new one for " + this.gameObject.name));
      this.colliderRoot = new GameObject("ColliderRoot").transform;
      this.colliderRoot.parent = this.transform;
      this.colliderRoot.localPosition = Vector3.zero;
      this.colliderRoot.localRotation = Quaternion.identity;
    }
    this.colliderRoot.KillAllChildren(true);
    float num = Vector3.Distance(from, to) / this.meshLength;
    Renderer componentInChildren = this.GetComponentInChildren<Renderer>();
    componentInChildren.material.SetFloat("_Hang", hang);
    if ((Object) this.hangCenter != (Object) null)
      this.hangCenter.position = BezierCurve.QuadraticBezier(from, mid, to, 0.5f);
    Vector3 a = from;
    for (int index = 0; index < 50; ++index)
    {
      float _t = (float) index / 49f;
      Vector3 b = BezierCurve.QuadraticBezier(from, mid, to, _t);
      GameObject gameObject = new GameObject("Collider");
      gameObject.transform.parent = this.colliderRoot;
      if (this.colliderType == JungleVine.ColliderType.Capsule)
      {
        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.radius = 0.25f;
        capsuleCollider.height = Vector3.Distance(a, b) + 0.5f;
        capsuleCollider.isTrigger = true;
      }
      else
        gameObject.AddComponent<BoxCollider>().size = new Vector3(this.boxShape.x * num, Vector3.Distance(a, b) + 0.5f, this.boxShape.y);
      gameObject.transform.rotation = HelperFunctions.GetRotationWithUp(Vector3.down, b - a);
      gameObject.transform.position = b - Vector3.down * this.colliderOffset;
      gameObject.gameObject.layer = 21;
      a = b;
    }
    Transform transform = this.transform.Find("Mesh");
    transform.transform.rotation = Quaternion.LookRotation(to - from);
    transform.transform.localScale = Vector3.one * num;
    if ((double) num < 0.5)
      componentInChildren.material.SetFloat("_LengthScale", num * 2f);
    Debug.Log((object) "Vine built, calling onFinish");
  }

  public bool ConfigVine(Vector3 to)
  {
    ((IEnumerable<Collider>) this.GetComponentsInChildren<Collider>()).KillAllGameObjects<Collider>(true);
    float hang = Random.Range(-this.maxDown, -this.minDown);
    Vector3 position = this.transform.position;
    Vector3 mid;
    if (!JungleVine.CheckVinePath(position, to, hang, out mid))
      return false;
    this.ForceBuildVine(position, to, hang, mid);
    return true;
  }

  public void ConnectDebug() => this.ConfigVine(this.connectTo.transform.position);

  public void SetRendererBounds()
  {
    Vector3 a = this.transform.position;
    Vector3 position1 = this.transform.position;
    if ((Object) this.colliderRoot != (Object) null)
    {
      for (int index = 0; index < Mathf.Min(this.colliderRoot.transform.childCount, this.segments); ++index)
      {
        Vector3 position2 = this.colliderRoot.transform.GetChild(index).transform.position;
        this.totalLength += Vector3.Distance(a, position2);
        a = position2;
        if ((double) position2.y < (double) position1.y)
          position1 = position2;
      }
    }
    Renderer componentInChildren = this.GetComponentInChildren<Renderer>();
    Bounds localBounds = componentInChildren.localBounds;
    localBounds.Encapsulate(componentInChildren.transform.InverseTransformPoint(position1));
    componentInChildren.localBounds = localBounds;
  }

  public float LengthFactor() => 1f / this.totalLength;

  public float GetPercentFromSegmentIndex(int segmentIndex)
  {
    return (float) segmentIndex / ((float) this.segments - 1f);
  }

  public int GetIndexFromPercentage(float percent)
  {
    return Mathf.RoundToInt(Mathf.Lerp(0.0f, (float) (this.segments - 1), percent));
  }

  internal Vector3 GetDir(Vector3 lookDirection_Flat, float percent)
  {
    Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
    if ((double) Vector3.Angle(lookDirection_Flat, up) > 90.0)
      up *= -1f;
    return up;
  }

  public float GetVineVel(Vector3 vel, float percent)
  {
    Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
    Vector3 dir = this.GetDir(vel, percent);
    float num = 1f;
    if ((double) Vector3.Angle(vel, up) > 90.0)
      num = -1f;
    vel = Vector3.Project(vel, up);
    return num * vel.magnitude * Mathf.InverseLerp(0.0f, -0.5f, dir.y);
  }

  public float GetSign(Vector3 dir, float percent)
  {
    Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
    float sign = 1f;
    if ((double) Vector3.Angle(dir, up) > 90.0)
      sign = -1f;
    return sign;
  }

  public Vector3 GetUp(float percent)
  {
    Vector3 up = this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up;
    if ((double) Vector3.Angle(Vector3.up, up) > 90.0)
      up *= -1f;
    return up;
  }

  public float UpMult(float percent)
  {
    return (double) Vector3.Angle(Vector3.up, this.colliderRoot.transform.GetChild(this.GetIndexFromPercentage(percent)).up) < 90.0 ? 1f : -1f;
  }

  public Vector3 GetPosition(float percent)
  {
    percent = Mathf.Clamp01(percent);
    double f = (double) percent * (double) (this.segments - 1);
    int num1 = Mathf.FloorToInt((float) f);
    int num2 = num1;
    if (num1 == 0)
      num1 = 1;
    if ((double) percent < 1.0)
      num2 = num1 + 1;
    float t = (float) f - (float) num1;
    int num3 = Mathf.Clamp(num1, 0, this.colliderRoot.transform.childCount - 1);
    int index = Mathf.Clamp(num2, num3, this.colliderRoot.transform.childCount - 1);
    return Vector3.Lerp(this.colliderRoot.transform.GetChild(num3).position, this.colliderRoot.transform.GetChild(index).position, t);
  }

  private int GetClosestChild(Vector3 center)
  {
    float num1 = 100000f;
    int closestChild = -1;
    for (int index = 0; index < this.colliderRoot.transform.childCount; ++index)
    {
      float num2 = Vector3.Distance(center, this.colliderRoot.transform.GetChild(index).position);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        closestChild = index;
      }
    }
    return closestChild;
  }

  public enum ColliderType
  {
    Capsule,
    Box,
  }
}
