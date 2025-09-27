// Decompiled with JetBrains decompiler
// Type: CharacterRopeHandling
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterRopeHandling : MonoBehaviour
{
  private Character character;
  public float climbForce;
  public float climbSpeed;
  public float climbSpeedMod = 1f;
  public float climbDrag = 0.85f;
  public float staminaUsage;
  public float staminaUsageUp;
  private PhotonView view;
  public float maxRopeAngle = 90f;

  private void Awake() => this.view = this.GetComponent<PhotonView>();

  private void Start() => this.character = this.GetComponent<Character>();

  private void Update()
  {
    if (!this.view.IsMine || !this.character.data.isRopeClimbing)
      return;
    if (!this.character.data.heldRope.UnityObjectExists<Rope>())
    {
      this.view.RPC("StopRopeClimbingRpc", RpcTarget.All);
    }
    else
    {
      if ((UnityEngine.Object) this.character.data.heldRope != (UnityEngine.Object) null)
      {
        float angleAtPercent = this.character.data.heldRope.climbingAPI.GetAngleAtPercent(this.character.data.ropePercent);
        if (!this.character.data.heldRope.IsActive() || (double) angleAtPercent > (double) this.maxRopeAngle && 180.0 - (double) angleAtPercent > (double) this.maxRopeAngle)
        {
          Debug.Log((object) $"Rope climbing failed. Angle up: {angleAtPercent} Angle down: {(ValueType) (float) (180.0 - (double) angleAtPercent)}");
          this.view.RPC("StopRopeClimbingRpc", RpcTarget.All);
          return;
        }
      }
      float num1 = (double) this.character.input.movementInput.y < 0.0 ? 3f : 1f;
      this.character.data.ropePercent += this.character.data.heldRope.climbingAPI.GetMove() * this.character.input.movementInput.y * num1 * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * this.character.data.heldRope.climbingAPI.UpMult(this.character.data.ropePercent);
      this.character.data.ropePercent = Mathf.Clamp01(this.character.data.ropePercent);
      float num2 = this.staminaUsage;
      if ((double) this.character.input.movementInput.y > 0.0099999997764825821)
        num2 = this.staminaUsageUp;
      if (!this.character.IsLocal || !this.character.input.jumpWasPressed && this.character.UseStamina(num2 * Time.deltaTime) && (double) this.character.data.currentRagdollControll >= 0.30000001192092896)
        return;
      this.view.RPC("StopRopeClimbingRpc", RpcTarget.All);
    }
  }

  [PunRPC]
  private void StopRopeClimbingRpc()
  {
    if ((UnityEngine.Object) this.character.data.heldRope != (UnityEngine.Object) null)
      this.character.data.heldRope.RemoveCharacterClimbing(this.character);
    this.character.data.isRopeClimbing = false;
    this.character.data.isJumping = false;
    this.character.data.sinceGrounded = 0.0f;
    this.character.data.heldRope = (Rope) null;
    Debug.Log((object) "Stop Climbing");
  }

  private void FixedUpdate()
  {
    if (this.character.data.isRopeClimbing)
      this.Climbing();
    else
      this.TryToStartWallClimb();
  }

  private void Climbing()
  {
    this.character.data.ropeClimbWorldNormal = this.character.data.ropeClimbNormal;
    this.character.data.ropeClimbWorldUp = this.character.data.heldRope.climbingAPI.GetUp(this.character.data.ropePercent);
    this.character.AddForce(this.ClimbForce());
  }

  private Vector3 ClimbForce()
  {
    return (this.GetPosition() - this.character.TorsoPos()) * this.climbForce;
  }

  private Vector3 GetPosition()
  {
    return this.character.data.heldRope.climbingAPI.GetPosition(this.character.data.ropePercent) + this.character.data.ropeClimbWorldNormal * 0.5f;
  }

  private void TryToStartWallClimb()
  {
  }

  [PunRPC]
  public void GrabRopeRpc(PhotonView ropeView, int segmentIndex)
  {
    Rope componentInChildren = ropeView.GetComponentInChildren<Rope>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Failed to get rope from network object");
    }
    else
    {
      Debug.Log((object) "Start Rope Climbing!");
      componentInChildren.AddCharacterClimbing(this.character);
      this.character.data.isRopeClimbing = true;
      this.character.data.heldRope = componentInChildren;
      this.character.data.ropePercent = componentInChildren.climbingAPI.GetPercentFromSegmentIndex(segmentIndex);
      this.character.data.ropeClimbNormal = -this.character.data.lookDirection_Flat;
      this.character.data.isClimbing = false;
      this.character.data.isVineClimbing = false;
    }
  }
}
