// Decompiled with JetBrains decompiler
// Type: CharacterData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Settings;

#nullable disable
public class CharacterData : MonoBehaviourPunCallbacks
{
  public float currentRagdollControll;
  [Range(0.0f, 1f)]
  public float passOutValue;
  public bool passedOut;
  public bool fullyPassedOut;
  public float lastPassedOut;
  public float deathTimer;
  public float sinceDied;
  public bool dead;
  [SerializeField]
  private bool _cannibalismPermitted = true;
  [SerializeField]
  internal bool isInvincible;
  public bool isGrounded;
  public float sinceGrounded;
  public Vector3 groundPos;
  public Vector3 groundNormal;
  public float targetHeadHeight;
  public float targetHipHeight;
  public Vector3 worldMovementInput;
  public Vector3 worldMovementInput_Grounded;
  public Vector3 worldMovementInput_Lerp;
  public Vector2 lookValues;
  public Vector3 lookDirection;
  public Vector3 lookDirection_Flat;
  public Vector3 lookDirection_Right;
  public Vector3 lookDirection_Up;
  public bool isSprinting;
  private Item _currentitem;
  [HideInInspector]
  public StickyItemComponent currentStickyItem;
  public Vector3 avarageVelocity;
  public Vector3 avarageLastFrameVelocity;
  public float sinceJump;
  public float sinceClimb;
  public float currentHeadHeight;
  public bool isJumping;
  public float groundedFor;
  public float lastGroundedHeight;
  public bool chargingJump;
  public bool isClimbing;
  public bool isRopeClimbing;
  public bool isVineClimbing;
  public Vector3 climbPos;
  public Vector3 climbNormal;
  public float spectateZoom;
  public bool isBlind;
  public bool wearingSunscreen;
  private float _stam;
  [FormerlySerializedAs("lastFrameStamina")]
  public float lastFrameTotalStamina;
  public float staminaDelta;
  public Rope heldRope;
  public JungleVine heldVine;
  public float vinePercent;
  public float ropePercent;
  public Vector3 ropeClimbNormal;
  public Vector3 ropeClimbWorldNormal;
  public Vector3 ropeClimbWorldUp;
  public float sinceUseStamina;
  public bool isCrouching;
  public bool isReaching;
  public FixedJoint grabJoint;
  public float sincePressClimb = 10f;
  public float sincePressReach = 10f;
  public float lastConsumedItem;
  public float sinceHeldItem;
  public float lastAddedStatusAmount;
  public bool isInFog;
  public bool[] badgeStatus;
  public float overrideIKForSeconds;
  public float extraStamina;
  public float outOfStaminaFor;
  public float staminaMod;
  public float sinceClimbJump;
  public int climbingSpikeCount;
  public float grabFriendDistance;
  public float sinceFallSlide;
  public ClimbHandle currentClimbHandle;
  public float sinceClimbHandle;
  public float sinceGrabFriend;
  public bool usingEmoteWheel;
  public bool usingBackpackWheel;
  public float fallSeconds;
  [HideInInspector]
  public bool launchedByCannon;
  public float sinceAddedCold = 10f;
  public float sinceStartClimb;
  public Character carriedPlayer;
  public Character carrier;
  public bool sprintJump;
  public int jumpsRemaining = 1;
  public ClimbModifierSurface climbMod;
  public float slippy;
  public RaycastHit climbHit;
  internal Character grabbedPlayer;
  internal Character grabbingPlayer;
  public Transform spawnPoint;
  private Character character;
  public bool isKinecmatic;
  public bool isCarried;
  public float sinceLetGoOfFriend;
  public float sinceStandOnPlayer;
  public float sincePalJump = 10f;
  public Character lastStoodOnPlayer;
  public float myersDistance;
  public float sinceItemAttach = 10f;
  public float sinceCanClimb = 10f;
  public bool hasClimbedSinceGrounded;
  public float passedOutOnTheBeach;
  public float sinceDead;
  public Vector3 groundedForward;
  public Vector3 groundedRight;
  public float ragdollControlClamp;
  public bool staticClimbCost;
  public float sinceUnstuck = 10f;
  public bool isScoutmaster;

  public float GetTargetRagdollControll()
  {
    if ((bool) (Object) this.carrier)
      return 1f;
    return (double) this.fallSeconds > 0.0 || this.passedOut || this.fullyPassedOut || this.dead ? 0.0f : Mathf.Clamp(Mathf.Min(1f, 1f - this.passOutValue), 0.0f, this.ragdollControlClamp);
  }

  public void RecalculateInvincibility()
  {
    this.isInvincible = false;
    if (!this.character.refs.afflictions.HasAfflictionType(Affliction.AfflictionType.BingBongShield, out Affliction _))
      return;
    this.isInvincible = true;
  }

  public bool cannibalismPermitted
  {
    get => this._cannibalismPermitted;
    private set
    {
      if (this._cannibalismPermitted == value)
        return;
      this._cannibalismPermitted = value;
      Debug.Log((object) $"Cannibalism permitted for {this.character.characterName} : {value}");
    }
  }

