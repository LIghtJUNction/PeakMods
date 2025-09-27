// Decompiled with JetBrains decompiler
// Type: StickyItemComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class StickyItemComponent : ItemComponent
{
  public static List<StickyItemComponent> ALL_STUCK_ITEMS = new List<StickyItemComponent>();
  public Vector3 stuckLocalOffset;
  public BodypartType stuckToBodypart;
  public Quaternion stuckLocalRotationOffset;
  protected Transform stuckToTransform;
  protected bool stuck;
  public float throwChargeRequirement;
  public int addWeightToStuckPlayer;
  public int addThornsToStuckPlayer;
  public Collider physicalCollider;
  public float spherecastRadius;
  protected ItemPhysicsSyncer physicsSyncer;
  private RaycastHit sphereCastHit;

  public Character stuckToCharacter { get; protected set; }

  public override void Awake()
  {
    base.Awake();
    this.physicsSyncer = this.GetComponent<ItemPhysicsSyncer>();
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    if (!this.photonView.IsMine || !(bool) (Object) this.stuckToCharacter)
      return;
    this.photonView.RPC("RPC_StickToCharacterRemote", newPlayer, (object) this.stuckToCharacter.photonView.ViewID, (object) (int) this.stuckToBodypart, (object) this.stuckLocalOffset);
  }

  [PunRPC]
  public void RPC_StickToCharacterRemote(int characterViewID, int bodyPartType, Vector3 offset)
  {
    PhotonView photonView = PhotonNetwork.GetPhotonView(characterViewID);
    if (!(bool) (Object) photonView)
      return;
    Character component = photonView.GetComponent<Character>();
    if (component.IsLocal)
      return;
    Bodypart bodypart = component.GetBodypart((BodypartType) bodyPartType);
    if ((Object) bodypart == (Object) null)
      return;
    this.StickToCharacterLocal(component, bodypart, offset);
  }

  internal virtual void StickToCharacterLocal(
    Character character,
    Bodypart bodypart,
    Vector3 worldOffset)
  {
    if (this.item.itemState != ItemState.Ground || (Object) character == (Object) null)
      return;
    this.stuck = true;
    this.stuckToCharacter = character;
    this.stuckToTransform = bodypart.transform;
    this.item.rig.isKinematic = true;
    this.physicalCollider.isTrigger = true;
    this.item.rig.angularVelocity = Vector3.zero;
    this.item.rig.linearVelocity = Vector3.zero;
    Debug.Log((object) $"Stuck to {character.gameObject.name} with offset distance {worldOffset.magnitude}");
    if (bodypart.partType == BodypartType.Foot_R || bodypart.partType == BodypartType.Foot_L)
      worldOffset.y = Mathf.Max(worldOffset.y, 0.0f);
    this.stuckToBodypart = bodypart.partType;
    this.stuckLocalOffset = this.stuckToTransform.InverseTransformVector(worldOffset);
    this.stuckLocalRotationOffset = this.stuckToTransform.rotation * Quaternion.Inverse(this.transform.rotation);
    this.physicsSyncer.shouldSync = false;
    if (!StickyItemComponent.ALL_STUCK_ITEMS.Contains(this))
      StickyItemComponent.ALL_STUCK_ITEMS.Add(this);
    character.refs.afflictions.UpdateWeight();
    if (!character.IsLocal)
      return;
    this.photonView.RPC("RPC_StickToCharacterRemote", RpcTarget.Others, (object) character.photonView.ViewID, (object) (int) bodypart.partType, (object) worldOffset);
  }

  private void Unstick()
  {
    Character stuckToCharacter = this.stuckToCharacter;
    this.stuck = false;
    this.item.rig.isKinematic = false;
    this.physicalCollider.isTrigger = false;
    this.stuckToCharacter = (Character) null;
    this.stuckToTransform = (Transform) null;
    StickyItemComponent.ALL_STUCK_ITEMS.Remove(this);
    this.physicsSyncer.shouldSync = true;
    if (!(bool) (Object) stuckToCharacter)
      return;
    stuckToCharacter.refs.afflictions.UpdateWeight();
  }

  private void OnDestroy()
  {
    StickyItemComponent.ALL_STUCK_ITEMS.Remove(this);
    if (!(bool) (Object) this.stuckToCharacter)
      return;
    this.stuckToCharacter.refs.afflictions.UpdateWeight();
  }

  private void Update()
  {
    if (!this.stuck || this.item.itemState == ItemState.Ground)
      return;
    this.Unstick();
  }

  protected virtual void FixedUpdate()
  {
    if (!this.stuck)
      return;
    this.item.rig.MovePosition(this.stuckToTransform.TransformPoint(this.stuckLocalOffset));
    this.item.rig.MoveRotation(this.stuckToTransform.rotation * this.stuckLocalRotationOffset);
  }

  public override void OnInstanceDataSet()
  {
  }
}
