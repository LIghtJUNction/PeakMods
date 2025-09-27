// Decompiled with JetBrains decompiler
// Type: Item
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using WebSocketSharp;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.Serizalization;
using Zorro.Settings;

#nullable disable
public class Item : MonoBehaviourPunCallbacks, IInteractible
{
  public static readonly int PROPERTY_INTERACTABLE = Shader.PropertyToID("_Interactable");
  public static List<Item> ALL_ITEMS = new List<Item>();
  public static List<Item> ALL_ACTIVE_ITEMS = new List<Item>();
  public Vector3 defaultPos;
  public Vector3 defaultForward = new Vector3(0.0f, 0.0f, 1f);
  public float mass = 5f;
  public float throwForceMultiplier = 1f;
  [SerializeField]
  private int carryWeight = 1;
  internal float lastThrownAmount;
  internal Character lastThrownCharacter;
  internal float lastThrownTime;
  public float usingTimePrimary;
  public bool showUseProgress = true;
  public Action OnPrimaryStarted;
  public Action OnPrimaryHeld;
  public Action OnPrimaryFinishedCast;
  public Action OnPrimaryReleased;
  public Action OnPrimaryCancelled;
  public Action OnConsumed;
  public Action OnSecondaryStarted;
  public Action OnSecondaryHeld;
  public Action OnSecondaryFinishedCast;
  public Action OnSecondaryCancelled;
  public Action<ItemState> OnStateChange;
  public Action<float> OnScrolled;
  public Action<float> OnScrolledMouseOnly;
  public Action OnScrollBackwardPressed;
  public Action OnScrollForwardPressed;
  public Action OnScrollBackwardHeld;
  public Action OnScrollForwardHeld;
  public Item.ItemUIData UIData;
  [NonSerialized]
  public Transform backpackSlotTransform;
  private Optionable<(byte, BackpackReference)> backpackReference;
  private Optionable<RigidbodySyncData> m_lastState = Optionable<RigidbodySyncData>.None;
  protected PhotonView view;
  public int totalUses = -1;
  public ItemInstanceData data;
  public Item.ItemTags itemTags;
  public Rigidbody rig;
  internal ItemActionBase[] itemActions;
  [HideInInspector]
  public Collider[] colliders;
  public ushort itemID;
  private MaterialPropertyBlock mpb;
  public Renderer mainRenderer;
  private double timeSinceTick;
  private ItemComponent[] itemComponents;
  protected Color originalTint;
  private ItemPhysicsSyncer physicsSyncer;
  [HideInInspector]
  public ItemParticles particles;
  private int packLayer;
  private float destroyTick;
  public Vector3 centerOfMass;
  private Character lastHolderCharacter;
  [ReadOnly]
  public Character wearerCharacter;
  [SerializeField]
  [ReadOnly]
  private Character _holderCharacter;
  [ReadOnly]
  public Character overrideHolderCharacter;
  public bool canUseOnFriend;
  [HideInInspector]
  public bool finishedCast;
  [HideInInspector]
  public float lastFinishedCast;
  internal float overrideProgress;
  internal Optionable<bool> overrideUsability;
  public Action onStashAction;
  internal bool overrideForceProgress;
  private float timeSinceWasActive;

  public ItemState itemState { get; set; }

  public int CarryWeight => this.carryWeight + Ascents.itemWeightModifier;

  public bool isUsingPrimary { get; private set; }

  public ItemCooking cooking { get; private set; }

  protected virtual void Awake()
  {
    this.view = this.GetComponent<PhotonView>();
    this.cooking = this.gameObject.GetOrAddComponent<ItemCooking>();
    this.AddPhysics();
    this.GetItemActions();
    this.AddPropertyBlock();
    this.particles = this.GetComponent<ItemParticles>();
    if (!(bool) (UnityEngine.Object) this.particles)
      this.particles = this.gameObject.AddComponent<ItemParticles>();
    this.itemComponents = this.GetComponents<ItemComponent>();
    this.physicsSyncer = this.GetComponent<ItemPhysicsSyncer>();
    Item.ALL_ITEMS.Add(this);
  }