  public bool fullyConscious => !this.passedOut && !this.fullyPassedOut && !this.dead;

  public bool isClimbingAnything => this.isClimbing || this.isRopeClimbing || this.isVineClimbing;

  public Item currentItem
  {
    get => this._currentitem;
    set
    {
      if (!((Object) this._currentitem != (Object) value))
        return;
      this._currentitem = value;
      StickyItemComponent component;
      if ((Object) value != (Object) null && value.TryGetComponent<StickyItemComponent>(out component))
        this.currentStickyItem = component;
      else
        this.currentStickyItem = (StickyItemComponent) null;
    }
  }

  public float currentStamina
  {
    get => this._stam;
    set
    {
      if (this.character.infiniteStam)
        return;
      this._stam = value;
    }
  }

  public float TotalStamina => this.currentStamina + this.extraStamina;

  private void Awake()
  {
    this.character = this.GetComponent<Character>();
    this.SetBadgeStatus();
  }

  private void Start()
  {
    if (!this.photonView.IsMine)
      return;
    this.cannibalismPermitted = GameHandler.Instance.SettingsHandler.GetSetting<CannibalismSetting>().Value == OffOnMode.ON;
    if (!PhotonNetwork.InRoom)
      return;
    this.photonView.RPC("RPCA_SyncCanBeCannibalized", RpcTarget.Others, (object) this.cannibalismPermitted);
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.character.photonView.RPC("RPC_SyncOnJoin", newPlayer, (object) this.passedOut, (object) this.fullyPassedOut, (object) this.dead, (object) this.isSprinting, (object) this.currentItem?.photonView, (object) this.isJumping, (object) this.isClimbing, (object) this.isRopeClimbing, (object) this.isVineClimbing, (object) this.vinePercent, (object) this.ropePercent, (object) this.isCrouching, (object) this.isReaching, (object) this.heldVine?.photonView, (object) this.heldRope?.photonView, (object) this.sprintJump, (object) this.badgeStatus, (object) this.cannibalismPermitted);
  }

  [PunRPC]
  public void RPC_SyncOnJoin(
    bool passedOut,
    bool fullyPassedOut,
    bool dead,
    bool isSprinting,
    PhotonView currentItem,
    bool isJumping,
    bool isClimbing,
    bool isRopeClimbing,
    bool isVineClimbing,
    float vinePercent,
    float ropePercent,
    bool isCrouching,
    bool isReaching,
    PhotonView heldVine,
    PhotonView heldRope,
    bool sprintJump,
    bool[] badgeStatus,
    bool cannibalismPermitted)
  {
    Debug.Log((object) $"RPC_SyncOnJoin: {passedOut}, {fullyPassedOut}");
    this.passedOut = passedOut;
    this.fullyPassedOut = fullyPassedOut;
    this.dead = dead;
    this.isSprinting = isSprinting;
    this.currentItem = currentItem?.GetComponent<Item>();
    this.isJumping = isJumping;
    this.isClimbing = isClimbing;
    this.isRopeClimbing = isRopeClimbing;
    this.isVineClimbing = isVineClimbing;
    this.vinePercent = vinePercent;
    this.ropePercent = ropePercent;
    this.isCrouching = isCrouching;
    this.isReaching = isReaching;
    this.heldVine = heldVine?.GetComponent<JungleVine>();
    this.heldRope = heldRope?.GetComponent<Rope>();
    this.sprintJump = sprintJump;
    this.badgeStatus = badgeStatus;
    this.cannibalismPermitted = cannibalismPermitted;
    if ((Object) this.character.refs.badgeUnlocker == (Object) null)
      Debug.LogError((object) "Badge unlocker not found...");
    else
      this.character.refs.badgeUnlocker.BadgeUnlockVisual();
  }

  internal void SetBadgeStatus()
  {
    if (!this.character.IsLocal)
      return;
    this.badgeStatus = new bool[GUIManager.instance.mainBadgeManager.badgeData.Length];
    for (int index = 0; index < this.badgeStatus.Length; ++index)
      this.badgeStatus[index] = (Object) GUIManager.instance.mainBadgeManager.badgeData[index] != (Object) null && !GUIManager.instance.mainBadgeManager.badgeData[index].IsLocked;
    this.photonView.RPC("SyncBadgeStatus", RpcTarget.All, (object) this.badgeStatus);
  }

  [PunRPC]
  public void RPCA_SyncCanBeCannibalized(bool canBeCannibalized)
  {
    this.cannibalismPermitted = canBeCannibalized;
  }

  [PunRPC]
  private void SyncBadgeStatus(bool[] statusArray)
  {
    this.badgeStatus = statusArray;
    this.character.refs.badgeUnlocker.BadgeUnlockVisual();
  }

  internal bool GetBadgeStatus(int index)
  {
    return index >= 0 && index < this.badgeStatus.Length && this.badgeStatus[index];
  }

  public bool usingWheel => this.usingEmoteWheel || this.usingBackpackWheel;
}
