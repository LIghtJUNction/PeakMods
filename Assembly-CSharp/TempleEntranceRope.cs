// Decompiled with JetBrains decompiler
// Type: TempleEntranceRope
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class TempleEntranceRope : RopeAnchorWithRope
{
  public float baseScoutWeight;
  public float weightPerSegment;
  public float currentWeight;
  [Header("Weight at which the door will lock open.")]
  public float lockWeight;
  public Rigidbody doorRb;
  public float doorHeightPerWeight;
  public float maxDoorHeight;
  public float doorLerpSpeedUp;
  public float maxDoorMoveSpeedUp;
  public float doorMoveSpeedDown;
  private Vector3 doorStartingPosition;
  private Vector3 currentDoorTarget;
  private bool lockedOpen;

  public override void Awake()
  {
    base.Awake();
    this.doorStartingPosition = this.doorRb.transform.position;
  }

  public void Update()
  {
    if (!PhotonNetwork.InRoom)
      return;
    this.UpdateWeight();
  }

  public void FixedUpdate() => this.UpdateDoorOpen();

  [PunRPC]
  private void SetWeightRPC(float weight)
  {
    Debug.Log((object) $"Received weight RPC. {weight}");
    this.currentWeight = weight;
    if ((double) this.currentWeight <= (double) this.lockWeight)
      return;
    this.lockedOpen = true;
  }

  private void UpdateDescent()
  {
    double num = (double) this.currentWeight / (double) this.weightPerSegment;
  }

  private void UpdateDoorOpen()
  {
    this.currentDoorTarget = this.doorStartingPosition + Vector3.up * Mathf.Min(this.doorHeightPerWeight * this.currentWeight, this.maxDoorHeight);
    Vector3 vector3 = this.currentDoorTarget - this.doorRb.transform.position;
    if ((double) vector3.y > 0.0)
    {
      this.doorRb.MovePosition(this.doorRb.position + Vector3.ClampMagnitude(Vector3.Lerp(this.doorRb.position, this.currentDoorTarget, this.doorLerpSpeedUp * Time.fixedDeltaTime) - this.doorRb.position, this.maxDoorMoveSpeedUp * Time.fixedDeltaTime));
    }
    else
    {
      if ((double) vector3.y >= 0.0)
        return;
      this.doorRb.MovePosition(Vector3.MoveTowards(this.doorRb.position, this.currentDoorTarget, this.doorMoveSpeedDown * Time.fixedDeltaTime));
    }
  }

  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    if (!this.photonView.IsMine)
      return;
    this.photonView.RPC("SetWeightRPC", RpcTarget.All, (object) this.currentWeight);
  }

  private void UpdateWeight()
  {
    if (!this.photonView.IsMine || !(bool) (Object) this.rope)
      return;
    float num = 0.0f;
    foreach (Character character in this.rope.charactersClimbing)
    {
      num += this.baseScoutWeight;
      num += character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Weight);
    }
    if (this.lockedOpen && (double) num <= (double) this.currentWeight || (double) this.currentWeight == (double) num)
      return;
    this.photonView.RPC("SetWeightRPC", RpcTarget.All, (object) num);
  }
}