  protected virtual void Start()
  {
    if (!this.HasData(DataEntryKey.ItemUses))
    {
      OptionableIntItemData data = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
      data.HasData = this.totalUses != -1;
      data.Value = this.totalUses;
      if (this.totalUses > 0)
        this.SetUseRemainingPercentage(1f);
    }
    if (!this.rig.isKinematic)
      this.WasActive();
    this.packLayer = 1 << LayerMask.NameToLayer("Exclude Collisions");
  }

  public string GetItemName(ItemInstanceData data = null)
  {
    int num = 0;
    if (data == null)
    {
      num = this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value;
    }
    else
    {
      IntItemData intItemData;
      if (data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
        num = intItemData.Value;
    }
    string itemName;
    if (num < 4)
    {
      switch (num)
      {
        case 1:
          itemName = LocalizedText.GetText("COOKED_COOKED").Replace("#", this.GetName());
          break;
        case 2:
          itemName = LocalizedText.GetText("COOKED_WELLDONE").Replace("#", this.GetName());
          break;
        case 3:
          itemName = LocalizedText.GetText("COOKED_BURNT").Replace("#", this.GetName());
          break;
        default:
          itemName = this.GetName();
          break;
      }
    }
    else
      itemName = LocalizedText.GetText("COOKED_INCINERATED").Replace("#", this.GetName());
    return itemName;
  }

  private void AddPropertyBlock()
  {
    this.mpb = new MaterialPropertyBlock();
    this.mainRenderer = (Renderer) this.GetComponentInChildren<MeshRenderer>();
    if (!(bool) (UnityEngine.Object) this.mainRenderer)
      this.mainRenderer = (Renderer) this.GetComponentInChildren<SkinnedMeshRenderer>();
    this.mainRenderer.GetPropertyBlock(this.mpb);
  }

  private void GetItemActions()
  {
    this.itemActions = this.GetComponentsInChildren<ItemActionBase>();
  }

  protected virtual void AddPhysics()
  {
    this.rig = this.gameObject.GetOrAddComponent<Rigidbody>();
    this.rig.mass = this.mass;
    this.centerOfMass = this.rig.centerOfMass;
    this.rig.interpolation = RigidbodyInterpolation.Interpolate;
    this.rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    this.colliders = this.GetComponentsInChildren<Collider>();
  }

  protected virtual void Update()
  {
    if (this.itemState == ItemState.InBackpack)
    {
      if ((UnityEngine.Object) this.backpackSlotTransform == (UnityEngine.Object) null || !this.backpackSlotTransform.UnityObjectExists<Transform>())
      {
        this.transform.position = new Vector3(0.0f, -500f, 0.0f);
      }
      else
      {
        this.transform.position = this.backpackSlotTransform.position - this.backpackSlotTransform.rotation * this.centerOfMass * 0.5f;
        this.transform.rotation = this.backpackSlotTransform.rotation;
      }
    }
    else if (this.itemState == ItemState.Ground && this.photonView.IsMine)
    {
      if ((double) this.transform.position.y < -2000.0 || (double) this.transform.position.y > 4000.0)
      {
        this.destroyTick += Time.deltaTime;
        if ((double) this.destroyTick > 2.0)
          PhotonNetwork.Destroy(this.gameObject);
      }
      else
        this.destroyTick = 0.0f;
    }
    else if (this.itemState == ItemState.Held)
      this.WasActive();
    this.UpdateEntryInActiveList();
    this.UpdateCollisionDetectionMode();
  }

  private void UpdateCollisionDetectionMode()
  {
    if (this.itemState == ItemState.Ground)
      this.rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    else
      this.rig.collisionDetectionMode = CollisionDetectionMode.Discrete;
  }

  public virtual void Interact(Character interactor)
  {
    if (!interactor.player.HasEmptySlot(this.itemID) || (double) interactor.refs.items.lastEquippedSlotTime + 0.25 > (double) Time.time)
      return;
    if (interactor.data.isClimbing && !this.UIData.canPocket)
    {
      this.SetKinematicNetworked(false);
    }
    else
    {
      GlobalEvents.TriggerItemRequested(this, interactor);
      this.gameObject.SetActive(false);
      this.view.RPC("RequestPickup", RpcTarget.MasterClient, (object) interactor.GetComponent<PhotonView>());
      Debug.Log((object) ("Picking up " + this.gameObject.name));
      ItemBackpackVisuals component;
      if (!this.TryGetComponent<ItemBackpackVisuals>(out component))
        return;
      component.RemoveVisuals();
    }
  }

  [PunRPC]
  public void DenyPickupRPC()
  {
    this.gameObject.SetActive(true);
    this.SetKinematicNetworked(false, this.transform.position, this.transform.rotation);
  }

  [PunRPC]
  public void RequestPickup(PhotonView characterView)
  {
    Character component = characterView.GetComponent<Character>();
    ItemSlot slot;
    bool flag = component.player.AddItem(this.itemID, this.data, out slot);
    if (this.itemState != ItemState.InBackpack)
    {
      if (flag)
      {
        component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, (object) slot.itemSlotID);
        PhotonNetwork.Destroy(this.view);
      }
      else
        this.view.RPC("DenyPickupRPC", component.player.photonView.Owner);
    }
    else
    {
      if (!this.backpackReference.IsSome)
        return;
      if (flag)
      {
        this.ClearDataFromBackpack();
        component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, (object) slot.itemSlotID);
      }
      else
        this.view.RPC("DenyPickupRPC", component.player.photonView.Owner);
    }
  }

  public void ClearDataFromBackpack()
  {
    if (this.backpackReference.IsNone)
      return;
    (byte index, BackpackReference backpackReference) = this.backpackReference.Value;
    backpackReference.GetData().itemSlots[(int) index].EmptyOut();
    if (backpackReference.type == BackpackReference.BackpackType.Item)
    {
      backpackReference.view.RPC("SetItemInstanceDataRPC", RpcTarget.Others, (object) backpackReference.GetItemInstanceData());
    }
    else
    {
      Character component = backpackReference.view.GetComponent<Character>();
      byte[] managedArray = IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(component.player.itemSlots, component.player.backpackSlot, component.player.tempFullSlot));
      component.player.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, (object) managedArray, (object) false);
    }
    backpackReference.GetVisuals().RefreshVisuals();
  }

  public Vector3 Center()
  {
    return !this.mainRenderer.UnityObjectExists<Renderer>() ? this.transform.position : this.mainRenderer.bounds.center;
  }

  public Transform GetTransform() => this.transform;

  public virtual string GetInteractionText() => LocalizedText.GetText("PICKUP");

  public string GetName()
  {
    return LocalizedText.GetText(LocalizedText.GetNameIndex(this.UIData.itemName));
  }

  public virtual bool IsInteractible(Character interactor)
  {
    return this.itemState != ItemState.Held && this.itemState != ItemState.InBackpack;
  }

  internal void Move(Vector3 position, Quaternion rotation)
  {
    this.transform.position = position;
    this.transform.rotation = rotation;
    this.rig.position = position;
    this.rig.rotation = rotation;
    this.rig.linearVelocity *= 0.0f;
    this.rig.angularVelocity *= 0.0f;
  }

  public Character holderCharacter
  {
    get
    {
      return (bool) (UnityEngine.Object) this.overrideHolderCharacter ? this.overrideHolderCharacter : this._holderCharacter;
    }
    set
    {
      if ((UnityEngine.Object) value != (UnityEngine.Object) null)
        this.lastHolderCharacter = value;
      this._holderCharacter = value;
    }
  }

  public Character trueHolderCharacter => this._holderCharacter;

  private void SetColliders(bool enabled, bool isTrigger, bool excludeLayer = false)
  {
    for (int index = 0; index < this.colliders.Length; ++index)
    {
      this.colliders[index].enabled = enabled;
      this.colliders[index].isTrigger = isTrigger;
    }
    if (excludeLayer)
      this.rig.excludeLayers = (LayerMask) (1 << LayerMask.NameToLayer("Default"));
    else
      this.rig.excludeLayers = (LayerMask) 0;
  }

  internal void SetState(ItemState setState, Character character = null)
  {
    Debug.Log((object) $"Setting Item State: {setState}");
    this.itemState = setState;
    Action<ItemState> onStateChange = this.OnStateChange;
    if (onStateChange != null)
      onStateChange(setState);
    switch (setState)
    {
      case ItemState.Ground:
        this.holderCharacter = (Character) null;
        this.rig.useGravity = true;
        this.rig.isKinematic = false;
        this.rig.interpolation = RigidbodyInterpolation.Interpolate;
        this.centerOfMass = this.rig.centerOfMass;
        if (this is Backpack)
          this.wearerCharacter = (Character) null;
        this.SetColliders(true, false);
        this.transform.localScale = Vector3.one;
        break;
      case ItemState.Held:
        this.holderCharacter = character;
        this.rig.useGravity = false;
        this.rig.isKinematic = false;
        this.rig.interpolation = RigidbodyInterpolation.Interpolate;
        if (this is Backpack)
          this.wearerCharacter = (Character) null;
        if ((UnityEngine.Object) character != (UnityEngine.Object) null && PhotonNetwork.IsMasterClient)
          this.photonView.TransferOwnership(character.GetComponent<PhotonView>().Owner);
        this.SetColliders(true, false, true);
        this.transform.localScale = Vector3.one;
        break;
      case ItemState.InBackpack:
        this.holderCharacter = (Character) null;
        this.rig.useGravity = false;
        this.rig.isKinematic = true;
        this.rig.interpolation = RigidbodyInterpolation.None;
        this.SetColliders(false, true);
        this.transform.localScale = Vector3.one * 0.5f;
        break;
    }
  }

  private void HideRenderers()
  {
    ((IEnumerable<Renderer>) this.GetComponentsInChildren<Renderer>()).ForEach<Renderer>((Action<Renderer>) (meshRenderer => meshRenderer.enabled = false));
  }

  public bool isUsingSecondary { get; private set; }

  public float castProgress { get; private set; }

  public float progress => Mathf.Max(this.overrideProgress, this.castProgress);

  public bool shouldShowCastProgress
  {
    get
    {
      return this.showUseProgress && (double) this.castProgress > 0.0 && !this.finishedCast || this.overrideForceProgress;
    }
  }

  public virtual bool CanUsePrimary()
  {
    if (!this.overrideUsability.IsNone)
      return this.overrideUsability.Value;
    OptionableIntItemData data = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
    return !data.HasData || data.Value == -1 || data.Value > 0;
  }

  public virtual bool CanUseSecondary()
  {
    bool flag = true;
    OptionableIntItemData data = this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
    if (data.HasData)
      flag = data.Value == -1 || data.Value > 0;
    if (!flag)
      return false;
    if (this.canUseOnFriend)
    {
      if (Interaction.instance.hasValidTargetCharacter)
        return true;
    }
    else if (this.UIData.hasSecondInteract)
      return true;
    return false;
  }

  public void StartUsePrimary()
  {
    if (this.isUsingSecondary)
      this.CancelUseSecondary();
    this.isUsingPrimary = true;
    this.castProgress = 0.0f;
    this.finishedCast = false;
    if (this.OnPrimaryStarted == null)
      return;
    this.OnPrimaryStarted();
  }

  public void ContinueUsePrimary()
  {
    if (this.isUsingSecondary)
      this.CancelUseSecondary();
    if (!this.isUsingPrimary)
      return;
    if ((double) this.usingTimePrimary > 0.0)
    {
      this.castProgress += 1f / this.usingTimePrimary * Time.deltaTime;
      if ((double) this.castProgress < 1.0)
        return;
      if (this.OnPrimaryHeld != null)
        this.OnPrimaryHeld();
      if (this.finishedCast)
        return;
      this.FinishCastPrimary();
    }
    else
    {
      if (!this.finishedCast)
        this.FinishCastPrimary();
      if (this.OnPrimaryHeld == null)
        return;
      this.OnPrimaryHeld();
    }
  }

  protected virtual void FinishCastPrimary()
  {
    if ((bool) (UnityEngine.Object) this.GetComponent<ItemUseFeedback>())
    {
      this.holderCharacter.refs.animator.SetBool(this.GetComponent<ItemUseFeedback>().useAnimation, false);
      if ((bool) (UnityEngine.Object) this.GetComponent<ItemUseFeedback>().sfxUsed)
        this.GetComponent<ItemUseFeedback>().sfxUsed.Play(this.transform.position);
    }
    this.finishedCast = true;
    this.lastFinishedCast = Time.time;
    this.castProgress = 0.0f;
    if (this.OnPrimaryFinishedCast == null)
      return;
    this.OnPrimaryFinishedCast();
  }

  public void CancelUsePrimary()
  {
    this.isUsingPrimary = false;
    this.castProgress = 0.0f;
    this.finishedCast = false;
    if (this.OnPrimaryCancelled != null)
      this.OnPrimaryCancelled();
    if ((UnityEngine.Object) Player.localPlayer == (UnityEngine.Object) null)
      Debug.LogError((object) "Player.localPlayer is null, cannot play movement animation");
    else if ((UnityEngine.Object) Player.localPlayer.character == (UnityEngine.Object) null)
      Debug.LogError((object) "Player.localPlayer.character is null, cannot play movement animation");
    else if (Player.localPlayer.character.refs == null)
      Debug.LogError((object) "Player.localPlayer.character.refs is null, cannot play movement animation");
    else if ((UnityEngine.Object) Player.localPlayer.character.refs.animations == (UnityEngine.Object) null)
      Debug.LogError((object) "Player.localPlayer.character.refs.animations is null, cannot play movement animation");
    else
      Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
  }

  public void ScrollButtonBackwardPressed()
  {
    if (this.OnScrollBackwardPressed == null)
      return;
    this.OnScrollBackwardPressed();
  }

  public void ScrollButtonForwardPressed()
  {
    if (this.OnScrollForwardPressed == null)
      return;
    this.OnScrollForwardPressed();
  }

  public void ScrollButtonBackwardHeld()
  {
    if (this.OnScrollBackwardHeld == null)
      return;
    this.OnScrollBackwardHeld();
  }

  public void ScrollButtonForwardHeld()
  {
    if (this.OnScrollForwardHeld == null)
      return;
    this.OnScrollForwardHeld();
  }

  public void Scroll(float value)
  {
    if (this.OnScrolled != null)
      this.OnScrolled(value);
    if (InputHandler.GetCurrentUsedInputScheme() != InputScheme.KeyboardMouse || this.OnScrolledMouseOnly == null)
      return;
    this.OnScrolledMouseOnly(value);
  }

  public void StartUseSecondary()
  {
    if (this.isUsingPrimary || this.isUsingSecondary)
      return;
    this.isUsingSecondary = true;
    this.castProgress = 0.0f;
    this.finishedCast = false;
    if ((bool) (UnityEngine.Object) this.holderCharacter && this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
      this.photonView.RPC("SendFeedDataRPC", RpcTarget.All, (object) this.holderCharacter.photonView.ViewID, (object) Interaction.instance.bestCharacter.character.photonView.ViewID, (object) (int) this.itemID, (object) this.totalSecondaryUsingTime);
    if (this.OnSecondaryStarted == null)
      return;
    this.OnSecondaryStarted();
  }

  [PunRPC]
  internal void SendFeedDataRPC(int giverID, int recieverID, int itemID, float totalUsingTime)
  {
    GameUtils.instance.StartFeed(giverID, recieverID, (ushort) itemID, totalUsingTime);
  }

  [PunRPC]
  internal void RemoveFeedDataRPC(int giverID) => GameUtils.instance.EndFeed(giverID);

  public float totalSecondaryUsingTime
  {
    get => !this.canUseOnFriend ? this.usingTimePrimary : this.usingTimePrimary * 0.7f;
  }

  public void ContinueUseSecondary()
  {
    if (this.isUsingPrimary || !this.isUsingSecondary)
      return;
    if ((double) this.usingTimePrimary > 0.0)
    {
      this.castProgress += 1f / this.totalSecondaryUsingTime * Time.deltaTime;
      if ((double) this.castProgress < 1.0)
        return;
      if (this.OnSecondaryHeld != null)
        this.OnSecondaryHeld();
      if (this.finishedCast)
        return;
      this.FinishCastSecondary();
    }
    else
    {
      if (this.OnSecondaryHeld == null)
        return;
      this.OnSecondaryHeld();
    }
  }

  public void FinishCastSecondary()
  {
    this.finishedCast = true;
    this.lastFinishedCast = Time.time;
    this.castProgress = 0.0f;
    if (this.canUseOnFriend && Interaction.instance.hasValidTargetCharacter)
    {
      if ((bool) (UnityEngine.Object) this.holderCharacter)
      {
        this.holderCharacter.data.lastConsumedItem = Time.time;
        this.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, (object) this.holderCharacter.photonView.ViewID);
      }
      Interaction.instance.bestCharacter.character.FeedItem(this);
      this.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, (object) (int) this.itemID);
    }
    else
    {
      if (this.OnSecondaryFinishedCast == null)
        return;
      this.OnSecondaryFinishedCast();
    }
  }

  public void CancelUseSecondary()
  {
    this.isUsingSecondary = false;
    this.castProgress = 0.0f;
    this.finishedCast = false;
    if (this.OnSecondaryCancelled != null)
      this.OnSecondaryCancelled();
    Player.localPlayer.character.refs.animations.PlaySpecificAnimation("Movement");
    if (!(bool) (UnityEngine.Object) this.lastHolderCharacter)
      return;
    this.photonView.RPC("RemoveFeedDataRPC", RpcTarget.All, (object) this.lastHolderCharacter.photonView.ViewID);
  }

  public bool consuming { get; private set; }

  public IEnumerator ConsumeDelayed(bool ignoreActions = false)
  {
    Item obj = this;
    int consumerID = -1;
    if ((bool) (UnityEngine.Object) obj.holderCharacter)
      consumerID = obj.holderCharacter.photonView.ViewID;
    obj.consuming = true;
    if (!ignoreActions && obj.OnConsumed != null)
      obj.OnConsumed();
    yield return (object) null;
    obj.photonView.RPC("Consume", RpcTarget.All, (object) consumerID);
  }

  [PunRPC]
  public void Consume(int consumerID)
  {
    if ((bool) (UnityEngine.Object) this.holderCharacter)
    {
      if (consumerID != -1 && PhotonNetwork.GetPhotonView(consumerID).IsMine)
        GlobalEvents.TriggerItemConsumed(this, Character.localCharacter);
      this.holderCharacter.data.lastConsumedItem = Time.time;
      if ((UnityEngine.Object) this.holderCharacter.data.currentItem == (UnityEngine.Object) this)
      {
        Optionable<byte> currentSelectedSlot = this.holderCharacter.refs.items.currentSelectedSlot;
        this.holderCharacter.refs.animator.SetBool("Consumed Item", true);
        if (this.holderCharacter.IsLocal)
        {
          if (currentSelectedSlot.IsSome)
          {
            this.holderCharacter.player.EmptySlot(currentSelectedSlot);
            this.holderCharacter.refs.items.EquipSlot(currentSelectedSlot);
          }
          else
            Debug.LogError((object) "No Item Selected locally but still consuming?? THIS IS BAD. CALL ZORRO");
        }
      }
    }
    this.gameObject.SetActive(false);
  }

  public virtual void OnStash()
  {
    Action onStashAction = this.onStashAction;
    if (onStashAction != null)
      onStashAction();
    this.CancelUsePrimary();
    this.CancelUseSecondary();
  }

  [ContextMenu("Add Default Food Scripts")]
  public void AddDefaultFoodScripts()
  {
    this.usingTimePrimary = 1.2f;
    Action_PlayAnimation actionPlayAnimation = this.gameObject.AddComponent<Action_PlayAnimation>();
    actionPlayAnimation.OnPressed = true;
    actionPlayAnimation.animationName = "PlayerEat";
    Action_ModifyStatus actionModifyStatus = this.gameObject.AddComponent<Action_ModifyStatus>();
    actionModifyStatus.OnCastFinished = true;
    actionModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Hunger;
    actionModifyStatus.changeAmount = -0.1f;
    this.gameObject.AddComponent<Action_Consume>().OnCastFinished = true;
  }

  public void HoverEnter()
  {
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    this.mainRenderer.SetPropertyBlock(this.mpb);
  }

  public void HoverExit()
  {
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    this.mainRenderer.SetPropertyBlock(this.mpb);
  }

  public void SetKinematicNetworked(bool value)
  {
    this.photonView.RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) value, (object) this.transform.position, (object) this.transform.rotation);
  }

  public void SetKinematicNetworked(bool value, Vector3 position, Quaternion rotation)
  {
    this.photonView.RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) value, (object) position, (object) rotation);
  }

  [PunRPC]
  public void SetKinematicAndResetSyncData(bool value, Vector3 position, Quaternion rotation)
  {
    this.rig.isKinematic = value;
    this.rig.position = position;
    this.rig.rotation = rotation;
    if (!value)
      return;
    this.rig.linearVelocity = Vector3.zero;
    this.rig.angularVelocity = Vector3.zero;
    this.physicsSyncer.ResetRecievedData();
  }

  [PunRPC]
  public void SetKinematicRPC(bool value, Vector3 position, Quaternion rotation)
  {
    this.rig.isKinematic = value;
    this.rig.position = position;
    this.rig.rotation = rotation;
  }

  public bool HasData(DataEntryKey key) => this.data != null && this.data.HasData(key);

  public T GetData<T>(DataEntryKey key, Func<T> createDefault) where T : DataEntryValue, new()
  {
    if (this.data == null)
    {
      this.data = new ItemInstanceData(Guid.NewGuid());
      ItemInstanceDataHandler.AddInstanceData(this.data);
    }
    T data;
    if (this.data.TryGetDataEntry<T>(key, out data))
      return data;
    return createDefault != null ? this.data.RegisterEntry<T>(key, createDefault()) : this.data.RegisterNewEntry<T>(key);
  }

  public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
  {
    return this.GetData<T>(key, (Func<T>) null);
  }

  internal void ForceSyncForFrames(int frames = 10)
  {
    if (!((UnityEngine.Object) this.physicsSyncer != (UnityEngine.Object) null))
      return;
    this.physicsSyncer.ForceSyncForFrames(frames);
  }

  [PunRPC]
  public void SetItemInstanceDataRPC(ItemInstanceData instanceData)
  {
    this.data = instanceData;
    if (this.data == null)
      return;
    this.OnInstanceDataRecieved();
    foreach (ItemComponent itemComponent in this.itemComponents)
      itemComponent.OnInstanceDataSet();
  }

  public virtual void OnInstanceDataRecieved()
  {
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.ForceSyncForFrames();
    switch (this.itemState)
    {
      case ItemState.Ground:
      case ItemState.Held:
      case ItemState.InBackpack:
        if (this.data != null)
        {
          this.view.RPC("SetItemInstanceDataRPC", newPlayer, (object) this.data);
          break;
        }
        break;
    }
    if (this.itemState == ItemState.InBackpack)
    {
      (byte num, BackpackReference backpackReference) = this.backpackReference.Value;
      this.view.RPC("PutInBackpackRPC", newPlayer, (object) num, (object) backpackReference);
    }
    if (!this.rig.isKinematic)
      return;
    this.view.RPC("SetKinematicRPC", newPlayer, (object) this.rig.isKinematic, (object) this.rig.position, (object) this.rig.rotation);
  }

  [PunRPC]
  public void PutInBackpackRPC(byte slotID, BackpackReference backpackReference)
  {
    Transform[] backpackSlots = backpackReference.GetVisuals().backpackSlots;
    this.backpackReference = Optionable<(byte, BackpackReference)>.Some((slotID, backpackReference));
    this.backpackSlotTransform = backpackSlots[(int) slotID];
    this.SetState(ItemState.InBackpack);
    backpackReference.GetVisuals().SetSpawnedBackpackItem(slotID, this);
    if (!backpackReference.IsOnMyBack())
      return;
    this.HideRenderers();
  }

  [PunRPC]
  public void SetCookedAmountRPC(int amount)
  {
    this.GetData<IntItemData>(DataEntryKey.CookedAmount).Value = amount;
    this.cooking.UpdateCookedBehavior();
  }

  public void SetUseRemainingPercentage(float percentage)
  {
    this.GetData<FloatItemData>(DataEntryKey.UseRemainingPercentage).Value = Mathf.Clamp01(percentage);
  }

  public bool inActiveList { get; private set; }

  public void WasActive()
  {
    if (!this.inActiveList)
      Item.ALL_ACTIVE_ITEMS.Add(this);
    this.inActiveList = true;
    this.timeSinceWasActive = 0.0f;
  }

  private void UpdateEntryInActiveList()
  {
    if (!this.inActiveList)
      return;
    this.timeSinceWasActive += Time.deltaTime;
    if ((double) this.timeSinceWasActive <= 30.0)
      return;
    this.RemoveFromActiveList();
  }

  private void RemoveFromActiveList()
  {
    if (!this.inActiveList)
      return;
    Item.ALL_ACTIVE_ITEMS.Remove(this);
    this.inActiveList = false;
  }

  private void OnDestroy()
  {
    this.RemoveFromActiveList();
    Item.ALL_ITEMS.Remove(this);
  }

  public bool TryGetFeeder(out Character feeder)
  {
    if ((UnityEngine.Object) this.trueHolderCharacter != (UnityEngine.Object) null && (UnityEngine.Object) this.trueHolderCharacter != (UnityEngine.Object) this.holderCharacter)
    {
      feeder = this.trueHolderCharacter;
      return true;
    }
    feeder = (Character) null;
    return false;
  }

  public bool IsValidToSpawn()
  {
    LootData component = this.GetComponent<LootData>();
    return !(bool) (UnityEngine.Object) component || component.IsValidToSpawn();
  }

  public void AddNameToCSV()
  {
    LocalizedText.AppendCSVLine($"NAME_{this.UIData.itemName.ToUpperInvariant()},{this.UIData.itemName.ToUpperInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
  }

  public List<string> AddPromptToCSV(List<string> totalStrings)
  {
    List<string> csv = new List<string>();
    if (!this.UIData.mainInteractPrompt.IsNullOrEmpty() && !totalStrings.Contains(this.UIData.mainInteractPrompt.ToUpperInvariant()))
    {
      LocalizedText.AppendCSVLine($"{this.UIData.mainInteractPrompt.ToUpperInvariant()},{this.UIData.mainInteractPrompt.ToLowerInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
      csv.Add(this.UIData.mainInteractPrompt.ToUpperInvariant());
    }
    if (!this.UIData.scrollInteractPrompt.IsNullOrEmpty() && !totalStrings.Contains(this.UIData.scrollInteractPrompt.ToUpperInvariant()))
    {
      LocalizedText.AppendCSVLine($"{this.UIData.scrollInteractPrompt.ToUpperInvariant()},{this.UIData.scrollInteractPrompt.ToLowerInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
      csv.Add(this.UIData.scrollInteractPrompt.ToUpperInvariant());
    }
    if (!this.UIData.secondaryInteractPrompt.IsNullOrEmpty() && !totalStrings.Contains(this.UIData.secondaryInteractPrompt.ToUpperInvariant()))
    {
      LocalizedText.AppendCSVLine($"{this.UIData.secondaryInteractPrompt.ToUpperInvariant()},{this.UIData.secondaryInteractPrompt.ToLowerInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
      csv.Add(this.UIData.secondaryInteractPrompt.ToUpperInvariant());
    }
    return csv;
  }

  [PunRPC]
  public void RPC_SetThrownData(int characterID, float thrownAmount)
  {
    PhotonView photonView = PhotonNetwork.GetPhotonView(characterID);
    if ((bool) (UnityEngine.Object) photonView)
      photonView.TryGetComponent<Character>(out this.lastThrownCharacter);
    this.lastThrownAmount = thrownAmount;
    this.lastThrownTime = Time.time;
    GlobalEvents.TriggerItemThrown(this);
  }

  [Flags]
  public enum ItemTags
  {
    None = 0,
    Mystical = 1,
    PackagedFood = 2,
    Berry = 4,
    Mushroom = 8,
    BingBong = 16, // 0x00000010
    GourmandRequirement = 32, // 0x00000020
    GoldenIdol = 64, // 0x00000040
    Bird = 128, // 0x00000080
  }

  [Serializable]
  public class ItemUIData
  {
    public string itemName;
    public Texture2D icon;
    public bool hasAltIcon;
    public bool hasColorBlindIcon;
    public Texture2D altIcon;
    public bool hasMainInteract = true;
    public string mainInteractPrompt;
    public bool hasSecondInteract;
    public string secondaryInteractPrompt;
    public bool hasScrollingInteract;
    public string scrollInteractPrompt;
    public bool canDrop = true;
    public bool canPocket = true;
    public bool canBackpack = true;
    public bool canThrow = true;
    public bool isShootable;
    public Vector3 iconPositionOffset;
    public Vector3 iconRotationOffset;
    public float iconScaleOffset = 1f;

    public Texture2D GetIcon()
    {
      return this.hasAltIcon && GameHandler.Instance.SettingsHandler.GetSetting<BugPhobiaSetting>().Value == OffOnMode.ON || this.hasColorBlindIcon && GUIManager.instance.colorblindness ? this.altIcon : this.icon;
    }
  }
}
