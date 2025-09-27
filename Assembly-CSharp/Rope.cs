// Decompiled with JetBrains decompiler
// Type: Rope
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

#nullable disable
[DefaultExecutionOrder(100000)]
public class Rope : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
  public float spacing = 0.75f;
  public float climberGravity = 1f;
  public float slurpTime = 10f;
  public bool antigrav;
  public bool isHelicopterRope;
  public GameObject ropeSegmentPrefab;
  public GameObject remoteSegmentPrefab;
  public Rope.ATTACHMENT attachmenState;
  public bool isClimbable;
  public PhotonView view;
  private readonly List<Transform> remoteColliderSegments = new List<Transform>();
  private readonly List<Transform> simulationSegments = new List<Transform>();
  [NonSerialized]
  public List<Character> charactersClimbing = new List<Character>();
  [NonSerialized]
  public RopeClimbingAPI climbingAPI;
  private Item itemSpool;
  private RopeBoneVisualizer ropeBoneVisualizer;
  private float segments;
  private RopeSpool spool;
  private Vector3 startAnchorOf2ndSegment;
  private float timeSinceRemoved;
  public bool creatorLeft;
  private RopeAnchor attachedToAnchor;

  public float Segments
  {
    get => this.segments;
    set => this.segments = Mathf.Clamp(value, 0.0f, (float) Rope.MaxSegments);
  }

  public static int MaxSegments => 40;

  public int SegmentCount
  {
    get
    {
      return this.photonView.IsMine ? this.simulationSegments.Count : this.remoteColliderSegments.Count;
    }
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.view.RPC("OnRejoinSyncRPC", newPlayer, (object) this.attachmenState);
  }

  [PunRPC]
  public void OnRejoinSyncRPC(Rope.ATTACHMENT attachmentState)
  {
    this.attachmenState = attachmentState;
  }

  private void Awake()
  {
    this.itemSpool = this.GetComponentInParent<Item>();
    this.climbingAPI = this.GetComponent<RopeClimbingAPI>();
    this.view = this.GetComponent<PhotonView>();
    this.ropeBoneVisualizer = this.GetComponentInChildren<RopeBoneVisualizer>();
  }

  private void Update()
  {
    bool flag;
    switch (this.attachmenState)
    {
      case Rope.ATTACHMENT.unattached:
        flag = false;
        break;
      case Rope.ATTACHMENT.inSpool:
        flag = false;
        break;
      case Rope.ATTACHMENT.anchored:
        flag = true;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    this.isClimbable = flag;
    if (!this.photonView.IsMine || this.creatorLeft)
    {
      if (this.simulationSegments.Count <= 0)
        return;
      this.Clear();
    }
    else
    {
      this.timeSinceRemoved += Time.deltaTime;
      int num = Mathf.Clamp(Mathf.FloorToInt(this.Segments), 1, int.MaxValue);
      if (this.simulationSegments.Count > num)
      {
        if (this.simulationSegments.Count > 1)
          this.RemoveSegment();
      }
      else if (this.simulationSegments.Count < num)
        this.AddSegment();
      if (this.simulationSegments.Count <= 1)
        return;
      float t = this.Segments % 1f;
      List<Transform> simulationSegments = this.simulationSegments;
      ConfigurableJoint component = simulationSegments[simulationSegments.Count - 1].GetComponent<ConfigurableJoint>();
      Vector3 b = Vector3.Lerp(this.startAnchorOf2ndSegment, -this.spacing.oxo(), Mathf.Clamp01(this.timeSinceRemoved / this.slurpTime));
      component.connectedAnchor = Vector3.Lerp(this.spacing.oxo(), b, t);
      component.GetComponent<Collider>().enabled = true;
    }
  }

  private void FixedUpdate()
  {
    if (!this.photonView.IsMine || this.creatorLeft)
      return;
    if (this.antigrav)
    {
      foreach (Component simulationSegment in this.simulationSegments)
        simulationSegment.GetComponent<Rigidbody>().AddForce(-Physics.gravity * 2f, ForceMode.Acceleration);
    }
    else
    {
      foreach (Character character in this.charactersClimbing)
        this.climbingAPI.GetSegmentFromPercent(character.data.ropePercent).GetComponent<Rigidbody>().AddForce(Vector3.down * this.climberGravity, ForceMode.Acceleration);
    }
  }

  public override void OnEnable()
  {
    base.OnEnable();
    PhotonNetwork.AddCallbackTarget((object) this);
  }

  public override void OnDisable()
  {
    base.OnDisable();
    PhotonNetwork.RemoveCallbackTarget((object) this);
  }

  public List<Transform> GetRopeSegments()
  {
    return this.photonView.IsMine ? this.simulationSegments : this.remoteColliderSegments;
  }

  public bool IsActive()
  {
    bool flag = true;
    if ((UnityEngine.Object) this.itemSpool != (UnityEngine.Object) null && this.itemSpool.itemState != ItemState.Held)
      flag = false;
    return flag;
  }

  [PunRPC]
  public void Detach_Rpc()
  {
    if ((UnityEngine.Object) this.spool != (UnityEngine.Object) null)
    {
      this.spool.ropeInstance = (GameObject) null;
      this.spool.rope = (Rope) null;
      this.spool.Segments = 0.0f;
      this.spool.ClearRope();
      this.spool.RopeFuel -= this.segments;
    }
    if (this.view.IsMine)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.simulationSegments.First<Transform>().GetComponent<ConfigurableJoint>());
    this.spool = (RopeSpool) null;
    this.attachmenState = Rope.ATTACHMENT.unattached;
    Debug.Log((object) $"Detach_Rpc: {this.attachmenState}");
    this.ropeBoneVisualizer.StartTransform = (Transform) null;
  }

  public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
  {
  }

  public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
  {
    base.OnMasterClientSwitched(newMasterClient);
    this.creatorLeft = true;
    Debug.Log((object) $"OnMasterClientSwitched: {newMasterClient}, isMaster: {PhotonNetwork.IsMasterClient}, frame: {Time.frameCount}");
  }

  public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
  {
    if ((UnityEngine.Object) targetView != (UnityEngine.Object) this.photonView)
      return;
    Debug.Log((object) "Trasfered ownership to me");
    this.creatorLeft = true;
    if (this.attachmenState != Rope.ATTACHMENT.inSpool)
      return;
    Debug.Log((object) $"attached to spool, deleting rope: {this.view}");
    PhotonNetwork.Destroy(this.view);
  }

  public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
  {
  }

  [PunRPC]
  public void AttachToAnchor_Rpc(PhotonView anchorView)
  {
    if ((UnityEngine.Object) this.ropeBoneVisualizer == (UnityEngine.Object) null)
      this.ropeBoneVisualizer = this.GetComponentInChildren<RopeBoneVisualizer>();
    if (this.attachmenState == Rope.ATTACHMENT.inSpool)
      this.Detach_Rpc();
    this.attachedToAnchor = anchorView.GetComponent<RopeAnchor>();
    this.attachmenState = Rope.ATTACHMENT.anchored;
    Debug.Log((object) $"AttachToAnchor_Rpc: {this.attachmenState}");
    this.ropeBoneVisualizer.StartTransform = this.attachedToAnchor.anchorPoint;
    if (!this.photonView.IsMine)
      return;
    List<Transform> ropeSegments = this.GetRopeSegments();
    if (ropeSegments.Count <= 0)
      return;
    ropeSegments[0].GetComponent<RopeSegment>().Tie(this.attachedToAnchor.anchorPoint.position);
  }

  public float GetLengthInMeters() => Rope.GetLengthInMeters((float) this.GetRopeSegments().Count);

  public static float GetLengthInMeters(float segmentCount) => segmentCount * 0.25f;

  [PunRPC]
  public void AttachToSpool_Rpc(PhotonView viewSpool)
  {
    this.spool = viewSpool.GetComponent<RopeSpool>();
    if ((UnityEngine.Object) this.spool == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Spool is null");
    }
    else
    {
      this.spool.ropeInstance = this.gameObject;
      this.spool.rope = this;
      this.ropeBoneVisualizer.StartTransform = this.spool.ropeStart;
      this.transform.position = this.spool.ropeBase.position;
      this.transform.rotation = this.spool.ropeBase.rotation;
      this.attachmenState = Rope.ATTACHMENT.inSpool;
      Debug.Log((object) $"AttachToSpool_Rpc: {this.attachmenState}");
      this.Segments = 0.0f;
      Physics.SyncTransforms();
    }
  }

  public void AddSegment()
  {
    int num = this.simulationSegments.Count == 0 ? 1 : 0;
    Transform transform1 = (Transform) null;
    if (num == 0)
      transform1 = this.simulationSegments[0];
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ropeSegmentPrefab, num != 0 ? this.transform.position : transform1.transform.position, num != 0 ? this.transform.rotation : transform1.transform.rotation, this.transform);
    gameObject.gameObject.name = "RopeSegment: " + this.simulationSegments.Count.ToString();
    ConfigurableJoint component1 = gameObject.GetComponent<ConfigurableJoint>();
    if (num != 0)
    {
      component1.autoConfigureConnectedAnchor = true;
      if ((UnityEngine.Object) this.spool != (UnityEngine.Object) null)
      {
        component1.transform.position = this.spool.ropeBase.position;
        component1.transform.rotation = this.spool.ropeBase.rotation;
        component1.autoConfigureConnectedAnchor = true;
        component1.connectedBody = this.spool.rig;
        component1.angularXMotion = ConfigurableJointMotion.Limited;
        ConfigurableJoint configurableJoint1 = component1;
        SoftJointLimitSpring jointLimitSpring1 = new SoftJointLimitSpring();
        jointLimitSpring1.spring = 35f;
        jointLimitSpring1.damper = 45f;
        SoftJointLimitSpring jointLimitSpring2 = jointLimitSpring1;
        configurableJoint1.angularXLimitSpring = jointLimitSpring2;
        ConfigurableJoint configurableJoint2 = component1;
        jointLimitSpring1 = new SoftJointLimitSpring();
        jointLimitSpring1.spring = 35f;
        jointLimitSpring1.damper = 45f;
        SoftJointLimitSpring jointLimitSpring3 = jointLimitSpring1;
        configurableJoint2.angularYZLimitSpring = jointLimitSpring3;
        component1.angularZMotion = ConfigurableJointMotion.Limited;
      }
    }
    else
      component1.connectedBody = transform1.GetComponent<Rigidbody>();
    this.simulationSegments.Add(gameObject.transform);
    if (this.simulationSegments.Count <= 2)
      return;
    List<Transform> simulationSegments = this.simulationSegments;
    Transform transform2 = simulationSegments[simulationSegments.Count - 2];
    Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
    ConfigurableJoint component3 = transform2.GetComponent<ConfigurableJoint>();
    component3.connectedBody = component2;
    this.startAnchorOf2ndSegment = new Vector3(0.0f, -this.spacing, 0.0f);
    component3.connectedAnchor = this.startAnchorOf2ndSegment;
  }

  private void RemoveSegment()
  {
    List<Transform> simulationSegments1 = this.simulationSegments;
    Transform transform1 = simulationSegments1[simulationSegments1.Count - 1];
    List<Transform> simulationSegments2 = this.simulationSegments;
    Transform transform2 = simulationSegments2[simulationSegments2.Count - 2];
    Transform simulationSegment = this.simulationSegments[0];
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) transform1.gameObject);
    this.simulationSegments.RemoveLast<Transform>();
    ConfigurableJoint component = transform2.GetComponent<ConfigurableJoint>();
    if ((UnityEngine.Object) transform2 == (UnityEngine.Object) simulationSegment)
    {
      Debug.LogError((object) "Attempting to connect joint to itself");
    }
    else
    {
      this.timeSinceRemoved = 0.0f;
      component.connectedBody = simulationSegment.GetComponent<Rigidbody>();
      this.startAnchorOf2ndSegment = simulationSegment.InverseTransformPoint(component.transform.position);
      component.connectedAnchor = this.startAnchorOf2ndSegment;
    }
  }

  public RopeSyncData GetSyncData()
  {
    RopeSyncData syncData = new RopeSyncData()
    {
      isVisible = this.isClimbable,
      segments = new RopeSyncData.SegmentData[this.simulationSegments.Count]
    };
    for (int index = 0; index < this.simulationSegments.Count; ++index)
      syncData.segments[index] = new RopeSyncData.SegmentData()
      {
        position = (float3) this.simulationSegments[index].position,
        rotation = this.simulationSegments[index].rotation
      };
    return syncData;
  }

  public void SetSyncData(RopeSyncData data)
  {
    if (data.updateVisualizerManually)
      this.ropeBoneVisualizer.ManuallyUpdateNextFrame = Optionable<bool>.Some(true);
    if (this.creatorLeft)
      return;
    this.isClimbable = data.isVisible;
    int length = data.segments.Length;
    int count = this.remoteColliderSegments.Count;
    if (length < count)
    {
      int num = count - length;
      for (int index = 0; index < num; ++index)
      {
        List<Transform> colliderSegments = this.remoteColliderSegments;
        Transform transform = colliderSegments[colliderSegments.Count - 1];
        this.remoteColliderSegments.RemoveLast<Transform>();
        UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
      }
    }
    else if (length > count)
    {
      int num = length - count;
      for (int index = 0; index < num; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.remoteSegmentPrefab, Vector3.zero, Quaternion.identity, this.transform);
        gameObject.GetComponent<RopeSegment>().rope = this;
        this.remoteColliderSegments.Add(gameObject.transform);
      }
    }
    if (length != this.remoteColliderSegments.Count)
    {
      Debug.LogError((object) "Remote Segment Logic Failed");
    }
    else
    {
      for (int index = 0; index < data.segments.Length; ++index)
      {
        this.remoteColliderSegments[index].position = (Vector3) data.segments[index].position;
        this.remoteColliderSegments[index].rotation = data.segments[index].rotation;
      }
      this.ropeBoneVisualizer.SetData(data);
    }
  }

  public float GetTotalLength() => (float) this.SegmentCount * this.spacing;

  public void Clear(bool alsoRemoveRemote = false)
  {
    Debug.Log((object) "Rope Clear!");
    if (this.simulationSegments.Count > 0)
    {
      for (int index = this.simulationSegments.Count - 1; index >= 0; --index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.simulationSegments[index].gameObject);
      this.simulationSegments.Clear();
    }
    if (!alsoRemoveRemote)
      return;
    for (int index = this.remoteColliderSegments.Count - 1; index >= 0; --index)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.remoteColliderSegments[index].gameObject);
    this.remoteColliderSegments.Clear();
  }

  public void AddCharacterClimbing(Character character) => this.charactersClimbing.Add(character);

  public void RemoveCharacterClimbing(Character character)
  {
    this.charactersClimbing.Remove(character);
  }

  public enum ATTACHMENT
  {
    unattached,
    inSpool,
    anchored,
  }
}
