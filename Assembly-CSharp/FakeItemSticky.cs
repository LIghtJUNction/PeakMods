// Decompiled with JetBrains decompiler
// Type: FakeItemSticky
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
public class FakeItemSticky : FakeItem
{
  public bool stickOnCollision = true;
  public Collider physicalCollider;
  private bool collided;

  private void Start()
  {
    if (!Application.isPlaying)
      return;
    this.physicalCollider.GetComponent<CollisionModifier>().onCollide += new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide);
  }

  private void OnCollide(
    Character character,
    CollisionModifier modifier,
    Collision collision,
    Bodypart bodyPart)
  {
    if (!character.IsLocal || character.data.isInvincible || this.collided)
      return;
    this.collided = true;
    FakeItemManager.Instance.photonView.RPC("RPC_RequestStickFakeItemToPlayer", RpcTarget.MasterClient, (object) character.photonView.ViewID, (object) this.index, (object) (int) bodyPart.partType, (object) (this.transform.position - bodyPart.transform.position));
  }

  public override void UnPickUpVisibly()
  {
    base.UnPickUpVisibly();
    this.collided = false;
  }
}
