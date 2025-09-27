// Decompiled with JetBrains decompiler
// Type: CharacterMovement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Settings;

#nullable disable
public class CharacterMovement : MonoBehaviour
{
  private Character character;
  public float movementForce = 10f;
  public float movementModifier = 1f;
  public float sprintMultiplier = 1f;
  public float sprintStaminaUsage = 0.025f;
  public float drag = 0.85f;
  public float movementTurnSpeed = 2f;
  public float animationForce = 100f;
  public float animationTorque = 10f;
  public float standForce;
  public float standSmooth = 0.2f;
  public float jumpImpulse;
  public float jumpGravity = 10f;
  public float jumpStaminaUsage;
  public float jumpStaminaUsageSprinting;
  public float maxGravity = -20f;
  public AnimationCurve jumpGravityCurve;
  public float gravityCurveSpeed = 1f;
  public float airMovementTurnSpeed = 2f;
  public SFX_Instance[] boostPlayer;
  private MouseSensitivitySetting mouseSensSetting;
  private ControllerSensitivitySetting controllerSensSetting;
  private InvertXSetting invertXSetting;
  private InvertYSetting invertYSetting;
  private bool sprintToggleEnabled;
  private bool crouchToggleEnabled;
  public float balloonFloatMultiplier;
  public float balloonJumpMultiplier;
  private float fallDamageTime = 1.5f;
  private float shakeCooldown;
  private float maxAngle = 50f;
  private List<CharacterMovement.PlayerGroundSample> groundSamples = new List<CharacterMovement.PlayerGroundSample>();
  private List<CharacterMovement.PlayerGroundSample> groundSamples_All = new List<CharacterMovement.PlayerGroundSample>();

  private void Start()
  {
    this.character = this.GetComponent<Character>();
    this.mouseSensSetting = GameHandler.Instance.SettingsHandler.GetSetting<MouseSensitivitySetting>();
    this.controllerSensSetting = GameHandler.Instance.SettingsHandler.GetSetting<ControllerSensitivitySetting>();
    this.invertXSetting = GameHandler.Instance.SettingsHandler.GetSetting<InvertXSetting>();
    this.invertYSetting = GameHandler.Instance.SettingsHandler.GetSetting<InvertYSetting>();
  }

  internal bool CanMoveCamera() => !this.character.data.usingWheel;

  private void Update()
  {
    if ((bool) (UnityEngine.Object) this.character.data.lastStoodOnPlayer)
      this.CheckForPalJump(this.character.data.lastStoodOnPlayer);
    if (this.character.IsLocal)
    {
      if ((bool) (UnityEngine.Object) Singleton<MainCameraMovement>.Instance && Singleton<MainCameraMovement>.Instance.isGodCam)
        this.character.input.ResetInput();
      else
        this.character.input.Sample(this.character.CanDoInput());
    }
    if (this.CanMoveCamera())
      this.CameraLook();
    if (this.character.input.jumpWasPressed)
      this.TryToJump();
    this.SetMovementState();
    this.character.CalculateWorldMovementDir();
  }

  private void SetCrouch(bool setCrouch)
  {
    if (setCrouch == this.character.data.isCrouching)
      return;
    this.character.refs.view.RPC("RPCA_SetCrouch", RpcTarget.All, (object) setCrouch);
  }

  [PunRPC]
  public void RPCA_SetCrouch(bool setCrouch) => this.character.data.isCrouching = setCrouch;

