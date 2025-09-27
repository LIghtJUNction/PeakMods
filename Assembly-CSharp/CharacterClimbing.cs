// Decompiled with JetBrains decompiler
// Type: CharacterClimbing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Settings;

#nullable disable
public class CharacterClimbing : MonoBehaviour
{
  private Character character;
  public float outOfStamAttachSlide = 1f;
  public float climbForce;
  public float climbSpeed;
  public float climbSpeedMod = 1f;
  public float climbDrag = 0.85f;
  public float maxStaminaUsage = 0.2f;
  public float minStaminaUsage = 0.02f;
  private PhotonView view;
  public Vector2 playerSlide;
  private float sinceShake;
  private Vector2 handleOffset;
  private bool sprintHasBeenPressedSinceClimb;
  private bool climbToggledOn;
  private float sinceLastClimbStarted;

  private void Start()
  {
    this.view = this.GetComponent<PhotonView>();
    this.character = this.GetComponent<Character>();
    this.character.dragTowardsAction += new Action<Vector3, float>(this.GetDragged);
  }

  private void FixedUpdate()
  {
    if ((bool) (UnityEngine.Object) this.character.data.currentClimbHandle)
      this.HandleClimbHandle();
    if (!this.character.data.isClimbing)
      return;
    this.Climbing();
  }

  private void Update()
  {
    if (!this.view.IsMine)
      return;
    this.ClimbHandleUpdate();
    this.sinceLastClimbStarted += Time.deltaTime;
    if (!this.character.data.isClimbing)
    {
      this.sprintHasBeenPressedSinceClimb = false;
      this.climbToggledOn = false;
      if (!((UnityEngine.Object) this.character.data.currentClimbHandle == (UnityEngine.Object) null))
        return;
      this.TryToStartWallClimb();
    }
    else
    {
      if (this.character.input.sprintWasPressed || this.character.input.sprintToggleWasPressed)
        this.sprintHasBeenPressedSinceClimb = true;
      if (this.sprintHasBeenPressedSinceClimb && (this.character.input.sprintIsPressed || this.character.input.sprintToggleIsPressed) && (double) this.character.data.sinceClimbJump > 1.0 && (double) this.character.data.outOfStaminaFor < 0.5 && (double) this.character.input.movementInput.magnitude > 0.10000000149011612 && (double) this.character.input.movementInput.normalized.y > -0.89999997615814209)
        this.character.refs.view.RPC("RPCA_ClimbJump", RpcTarget.All);
      this.sinceShake += Time.deltaTime;
      if (this.character.OutOfStamina() && (double) this.sinceShake > 0.10000000149011612 && this.character.refs.view.IsMine)
      {
        GamefeelHandler.instance.AddPerlinShake(3f * Mathf.Clamp01(this.character.data.outOfStaminaFor * 1f), scale: 10f);
        this.sinceShake = 0.0f;
      }
      float num = Mathf.Clamp(this.maxStaminaUsage * Mathf.Clamp(this.character.input.movementInput.magnitude, 0.0f, 1f), this.minStaminaUsage, this.maxStaminaUsage);
      if (!this.character.data.staticClimbCost)
        num *= this.GetAngleUsage();
      this.character.UseStamina(num * Time.deltaTime * this.character.data.staminaMod);
      this.TestAchievement();
      if (!this.character.input.jumpWasPressed && (!this.character.input.usePrimaryWasReleased || this.climbToggledOn) && (double) this.character.data.currentRagdollControll >= 0.25)
        return;
      this.view.RPC("StopClimbingRpc", RpcTarget.All, (object) this.GetFallSpeed());
    }
  }

