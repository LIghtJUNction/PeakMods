// Decompiled with JetBrains decompiler
// Type: BingBongForceAbilities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[DefaultExecutionOrder(1000002)]
public class BingBongForceAbilities : MonoBehaviour
{
  private PhotonView view;
  private Transform bingbong;
  public BingBongPhysics.PhysicsType physicsType;
  public float force;
  public float drag;
  public float fallAmount;
  public float removeAfterSeconds = 2f;
  public float effectTime = 2f;

  private void Start()
  {
    this.view = this.GetComponent<PhotonView>();
    if (this.physicsType != BingBongPhysics.PhysicsType.ForcePush_Gentle && this.physicsType != BingBongPhysics.PhysicsType.ForcePush)
      return;
    this.DoEffect();
  }

  [PunRPC]
  public void RPCA_BingBongInitObj(int bingbongID)
  {
    this.bingbong = PhotonView.Find(bingbongID).transform;
  }

  private void LateUpdate()
  {
    this.transform.position = this.bingbong.position;
    this.transform.rotation = this.bingbong.rotation;
  }

  private void Update()
  {
    this.removeAfterSeconds -= Time.deltaTime;
    this.effectTime -= Time.deltaTime;
    if (!this.view.IsMine || (double) this.removeAfterSeconds > 0.0)
      return;
    PhotonNetwork.Destroy(this.gameObject);
  }

  private void FixedUpdate()
  {
    if ((double) this.effectTime <= 0.0 || this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle || this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
      return;
    this.DoEffect();
  }

  private void DoEffect()
  {
    foreach (Character target in this.GetTargets())
    {
      target.refs.movement.ApplyExtraDrag(this.drag, true);
      target.AddForce(this.GetForceDirection(target.Center) * this.force);
      target.data.sinceGrounded = Mathf.Clamp(target.data.sinceGrounded, 0.0f, 0.25f);
      if ((double) this.fallAmount > 0.0099999997764825821 && target.IsLocal)
        target.Fall(this.fallAmount);
    }
  }

  private Vector3 GetForceDirection(Vector3 playerPos)
  {
    if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
      return this.bingbong.forward;
    if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
      return -this.bingbong.forward;
    if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush || this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
      return this.bingbong.forward;
    return this.physicsType == BingBongPhysics.PhysicsType.ForceGrab ? this.TargetPos() - playerPos : Vector3.zero;
  }

  private List<Character> GetTargets()
  {
    Vector3 a = this.TargetPos();
    float num = 5f;
    List<Character> targets = new List<Character>();
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if ((double) Vector3.Distance(a, allCharacter.Center) < (double) num)
        targets.Add(allCharacter);
    }
    return targets;
  }

  private Vector3 TargetPos() => this.transform.TransformPoint(Vector3.forward * 5f);
}
