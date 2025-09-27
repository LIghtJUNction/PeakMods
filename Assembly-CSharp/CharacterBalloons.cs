// Decompiled with JetBrains decompiler
// Type: CharacterBalloons
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterBalloons : MonoBehaviourPunCallbacks
{
  internal Character character;
  internal List<TiedBalloon> tiedBalloons = new List<TiedBalloon>();
  public GameObject popParticle;
  public Material[] balloonColors;
  public SFX_Instance[] balloonTie;
  public int heldBalloonCount;
  public float balloonSinceGroundedCapAmount = 0.2f;
  public float balloonFloatAmount = 0.1f;
  public float balloonJumpAmount = 0.25f;
  public float headOffset = 0.5f;
  public float extraFloatUpwardMultiplier = 1f;
  private bool lastBalloonCount;

  public int currentBalloonCount => this.heldBalloonCount + this.tiedBalloons.Count;

  private void Awake() => this.character = this.GetComponent<Character>();

  private void FixedUpdate()
  {
    this.character.refs.movement.balloonFloatMultiplier = (float) (1.0 - (double) this.balloonFloatAmount * (double) this.currentBalloonCount);
    this.character.refs.movement.balloonJumpMultiplier = (float) (1.0 + (double) this.balloonJumpAmount * (double) this.currentBalloonCount);
    if ((double) this.character.refs.movement.balloonFloatMultiplier < 0.0)
      this.character.refs.movement.balloonFloatMultiplier -= this.extraFloatUpwardMultiplier;
    for (int index = 0; index < this.tiedBalloons.Count; ++index)
      this.tiedBalloons[index].anchor.position = this.character.Head + Vector3.up * this.headOffset;
    if (this.currentBalloonCount <= 0)
      return;
    float num = Mathf.Clamp((float) (2.0 - (double) this.balloonSinceGroundedCapAmount * (double) this.currentBalloonCount), 0.5f, 2f);
    if ((double) this.character.data.sinceGrounded > (double) num)
      this.character.data.sinceGrounded = num;
    if (this.currentBalloonCount < 6 || this.character.data.isGrounded)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AeronauticsBadge);
  }

  public void TieNewBalloon(int colorIndex)
  {
    PhotonNetwork.Instantiate("TiedBalloon", this.character.Center, Quaternion.identity).GetComponent<TiedBalloon>().Init(this, this.character.Center.y, colorIndex);
    for (int index = 0; index < this.balloonTie.Length; ++index)
      this.balloonTie[index].Play(this.character.Center);
  }

  public void RemoveBalloon(TiedBalloon balloon)
  {
    if (!this.tiedBalloons.Remove(balloon))
      return;
    Object.Instantiate<GameObject>(this.popParticle, balloon.rb.transform.position, Quaternion.identity);
    this.character.data.sinceGrounded = 0.0f;
  }
}