  private float GetAngleUsage()
  {
    return Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(40f, 60f, Vector3.Angle(Vector3.up, this.character.data.climbNormal)));
  }

  private void ClimbHandleUpdate()
  {
    if ((bool) (UnityEngine.Object) this.character.data.currentClimbHandle && this.view.IsMine)
    {
      if (this.character.data.fullyPassedOut || this.character.data.dead)
        this.CancelHandle(false);
      else if (this.character.input.jumpWasPressed)
      {
        if (GameHandler.Instance.SettingsHandler.GetSetting<JumpToClimbSetting>().Value == OffOnMode.ON)
          this.CancelHandle();
        else
          this.CancelHandle(false);
      }
      else if (this.character.data.isRopeClimbing)
      {
        this.CancelHandle(false);
      }
      else
      {
        if (!this.character.data.isVineClimbing)
          return;
        this.CancelHandle(false);
      }
    }
    else
      this.handleOffset = Vector2.zero;
  }

  public void CancelHandle(bool grabWall = true)
  {
    if (grabWall && this.character.IsLocal)
      this.TryToStartWallClimb(true, this.character.data.currentClimbHandle.transform.forward);
    this.character.data.currentClimbHandle.view.RPC("RPCA_UnHang", RpcTarget.All, (object) this.view);
    this.handleOffset = Vector2.zero;
  }

  private void HandleClimbHandle()
  {
    this.handleOffset = Vector2.Lerp(this.handleOffset, this.character.input.movementInput, Time.fixedDeltaTime);
    if ((double) this.handleOffset.magnitude > 0.30000001192092896 && this.view.IsMine)
    {
      this.CancelHandle();
    }
    else
    {
      this.character.data.sinceGrounded = 0.0f;
      Vector3 vector3_1 = (this.character.GetBodypartRig(BodypartType.Hand_R).position + this.character.GetBodypartRig(BodypartType.Hand_L).position) * 0.5f;
      Vector3 pos = this.character.data.currentClimbHandle.transform.TransformPoint(new Vector3(0.0f, -0.7f, -0.3f));
      this.character.MoveBodypartTowardsPoint(BodypartType.Hand_L, pos, 100f);
      this.character.MoveBodypartTowardsPoint(BodypartType.Hand_R, pos, 100f);
      Vector3 vector3_2 = this.character.TorsoPos() - vector3_1;
      this.character.AddForce((pos + vector3_2 - this.character.TorsoPos() + this.character.data.currentClimbHandle.transform.up * this.handleOffset.y + this.character.data.currentClimbHandle.transform.right * this.handleOffset.x) * 50f);
    }
  }

  public void StopClimbing()
  {
    if (!this.view.IsMine)
      return;
    Debug.Log((object) nameof (StopClimbing));
    this.view.RPC("StopClimbingRpc", RpcTarget.All, (object) this.GetFallSpeed());
  }

  [PunRPC]
  public void RPCA_ClimbJump()
  {
    this.character.data.sinceClimbJump = 0.0f;
    this.character.UseStamina(0.2f);
    this.playerSlide += this.character.input.movementInput.normalized * 8f;
    if (!this.view.IsMine || this.character.isBot)
      return;
    GamefeelHandler.instance.AddPerlinShake(10f, 0.5f, 10f);
    GUIManager.instance.ClimbJump();
  }

  private void GetDragged(Vector3 targetPos, float force)
  {
    this.character.data.climbPos += Vector3.ClampMagnitude(targetPos - this.character.Center, 1f) * (float) ((double) force * (double) Time.fixedDeltaTime * 0.10000000149011612);
  }

  private void Climbing()
  {
    if (!this.character.OutOfStamina())
      this.character.data.sinceGrounded = 0.0f;
    if ((double) this.character.data.sinceClimbJump > 0.5)
      this.playerSlide += Vector2.down * 200f * Mathf.Clamp01(Mathf.Pow(this.character.data.outOfStaminaFor * 0.15f, 2f)) * Time.fixedDeltaTime;
    if (!(bool) (UnityEngine.Object) this.SampleWall(this.GetRequestedPostition()).transform)
    {
      if (!this.view.IsMine)
        return;
      this.view.RPC("StopClimbingRpc", RpcTarget.All, (object) this.GetFallSpeed());
    }
    else
    {
      this.character.refs.movement.ApplyExtraDrag(this.climbDrag);
      this.character.AddForce(this.GetClimbDirection());
    }
  }

  private float GetFallSpeed()
  {
    return Mathf.Max(Mathf.InverseLerp(-5f, -60f, this.playerSlide.y) * 5f, 0.0f);
  }

  private Vector3 GetRequestedPostition()
  {
    Vector3 normalized1 = Vector3.ProjectOnPlane(Vector3.up, this.character.data.climbNormal).normalized;
    Vector3 normalized2 = Vector3.Cross(normalized1, this.character.data.climbNormal).normalized;
    Vector3 zero = Vector3.zero;
    ClimbModifierSurface climbMod = this.character.data.climbMod;
    float num = 1f;
    if ((bool) (UnityEngine.Object) climbMod)
      num = climbMod.speedMultiplier;
    if ((bool) (UnityEngine.Object) climbMod && climbMod.onlySlideDown)
      zero += normalized1 * -3f;
    else if ((double) this.character.data.sinceClimbJump > 0.5 && !this.character.OutOfStamina())
    {
      if ((double) this.character.input.movementInput.y < 0.0)
        zero += normalized1 * -3f;
      else
        zero += normalized1 * (this.character.input.movementInput.y * this.character.data.staminaMod * num);
    }
    Vector3 vector3 = zero + this.playerSlide.y * normalized1 * num + this.playerSlide.x * -normalized2 * num + normalized1 * -0.5f * Mathf.Clamp01(this.character.data.slippy);
    this.playerSlide *= 0.97f;
    this.playerSlide = Vector2.MoveTowards(this.playerSlide, Vector2.zero, Time.fixedDeltaTime * 15f);
    Vector3 a = vector3 + -normalized2 * (this.character.input.movementInput.x * this.character.data.staminaMod * num);
    if ((bool) (UnityEngine.Object) this.character.data.currentClimbHandle)
    {
      Vector3 b = Vector3.ClampMagnitude(this.HandlePos() - this.character.data.climbPos, 1f) * 5f;
      float t = 1f;
      if ((double) this.character.data.sinceClimbHandle > 0.5)
        t = Mathf.Lerp(1f, 0.15f, this.character.input.movementInput.magnitude);
      a = Vector3.Lerp(a, b, t);
    }
    return this.character.data.climbPos + a * (this.climbSpeed * Time.fixedDeltaTime * this.climbSpeedMod);
  }

  private Vector3 HandlePos()
  {
    return this.character.data.currentClimbHandle.transform.position + Vector3.down * 1.5f;
  }

  private Vector3 GetClimbDirection()
  {
    return (this.VisualClimberPos() - this.character.TorsoPos()) * this.climbForce;
  }

  private Vector3 VisualClimberPos()
  {
    return this.GetVisualClimberPos(this.character.data.climbPos, this.character.data.climbNormal);
  }

  private Vector3 GetVisualClimberPos(Vector3 samplePos, Vector3 sampleNormal)
  {
    return samplePos + sampleNormal * 0.4f;
  }

  private RaycastHit SampleWall(Vector3 samplePos)
  {
    this.character.data.staticClimbCost = false;
    Vector3 from = this.RaycastPos();
    Vector3 to1 = samplePos + this.character.data.climbNormal * 0.5f;
    Vector3 to2 = samplePos + this.character.data.climbNormal * -1f;
    RaycastHit hit = HelperFunctions.LineCheck(from, to1, HelperFunctions.LayerType.TerrainMap);
    if ((UnityEngine.Object) hit.transform == (UnityEngine.Object) null)
      hit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap);
    if ((UnityEngine.Object) hit.transform == (UnityEngine.Object) null)
      hit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.1f);
    if ((UnityEngine.Object) hit.transform == (UnityEngine.Object) null)
      hit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.2f);
    if ((UnityEngine.Object) hit.transform == (UnityEngine.Object) null)
      hit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.3f);
    if ((UnityEngine.Object) hit.transform == (UnityEngine.Object) null)
      hit = HelperFunctions.LineCheck(from, to2, HelperFunctions.LayerType.TerrainMap, 0.4f);
    if ((UnityEngine.Object) hit.transform == (UnityEngine.Object) null)
      return new RaycastHit();
    if ((bool) (UnityEngine.Object) hit.transform)
    {
      this.character.data.climbMod = hit.collider.GetComponent<ClimbModifierSurface>();
      float climbAngle = Vector3.Angle(hit.normal, Vector3.up);
      if ((bool) (UnityEngine.Object) this.character.data.climbMod)
      {
        climbAngle = this.character.data.climbMod.OverrideClimbAngle(this.character, climbAngle);
        this.character.data.staticClimbCost = this.character.data.climbMod.staticClimbCost;
      }
      float f = climbAngle - 90f;
      if ((double) f > 0.0)
      {
        if ((double) Mathf.Abs(f) > (this.character.OutOfStamina() ? 60.0 : 80.0))
          return new RaycastHit();
      }
      else if ((double) this.character.data.sinceClimbJump > 0.30000001192092896)
      {
        if ((double) this.character.input.movementInput.magnitude < 0.10000000149011612)
        {
          if ((double) Mathf.Abs(f) > 60.0)
          {
            this.CheckFallDamage(hit);
            return new RaycastHit();
          }
        }
        else if ((double) Mathf.Abs(f) > 40.0)
        {
          this.CheckFallDamage(hit);
          return new RaycastHit();
        }
      }
      if ((UnityEngine.Object) this.character.data.climbMod != (UnityEngine.Object) null)
        this.character.data.climbMod.OnClimb(this.character);
      this.character.data.climbPos = hit.point;
      this.character.data.climbNormal = hit.normal;
      this.character.data.climbHit = hit;
    }
    return hit;
  }

  private void CheckFallDamage(RaycastHit hit)
  {
    if ((double) this.playerSlide.y > 0.0)
      return;
    float num1 = (float) (((double) Mathf.Abs(this.playerSlide.y) - 15.0) * 0.035000000149011612);
    if ((double) num1 < 0.15000000596046448)
      return;
    float num2 = num1 - 0.05f;
    this.character.data.sinceGrounded = 0.0f;
    this.playerSlide = Vector2.zero;
    if ((double) num2 > 0.30000001192092896 && this.character.IsLocal)
      this.character.Fall(num2 * 5f);
    Debug.Log((object) ("Damage: " + num2.ToString()));
    float num3 = num2 * Ascents.fallDamageMultiplier;
    if (!this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num3))
      return;
    Singleton<AchievementManager>.Instance.AddToRunBasedFloat(RUNBASEDVALUETYPE.FallDamageTaken, num3);
  }

  private bool AcceptableGrabAngle(Vector3 normal, Collider collider)
  {
    float climbAngle = Vector3.Angle(normal, Vector3.up);
    ClimbModifierSurface component = collider.GetComponent<ClimbModifierSurface>();
    if ((bool) (UnityEngine.Object) component)
      climbAngle = component.OverrideClimbAngle(this.character, climbAngle);
    float f = climbAngle - 90f;
    if ((double) f > 0.0)
    {
      if ((double) Mathf.Abs(f) > 80.0)
        return false;
    }
    else if ((double) Mathf.Abs(f) > 40.0)
      return false;
    return true;
  }

  private void TryToStartWallClimb(bool forceAttempt = false, Vector3 overide = default (Vector3), bool botGrab = false)
  {
    if (!this.CanClimb() || this.character.isBot && !botGrab || !this.view.IsMine)
      return;
    Vector3 from = MainCamera.instance.transform.position;
    Vector3 vector3 = this.character.data.lookDirection;
    if (botGrab)
    {
      from = this.character.Center;
      vector3 = this.character.data.lookDirection_Flat.normalized;
    }
    if (forceAttempt)
      vector3 = overide;
    Vector3 to = from + vector3 * 1.25f;
    RaycastHit raycastHit = HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap);
    if (!(bool) (UnityEngine.Object) raycastHit.transform)
      raycastHit = HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap, 0.05f);
    if (!(bool) (UnityEngine.Object) raycastHit.transform)
      raycastHit = HelperFunctions.LineCheck(from, to, HelperFunctions.LayerType.TerrainMap, 0.1f);
    if (!(bool) (UnityEngine.Object) raycastHit.transform || !this.AcceptableGrabAngle(raycastHit.normal, raycastHit.collider) || (double) this.sinceLastClimbStarted <= 1.0 && this.character.OutOfStamina())
      return;
    this.character.data.sinceCanClimb = 0.0f;
    if ((((double) this.character.data.sincePressClimb < 0.10000000149011612 ? 1 : (this.validJumpToClimb ? 1 : 0)) | (forceAttempt ? 1 : 0) | (botGrab ? 1 : 0)) == 0)
      return;
    this.character.refs.items.EquipSlot(Optionable<byte>.None);
    if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
      this.climbToggledOn = true;
    this.sinceLastClimbStarted = 0.0f;
    this.view.RPC("StartClimbRpc", RpcTarget.All, (object) raycastHit.point, (object) raycastHit.normal);
  }

  private bool validJumpToClimb
  {
    get
    {
      return this.character.input.jumpWasPressed && (double) this.character.data.sinceGrounded > 0.10000000149011612 && GameHandler.Instance.SettingsHandler.GetSetting<JumpToClimbSetting>().Value == OffOnMode.ON;
    }
  }

  public bool CanClimb()
  {
    return (double) this.character.data.sinceClimb >= 0.20000000298023224 && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
  }

  private Vector3 RaycastPos()
  {
    return this.character.data.climbPos + this.character.data.climbNormal * 0.4f;
  }

  [PunRPC]
  private void StartClimbRpc(Vector3 climbPos, Vector3 climbNormal)
  {
    float y = 0.0f;
    if (this.character.data.hasClimbedSinceGrounded)
    {
      Vector3 lhs = Vector3.ProjectOnPlane((this.GetVisualClimberPos(climbPos, climbNormal) - (this.character.Center + Vector3.up * 0.5f)) * 1.5f, climbNormal);
      float a = lhs.magnitude;
      if ((double) Vector3.Dot(lhs, Vector3.up) < 0.0)
        a = 0.0f;
      float num = Mathf.Max(a, 0.1f);
      this.character.UseStamina(0.15f * num);
      if (this.character.OutOfStamina())
        y += -num * this.outOfStamAttachSlide;
    }
    if ((double) this.character.data.avarageVelocity.y < 0.0)
      y += this.character.data.avarageVelocity.y * 1.5f;
    this.character.OutOfStamina();
    this.playerSlide = new Vector2(this.playerSlide.x, y);
    this.character.data.climbPos = climbPos;
    this.character.data.climbNormal = climbNormal;
    this.character.data.hasClimbedSinceGrounded = true;
    this.character.data.isClimbing = true;
    this.character.data.isGrounded = false;
    this.character.data.sinceStartClimb = 0.0f;
    this.character.OnStartClimb();
  }

  [PunRPC]
  public void StopClimbingRpc(float setFall)
  {
    this.character.data.isClimbing = false;
    this.character.data.isJumping = false;
    this.character.data.sinceGrounded = setFall;
    if (this.character.OutOfStamina())
      this.character.data.sinceGrounded = Mathf.Clamp(this.character.data.sinceGrounded, 0.5f, 1000f);
    this.playerSlide = Vector2.zero;
    this.climbToggledOn = false;
    Debug.Log((object) "Stop Climbing");
  }

  internal void StartHang(ClimbHandle climbHandle)
  {
    this.character.data.currentClimbHandle = climbHandle;
    this.character.data.sinceClimbHandle = 0.0f;
    this.character.data.isClimbing = false;
    this.character.data.sinceGrounded = 0.0f;
  }

  internal void TryClimb() => this.TryToStartWallClimb(botGrab: true);

  internal void TestAchievement()
  {
    if (!this.character.IsLocal || !this.character.data.isClimbing || ((double) this.character.Center.y - (double) this.character.data.lastGroundedHeight) * (double) CharacterStats.unitsToMeters < 50.0)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.EnduranceBadge);
  }
}
