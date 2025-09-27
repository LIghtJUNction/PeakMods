// Decompiled with JetBrains decompiler
// Type: Character
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
[DefaultExecutionOrder(-100)]
public class Character : MonoBehaviourPun
{
  public bool isBot;
  public static Character localCharacter;
  public CharacterInput input;
  public CharacterData data;
  public Character.CharacterRefs refs;
  private PhotonView view;
  public static List<Character> AllCharacters = new List<Character>();
  private Vector3 smoothedCamPos;
  private float passOutFailsafeTick;
  public SFX_Instance[] poofSFX;
  private static bool forceWin;
  private bool UnPassOutCalled;
  public Action UnPassOutAction;
  private bool unPassOutCalled;
  public Action<float> landAction;
  public Action startJumpAction;
  public Action jumpAction;
  internal Action startClimbAction;
  public Action<Vector3, float> dragTowardsAction;
  public Action reviveAction;
  public Action<string, float> illegalStatusAction;
  private List<StickPart> stickParts = new List<StickPart>();

  public Player player => PlayerHandler.GetPlayer(this.view.Owner);

  public static Character observedCharacter
  {
    get
    {
      Character specCharacter = MainCameraMovement.specCharacter;
      return (bool) (UnityEngine.Object) specCharacter ? specCharacter : Character.localCharacter;
    }
  }

  public PlayerGhost Ghost { get; set; }

  public bool IsLocal => (UnityEngine.Object) this == (UnityEngine.Object) Character.localCharacter;

  public Vector3 Center => this.GetBodypart(BodypartType.Torso).transform.position;

  public Vector3 Head => this.GetBodypart(BodypartType.Head).transform.position;

  public string characterName => !this.isBot ? this.view.Owner.NickName : "Bot";