  private void SetMovementState()
  {
    if (!this.character.refs.view.IsMine)
      return;
    if (this.character.input.crouchToggleWasPressed)
      this.crouchToggleEnabled = !this.crouchToggleEnabled;
    if ((this.crouchToggleEnabled || this.character.input.crouchIsPressed) && this.character.data.isGrounded)
      this.SetCrouch(true);
    else
      this.SetCrouch(false);
    if ((double) this.character.data.sinceGrounded > 0.20000000298023224 || this.character.data.isSprinting || this.character.data.isClimbing || this.character.data.isRopeClimbing)
      this.SetCrouch(false);
    if (!this.character.data.isGrounded || this.character.data.isSprinting)
      this.crouchToggleEnabled = false;
    if (this.character.data.isGrounded)
    {
      if (this.character.input.sprintToggleWasPressed)
        this.sprintToggleEnabled = true;
      this.character.data.isSprinting = (double) this.character.input.movementInput.y > 0.0099999997764825821 && (this.character.input.sprintIsPressed || this.sprintToggleEnabled) && this.character.CheckSprint() && !this.character.OutOfRegularStamina();
      if (this.character.data.isSprinting)
        this.character.UseStamina(this.sprintStaminaUsage * Time.deltaTime);
      else
        this.sprintToggleEnabled = false;
    }
    else
    {
      this.character.data.isSprinting = (double) this.character.input.movementInput.y > 0.0099999997764825821 && (this.character.input.sprintIsPressed || this.sprintToggleEnabled) && this.character.CheckSprint();
      if (this.character.data.isSprinting)
        return;
      this.sprintToggleEnabled = false;
    }
  }

  private void CameraLook()
  {
    float num1 = 0.1f;
    float num2 = InputHandler.GetCurrentUsedInputScheme() != InputScheme.KeyboardMouse ? num1 * this.controllerSensSetting.Value : num1 * this.mouseSensSetting.Value;
    this.character.data.lookValues.x += (float) ((double) this.character.input.lookInput.x * (double) num2 * (this.invertXSetting.Value == OffOnMode.OFF ? 1.0 : -1.0));
    this.character.data.lookValues.y += (float) ((double) this.character.input.lookInput.y * (double) num2 * (this.invertYSetting.Value == OffOnMode.OFF ? 1.0 : -1.0));
    this.character.data.lookValues.y = Mathf.Clamp(this.character.data.lookValues.y, -85f, 85f);
    this.character.RecalculateLookDirections();
  }

  private void FixedUpdate()
  {
    this.UpdateVariables();
    this.RaycastGroundCheck();
    this.EvaluateGroundChecks();
    if (this.character.data.isGrounded && this.character.CheckStand())
      this.Stand();
    Vector3 gravityForce = this.GetGravityForce();
    float movementForce = this.GetMovementForce();
    if ((bool) (UnityEngine.Object) this.character.data.currentItem)
    {
      this.character.refs.items.AddGravity(gravityForce);
      this.character.refs.items.AddMovementForce(movementForce);
      this.character.refs.items.AddDrag(this.drag);
    }
    for (int index = 0; index < this.character.refs.ragdoll.partList.Count; ++index)
    {
      this.character.refs.ragdoll.partList[index].Animate(this.animationForce * this.character.data.currentRagdollControll, this.animationTorque * this.character.data.currentRagdollControll);
      if (!this.character.data.isGrounded)
        this.character.refs.ragdoll.partList[index].Gravity(gravityForce * this.character.data.currentRagdollControll * this.balloonFloatMultiplier);
      this.character.refs.ragdoll.partList[index].ToggleUseGravity((double) this.character.data.currentRagdollControll < 0.89999997615814209);
      this.character.refs.ragdoll.partList[index].AddMovementForce(movementForce * this.character.data.currentRagdollControll);
      this.character.refs.ragdoll.partList[index].Drag(this.drag);
      this.character.refs.ragdoll.partList[index].ApplyForces();
    }
  }

  public void ApplyExtraDrag(float extraDrag, bool ignoreRagdoll = false)
  {
    if ((bool) (UnityEngine.Object) this.character.data.currentItem)
      this.character.refs.items.AddDrag(Mathf.Lerp(1f, extraDrag, this.character.data.currentRagdollControll));
    for (int index = 0; index < this.character.refs.ragdoll.partList.Count; ++index)
      this.character.refs.ragdoll.partList[index].Drag(extraDrag, ignoreRagdoll);
  }

