// Decompiled with JetBrains decompiler
// Type: CharacterVineClimbing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class CharacterVineClimbing : MonoBehaviour
{
  private Character character;
  public float climbForce;
  public float climbSpeed;
  public float climbSpeedMod = 1f;
  public float climbDrag = 0.85f;
  public float staminaUsage;
  private PhotonView view;
  private float attachVel;
  private float syncC;

  private void Awake() => this.view = this.GetComponent<PhotonView>();

  private void Start() => this.character = this.GetComponent<Character>();

  private void Update()
  {
    if (!this.character.data.isVineClimbing || (double) Time.timeScale == 0.0)
      return;
    float num1 = this.character.data.heldVine.LengthFactor();
    float num2 = 0.005f;
    if (this.Sliding())
    {
      this.character.data.vinePercent += num1 * 2f * Time.deltaTime * this.attachVel;
    }
    else
    {
      float sign = this.character.data.heldVine.GetSign(this.character.data.lookDirection_Flat, this.character.data.vinePercent);
      this.character.data.vinePercent = Mathf.Clamp(this.character.data.vinePercent + num1 * this.climbSpeed * this.climbSpeedMod * Time.deltaTime * sign * this.character.input.movementInput.y, 0.01f, 0.99f);
      if ((double) Mathf.Abs(this.character.input.movementInput.y) > 0.0099999997764825821)
        num2 = this.staminaUsage;
    }
    this.character.data.vinePercent = Mathf.Clamp01(this.character.data.vinePercent);
    if (this.character.IsLocal && (this.character.input.jumpWasPressed || !this.character.UseStamina(num2 * Time.deltaTime) || (double) this.character.data.currentRagdollControll < 0.5))
      this.view.RPC("StopVineClimbingRpc", RpcTarget.All);
    this.syncC += Time.deltaTime;
    if ((double) this.syncC <= 0.25 || !this.character.IsLocal)
      return;
    this.syncC = 0.0f;
    this.view.RPC("RPCA_SyncVineClimb", RpcTarget.Others, (object) this.character.data.vinePercent, (object) this.attachVel);
  }

  [PunRPC]
  private void RPCA_SyncVineClimb(float p, float vel)
  {
    this.character.data.vinePercent = p;
    this.attachVel = vel;
  }

  private float SlideAngleMult()
  {
    return Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0.5f, -0.5f, this.character.data.heldVine.GetDir(this.character.data.lookDirection_Flat * this.character.input.movementInput.y, this.character.data.vinePercent).y));
  }

  private void FixedUpdate()
  {
    if (this.Sliding())
    {
      if ((double) this.character.data.vinePercent > 0.99000000953674316 || (double) this.character.data.vinePercent < 0.0099999997764825821)
        this.attachVel *= 0.0f;
      this.attachVel *= 0.99f;
    }
    else
      this.attachVel *= 0.95f;
    if (!this.character.data.isVineClimbing)
      return;
    this.Climbing();
  }

  public bool Sliding() => (double) Mathf.Abs(this.attachVel) > 3.0;

  [PunRPC]
  private void StopVineClimbingRpc()
  {
    this.character.data.isVineClimbing = false;
    this.character.data.isJumping = false;
    this.character.data.sinceGrounded = 0.0f;
  }

  private void Climbing() => this.character.AddForce(this.ClimbForce());

  private Vector3 ClimbForce()
  {
    return (this.GetPosition() - this.character.TorsoPos()) * this.climbForce;
  }

  private Vector3 GetPosition()
  {
    return this.character.data.heldVine.GetPosition(this.character.data.vinePercent) + Vector3.down * 1f;
  }

  [PunRPC]
  public void GrabVineRpc(PhotonView ropeView, int segmentIndex)
  {
    JungleVine component = ropeView.GetComponent<JungleVine>();
    if ((Object) component == (Object) null)
    {
      Debug.LogError((object) "Failed to get rope from network object");
    }
    else
    {
      Debug.Log((object) "Start Rope Climbing!");
      this.character.data.isRopeClimbing = false;
      this.character.data.isClimbing = false;
      this.character.data.isVineClimbing = true;
      this.character.data.heldVine = component;
      this.character.data.vinePercent = component.GetPercentFromSegmentIndex(segmentIndex);
      this.attachVel = component.GetVineVel(this.character.data.avarageVelocity, this.character.data.vinePercent);
    }
  }
}