  public static bool GetCharacterWithPhotonID(int photonID, out Character characterResult)
  {
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      if ((UnityEngine.Object) Character.AllCharacters[index] != (UnityEngine.Object) null && Character.AllCharacters[index].photonView.ViewID == photonID)
      {
        characterResult = Character.AllCharacters[index];
        return true;
      }
    }
    characterResult = (Character) null;
    return false;
  }

  private void OnDestroy() => Character.AllCharacters.Remove(this);

  private void Awake()
  {
    if (!this.isBot)
      Character.AllCharacters.Add(this);
    this.view = this.GetComponent<PhotonView>();
    if ((UnityEngine.Object) this.view != (UnityEngine.Object) null)
    {
      if (!this.isBot)
      {
        PlayerHandler.RegisterCharacter(this);
        this.gameObject.name = $"Character [{this.view.Owner.NickName} : {this.view.Owner.ActorNumber}]";
        if (this.view.IsMine)
        {
          Character.localCharacter = this;
          VoiceClientHandler.LocalPlayerAssigned(this.GetComponentInChildren<Recorder>());
        }
      }
      else
        this.gameObject.name = "Bot";
    }
    this.refs.animatedVariables = this.GetComponentInChildren<AnimatedVariables>();
    this.refs.movement = this.GetComponent<CharacterMovement>();
    this.refs.carriying = this.GetComponent<CharacterCarrying>();
    this.refs.ragdoll = this.GetComponent<CharacterRagdoll>();
    this.refs.balloons = this.GetComponent<CharacterBalloons>();
    this.refs.ropeHandling = this.GetComponent<CharacterRopeHandling>();
    this.refs.rigCreator = this.GetComponentInChildren<RigCreator>();
    this.refs.animations = this.GetComponentInChildren<CharacterAnimations>();
    this.refs.animator = this.refs.rigCreator.GetComponent<Animator>();
    this.refs.items = this.GetComponent<CharacterItems>();
    this.refs.climbing = this.GetComponent<CharacterClimbing>();
    this.refs.afflictions = this.GetComponent<CharacterAfflictions>();
    this.refs.view = this.GetComponent<PhotonView>();
    this.refs.heatEmission = this.GetComponentInChildren<CharacterHeatEmission>();
    this.refs.vineClimbing = this.GetComponentInChildren<CharacterVineClimbing>();
    this.refs.interactible = this.GetComponent<CharacterInteractible>();
    this.refs.customization = this.GetComponentInChildren<CharacterCustomization>();
    this.refs.stats = this.GetComponentInChildren<CharacterStats>();
    this.refs.grabbing = this.GetComponent<CharacterGrabbing>();
    this.refs.hideTheBody = this.GetComponentInChildren<HideTheBody>();
    this.refs.badgeUnlocker = this.GetComponent<BadgeUnlocker>();
    this.jumpAction += new Action(this.JumpStickEffect);
    this.refs.ikRigBuilder = this.refs.rigCreator.GetComponent<RigBuilder>();
    if ((bool) (UnityEngine.Object) this.refs.ikRigBuilder)
    {
      this.refs.ikRig = this.refs.rigCreator.GetComponentInChildren<Rig>();
      this.refs.IKHandTargetLeft = this.refs.ikRig.transform.Find("IK_Arm_Left/Target");
      this.refs.IKHandTargetRight = this.refs.ikRig.transform.Find("IK_Arm_Right/Target");
      if ((bool) (UnityEngine.Object) this.refs.IKHandTargetLeft)
      {
        this.refs.ikLeft = this.refs.IKHandTargetLeft.transform.parent.GetComponent<TwoBoneIKConstraint>();
        this.refs.ikRight = this.refs.IKHandTargetRight.transform.parent.GetComponent<TwoBoneIKConstraint>();
      }
    }
    this.CreateHelperObjects();
    this.input.Init();
  }

  internal void AddForceAtPosition(Vector3 force, Vector3 point, float radius)
  {
    this.view.RPC("RPCA_AddForceAtPosition", RpcTarget.All, (object) force, (object) point, (object) radius);
  }

  [PunRPC]
  public void RPCA_AddForceAtPosition(Vector3 force, Vector3 point, float radius)
  {
    foreach (Bodypart part in this.refs.ragdoll.partList)
    {
      float num1 = Vector3.Distance(part.Rig.worldCenterOfMass, point);
      float num2 = Mathf.InverseLerp(radius, radius * 0.1f, num1);
      Rigidbody rig = part.Rig;
      Vector3 position = Vector3.Lerp(point, rig.worldCenterOfMass, 0.5f);
      rig.AddForceAtPosition(force * num2, position, ForceMode.Impulse);
    }
  }

  [ConsoleCommand]
  public static void GainFullStamina() => Character.localCharacter.AddStamina(1f);

  private void CreateHelperObjects()
  {
    this.refs.helperObjects = new GameObject("helperObjects").transform;
    this.refs.helperObjects.transform.SetParent(this.transform);
    this.refs.helperObjects.transform.localPosition = Vector3.zero;
    this.refs.helperObjects.transform.localRotation = Quaternion.identity;
    this.refs.animationHeadTransform = UnityEngine.Object.Instantiate<GameObject>(this.refs.helperObjects.gameObject, this.refs.helperObjects).transform;
    this.refs.animationHeadTransform.gameObject.name = "animationHead";
    this.refs.animationHipTransform = UnityEngine.Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
    this.refs.animationHipTransform.gameObject.name = "animationHip";
    this.refs.animationItemTransform = UnityEngine.Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
    this.refs.animationItemTransform.gameObject.name = "animationItem";
    this.refs.animationLookTransform = UnityEngine.Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
    this.refs.animationLookTransform.gameObject.name = "animationLook";
    this.refs.animationPositionTransform = UnityEngine.Object.Instantiate<GameObject>(this.refs.animationHeadTransform.gameObject, this.refs.helperObjects).transform;
    this.refs.animationPositionTransform.gameObject.name = "animationPosition";
  }

  private void Start()
  {
    this.refs.hip = this.GetBodypart(BodypartType.Hip);
    this.refs.head = this.GetBodypart(BodypartType.Head);
    this.gameObject.name = $"Character [{this.view.Owner.NickName} : {this.view.Owner.ActorNumber}]";
    this.refs.afflictions.OnAddedIncrementalStatus += new Action<CharacterAfflictions.STATUSTYPE, float>(this.OnAddedStatus);
    this.smoothedCamPos = this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f);
  }

  private void OnAddedStatus(CharacterAfflictions.STATUSTYPE sTATUSTYPE, float amount)
  {
    if (sTATUSTYPE != CharacterAfflictions.STATUSTYPE.Cold || (double) amount <= 0.0)
      return;
    this.data.sinceAddedCold = 0.0f;
  }

  private void Update()
  {
    this.HandleStickUpdate();
    this.UpdateVariables();
    if (!this.data.dead)
      this.data.sinceDied = 0.0f;
    if (!this.IsLocal)
      return;
    if (this.data.dead)
      this.HandleDeath();
    else if (this.data.passedOut || this.data.fullyPassedOut)
      this.HandlePassedOut();
    else
      this.HandleLife();
  }

  private void UpdateVariables()
  {
    this.data.ragdollControlClamp = Mathf.MoveTowards(this.data.ragdollControlClamp, 1f, Time.deltaTime * 5f);
    this.data.sinceUnstuck += Time.deltaTime;
  }

  private Vector3 DeathPos() => new Vector3(0.0f, 5000f, -5000f);

  private void HandleDeath() => this.data.sinceDied += Time.deltaTime;

  private void HandlePassedOut()
  {
    if ((double) this.refs.afflictions.statusSum < 1.0 && (double) Time.time - (double) this.data.lastPassedOut > 3.0)
    {
      if (!this.UnPassOutCalled)
      {
        this.view.RPC("RPCA_UnPassOut", RpcTarget.All);
        this.passOutFailsafeTick = 0.0f;
      }
      else
      {
        this.passOutFailsafeTick += Time.deltaTime;
        if ((double) this.passOutFailsafeTick > 3.0)
        {
          Debug.Log((object) "Passed out failsafe triggered.");
          this.UnPassOutCalled = false;
        }
      }
    }
    if ((double) this.data.deathTimer <= 1.0)
      return;
    this.refs.items.EquipSlot(Optionable<byte>.None);
    Debug.Log((object) "DYING");
    this.view.RPC("RPCA_Die", RpcTarget.All, (object) (this.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f));
  }

  [ConsoleCommand]
  public static void Die()
  {
    Character.localCharacter.refs.items.EquipSlot(Optionable<byte>.None);
    Debug.Log((object) "DYING");
    Character.localCharacter.view.RPC("RPCA_Die", RpcTarget.All, (object) (Character.localCharacter.Center + Vector3.up * 0.2f + Vector3.forward * 0.1f));
  }

  internal void DieInstantly() => this.view.RPC("RPCA_Die", RpcTarget.All, (object) this.Center);

  [PunRPC]
  public void RPCA_Die(Vector3 itemSpawnPoint)
  {
    this.refs.items.EquipSlot(Optionable<byte>.None);
    this.data.dead = true;
    this.data.fullyPassedOut = true;
    this.data.deathTimer = 1f;
    this.data.passedOut = true;
    this.refs.stats.justDied = true;
    this.refs.stats.Record();
    ItemSlot[] itemSlots = this.player.itemSlots;
    this.refs.items.DropAllItems(true);
    Debug.Log((object) (this.gameObject.name + " died"));
    ((GameObject) UnityEngine.Object.Instantiate(Resources.Load("Skeleton"))).GetComponent<Skelleton>().SpawnSkelly(this);
    this.WarpPlayer(this.DeathPos(), false);
    this.CheckEndGame();
    Debug.Log((object) "DIE");
  }

  public void CheckEndGame()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    bool flag = true;
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      if (!Character.AllCharacters[index].data.dead)
        flag = false;
    }
    if (!flag)
      return;
    this.EndGame();
  }

  [ConsoleCommand]
  internal static void TestWin()
  {
    Character.localCharacter.photonView.RPC("RPCEndGame_ForceWin", RpcTarget.All);
  }

  internal void EndGame()
  {
    this.photonView.RPC("RPCEndGame", RpcTarget.All);
    RunManager.Instance.EndGame();
  }

  [PunRPC]
  private void RPCEndGame_ForceWin()
  {
    Character.forceWin = true;
    this.RPCEndGame();
    Character.forceWin = false;
  }

  [PunRPC]
  private void RPCEndGame()
  {
    bool somebodyElseWon = false;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (Character.CheckWinCondition(allCharacter))
      {
        allCharacter.refs.stats.Win();
        somebodyElseWon = true;
      }
    }
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (!Character.CheckWinCondition(allCharacter))
        allCharacter.refs.stats.Lose(somebodyElseWon);
    }
    MenuWindow.CloseAllWindows();
    if (somebodyElseWon)
    {
      GlobalEvents.TriggerSomeoneWonRun();
      Singleton<PeakHandler>.Instance.EndCutscene();
    }
    else
      GUIManager.instance.endScreen.Open();
    GlobalEvents.TriggerRunEnded();
  }

  public static bool CheckWinCondition(Character c)
  {
    if (Character.forceWin)
      return true;
    return (c.data.isRopeClimbing && c.data.heldRope.isHelicopterRope || Singleton<MountainProgressHandler>.Instance.IsAtPeak(c.Center)) && !c.data.dead;
  }

  [PunRPC]
  private void RPCA_UnPassOut()
  {
    this.UnPassOutCalled = true;
    this.data.deathTimer = 0.0f;
    if (this.IsLocal)
      Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action(this.UnPassOutDone));
    else
      this.UnPassOutDone();
  }

  private void UnPassOutDone()
  {
    Debug.Log((object) "UhPassOut");
    Action unPassOutAction = this.UnPassOutAction;
    if (unPassOutAction != null)
      unPassOutAction();
    this.data.fullyPassedOut = false;
    this.data.passedOut = false;
  }

  [ConsoleCommand]
  public static void PassOut()
  {
    CharacterAfflictions.Starve();
    Character.localCharacter.view.RPC("RPCA_PassOut", RpcTarget.All);
  }

  [PunRPC]
  public void RPCA_PassOut()
  {
    this.UnPassOutCalled = false;
    this.data.passedOut = true;
    this.refs.stats.justPassedOut = true;
    this.data.lastPassedOut = Time.time;
    this.refs.stats.Record();
    GlobalEvents.OnCharacterPassedOut(this);
    if (this.IsLocal)
    {
      // ISSUE: method pointer
      Transitions.instance.PlayTransition(TransitionType.FadeToBlack, new Action((object) this, __methodptr(\u003CRPCA_PassOut\u003Eg__PassOutDone\u007C57_0)));
    }
    else
      PassOutDone();
    Debug.Log((object) "PASS OUT");

    void PassOutDone()
    {
      this.data.fullyPassedOut = true;
      this.refs.items.DropAllItems(false);
    }
  }

  private void HandleLife()
  {
    if ((double) this.refs.afflictions.statusSum >= 1.0)
    {
      this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 1f, Time.deltaTime / 5f);
      if ((double) this.data.passOutValue <= 0.99900001287460327)
        return;
      this.view.RPC("RPCA_PassOut", RpcTarget.All);
    }
    else
      this.data.passOutValue = Mathf.MoveTowards(this.data.passOutValue, 0.0f, Time.deltaTime / 5f);
  }

  public void PassOutInstantly()
  {
    this.data.passOutValue = 1f;
    this.view.RPC("RPCA_PassOut", RpcTarget.All);
  }

  private void FixedUpdate()
  {
    this.UpdateVariablesFixed();
    if (!this.data.dead)
      return;
    this.refs.ragdoll.MoveAllRigsInDirection(this.DeathPos() - this.Center);
    this.refs.ragdoll.HaltBodyVelocity();
  }

  private void UpdateVariablesFixed()
  {
    float targetRagdollControll = this.data.GetTargetRagdollControll();
    this.data.currentRagdollControll = (double) targetRagdollControll >= (double) this.data.currentRagdollControll ? ((double) this.data.currentRagdollControll <= 0.5 ? Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 0.5f) : Mathf.MoveTowards(this.data.currentRagdollControll, targetRagdollControll, Time.fixedDeltaTime * 1f)) : targetRagdollControll;
    this.data.staminaDelta = this.data.currentStamina + this.data.extraStamina - this.data.lastFrameTotalStamina;
    this.data.lastFrameTotalStamina = this.data.currentStamina + this.data.extraStamina;
    if (this.data.isGrounded)
    {
      this.data.groundedFor += Time.fixedDeltaTime;
      this.data.sinceGrounded = 0.0f;
      this.data.lastGroundedHeight = this.Center.y;
    }
    else
    {
      this.data.groundedFor = 0.0f;
      if ((double) this.data.sinceGrounded < 1.0 || (double) this.data.avarageVelocity.y < -1.0)
        this.data.sinceGrounded += Time.fixedDeltaTime;
    }
    if (this.data.isClimbing || this.data.isRopeClimbing || this.data.isVineClimbing)
      this.data.sinceClimb = 0.0f;
    if (this.data.dead)
      this.data.sinceDead = 0.0f;
    if (this.OutOfStamina())
      this.data.outOfStaminaFor += Time.fixedDeltaTime;
    else
      this.data.outOfStaminaFor = 0.0f;
    this.data.staminaMod = Mathf.Max(Mathf.Clamp01(this.GetTotalStamina() * 5f), 0.2f);
    this.data.sinceClimbJump += Time.fixedDeltaTime;
    if ((double) this.data.fallSeconds > 0.0)
    {
      if (this.data.isGrounded)
        this.data.fallSeconds -= Time.fixedDeltaTime;
      else
        this.data.fallSeconds -= Time.fixedDeltaTime * 0.2f;
      if ((double) this.data.fallSeconds <= 0.0)
        this.StoppedForcedRagdolling();
    }
    if (this.data.fullyPassedOut)
    {
      if (this.input.interactIsPressed)
        this.data.deathTimer += Time.fixedDeltaTime * 0.33f;
      else if (!(bool) (UnityEngine.Object) this.data.carrier)
      {
        if (!this.HasMeaningfulTempStatuses() && this.NobodyIsAlive())
          this.data.deathTimer += Time.fixedDeltaTime / 10f;
        else
          this.data.deathTimer += Time.fixedDeltaTime / 60f;
      }
    }
    else
      this.data.sinceDied = 0.0f;
    if (this.input.usePrimaryIsPressed && (UnityEngine.Object) this.data.currentItem == (UnityEngine.Object) null)
      this.data.sincePressClimb = 0.0f;
    if (this.input.useSecondaryIsPressed && (UnityEngine.Object) this.data.currentItem == (UnityEngine.Object) null)
      this.data.sincePressReach = 0.0f;
    this.data.sincePressClimb += Time.fixedDeltaTime;
    this.data.sincePressReach += Time.fixedDeltaTime;
    this.data.sinceAddedCold += Time.fixedDeltaTime;
    this.data.sinceStartClimb += Time.fixedDeltaTime;
    this.data.sinceGrabFriend += Time.fixedDeltaTime;
    this.data.sinceClimbHandle += Time.fixedDeltaTime;
    this.data.sinceFallSlide += Time.fixedDeltaTime;
    this.data.sinceUseStamina += Time.fixedDeltaTime;
    this.data.sinceClimb += Time.fixedDeltaTime;
    this.data.sinceJump += Time.fixedDeltaTime;
    this.data.sinceDead += Time.fixedDeltaTime;
    this.data.overrideIKForSeconds -= Time.fixedDeltaTime;
    this.data.slippy -= Time.deltaTime;
    this.data.sinceLetGoOfFriend += Time.fixedDeltaTime;
    this.data.sinceStandOnPlayer += Time.fixedDeltaTime;
    this.data.sincePalJump += Time.fixedDeltaTime;
    this.data.sinceItemAttach += Time.fixedDeltaTime;
    this.data.sinceCanClimb += Time.fixedDeltaTime;
    this.data.passedOutOnTheBeach -= Time.fixedDeltaTime;
    if (!this.CanRegenStamina())
      return;
    this.AddStamina(Time.fixedDeltaTime * 0.2f);
  }

  private bool NobodyIsAlive()
  {
    List<Character> allCharacters = Character.AllCharacters;
    for (int index = 0; index < allCharacters.Count; ++index)
    {
      if (allCharacters[index].data.fullyConscious)
        return false;
    }
    return true;
  }

  private bool HasMeaningfulTempStatuses()
  {
    float num = this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) + this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Thorns);
    if (!this.data.isInFog)
      num += this.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
    return (double) this.refs.afflictions.statusSum - (double) num < 1.0;
  }

  private bool CanRegenStamina()
  {
    if ((bool) (UnityEngine.Object) this.data.currentClimbHandle || this.IsStuck())
      return true;
    float num = (double) this.data.currentStamina > 0.0 ? 1f : 2f;
    return (double) this.data.sinceGrounded <= 0.20000000298023224 && (double) this.data.sinceUseStamina >= (double) num;
  }

  public float GetTotalStamina() => this.data.currentStamina + this.data.extraStamina;

  internal Bodypart GetBodypart(BodypartType head) => this.refs.ragdoll.partDict[head];

  internal Rigidbody GetBodypartRig(BodypartType head) => this.refs.ragdoll.partDict[head].Rig;

  internal void CalculateWorldMovementDir()
  {
    Vector3 vector3_1 = (new Vector3() + this.data.lookDirection * this.input.movementInput.y) with
    {
      y = 0.0f
    };
    vector3_1 = vector3_1.normalized;
    this.data.worldMovementInput = (vector3_1 + this.data.lookDirection_Right * this.input.movementInput.x).normalized;
    Vector3 lookDirection = this.data.lookDirection;
    Vector3 lookDirectionRight = this.data.lookDirection_Right;
    lookDirection.y = 0.0f;
    lookDirection.Normalize();
    Vector3 planeNormal = this.data.groundNormal;
    if ((double) this.data.sinceGrounded > 0.20000000298023224)
      planeNormal = Vector3.up;
    Vector3 vector3_2 = HelperFunctions.GroundDirection(planeNormal, -lookDirectionRight);
    Vector3 vector3_3 = HelperFunctions.GroundDirection(planeNormal, lookDirection);
    if ((double) this.data.sinceGrounded < 0.20000000298023224)
    {
      this.data.groundedForward = vector3_2;
      this.data.groundedRight = vector3_3;
    }
    this.data.worldMovementInput_Grounded = Vector3.ClampMagnitude(vector3_2 * this.input.movementInput.y + vector3_3 * this.input.movementInput.x, 1f);
    Vector3 target = this.data.worldMovementInput_Grounded;
    float num = Mathf.Lerp(this.refs.movement.movementTurnSpeed, this.refs.movement.airMovementTurnSpeed, this.data.sinceGrounded * 4f);
    if (!this.data.isGrounded)
      target = this.data.worldMovementInput;
    this.data.worldMovementInput_Lerp = Vector3.MoveTowards(this.data.worldMovementInput_Lerp, target, Time.deltaTime * num);
  }

  internal void RecalculateLookDirections()
  {
    Vector3 normalized = HelperFunctions.LookToDirection((Vector3) this.data.lookValues, Vector3.forward).normalized;
    this.data.lookDirection = normalized;
    normalized.y = 0.0f;
    normalized.Normalize();
    this.data.lookDirection_Flat = normalized;
    this.data.lookDirection_Right = Vector3.Cross(Vector3.up, this.data.lookDirection).normalized;
    this.data.lookDirection_Up = Vector3.Cross(this.data.lookDirection, this.data.lookDirection_Right).normalized;
  }

  internal Vector3 GetCameraPos(float forwardOffset)
  {
    return this.GetBodypart(BodypartType.Head).transform.TransformPoint(Vector3.up * 1f + Vector3.forward * forwardOffset);
  }

  internal Vector3 GetAnimationRelativePosition(Vector3 position)
  {
    return this.refs.hip.Rig.position + (position - this.refs.animationHipTransform.position);
  }

  internal void OnLand(float sinceGrounded)
  {
    Action<float> landAction = this.landAction;
    if (landAction == null)
      return;
    landAction(sinceGrounded);
  }

  internal void OnStartJump()
  {
    Action startJumpAction = this.startJumpAction;
    if (startJumpAction == null)
      return;
    startJumpAction();
  }

  internal void OnJump()
  {
    Action jumpAction = this.jumpAction;
    if (jumpAction == null)
      return;
    jumpAction();
  }

  internal void OnStartClimb()
  {
    Action startClimbAction = this.startClimbAction;
    if (startClimbAction == null)
      return;
    startClimbAction();
  }

  internal Vector3 HipPos() => this.GetBodypart(BodypartType.Hip).Rig.position;

  internal Vector3 TorsoPos() => this.GetBodypart(BodypartType.Torso).Rig.position;

  internal void AddForce(Vector3 move, float minRandomMultiplier = 1f, float maxRandomMultiplier = 1f)
  {
    foreach (Bodypart part in this.refs.ragdoll.partList)
    {
      Vector3 vector3 = move;
      if ((double) minRandomMultiplier != (double) maxRandomMultiplier)
        vector3 *= UnityEngine.Random.Range(minRandomMultiplier, maxRandomMultiplier);
      Vector3 force = vector3;
      part.AddForce(force, ForceMode.Acceleration);
    }
  }

  internal bool CheckStand()
  {
    return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing;
  }

  internal bool CheckGravity()
  {
    return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !((UnityEngine.Object) this.data.currentClimbHandle != (UnityEngine.Object) null);
  }

  internal bool CheckMovement()
  {
    return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !((UnityEngine.Object) this.data.currentClimbHandle != (UnityEngine.Object) null);
  }

  internal bool CheckJump()
  {
    return !this.data.fullyPassedOut && !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !((UnityEngine.Object) this.data.currentClimbHandle != (UnityEngine.Object) null);
  }

  internal bool CheckSprint()
  {
    return !this.data.isClimbing && !this.data.isRopeClimbing && !this.data.isVineClimbing && !((UnityEngine.Object) this.data.currentClimbHandle != (UnityEngine.Object) null) && this.data.fullyConscious && (!(bool) (UnityEngine.Object) this.data.currentItem || !this.data.currentItem.isUsingPrimary && !this.data.currentItem.isUsingSecondary);
  }

  internal void SetRotation()
  {
    if ((bool) (UnityEngine.Object) this.data.carrier)
      this.refs.rigCreator.transform.rotation = this.data.carrier.refs.carryPosRef.rotation;
    else if (this.data.isRopeClimbing)
      this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.ropeClimbWorldNormal, this.data.ropeClimbWorldUp);
    else if (this.data.isClimbing)
      this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(-this.data.climbNormal);
    else
      this.refs.rigCreator.transform.rotation = Quaternion.LookRotation(this.data.lookDirection_Flat);
  }

  internal bool UseStamina(float usage, bool useBonusStamina = true)
  {
    if ((double) usage == 0.0)
      return false;
    usage *= Ascents.climbStaminaMultiplier;
    if (!this.view.IsMine)
      return (double) this.data.currentStamina + (double) this.data.extraStamina > (double) usage;
    if ((double) this.data.currentStamina == 0.0)
    {
      if (!((double) this.data.extraStamina > 0.0 & useBonusStamina))
        return false;
      this.data.extraStamina -= usage;
      this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0.0f, 1f);
      this.data.sinceUseStamina = 0.0f;
      GUIManager.instance.bar.ChangeBar();
      return true;
    }
    this.data.currentStamina -= usage;
    this.data.sinceUseStamina = 0.0f;
    GUIManager.instance.bar.ChangeBar();
    if ((double) this.data.currentStamina > 0.0)
      return true;
    this.ClampStamina();
    return (double) this.data.extraStamina > 0.0;
  }

  [PunRPC]
  public void MoraleBoost(float staminaAdd, int scoutCount)
  {
    GUIManager.instance.bar.PlayMoraleBoost(scoutCount);
    this.AddExtraStamina(staminaAdd);
  }

  public void AddStamina(float add)
  {
    if (!this.view.IsMine)
      return;
    this.data.currentStamina += add;
    this.ClampStamina();
    GUIManager.instance.bar.ChangeBar();
  }

  public void ClampStamina()
  {
    this.data.currentStamina = Mathf.Clamp(this.data.currentStamina, 0.0f, this.GetMaxStamina());
  }

  public float GetMaxStamina() => Mathf.Max(1f - this.refs.afflictions.statusSum, 0.0f);

  public void SetExtraStamina(float amt)
  {
    if (!this.view.IsMine)
      return;
    this.data.extraStamina = Mathf.Clamp(amt, 0.0f, 1f);
    GUIManager.instance.bar.ChangeBar();
  }

  public void AddExtraStamina(float add)
  {
    if (!this.view.IsMine)
      return;
    this.data.extraStamina += add;
    this.data.extraStamina = Mathf.Clamp(this.data.extraStamina, 0.0f, 1f);
    GUIManager.instance.bar.ChangeBar();
  }

  public void FeedItem(Item item)
  {
    this.photonView.RPC("GetFedItemRPC", this.photonView.Owner, (object) item.photonView.ViewID);
  }

  [PunRPC]
  public void GetFedItemRPC(int itemPhotonID)
  {
    if (!this.photonView.IsMine)
      return;
    PhotonView photonView = PhotonView.Find(itemPhotonID);
    if ((UnityEngine.Object) photonView == (UnityEngine.Object) null)
      return;
    Item component = photonView?.GetComponent<Item>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    Debug.Log((object) ("I just got fed a: " + component.UIData.itemName));
    component.overrideHolderCharacter = this;
    if (component.OnPrimaryFinishedCast != null)
      component.OnPrimaryFinishedCast();
    if (component.consuming)
      return;
    component.overrideHolderCharacter = (Character) null;
  }

  internal void DragTowards(Vector3 target, float force)
  {
    Action<Vector3, float> dragTowardsAction = this.dragTowardsAction;
    if (dragTowardsAction != null)
      dragTowardsAction(target, force);
    this.AddForce(Vector3.ClampMagnitude(target - this.Center, 1f) * force);
  }

  internal bool OutOfStamina()
  {
    return (double) this.data.currentStamina < 0.004999999888241291 && (double) this.data.extraStamina < 1.0 / 1000.0;
  }

  internal bool OutOfRegularStamina() => (double) this.data.currentStamina < 0.004999999888241291;

  internal bool IsSliding() => this.data.isClimbing && this.OutOfStamina();

  internal bool CanDoInput()
  {
    return !GUIManager.instance.windowBlockingInput && !GUIManager.instance.wheelActive;
  }

  internal int GetPlayerListID(List<Character> playerList)
  {
    for (int index = 0; index < playerList.Count; ++index)
    {
      if ((UnityEngine.Object) playerList[index] == (UnityEngine.Object) this)
        return index;
    }
    return -1;
  }

  internal void Fall(float seconds, float screenShake = 0.0f)
  {
    if ((double) screenShake <= 9.9999997473787516E-06 || !this.refs.view.IsMine)
      this.refs.view.RPC("RPCA_Fall", RpcTarget.All, (object) seconds);
    else
      this.refs.view.RPC("RPCA_FallWithScreenShake", RpcTarget.All, (object) seconds, (object) screenShake);
  }

  [PunRPC]
  public void RPCA_UnFall() => this.data.fallSeconds = 0.0f;

  [PunRPC]
  public void RPCA_Fall(float seconds)
  {
    if (this.photonView.IsMine)
      Debug.Log((object) $"I fell for {seconds} seconds");
    if ((double) seconds <= (double) this.data.fallSeconds)
      return;
    this.data.fallSeconds = seconds;
  }

  [PunRPC]
  public void RPCA_FallWithScreenShake(float seconds, float shake)
  {
    if (this.photonView.IsMine)
      Debug.Log((object) $"I fell for {seconds} seconds");
    GamefeelHandler.instance.AddPerlinShake(shake, 0.4f);
    if ((double) seconds <= (double) this.data.fallSeconds)
      return;
    this.data.fallSeconds = seconds;
  }

  [ConsoleCommand]
  public static void Revive()
  {
    Debug.Log((object) $"Reviving, status: {Character.localCharacter.data.dead}, fullyPassedOut: {Character.localCharacter.data.fullyPassedOut}");
    if (!Character.localCharacter.data.dead && !Character.localCharacter.data.fullyPassedOut)
      return;
    Character.localCharacter.view.RPC("RPCA_Revive", RpcTarget.All, (object) true);
  }

  [PunRPC]
  internal void RPCA_Revive(bool applyStatus)
  {
    Action reviveAction = this.reviveAction;
    if (reviveAction != null)
      reviveAction();
    this.data.dead = false;
    this.data.deathTimer = 0.0f;
    this.data.passedOut = false;
    this.data.fullyPassedOut = false;
    this.data.sinceGrounded = 0.0f;
    this.refs.afflictions.ClearAllStatus();
    this.refs.afflictions.RemoveAllThorns();
    this.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Crab, 0.0f);
    this.data.fallSeconds = 0.0f;
    if (!applyStatus)
      return;
    this.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.05f);
    this.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.3f);
  }

  [PunRPC]
  internal void RPCA_ReviveAtPosition(Vector3 position, bool applyStatus)
  {
    this.refs.items.DropAllItems(true);
    this.RPCA_Revive(applyStatus);
    this.WarpPlayer(position, true);
    this.refs.stats.justDied = false;
    this.refs.stats.justRevived = true;
    this.refs.stats.Record(true, position.y);
  }

  [PunRPC]
  public void WarpPlayerRPC(Vector3 position, bool poof) => this.WarpPlayer(position, poof);

  public void PlayPoofVFX(Vector3 pos)
  {
    this.refs.poof.transform.position = pos;
    this.refs.poof.main.startColor = (ParticleSystem.MinMaxGradient) this.refs.customization.PlayerColor;
    this.refs.poof.Play();
    for (int index = 0; index < this.poofSFX.Length; ++index)
      this.poofSFX[index].Play(pos);
  }

  public bool warping { get; private set; }

  private void WarpPlayer(Vector3 position, bool poof)
  {
    this.StartCoroutine(IMove());
    if (!poof)
      return;
    this.PlayPoofVFX(position);

    IEnumerator IMove()
    {
      this.warping = true;
      int c = 0;
      while (c < 5)
      {
        ++c;
        this.refs.ragdoll.MoveAllRigsInDirection(position - this.Center);
        this.refs.ragdoll.HaltBodyVelocity();
        yield return (object) new WaitForFixedUpdate();
      }
      this.refs.ragdoll.HaltBodyVelocity();
      this.warping = false;
    }
  }

  internal void MoveBodypartTowardsPoint(
    BodypartType bodypart,
    Vector3 pos,
    float force,
    float clampDistance = 1f)
  {
    Bodypart bodypart1 = this.GetBodypart(bodypart);
    bodypart1.AddForce(Vector3.ClampMagnitude(pos - bodypart1.Rig.position, clampDistance) * force, ForceMode.Acceleration);
  }

  public static bool PlayerIsDeadOrDown()
  {
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (allCharacter.data.dead || allCharacter.data.fullyPassedOut)
        return true;
    }
    return false;
  }

  internal BodypartType GetPartType(Rigidbody rigidbody)
  {
    foreach (Bodypart part in this.refs.ragdoll.partList)
    {
      if ((UnityEngine.Object) part.Rig == (UnityEngine.Object) rigidbody)
        return part.partType;
    }
    return ~BodypartType.Hip;
  }

  internal void LimitFalling()
  {
    this.data.sinceGrounded = Mathf.Min(this.data.sinceGrounded, 0.5f);
    this.data.sinceJump = Mathf.Min(this.data.sinceJump, 0.5f);
  }

  internal void AddIllegalStatus(string illegalStatus, float amount)
  {
    Action<string, float> illegalStatusAction = this.illegalStatusAction;
    if (illegalStatusAction == null)
      return;
    illegalStatusAction(illegalStatus, amount);
  }

  public bool infiniteStam { get; private set; }

  [ConsoleCommand]
  public static void InfiniteStamina()
  {
    if (!Character.localCharacter.infiniteStam)
      Character.localCharacter.data.currentStamina = 1f;
    Character.localCharacter.infiniteStam = !Character.localCharacter.infiniteStam;
    Debug.LogError((object) $"Infinite Stamina: {Character.localCharacter.infiniteStam}");
  }

  public bool statusesLocked { get; private set; }

  [ConsoleCommand]
  public static void LockStatuses()
  {
    Character.localCharacter.statusesLocked = !Character.localCharacter.statusesLocked;
    Debug.LogError((object) $"Statuses Locked: {Character.localCharacter.statusesLocked}");
  }

  private void OnGetMic(float db)
  {
  }

  internal void StartPassedOutOnTheBeach()
  {
    Debug.Log((object) "Starting passed out!");
    this.data.passedOutOnTheBeach = 3f;
    this.Fall(7f);
  }

  public void AddForceToBodyPart(Rigidbody rig, Vector3 partForce, Vector3 wholeBodyForce)
  {
    Bodypart component = rig.GetComponent<Bodypart>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    this.view.RPC("RPCA_AddForceToBodyPart", RpcTarget.All, (object) component.partType, (object) partForce, (object) wholeBodyForce);
  }

  [PunRPC]
  public void RPCA_AddForceToBodyPart(
    BodypartType bodypartType,
    Vector3 force,
    Vector3 wholeBodyForce)
  {
    this.GetBodypart(bodypartType).AddForce(force, ForceMode.Acceleration);
    this.AddForce(wholeBodyForce);
  }

  internal void ClampSinceGrounded(float maxSinceGrounded)
  {
    this.data.sinceGrounded = Mathf.Clamp(this.data.sinceGrounded, 0.0f, maxSinceGrounded);
  }

  internal void ClampRagdollControl(float maxRagdollControlClamp)
  {
    this.data.ragdollControlClamp = maxRagdollControlClamp;
  }

  private void HandleStickUpdate()
  {
    this.data.sinceUnstuck += Time.deltaTime;
    if (this.stickParts.Count == 0)
      return;
    bool flag = true;
    foreach (StickPart stickPart in this.stickParts)
    {
      if ((bool) (UnityEngine.Object) stickPart.joint)
        flag = false;
      stickPart.sinceStick += Time.deltaTime;
      if (this.view.IsMine && (double) stickPart.sinceStick > 4.0 && (bool) (UnityEngine.Object) stickPart.joint)
        this.view.RPC("RPCA_ClearJoint", RpcTarget.All, (object) stickPart.bodypart.partType);
    }
    if (flag && this.view.IsMine)
      this.view.RPC("RPCA_ClearStickData", RpcTarget.All);
    else
      this.ClampSinceGrounded(0.5f);
  }

  [PunRPC]
  public void RPCA_ClearStickData()
  {
    this.stickParts.Clear();
    this.data.sinceUnstuck = 0.0f;
  }

  [PunRPC]
  public void RPCA_ClearJoint(BodypartType bodypartType)
  {
    foreach (StickPart stickPart in this.stickParts)
    {
      if (bodypartType == stickPart.bodypart.partType)
        UnityEngine.Object.Destroy((UnityEngine.Object) stickPart.joint);
    }
  }

  private void JumpStickEffect()
  {
    foreach (StickPart stickPart in this.stickParts)
      stickPart.sinceStick += UnityEngine.Random.Range(0.5f, 1.5f);
  }

  public bool IsStuck() => this.stickParts.Count > 0;

  internal bool TryStickBodypart(
    Bodypart bodypart,
    Vector3 stickAnchor,
    CharacterAfflictions.STATUSTYPE statusType,
    float statusAmount)
  {
    if ((double) this.data.sinceUnstuck < 3.0 || this.StickPartExists(bodypart))
      return false;
    this.view.RPC("RPCA_Stick", RpcTarget.All, (object) bodypart.partType, (object) bodypart.transform.position, (object) stickAnchor, (object) statusType, (object) statusAmount);
    return true;
  }

  [PunRPC]
  private void RPCA_Stick(
    BodypartType bodypartType,
    Vector3 pos,
    Vector3 stickAnchor,
    CharacterAfflictions.STATUSTYPE statusType,
    float statusAmount)
  {
    Bodypart bodypart = this.GetBodypart(bodypartType);
    bodypart.Rig.transform.position = pos;
    StickPart stickPart = new StickPart();
    stickPart.bodypart = bodypart;
    stickPart.sinceStick = 0.0f;
    ConfigurableJoint configurableJoint = bodypart.Rig.gameObject.AddComponent<ConfigurableJoint>();
    stickPart.joint = (Joint) configurableJoint;
    this.stickParts.Add(stickPart);
    configurableJoint.xMotion = ConfigurableJointMotion.Locked;
    configurableJoint.yMotion = ConfigurableJointMotion.Locked;
    configurableJoint.zMotion = ConfigurableJointMotion.Locked;
    configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
    configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
    configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
    configurableJoint.anchor = bodypart.transform.InverseTransformPoint(stickAnchor);
    if ((double) statusAmount <= 0.0)
      return;
    this.refs.afflictions.AddStatus(statusType, statusAmount, true);
  }

  internal void UnStick() => this.refs.view.RPC("RPCA_Unstick", RpcTarget.All);

  [PunRPC]
  public void RPCA_Unstick()
  {
    for (int index = this.stickParts.Count - 1; index >= 0; --index)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.stickParts[index].joint);
    this.RPCA_ClearStickData();
  }

  private bool StickPartExists(Bodypart bodypart)
  {
    foreach (StickPart stickPart in this.stickParts)
    {
      if ((UnityEngine.Object) bodypart == (UnityEngine.Object) stickPart.bodypart)
        return true;
    }
    return false;
  }

  internal void AddForce(object value) => throw new NotImplementedException();

  private void StoppedForcedRagdolling() => this.data.launchedByCannon = false;

  [Serializable]
  public class CharacterRefs
  {
    public Transform carryPosRef;
    public CharacterRopeHandling ropeHandling;
    public CharacterClimbing climbing;
    public CharacterMovement movement;
    public CharacterRagdoll ragdoll;
    public CharacterBalloons balloons;
    public CharacterInteractible interactible;
    public RigCreator rigCreator;
    public Bodypart head;
    public Bodypart hip;
    public CharacterAnimations animations;
    public Animator animator;
    public RigBuilder ikRigBuilder;
    public Rig ikRig;
    public TwoBoneIKConstraint ikLeft;
    public TwoBoneIKConstraint ikRight;
    public CharacterItems items;
    public AnimatedVariables animatedVariables;
    public CharacterAfflictions afflictions;
    public BadgeUnlocker badgeUnlocker;
    public PhotonView view;
    public CharacterHeatEmission heatEmission;
    public CharacterVineClimbing vineClimbing;
    public SkinnedMeshRenderer mainRenderer;
    public CharacterCarrying carriying;
    public CharacterCustomization customization;
    public CharacterStats stats;
    public CharacterGrabbing grabbing;
    public HideTheBody hideTheBody;
    public ParticleSystem poof;
    public Transform IKHandTargetLeft;
    public Transform IKHandTargetRight;
    public Transform helperObjects;
    public Transform animationHeadTransform;
    public Transform animationHipTransform;
    public Transform animationItemTransform;
    public Transform animationLookTransform;
    public Transform animationPositionTransform;
    public Transform backpackTransform;
  }
}