  public void ApplyParasolDrag(float extraDrag, float extraXZDrag, bool ignoreRagdoll = false)
  {
    if ((bool) (UnityEngine.Object) this.character.data.currentItem)
      this.character.refs.items.AddParasolDrag(Mathf.Lerp(1f, extraDrag, this.character.data.currentRagdollControll), Mathf.Lerp(1f, extraXZDrag, this.character.data.currentRagdollControll));
    for (int index = 0; index < this.character.refs.ragdoll.partList.Count; ++index)
      this.character.refs.ragdoll.partList[index].ParasolDrag(extraDrag, extraXZDrag, ignoreRagdoll);
  }

  private float GetMovementForce()
  {
    if (!this.character.CheckMovement())
      return 0.0f;
    float movementForce = this.movementForce * this.movementModifier;
    if (this.character.data.isSprinting)
      movementForce *= this.sprintMultiplier;
    if (this.character.data.isCrouching)
      movementForce *= 0.5f;
    return movementForce;
  }

  private void TryToJump()
  {
    if (this.character.data.jumpsRemaining <= 0 || !this.character.CheckJump() || (double) this.character.data.sinceGrounded > 0.20000000298023224 || (double) this.character.data.sinceJump < 0.30000001192092896 || this.character.data.chargingJump)
      return;
    this.character.refs.view.RPC("JumpRpc", RpcTarget.All, (object) false);
  }

  [PunRPC]
  public void JumpRpc(bool isPalJump)
  {
    float staminaCostMult = 1f;
    float jumpMult = 1f;
    Vector3 jumpDir = Vector3.up;
    if (isPalJump)
    {
      staminaCostMult = 0.0f;
      jumpMult = 2f;
      this.character.data.sincePalJump = 0.0f;
      jumpDir += this.character.data.lookDirection_Flat * 0.25f;
      for (int index = 0; index < this.boostPlayer.Length; ++index)
        this.boostPlayer[index].Play(this.character.Center);
    }
    --this.character.data.jumpsRemaining;
    this.character.data.isCrouching = false;
    this.character.data.chargingJump = true;
    this.character.OnStartJump();
    this.StartCoroutine(IDoJump());

    IEnumerator IDoJump()
    {
      float jumpImpulse = this.jumpImpulse;
      if (this.character.OutOfStamina())
      {
        float num = jumpImpulse * 0.5f;
      }
      float c = 0.0f;
      while ((double) c < 0.10000000149011612)
      {
        this.character.data.sinceGrounded = 0.0f;
        this.character.data.sinceJump = 0.0f;
        c += Time.deltaTime;
        yield return (object) null;
      }
      this.character.OnJump();
      this.character.data.chargingJump = false;
      this.character.data.isJumping = true;
      bool useBonusStamina = false;
      if ((double) this.character.GetTotalStamina() > (double) this.jumpStaminaUsageSprinting && this.character.input.sprintIsPressed)
        useBonusStamina = true;
      this.character.data.sprintJump = useBonusStamina;
      this.character.UseStamina((useBonusStamina ? this.jumpStaminaUsageSprinting : this.jumpStaminaUsage) * staminaCostMult, useBonusStamina);
      foreach (Bodypart part in this.character.refs.ragdoll.partList)
        part.AddForce(jumpDir * this.jumpImpulse * jumpMult * this.balloonJumpMultiplier, ForceMode.Acceleration);
    }
  }

  private void UpdateVariables()
  {
    if ((UnityEngine.Object) this.character.refs.ragdoll == (UnityEngine.Object) null || this.character.refs.ragdoll.partList == null)
      return;
    this.character.data.avarageLastFrameVelocity = this.character.data.avarageVelocity;
    this.character.data.avarageVelocity = Vector3.zero;
    for (int index = 0; index < this.character.refs.ragdoll.partList.Count; ++index)
      this.character.data.avarageVelocity += this.character.refs.ragdoll.partList[index].Rig.linearVelocity / (float) this.character.refs.ragdoll.partList.Count;
  }

