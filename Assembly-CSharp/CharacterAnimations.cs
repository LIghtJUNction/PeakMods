// Decompiled with JetBrains decompiler
// Type: CharacterAnimations
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;

#nullable disable
public class CharacterAnimations : MonoBehaviour
{
  public GameObject point;
  private Character character;
  public Transform lookRef;
  [HideInInspector]
  public float throwTime;
  private bool _headBobRemoved;
  public RuntimeAnimatorController defaultController;
  public RuntimeAnimatorController noHeadBobController;
  public HeadBobSetting headBobSetting;
  public StormAudio stormAudio;
  public AmbienceAudio ambienceAudio;
  private Vector3 ReachHandPos = new Vector3(-30f, -90f, -70f);
  private bool emoting;
  private float sinceEmoteStart = 10f;

  private void Awake() => this.character = this.GetComponent<Character>();

  private void Start()
  {
    this.character.landAction += new Action<float>(this.Land);
    this.character.startJumpAction += new Action(this.StartJump);
    this.character.jumpAction += new Action(this.Jump);
    this.character.startClimbAction += new Action(this.StartClimb);
    this.headBobSetting = GameHandler.Instance.SettingsHandler.GetSetting<HeadBobSetting>();
    this.UpdateHeadBob();
  }

  private void UpdateHeadBob()
  {
    if (!((UnityEngine.Object) this.character == (UnityEngine.Object) Character.localCharacter))
      return;
    this.character.refs.animator.runtimeAnimatorController = this.headBobSetting.Value == OffOnMode.ON ? this.noHeadBobController : this.defaultController;
  }

