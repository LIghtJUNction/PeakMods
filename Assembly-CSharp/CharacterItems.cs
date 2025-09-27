// Decompiled with JetBrains decompiler
// Type: CharacterItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterItems : MonoBehaviourPunCallbacks
{
  public SFX_Instance cookSfx;
  public float holdForce;
  public float holdTorque;
  public float throwChargeTime;
  public float minThrowForce;
  public float maxThrowForce;
  public float delayBeforeThrowCharge;
  [NonSerialized]
  public Optionable<byte> currentSelectedSlot;
  [NonSerialized]
  public Optionable<byte> lastSelectedSlot;
  private Character character;
  private PhotonView photonView;
  public float lastEquippedSlotTime;
  [HideInInspector]
  public float throwChargeLevel;
  public bool isChargingThrow;
  private float lastPressedDrop;
  private bool pressedDrop;
  public Action onSlotEquipped;
  public const int MAX_SLOT = 3;
  private float lastSwitched;
  private int timesSwitchedRecently;
  private float climbingSpikeTick;
  private bool readyToSpike = true;
  private bool spikingWithPrimary;
  private bool spikingWithSecondary;
  private ItemSlot currentClimbingSpikeItemSlot;
  private ClimbingSpikeComponent currentClimbingSpikeComponent;
  private GameObject currentClimbingSpikePreview;
  private RaycastHit climbingSpikeHit;

  private IEnumerator SubscribeRoutine(bool subscribe)
  {
    CharacterItems characterItems = this;
    while (!(bool) (UnityEngine.Object) characterItems.character.player)
      yield return (object) null;
    if (subscribe)
      characterItems.character.player.itemsChangedAction += new Action<ItemSlot[]>(characterItems.UpdateClimbingSpikeCount);
    else
      characterItems.character.player.itemsChangedAction -= new Action<ItemSlot[]>(characterItems.UpdateClimbingSpikeCount);
    if (subscribe)
      characterItems.character.player.itemsChangedAction += new Action<ItemSlot[]>(characterItems.UpdateBalloonCount);
    else
      characterItems.character.player.itemsChangedAction -= new Action<ItemSlot[]>(characterItems.UpdateBalloonCount);
  }

  private void Awake()
  {
    this.character = this.GetComponent<Character>();
    this.photonView = this.GetComponent<PhotonView>();
    this.currentSelectedSlot = Optionable<byte>.None;
    this.lastSelectedSlot = Optionable<byte>.Some((byte) 0);
    this.StartCoroutine(this.SubscribeRoutine(true));
  }

  private void OnDestroy()
  {
    if (!(bool) (UnityEngine.Object) this.character.player)
      return;
    this.character.player.itemsChangedAction -= new Action<ItemSlot[]>(this.UpdateClimbingSpikeCount);
    this.character.player.itemsChangedAction -= new Action<ItemSlot[]>(this.UpdateBalloonCount);
  }

  private void FixedUpdate()
  {
    if (!(bool) (UnityEngine.Object) this.character.data.currentItem)
      return;
    this.HoldItem(this.character.data.currentItem);
  }

  private void HoldItem(Item item)
  {
    Vector3 vector3_1 = this.GetItemHoldPos(item) - item.transform.position;
    item.rig.AddForce(vector3_1 * this.holdForce, ForceMode.Acceleration);
    Vector3 itemHoldForward = this.GetItemHoldForward(item);
    Vector3 itemHoldUp = this.GetItemHoldUp(item);
    Vector3 vector3_2 = Vector3.Cross(item.transform.forward, itemHoldForward).normalized * Vector3.Angle(item.transform.forward, itemHoldForward) + Vector3.Cross(item.transform.up, itemHoldUp).normalized * Vector3.Angle(item.transform.up, itemHoldUp);
    item.rig.AddTorque(vector3_2 * this.holdTorque, ForceMode.Acceleration);
  }

  private void Update()
  {
    this.DoSwitching();
    this.DoDropping();
    this.DoUsing();
    this.UpdateClimbingSpikeUse();
    if (!this.character.IsLocal)
      return;
    this.isChargingThrow = (double) this.throwChargeLevel > 0.0;
  }

  private void DoUsing()
  {
    if (!(bool) (UnityEngine.Object) this.character.data.currentItem || this.character.data.passedOut || this.character.data.fullyPassedOut)
      return;
    if (this.character.input.usePrimaryWasPressed && this.character.data.currentItem.CanUsePrimary())
      this.character.data.currentItem.StartUsePrimary();
    if (this.character.input.usePrimaryIsPressed && this.character.data.currentItem.CanUsePrimary())
      this.character.data.currentItem.ContinueUsePrimary();
    if (this.character.input.usePrimaryWasReleased || this.character.data.currentItem.isUsingPrimary && !this.character.data.currentItem.CanUsePrimary())
      this.character.data.currentItem.CancelUsePrimary();
    if (!this.character.CanDoInput())
      this.character.data.currentItem.CancelUsePrimary();
    if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
      this.character.data.currentItem.StartUseSecondary();
    if (this.character.input.useSecondaryIsPressed && this.character.data.currentItem.CanUseSecondary())
      this.character.data.currentItem.ContinueUseSecondary();
    if (this.character.input.useSecondaryWasReleased || this.character.data.currentItem.isUsingSecondary && !this.character.data.currentItem.CanUseSecondary())
      this.character.data.currentItem.CancelUseSecondary();
    if (this.character.input.scrollBackwardWasPressed)
      this.character.data.currentItem.ScrollButtonBackwardPressed();
    if (this.character.input.scrollForwardWasPressed)
      this.character.data.currentItem.ScrollButtonForwardPressed();
    if (this.character.input.scrollBackwardIsPressed)
      this.character.data.currentItem.ScrollButtonBackwardHeld();
    if (this.character.input.scrollForwardIsPressed)
      this.character.data.currentItem.ScrollButtonForwardHeld();
    if ((double) this.character.input.scrollInput == 0.0)
      return;
    this.character.data.currentItem.Scroll(this.character.input.scrollInput);
  }

  private void DoDropping()
  {
    if ((UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null && (double) this.character.data.currentItem.progress > 0.0)
    {
      this.throwChargeLevel = 0.0f;
    }
    else
    {
      if (this.character.input.dropWasPressed && (bool) (UnityEngine.Object) this.character.data.currentItem && this.character.data.currentItem.UIData.canDrop)
      {
        this.lastPressedDrop = Time.time;
        this.pressedDrop = true;
      }
      if (this.pressedDrop && this.character.input.dropWasReleased && (bool) (UnityEngine.Object) this.character.data.currentItem && this.currentSelectedSlot.IsSome)
      {
        Vector3 position = this.character.data.currentItem.transform.position;
        Vector3 vector3 = this.character.data.currentItem.rig.linearVelocity;
        if (this.character.data.currentItem is Backpack)
          vector3 = Vector3.zero;
        bool flag = false;
        if ((UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null)
        {
          StickyItemComponent currentStickyItem = this.character.data.currentStickyItem;
          if ((bool) (UnityEngine.Object) currentStickyItem && (double) this.throwChargeLevel < (double) currentStickyItem.throwChargeRequirement)
            flag = true;
        }
        if ((double) this.throwChargeLevel > 0.10000000149011612 | flag && (bool) (UnityEngine.Object) this.transform.GetComponent<CharacterAnimations>())
          this.transform.GetComponent<CharacterAnimations>().throwTime = 0.125f;
        if (flag)
          return;
        ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
        this.photonView.RPC("DropItemRpc", RpcTarget.All, (object) this.throwChargeLevel, (object) this.currentSelectedSlot.Value, (object) position, (object) vector3, (object) this.character.data.currentItem.transform.rotation, (object) itemSlot.data);
        this.throwChargeLevel = 0.0f;
        this.EquipSlot(Optionable<byte>.None);
      }
      if (this.pressedDrop && this.character.input.dropIsPressed && (double) Time.time - (double) this.lastPressedDrop > (double) this.delayBeforeThrowCharge)
        this.throwChargeLevel = Mathf.Min(this.throwChargeLevel + 1f / this.throwChargeTime * Time.deltaTime, 1f);
      else
        this.throwChargeLevel = 0.0f;
    }
  }

  internal void DropAllItems(bool includeBackpack)
  {
    if (!this.character.IsLocal)
      return;
    Transform transform = this.character.GetBodypart(BodypartType.Hip).transform;
    Vector3 lhs = transform.forward;
    if ((double) Vector3.Dot(lhs, Vector3.up) < 0.0)
      lhs = -lhs;
    Vector3 vector3 = transform.position + lhs * 0.6f;
    if (this.currentSelectedSlot.IsSome && (bool) (UnityEngine.Object) this.character.data.currentItem)
    {
      this.photonView.RPC("DropItemRpc", RpcTarget.All, (object) this.throwChargeLevel, (object) this.currentSelectedSlot.Value, (object) this.character.data.currentItem.transform.position, (object) Vector3.zero, (object) this.character.data.currentItem.transform.rotation, (object) this.character.player.GetItemSlot(this.currentSelectedSlot.Value).data);
      vector3 += Vector3.up * 0.5f;
    }
    for (int index = includeBackpack ? 3 : 2; index >= 0; --index)
    {
      this.photonView.RPC("DropItemFromSlotRPC", RpcTarget.All, (object) (byte) index, (object) vector3);
      vector3 += Vector3.up * 0.5f;
    }
  }

  [PunRPC]
  internal void DropItemFromSlotRPC(byte slotID, Vector3 spawnPosition)
  {
    Debug.Log((object) ("Trying to empty slot " + slotID.ToString()));
    ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
    if (!itemSlot.IsEmpty())
    {
      if (PhotonNetwork.IsMasterClient)
      {
        PhotonView component = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), spawnPosition, Quaternion.identity).GetComponent<PhotonView>();
        component.RPC("SetItemInstanceDataRPC", RpcTarget.All, (object) itemSlot.data);
        component.RPC("SetKinematicRPC", RpcTarget.All, (object) false, (object) component.transform.position, (object) component.transform.rotation);
      }
      this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
    }
    this.character.refs.afflictions.UpdateWeight();
  }

  [PunRPC]
  public void DestroyHeldItemRpc()
  {
    Item currentItem = this.character.data.currentItem;
    if ((UnityEngine.Object) currentItem == (UnityEngine.Object) null)
      return;
    this.UnAttatchEquipedItem();
    if (!currentItem.photonView.IsMine && (!currentItem.photonView.Controller.IsMasterClient || !PhotonNetwork.IsMasterClient))
      return;
    PhotonNetwork.Destroy(currentItem.gameObject);
  }

  [PunRPC]
  public void DropItemRpc(
    float throwCharge,
    byte slotID,
    Vector3 spawnPos,
    Vector3 velocity,
    Quaternion rotation,
    ItemInstanceData itemInstanceData)
  {
    if (!(bool) (UnityEngine.Object) this.character.data.currentItem)
      return;
    float num = 0.0f;
    if ((double) throwCharge > 0.0)
      num = this.minThrowForce + (this.maxThrowForce - this.minThrowForce) * throwCharge;
    Item currentItem = this.character.data.currentItem;
    this.UnAttatchEquipedItem();
    if (currentItem.photonView.IsMine || currentItem.photonView.Controller.IsMasterClient && PhotonNetwork.IsMasterClient)
      PhotonNetwork.Destroy(currentItem.gameObject);
    ItemSlot itemSlot = this.character.player.GetItemSlot(slotID);
    if (itemSlot == null)
      return;
    if (PhotonNetwork.IsMasterClient)
    {
      string prefabName = itemSlot.GetPrefabName();
      if (string.IsNullOrEmpty(prefabName))
        return;
      Vector3 normalized = HelperFunctions.LookToDirection((Vector3) this.character.data.lookValues, Vector3.forward).normalized;
      PhotonView component1 = PhotonNetwork.InstantiateItemRoom(prefabName, spawnPos, rotation).GetComponent<PhotonView>();
      GameUtils.instance.IgnoreCollisions(this.character.gameObject, component1.gameObject, 0.5f);
      Rigidbody component2 = component1.GetComponent<Rigidbody>();
      component1.RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) false, (object) component1.transform.position, (object) component1.transform.rotation);
      Item component3 = component1.GetComponent<Item>();
      if ((bool) (UnityEngine.Object) component3)
        component3.photonView.RPC("RPC_SetThrownData", RpcTarget.All, (object) this.character.photonView.ViewID, (object) throwCharge);
      component2.linearVelocity = velocity + normalized * num * 0.5f * component3.throwForceMultiplier;
      if (!(bool) (UnityEngine.Object) component3.GetComponent<Frisbee>())
        component2.angularVelocity = Vector3.Cross(normalized, Vector3.up) * num * 0.5f;
      component1.RPC("SetItemInstanceDataRPC", RpcTarget.All, (object) itemInstanceData);
    }
    this.character.player.EmptySlot(Optionable<byte>.Some(slotID));
    this.pressedDrop = false;
    this.character.refs.afflictions.UpdateWeight();
  }

  [PunRPC]
  public void OnPickupAccepted(byte slotID)
  {
    if (slotID != (byte) 3)
    {
      if (!this.character.data.isClimbingAnything)
        this.character.refs.items.EquipSlot(Optionable<byte>.Some(slotID));
    }
    else if ((UnityEngine.Object) this.character.data.carriedPlayer != (UnityEngine.Object) null)
      this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
    this.RefreshAllCharacterCarryWeight();
  }

  public void RefreshAllCharacterCarryWeight()
  {
    this.photonView.RPC("RefreshAllCharacterCarryWeightRPC", RpcTarget.All);
  }

  [PunRPC]
  public void RefreshAllCharacterCarryWeightRPC()
  {
    List<Character> playerCharacters = PlayerHandler.GetAllPlayerCharacters();
    for (int index = 0; index < playerCharacters.Count; ++index)
      playerCharacters[index].refs.afflictions.UpdateWeight();
  }

  public void EquipSlot(Optionable<byte> slotID)
  {
    this.lastEquippedSlotTime = Time.time;
    bool waitForFrames = false;
    if (slotID.IsSome)
      this.lastSelectedSlot = slotID;
    if (this.photonView.IsMine && (UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null)
    {
      this.character.data.currentItem.CancelUsePrimary();
      this.character.data.currentItem.CancelUseSecondary();
      if (!this.character.data.currentItem.UIData.canPocket || this.currentSelectedSlot.IsSome && this.currentSelectedSlot.Value == (byte) 250 && !this.character.player.GetItemSlot(this.currentSelectedSlot.Value).IsEmpty())
      {
        Vector3 vector3 = this.character.data.currentItem.transform.position + Vector3.down * 0.2f;
        Vector3 linearVelocity = this.character.data.currentItem.rig.linearVelocity;
        waitForFrames = true;
        ItemSlot itemSlot = this.character.player.GetItemSlot(this.currentSelectedSlot.Value);
        this.photonView.RPC("DropItemRpc", RpcTarget.All, (object) this.throwChargeLevel, (object) this.currentSelectedSlot.Value, (object) vector3, (object) linearVelocity, (object) this.character.data.currentItem.transform.rotation, (object) itemSlot.data);
      }
    }
    this.StartCoroutine(TheRest());

    IEnumerator TheRest()
    {
      if (waitForFrames)
        yield return (object) new WaitForSecondsRealtime(0.15f);
      this.currentSelectedSlot = slotID;
      if (this.photonView.IsMine)
      {
        int num = -1;
        if (slotID.IsSome)
        {
          ItemSlot itemSlot = this.character.player.GetItemSlot(slotID.Value);
          if (!itemSlot.IsEmpty())
          {
            Transform transform = this.character.GetBodypart(BodypartType.Torso).transform;
            PhotonView component = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), transform.position + transform.forward * 0.6f, Quaternion.identity).GetComponent<PhotonView>();
            component.RPC("SetItemInstanceDataRPC", RpcTarget.All, (object) itemSlot.data);
            num = component.ViewID;
          }
          this.photonView.RPC("EquipSlotRpc", RpcTarget.All, (object) (int) slotID.Value, (object) num);
        }
        else
          this.photonView.RPC("EquipSlotRpc", RpcTarget.All, (object) -1, (object) num);
        this.character.refs.afflictions.UpdateWeight();
      }
    }
  }

  [PunRPC]
  public void EquipSlotRpc(int slotID, int objectViewID)
  {
    if (!this.photonView.IsMine)
    {
      if (slotID == -1)
      {
        if (this.currentSelectedSlot.IsSome)
          this.lastSelectedSlot = this.currentSelectedSlot;
        this.currentSelectedSlot = Optionable<byte>.None;
      }
      else
        this.currentSelectedSlot = Optionable<byte>.Some((byte) slotID);
    }
    PhotonView photonView = (PhotonView) null;
    if (objectViewID != -1)
      photonView = PhotonNetwork.GetPhotonView(objectViewID);
    Item obj = !((UnityEngine.Object) photonView != (UnityEngine.Object) null) ? this.Equip((Item) null) : this.Equip(photonView.GetComponent<Item>());
    if (this.photonView.IsMine && (UnityEngine.Object) obj != (UnityEngine.Object) null)
    {
      obj.OnStash();
      Debug.Log((object) $"{this.character.gameObject.name} destroying {obj.gameObject.name}");
      PhotonNetwork.Destroy(obj.GetComponent<PhotonView>());
    }
    if (this.character.player.itemsChangedAction != null)
      this.character.player.itemsChangedAction(this.character.player.itemSlots);
    Action onSlotEquipped = this.onSlotEquipped;
    if (onSlotEquipped == null)
      return;
    onSlotEquipped();
  }

  public Item Equip(Item item)
  {
    Item currentItem = this.character.data.currentItem;
    this.pressedDrop = false;
    if ((bool) (UnityEngine.Object) this.character.data.currentItem)
      this.UnAttatchEquipedItem();
    if ((UnityEngine.Object) item == (UnityEngine.Object) null)
      return currentItem;
    this.character.data.currentItem = item;
    item.holderCharacter = this.character;
    item.SetState(ItemState.Held, this.character);
    this.StartCoroutine(IWait());
    return currentItem;

    IEnumerator IWait()
    {
      item.Move(this.GetItemHoldPos(item), this.GetItemHoldRotation(item));
      this.character.refs.ragdoll.FixedUpdate();
      this.character.refs.ragdoll.SnapToAnimation();
      int c = 0;
      foreach (Collider collider in item.colliders)
        collider.enabled = false;
      while (c < 3 && (bool) (UnityEngine.Object) item)
      {
        ++c;
        this.character.data.sinceItemAttach = 0.0f;
        item.Move(this.GetItemHoldPos(item), this.GetItemHoldRotation(item));
        this.character.refs.ragdoll.FixedUpdate();
        this.character.refs.ragdoll.SnapToAnimation();
        item.rig.angularVelocity *= 0.0f;
        item.rig.linearVelocity *= 0.0f;
        yield return (object) new WaitForFixedUpdate();
      }
      yield return (object) new WaitForFixedUpdate();
      foreach (Collider collider in item.colliders)
      {
        if ((UnityEngine.Object) collider != (UnityEngine.Object) null)
          collider.enabled = true;
        else
          Debug.LogError((object) "NULL ITEM");
      }
      if ((bool) (UnityEngine.Object) item)
        this.AttachItem(item);
    }
  }

  private void AttachItem(Item item)
  {
    this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(item);
    this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(item);
    this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(item);
    this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(item);
    this.character.GetBodypartRig(BodypartType.Hand_R).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
    this.character.GetBodypartRig(BodypartType.Hand_L).gameObject.AddComponent<FixedJoint>().connectedBody = item.rig;
  }

  public void UpdateAttachedItem()
  {
    if (!(bool) (UnityEngine.Object) this.character.data.currentItem)
      return;
    this.character.GetBodypartRig(BodypartType.Hand_R).transform.position = this.GetItemPosRightWorld(this.character.data.currentItem);
    this.character.GetBodypartRig(BodypartType.Hand_L).transform.position = this.GetItemPosLeftWorld(this.character.data.currentItem);
    this.character.GetBodypartRig(BodypartType.Hand_R).transform.rotation = this.GetItemRotRightWorld(this.character.data.currentItem);
    this.character.GetBodypartRig(BodypartType.Hand_L).transform.rotation = this.GetItemRotLeftWorld(this.character.data.currentItem);
  }

  private void UnAttachItem()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.character.GetBodypartRig(BodypartType.Hand_R).gameObject.GetComponent<FixedJoint>());
    UnityEngine.Object.Destroy((UnityEngine.Object) this.character.GetBodypartRig(BodypartType.Hand_L).gameObject.GetComponent<FixedJoint>());
  }

  private Quaternion GetItemHoldRotation(Item item)
  {
    return Quaternion.LookRotation(this.GetItemHoldForward(item), this.GetItemHoldUp(item));
  }

  private Vector3 GetItemHoldUp(Item item) => this.character.data.lookDirection_Up;

  private Vector3 GetItemHoldForward(Item item) => this.character.data.lookDirection;

  public Vector3 GetItemHoldPos(Item item)
  {
    return this.character.refs.hip.transform.position + (this.character.refs.animationItemTransform.position - this.character.refs.animationHipTransform.position);
  }

  public void UnAttatchEquipedItem()
  {
    this.UnAttachItem();
    this.character.data.currentItem = (Item) null;
  }

  private float equippedSlotCooldown => this.timesSwitchedRecently >= 3 ? 0.25f : 0.1f;

  private bool lockedFromSwitching
  {
    get
    {
      return (double) this.lastEquippedSlotTime + (double) this.equippedSlotCooldown > (double) Time.time;
    }
  }

  private void DoSwitching()
  {
    if (this.timesSwitchedRecently > 0 && (double) this.lastSwitched + 0.40000000596046448 < (double) Time.time)
      this.timesSwitchedRecently = 0;
    if ((UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null && ((double) this.character.data.currentItem.progress > 0.0 || (double) this.character.data.currentItem.lastFinishedCast + 0.10000000149011612 > (double) Time.time) || !this.character.data.fullyConscious || !this.character.IsLocal || !this.character.CanDoInput() || this.lockedFromSwitching || this.character.data.isClimbing || this.character.data.isRopeClimbing)
      return;
    if (this.character.input.selectSlotForwardWasPressed)
    {
      if ((bool) (UnityEngine.Object) this.character.data.currentStickyItem)
      {
        this.character.refs.animations.throwTime = 0.125f;
        return;
      }
      this.character.player.GetItemSlot((byte) 3).IsEmpty();
      byte num = Decimal.ToByte((Decimal) ((int) this.lastSelectedSlot.Value + 1));
      if ((int) num > this.character.player.itemSlots.Length)
      {
        Debug.Log((object) "Looping to start");
        num = (byte) 0;
      }
      this.lastSwitched = Time.time;
      ++this.timesSwitchedRecently;
      this.EquipSlot(Optionable<byte>.Some(num));
    }
    else if (this.character.input.selectSlotBackwardWasPressed)
    {
      if ((bool) (UnityEngine.Object) this.character.data.currentStickyItem)
      {
        this.character.refs.animations.throwTime = 0.125f;
        return;
      }
      this.character.player.GetItemSlot((byte) 3).IsEmpty();
      int num = (int) this.lastSelectedSlot.Value - 1;
      if (num < 0)
      {
        num = (int) Decimal.ToByte(3M);
        Debug.Log((object) "Looping to end");
      }
      this.lastSwitched = Time.time;
      ++this.timesSwitchedRecently;
      this.EquipSlot(Optionable<byte>.Some(Decimal.ToByte((Decimal) num)));
    }
    else if (this.character.input.unselectSlotWasPressed && !this.lockedFromSwitching)
    {
      if ((bool) (UnityEngine.Object) this.character.data.currentStickyItem)
      {
        this.character.refs.animations.throwTime = 0.125f;
        return;
      }
      if (this.currentSelectedSlot.IsSome)
      {
        this.lastSwitched = Time.time;
        ++this.timesSwitchedRecently;
        this.EquipSlot(Optionable<byte>.None);
      }
      else
      {
        this.lastSwitched = Time.time;
        ++this.timesSwitchedRecently;
        this.EquipSlot(this.lastSelectedSlot);
      }
    }
    for (byte index = 0; index <= (byte) 3; ++index)
    {
      if (this.character.input.SelectSlotWasPressed((int) index))
      {
        if ((bool) (UnityEngine.Object) this.character.data.currentStickyItem)
        {
          this.character.refs.animations.throwTime = 0.125f;
          break;
        }
        if (!this.character.player.itemSlots.WithinRange<ItemSlot>((int) index) && index != (byte) 3)
        {
          this.lastSwitched = Time.time;
          ++this.timesSwitchedRecently;
          this.EquipSlot(Optionable<byte>.None);
        }
        else
        {
          if (this.currentSelectedSlot.IsSome && (int) this.currentSelectedSlot.Value == (int) index)
          {
            this.lastSwitched = Time.time;
            ++this.timesSwitchedRecently;
            this.EquipSlot(Optionable<byte>.None);
          }
          else
          {
            this.lastSwitched = Time.time;
            ++this.timesSwitchedRecently;
            this.EquipSlot(Optionable<byte>.Some(index));
          }
          if (index == (byte) 3 && (UnityEngine.Object) this.character.data.carriedPlayer != (UnityEngine.Object) null)
            this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
        }
      }
    }
  }

  internal void AddGravity(Vector3 gravity)
  {
    this.character.data.currentItem.rig.AddForce(gravity, ForceMode.Acceleration);
  }

  internal void AddMovementForce(float movementForce)
  {
    this.character.data.currentItem.rig.AddForce(movementForce * this.character.data.worldMovementInput_Grounded, ForceMode.Acceleration);
  }

  internal void AddDrag(float drag, float factor = 1f)
  {
    drag = Mathf.Lerp(1f, drag, factor);
    this.character.data.currentItem.rig.linearVelocity *= drag;
    this.character.data.currentItem.rig.angularVelocity *= drag;
  }

  internal void AddParasolDrag(float drag, float xzDrag, float factor = 1f)
  {
    drag = Mathf.Lerp(1f, drag, factor);
    if ((double) this.character.data.currentItem.rig.linearVelocity.y >= 0.0)
      return;
    this.character.data.currentItem.rig.linearVelocity = new Vector3(this.character.data.currentItem.rig.linearVelocity.x * xzDrag, this.character.data.currentItem.rig.linearVelocity.y * drag, this.character.data.currentItem.rig.linearVelocity.z * xzDrag);
  }

  internal Vector3 GetItemPosRightWorld(Item item) => item.transform.Find("Hand_R").position;

  internal Vector3 GetItemPosLeftWorld(Item item) => item.transform.Find("Hand_L").position;

  internal Quaternion GetItemRotRightWorld(Item item) => item.transform.Find("Hand_R").rotation;

  internal Quaternion GetItemRotLeftWorld(Item item) => item.transform.Find("Hand_L").rotation;

  internal Vector3 GetItemPosRight(Item item)
  {
    return this.character.refs.animationItemTransform.TransformPoint(item.transform.Find("Hand_R").localPosition);
  }

  internal Quaternion GetItemRotRight(Item item)
  {
    Transform transform = item.transform.Find("Hand_R");
    Vector3 direction1 = item.transform.InverseTransformDirection(transform.forward);
    Vector3 direction2 = item.transform.InverseTransformDirection(transform.up);
    return Quaternion.LookRotation(this.character.refs.animationItemTransform.TransformDirection(direction1), this.character.refs.animationItemTransform.TransformDirection(direction2));
  }

  internal Quaternion GetItemRotLeft(Item item)
  {
    Transform transform = item.transform.Find("Hand_L");
    Vector3 direction1 = item.transform.InverseTransformDirection(transform.forward);
    Vector3 direction2 = item.transform.InverseTransformDirection(transform.up);
    return Quaternion.LookRotation(this.character.refs.animationItemTransform.TransformDirection(direction1), this.character.refs.animationItemTransform.TransformDirection(direction2));
  }

  internal Vector3 GetItemPosLeft(Item item)
  {
    return this.character.refs.animationItemTransform.TransformPoint(HelperFunctions.MultiplyVectors(item.transform.Find("Hand_L").localPosition, item.transform.lossyScale));
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!PhotonNetwork.IsMasterClient || !((UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null))
      return;
    Debug.Log((object) $"Setting {this.gameObject.name} to hold {this.character.data.currentItem.name}");
    this.photonView.RPC("RPC_InitHoldingItem", newPlayer, (object) this.character.data.currentItem.GetComponent<PhotonView>());
  }

  [PunRPC]
  public void RPC_InitHoldingItem(PhotonView item)
  {
    Debug.Log((object) ("Init holding item: " + item.name));
    this.Equip(item.GetComponent<Item>());
  }

  public void UpdateClimbingSpikeCount(ItemSlot[] slots)
  {
    int num = 0;
    this.currentClimbingSpikeComponent = (ClimbingSpikeComponent) null;
    this.currentClimbingSpikeItemSlot = (ItemSlot) null;
    for (int index = 0; index < slots.Length; ++index)
    {
      ItemSlot slot = slots[index];
      if (slot != null && (UnityEngine.Object) slot.prefab != (UnityEngine.Object) null)
      {
        ClimbingSpikeComponent component = slot.prefab.GetComponent<ClimbingSpikeComponent>();
        IntItemData intItemData;
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && (!slot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || intItemData.Value <= 0))
        {
          ++num;
          if ((UnityEngine.Object) this.currentClimbingSpikeComponent == (UnityEngine.Object) null)
          {
            this.currentClimbingSpikeComponent = component;
            this.currentClimbingSpikeItemSlot = slot;
          }
        }
      }
    }
    this.character.data.climbingSpikeCount = num;
  }

  public void UpdateBalloonCount(ItemSlot[] slots)
  {
    if (this.character.refs.items.currentSelectedSlot.IsSome)
    {
      ItemSlot itemSlot = this.character.player.GetItemSlot(this.character.refs.items.currentSelectedSlot.Value);
      if (!itemSlot.IsEmpty() && this.character.refs.items.currentSelectedSlot.Value != (byte) 3 && this.character.refs.items.currentSelectedSlot.IsSome && (bool) (UnityEngine.Object) itemSlot.prefab.GetComponent<Balloon>())
        this.character.refs.balloons.heldBalloonCount = 1;
      else
        this.character.refs.balloons.heldBalloonCount = 0;
    }
    else
      this.character.refs.balloons.heldBalloonCount = 0;
  }

  private bool WithinClimbingSpikePreviewRange()
  {
    if (!(bool) (UnityEngine.Object) this.currentClimbingSpikePreview)
      return false;
    float num = this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistance : this.currentClimbingSpikeComponent.climbingSpikePreviewDisableDistanceGrounded;
    return (double) Vector3.Distance(MainCamera.instance.transform.position, this.currentClimbingSpikePreview.transform.position) <= (double) num;
  }

  public float climbingSpikeCastProgress
  {
    get
    {
      return this.currentClimbingSpikeItemSlot == null || (UnityEngine.Object) this.currentClimbingSpikeItemSlot.prefab == (UnityEngine.Object) null ? 0.0f : this.climbingSpikeTick / this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary;
    }
  }

  private void UpdateClimbingSpikeUse()
  {
    if (this.character.data.climbingSpikeCount <= 0 || this.currentClimbingSpikeItemSlot == null)
      this.CancelClimbingSpike();
    else if ((double) this.climbingSpikeTick > 0.0)
    {
      if (!this.WithinClimbingSpikePreviewRange())
        this.CancelClimbingSpike();
      else if (this.spikingWithPrimary && !this.character.input.usePrimaryIsPressed || this.spikingWithSecondary && !this.character.input.useSecondaryIsPressed)
      {
        this.CancelClimbingSpike();
      }
      else
      {
        this.climbingSpikeTick += Time.deltaTime;
        if ((double) this.climbingSpikeTick < (double) this.currentClimbingSpikeItemSlot.prefab.usingTimePrimary)
          return;
        this.HammerClimbingSpike(this.climbingSpikeHit);
        this.CancelClimbingSpike();
      }
    }
    else if (!this.RaycastClimbingSpikeStart())
    {
      this.climbingSpikeTick = 0.0f;
    }
    else
    {
      if ((double) this.climbingSpikeTick != 0.0)
        return;
      if (this.character.input.usePrimaryIsPressed && !this.character.data.isClimbingAnything && this.climbingSpikeSelected)
      {
        this.spikingWithPrimary = true;
        this.spikingWithSecondary = false;
        this.climbingSpikeTick += Time.deltaTime;
        this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
      }
      else
      {
        if (!this.character.input.useSecondaryIsPressed || !this.climbingSpikeSelected && !this.character.data.isClimbingAnything)
          return;
        this.spikingWithPrimary = false;
        this.spikingWithSecondary = true;
        this.climbingSpikeTick += Time.deltaTime;
        this.InstantiateClimbingSpikePreview(this.climbingSpikeHit);
      }
    }
  }

  private bool climbingSpikeSelected
  {
    get
    {
      return this.currentClimbingSpikeItemSlot != null && this.currentSelectedSlot.IsSome && (int) this.currentSelectedSlot.Value == (int) this.currentClimbingSpikeItemSlot.itemSlotID;
    }
  }

  private void CancelClimbingSpike()
  {
    if ((bool) (UnityEngine.Object) this.currentClimbingSpikePreview)
    {
      Debug.Log((object) "Cancelling climbing spike");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.currentClimbingSpikePreview);
    }
    this.climbingSpikeTick = 0.0f;
  }

  private void InstantiateClimbingSpikePreview(RaycastHit hit)
  {
    if (!(bool) (UnityEngine.Object) this.currentClimbingSpikePreview && (UnityEngine.Object) this.currentClimbingSpikeComponent != (UnityEngine.Object) null)
      this.currentClimbingSpikePreview = UnityEngine.Object.Instantiate<GameObject>(this.currentClimbingSpikeComponent.climbingSpikePreviewPrefab);
    if (!(bool) (UnityEngine.Object) this.currentClimbingSpikePreview)
      return;
    this.currentClimbingSpikePreview.transform.position = this.climbingSpikeHit.point;
    this.currentClimbingSpikePreview.transform.rotation = Quaternion.LookRotation(-this.climbingSpikeHit.normal, Vector3.up);
  }

  public bool RaycastClimbingSpikeStart()
  {
    float maxDistance = this.character.data.isClimbingAnything ? this.currentClimbingSpikeComponent.climbingSpikeStartDistance : this.currentClimbingSpikeComponent.climbingSpikeStartDistanceGrounded;
    return Physics.Raycast(MainCamera.instance.transform.position, MainCamera.instance.transform.forward, out this.climbingSpikeHit, maxDistance, (int) HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap));
  }

  private void HammerClimbingSpike(RaycastHit hit)
  {
    if (!((UnityEngine.Object) this.currentClimbingSpikeComponent != (UnityEngine.Object) null) || !((UnityEngine.Object) PhotonNetwork.Instantiate("0_Items/" + this.currentClimbingSpikeComponent.hammeredVersionPrefab.gameObject.name, hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up)) != (UnityEngine.Object) null))
      return;
    if (this.currentClimbingSpikeItemSlot != null)
    {
      ItemSlot climbingSpikeItemSlot = this.currentClimbingSpikeItemSlot;
      this.currentClimbingSpikeItemSlot = (ItemSlot) null;
      this.currentClimbingSpikeComponent = (ClimbingSpikeComponent) null;
      this.character.player.EmptySlot(Optionable<byte>.Some(climbingSpikeItemSlot.itemSlotID));
      if ((UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null)
        this.EquipSlot(Optionable<byte>.None);
      this.UpdateClimbingSpikeCount(this.character.player.itemSlots);
      this.character.data.lastConsumedItem = Time.time;
    }
    this.character.refs.afflictions.UpdateWeight();
    Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PitonsPlaced, 1);
    GameUtils.instance.IncrementPermanentItemsPlaced();
  }

  internal void SpawnItemInHand(string objName)
  {
    this.photonView.RPC("RPC_SpawnItemInHandMaster", RpcTarget.MasterClient, (object) objName);
  }

  [PunRPC]
  private void RPC_SpawnItemInHandMaster(string objName)
  {
    PhotonNetwork.Instantiate("0_Items/" + objName, this.character.Center + Vector3.up * 3f, Quaternion.identity).GetComponent<Item>().Interact(this.character);
  }
}