  private Vector3 GetGravityForce()
  {
    float num = 0.0f;
    if (!this.character.data.isGrounded && this.character.CheckGravity())
    {
      float t = this.jumpGravityCurve.Evaluate(this.character.data.sinceGrounded * this.gravityCurveSpeed);
      num = !this.character.data.isJumping ? Mathf.Lerp(0.0f, this.maxGravity, t) : Mathf.Lerp(this.jumpGravity, this.maxGravity, t);
    }
    return num * Vector3.up;
  }

  private void Stand()
  {
    double targetHeadHeight = (double) this.character.data.targetHeadHeight;
    float num1 = Mathf.InverseLerp((float) targetHeadHeight, (float) targetHeadHeight - this.standSmooth, this.character.data.currentHeadHeight);
    float num2 = Mathf.InverseLerp((float) targetHeadHeight, (float) targetHeadHeight + this.standSmooth, this.character.data.currentHeadHeight);
    this.character.GetBodypart(BodypartType.Head).Rig.AddForce(Vector3.up * (num1 + -num2) * this.standForce * this.character.data.currentRagdollControll, ForceMode.Acceleration);
    this.character.GetBodypart(BodypartType.Torso).Rig.AddForce(Vector3.up * (num1 + -num2) * this.standForce * this.character.data.currentRagdollControll, ForceMode.Acceleration);
    this.character.GetBodypart(BodypartType.Hip).Rig.AddForce(Vector3.up * (num1 + -num2) * this.standForce * this.character.data.currentRagdollControll, ForceMode.Acceleration);
  }

  private void EvaluateGroundChecks()
  {
    CharacterMovement.PlayerGroundSample bestSample = (CharacterMovement.PlayerGroundSample) null;
    for (int index = 0; index < this.groundSamples.Count; ++index)
    {
      if (bestSample == null)
        bestSample = this.groundSamples[index];
      else if ((double) this.groundSamples[index].point.y > (double) bestSample.point.y)
        bestSample = this.groundSamples[index];
    }
    if (bestSample == null)
      bestSample = this.IsLodged();
    if (bestSample != null && this.CanStand())
    {
      if (!this.character.data.isGrounded)
        this.Land(bestSample);
      this.character.data.hasClimbedSinceGrounded = false;
      this.character.data.jumpsRemaining = 1;
      this.character.data.isJumping = false;
      this.character.data.isGrounded = true;
      this.character.data.groundNormal = bestSample.normal;
      this.character.data.groundPos = bestSample.point;
      this.character.data.currentHeadHeight = this.character.GetBodypart(BodypartType.Head).Rig.transform.position.y - bestSample.point.y;
    }
    else
      this.character.data.isGrounded = false;
    this.groundSamples.Clear();
    this.groundSamples_All.Clear();
  }

  private CharacterMovement.PlayerGroundSample IsLodged()
  {
    Vector3 vector3_1 = Vector3.zero;
    Vector3 vector3_2 = Vector3.zero;
    CharacterMovement.PlayerGroundSample playerGroundSample = (CharacterMovement.PlayerGroundSample) null;
    for (int index = 0; index < this.groundSamples_All.Count; ++index)
    {
      Vector3 normal = this.groundSamples_All[index].normal;
      if ((double) normal.y > 0.0)
      {
        vector3_2 = new Vector3(Mathf.Min(vector3_2.x, normal.x), Mathf.Min(vector3_2.y, normal.y), Mathf.Min(vector3_2.z, normal.z));
        vector3_1 = new Vector3(Mathf.Max(vector3_1.x, normal.x), Mathf.Max(vector3_1.y, normal.y), Mathf.Max(vector3_1.z, normal.z));
      }
      if (playerGroundSample == null)
        playerGroundSample = this.groundSamples_All[index];
      else if ((double) this.groundSamples_All[index].point.y > (double) playerGroundSample.point.y)
        playerGroundSample = this.groundSamples_All[index];
    }
    Vector3 from = (vector3_1 + vector3_2) / 2f;
    if ((double) from.magnitude < 0.10000000149011612)
      playerGroundSample = (CharacterMovement.PlayerGroundSample) null;
    if (playerGroundSample != null && !this.AcceptableAngle(Vector3.Angle(from, Vector3.up)))
      playerGroundSample = (CharacterMovement.PlayerGroundSample) null;
    return playerGroundSample;
  }