  private void Update()
  {
    Animator animator = this.character.refs.animator;
    bool flag = this.headBobSetting.Value == OffOnMode.ON;
    if (flag != this._headBobRemoved)
    {
      this._headBobRemoved = flag;
      this.UpdateHeadBob();
    }
    if ((bool) (UnityEngine.Object) this.point)
    {
      animator.SetBool("Point", true);
      animator.SetFloat("Point X", this.character.GetBodypart(BodypartType.Hip).transform.InverseTransformPoint(this.point.transform.position).x / (Vector3.Distance(this.character.GetBodypart(BodypartType.Hip).transform.position, this.point.transform.position) / 5f));
      animator.SetFloat("Point Y", this.character.GetBodypart(BodypartType.Hip).transform.InverseTransformPoint(this.point.transform.position).y / (Vector3.Distance(this.character.GetBodypart(BodypartType.Hip).transform.position, this.point.transform.position) / 5f));
    }
    else
      animator.SetBool("Point", false);
    animator.SetBool("Climb Surface", this.character.data.isClimbing);
    animator.SetBool("Climb Rope", this.character.data.isRopeClimbing);
    animator.SetFloat("Input X", this.character.input.movementInput.x, 0.125f, Time.deltaTime);
    animator.SetFloat("Input Y", this.character.input.movementInput.y, 0.125f, Time.deltaTime);
    animator.SetFloat("Throw Charge", this.character.refs.items.throwChargeLevel);
    animator.SetFloat("Throw", this.throwTime);
    if ((double) Mathf.Abs(animator.GetFloat("Input X")) < 0.125 && (double) Mathf.Abs(this.character.input.movementInput.x) < 0.125)
      animator.SetFloat("Input X", 0.0f);
    if ((double) Mathf.Abs(animator.GetFloat("Input Y")) < 0.125 && (double) Mathf.Abs(this.character.input.movementInput.y) < 0.125)
      animator.SetFloat("Input Y", 0.0f);
    animator.SetBool("Is Grounded", true);
    animator.SetFloat("Velocity Y", this.character.data.avarageVelocity.y);
    animator.SetFloat("Velocity Z", this.character.data.avarageVelocity.z);
    if ((bool) (UnityEngine.Object) this.lookRef)
    {
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.character.GetCameraPos(0.0f), Quaternion.LookRotation(this.character.data.lookDirection, Vector3.up), Vector3.one);
      this.lookRef.rotation = Quaternion.Euler(0.0f, matrix4x4.rotation.eulerAngles.y, 0.0f);
      animator.SetFloat("Look Y", matrix4x4.inverse.TransformDirection((float3) this.lookRef.forward).y);
      animator.SetFloat("Look X", this.character.input.lookInput.x, 0.25f, Time.deltaTime);
    }
    if ((double) this.character.data.sinceGrounded > 0.30000001192092896 || (double) this.character.data.avarageVelocity.y > 5.0 || this.character.data.isJumping || (double) this.character.data.sinceClimb < 0.25)
      animator.SetBool("Is Grounded", false);
    if (this.character.data.isSprinting)
      animator.SetFloat("Sprint", 1f, 0.125f, Time.deltaTime);
    if (!this.character.data.isSprinting)
      animator.SetFloat("Sprint", 0.0f, 0.125f, Time.deltaTime);
    animator.SetBool("Crouch", this.character.data.isCrouching);
    animator.SetBool("Reach", this.character.data.isReaching);
    animator.SetBool("Grab", (bool) (UnityEngine.Object) this.character.data.grabJoint);
    animator.SetBool("Vine Hang", this.character.data.isVineClimbing);
    if (this.character.data.isVineClimbing && (bool) (UnityEngine.Object) this.character.data.heldVine)
    {
      animator.SetBool("Vine Slide", this.character.refs.vineClimbing.Sliding());
      animator.SetInteger("Vine Type", this.character.data.heldVine.vineType);
    }
    animator.SetBool("Is Sliding", this.character.IsSliding());
    animator.SetBool("Climb Jump", (double) this.character.data.sinceClimbJump < 0.30000001192092896);
    if (!this.character.data.isSprinting && (double) animator.GetFloat("Sprint") < 0.75)
      animator.SetFloat("Sprint", 0.0f);
    animator.SetBool("Charge Jump", this.character.data.chargingJump);
    animator.SetBool("Jump", this.character.data.isJumping);
    animator.SetFloat("Since Grounded", this.character.data.sinceGrounded, 0.25f, Time.deltaTime);
    animator.SetInteger("Reach Type", 0);
    animator.SetFloat("Myers Distance", this.character.data.myersDistance);
    this.character.data.myersDistance = 1000f;
    animator.SetBool("Hang", (UnityEngine.Object) this.character.data.currentClimbHandle != (UnityEngine.Object) null);
    animator.SetBool("Help", false);
    animator.SetFloat("Floor Lean Forward", this.character.data.groundedForward.normalized.y, 0.2f, Time.deltaTime);
    animator.SetFloat("Floor Lean Right", this.character.data.groundedRight.normalized.y, 0.2f, Time.deltaTime);
    if ((double) this.character.data.grabFriendDistance <= 3.5 && !this.character.data.isClimbing)
      animator.SetBool("Help", true);
    if (!animator.GetBool("Is Grounded"))
      animator.SetInteger("Reach Type", 1);
    if (this.character.data.isCrouching)
      animator.SetInteger("Reach Type", 2);
    this.HandleIK();
    this.SetAnimSpeed();
    this.character.refs.animationPositionTransform.position = this.character.GetBodypart(BodypartType.Hip).transform.position;
    this.throwTime -= Time.deltaTime;
    if ((double) this.throwTime <= 0.0)
      this.throwTime = 0.0f;
    this.sinceEmoteStart += Time.deltaTime;
    if (!this.emoting || (double) this.sinceEmoteStart <= 2.0 && ((double) this.sinceEmoteStart <= 0.699999988079071 || (double) this.character.input.movementInput.magnitude <= 0.10000000149011612 && !this.character.input.jumpWasPressed && (double) this.character.data.sinceGrounded <= 0.20000000298023224))
      return;
    this.character.refs.animator.SetBool("Emote", false);
    this.emoting = false;
  }

  private void SetAnimSpeed()
  {
    if ((bool) (UnityEngine.Object) this.character.data.carrier)
      this.character.refs.animator.speed = 1f;
    else if (this.character.data.dead || this.character.data.fullyPassedOut)
      this.character.refs.animator.speed = 0.0f;
    else if (this.character.data.isClimbing && (double) this.character.data.sinceClimbJump > 0.5)
      this.character.refs.animator.speed = this.character.data.staminaMod;
    else
      this.character.refs.animator.speed = 1f;
  }

  private bool ReachIK()
  {
    return !this.character.data.isCrouching && this.character.data.isReaching && (double) this.character.data.sinceGrabFriend > 0.5;
  }

  private void HandleIK()
  {
    if (!(bool) (UnityEngine.Object) this.character.refs.ikRight)
      return;
    if (this.ReachIK())
    {
      this.character.refs.ikRig.weight = 1f;
      this.character.refs.ikRight.weight = 1f;
      this.character.refs.ikLeft.weight = 0.0f;
    }
    else if ((bool) (UnityEngine.Object) this.character.data.currentItem && (double) this.character.data.overrideIKForSeconds <= 0.0)
    {
      this.character.refs.ikRig.weight = 1f;
      this.character.refs.ikRight.weight = 1f;
      this.character.refs.ikLeft.weight = 1f;
    }
    else
      this.character.refs.ikRig.weight = 0.0f;
  }

  private void Land(float sinceGrounded)
  {
  }

  private void Jump()
  {
  }

  private void StartJump()
  {
  }

  private void StartClimb()
  {
  }

  public void PlaySpecificAnimation(string animationName)
  {
    if ((UnityEngine.Object) this.character.refs.animator == (UnityEngine.Object) null)
      return;
    this.character.refs.animator.Play(animationName, 0, 0.0f);
  }

  public void PrepIK()
  {
  }

  public void ConfigureIK()
  {
    if ((UnityEngine.Object) this.character.refs.IKHandTargetLeft == (UnityEngine.Object) null)
      return;
    if ((bool) (UnityEngine.Object) this.character.data.currentItem)
    {
      this.character.refs.IKHandTargetLeft.position = this.character.refs.items.GetItemPosLeft(this.character.data.currentItem);
      this.character.refs.IKHandTargetRight.position = this.character.refs.items.GetItemPosRight(this.character.data.currentItem);
      this.character.refs.IKHandTargetRight.rotation = this.character.refs.items.GetItemRotRight(this.character.data.currentItem);
      this.character.refs.IKHandTargetLeft.rotation = this.character.refs.items.GetItemRotLeft(this.character.data.currentItem);
    }
    else
    {
      if (!this.ReachIK())
        return;
      this.character.refs.IKHandTargetRight.position = this.character.refs.animationHeadTransform.position + this.character.refs.animationLookTransform.TransformDirection(new Vector3(0.15f, -0.1f, 1.5f));
      this.character.refs.IKHandTargetRight.localEulerAngles = new Vector3(this.ReachHandPos.x, this.ReachHandPos.y, this.ReachHandPos.z + this.character.data.lookValues.y);
    }
  }

  internal void PlayEmote(string emoteName)
  {
    this.character.refs.view.RPC("RPCA_PlayRemove", RpcTarget.All, (object) emoteName);
  }

  [PunRPC]
  private void RPCA_PlayRemove(string emoteName)
  {
    if (emoteName == "A_Scout_Emote_Flex")
    {
      this.character.Fall(3f);
    }
    else
    {
      this.character.refs.animator.SetBool("Emote", true);
      this.character.refs.animator.Play(emoteName, 0, 0.0f);
      this.sinceEmoteStart = 0.0f;
      this.emoting = true;
    }
  }

  internal void SetBool(string boolKey, bool boolValue)
  {
    this.character.refs.animator.SetBool(boolKey, boolValue);
  }
}