  private bool CanStand()
  {
    return (double) this.character.data.sinceJump > 0.30000001192092896 && (UnityEngine.Object) this.character.data.currentClimbHandle == (UnityEngine.Object) null && !this.character.data.isClimbing;
  }

  private void Land(CharacterMovement.PlayerGroundSample bestSample)
  {
    if ((double) this.character.data.sinceGrounded <= 0.5)
      return;
    this.CheckFallDamage();
    if (this.character.IsLocal)
      GUIManager.instance.ReticleLand();
    this.character.OnLand(this.character.data.sinceGrounded);
  }

  private void CheckFallDamage()
  {
    if ((double) this.FallTime() <= (double) this.fallDamageTime)
      return;
    float a = Mathf.Max(this.FallFactor(), 0.05f);
    float num1 = a;
    float num2 = Mathf.Min(a, this.MaxVelDmg());
    double num3 = (double) num2 / (double) num1;
    if ((double) num2 < 0.02500000037252903)
      return;
    Debug.Log((object) $"Fall damage: {(num2 * 100f).ToString("F0")}%");
    if ((double) num2 > 0.30000001192092896 && this.character.IsLocal)
      this.character.Fall(num2 * 5f);
    float num4 = num2 * Ascents.fallDamageMultiplier;
    if (!this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num4))
      return;
    Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken, num4);
  }

  private float MaxVelDmg()
  {
    return Mathf.Pow(Mathf.InverseLerp(10f, 20f, Mathf.Abs(Mathf.Min(this.character.data.avarageLastFrameVelocity.y, 0.0f))), 1.5f);
  }

  private float FallTime()
  {
    float num = Mathf.Min(this.character.data.sinceJump, this.character.data.sinceGrounded);
    if ((double) this.character.data.sinceGrounded - (double) this.character.data.sinceJump > -0.05000000074505806)
      num -= 0.5f;
    return num;
  }

  private float FallFactor(float maxTime = 3f, float pow = 1.5f)
  {
    float num = this.FallTime();
    return Mathf.Pow(Mathf.InverseLerp(this.fallDamageTime, maxTime, num), 1.5f);
  }

  public void AddGroundSample(CharacterMovement.PlayerGroundSample sample)
  {
    this.groundSamples.Add(sample);
  }

  public void AddGroundSample_All(CharacterMovement.PlayerGroundSample sample)
  {
    this.groundSamples_All.Add(sample);
  }

  private bool AcceptableAngle(float angle)
  {
    float maxAngle = this.maxAngle;
    return (double) angle < (double) maxAngle;
  }

  private void RaycastGroundCheck()
  {
    Vector3 position = this.character.GetBodypartRig(BodypartType.Hip).position;
    RaycastHit raycastHit = HelperFunctions.LineCheck(position, position + Vector3.down * (this.character.data.targetHipHeight + 0.3f), HelperFunctions.LayerType.TerrainMap);
    if (!(bool) (UnityEngine.Object) raycastHit.transform)
      return;
    CollisionModifier component = raycastHit.collider.GetComponent<CollisionModifier>();
    if ((bool) (UnityEngine.Object) component && (!component.standable || !component.CanStand(this.character)))
      return;
    float num = Vector3.Angle(Vector3.up, raycastHit.normal);
    if (!this.AcceptableAngle(num))
    {
      if (this.character.data.isClimbing || this.character.data.isRopeClimbing || (double) this.character.data.sinceFallSlide >= 0.2 && (double) this.character.data.sinceGrounded >= 2.0)
        return;
      this.character.data.sinceFallSlide = 0.0f;
      this.shakeCooldown += Time.deltaTime;
      this.ApplyExtraDrag(0.9f);
      this.LowerFall(num);
      if ((double) this.shakeCooldown <= 0.10000000149011612 || (double) this.FallTime() <= (double) this.fallDamageTime)
        return;
      if (this.character.IsLocal)
        GamefeelHandler.instance.AddPerlinShake(5f * this.FallFactor(pow: 1f), scale: 10f);
      this.shakeCooldown = 0.0f;
    }
    else
    {
      if (!this.StandableRig(raycastHit.rigidbody) || !this.DoGroundChecks() || (double) this.character.data.groundedFor <= 0.10000000149011612)
        return;
      this.AddGroundSample(new CharacterMovement.PlayerGroundSample(raycastHit.point, raycastHit.normal));
    }
  }

  private void LowerFall(float upAngle)
  {
    float num = Mathf.InverseLerp(60f, 40f, upAngle);
    if ((double) this.character.data.sinceGrounded <= 1.0)
      return;
    this.character.data.sinceGrounded = Mathf.MoveTowards(this.character.data.sinceGrounded, 1f, (float) ((double) num * (double) Time.deltaTime * 2.0));
  }

  internal void OnCollision(Collision collision, bool collisionEnter, Bodypart bodypart)
  {
    CollisionModifier component = collision.collider.GetComponent<CollisionModifier>();
    if ((bool) (UnityEngine.Object) component)
    {
      component.Collide(this.character, collision.contacts[0], collision, bodypart);
      if (!component.standable || !component.CanStand(this.character))
        return;
    }
    bool flag = false;
    if (this.StandOnPlayer(collision))
      flag = true;
    else if (!this.StandableRig(collision.rigidbody))
      return;
    float angle = Vector3.Angle(Vector3.up, collision.contacts[0].normal);
    if (!this.DoGroundChecks())
      return;
    if (this.AcceptableAngle(angle) | flag)
      this.AddGroundSample(new CharacterMovement.PlayerGroundSample(collision.contacts[0].point, collision.contacts[0].normal));
    this.AddGroundSample_All(new CharacterMovement.PlayerGroundSample(collision.contacts[0].point, collision.contacts[0].normal));
  }

  private bool StandOnPlayer(Collision collision)
  {
    if ((double) this.character.data.sincePalJump < 0.5 || !(bool) (UnityEngine.Object) collision.rigidbody)
      return false;
    Character componentInParent = collision.rigidbody.GetComponentInParent<Character>();
    if ((UnityEngine.Object) componentInParent == (UnityEngine.Object) this.character || !(bool) (UnityEngine.Object) componentInParent || this.character.data.isCrouching || !componentInParent.data.isCrouching)
      return false;
    this.character.data.sinceStandOnPlayer = 0.0f;
    this.character.data.lastStoodOnPlayer = componentInParent;
    return true;
  }

  private void CheckForPalJump(Character c)
  {
    if ((double) this.character.data.sinceStandOnPlayer >= 0.30000001192092896 || (double) c.data.sinceJump >= 0.30000001192092896)
      return;
    this.character.data.lastStoodOnPlayer = (Character) null;
    if (!this.character.refs.view.IsMine)
      return;
    this.character.refs.view.RPC("JumpRpc", RpcTarget.All, (object) true);
  }

  private bool StandableRig(Rigidbody rig)
  {
    return (UnityEngine.Object) rig == (UnityEngine.Object) null || (double) rig.mass > 500.0 || rig.isKinematic;
  }

  private bool DoGroundChecks() => !this.character.data.isClimbing;

  [Serializable]
  public class PlayerGroundSample
  {
    public Vector3 point;
    public Vector3 normal;

    public PlayerGroundSample(Vector3 point, Vector3 normal)
    {
      this.point = point;
      this.normal = normal;
    }
  }
}
