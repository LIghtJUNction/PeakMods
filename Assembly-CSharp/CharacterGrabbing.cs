// Decompiled with JetBrains decompiler
// Type: CharacterGrabbing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
public class CharacterGrabbing : MonoBehaviour
{
  private Character character;

  private void Start()
  {
    this.character = this.GetComponent<Character>();
    this.character.GetBodypart(BodypartType.Hand_R).collisionStayAction += new Action<Collision>(this.GrabAction);
  }

  private void GrabAction(Collision collision)
  {
    if (!this.character.data.isScoutmaster || !this.character.photonView.IsMine || (bool) (UnityEngine.Object) this.character.data.grabJoint || !this.character.data.isReaching || (double) this.character.data.sinceLetGoOfFriend < 0.34999999403953552 || !(bool) (UnityEngine.Object) collision.rigidbody)
      return;
    Character componentInParent = collision.transform.GetComponentInParent<Character>();
    if (!(bool) (UnityEngine.Object) componentInParent || (UnityEngine.Object) componentInParent == (UnityEngine.Object) this.character)
      return;
    BodypartType partType = componentInParent.GetPartType(collision.rigidbody);
    if (partType == ~BodypartType.Hip)
      return;
    this.character.photonView.RPC("RPCA_GrabAttach", RpcTarget.All, (object) componentInParent.photonView, (object) (int) partType, (object) collision.rigidbody.transform.InverseTransformPoint(this.character.GetBodypart(BodypartType.Hand_R).Rig.transform.position));
  }

  [PunRPC]
  public void RPCA_GrabAttach(PhotonView view, int bodyPartID, Vector3 relativePos)
  {
    BodypartType head = (BodypartType) bodyPartID;
    Character component = view.GetComponent<Character>();
    if (!(bool) (UnityEngine.Object) component)
      return;
    Rigidbody rig1 = component.GetBodypart(head).Rig;
    Rigidbody rig2 = this.character.GetBodypart(BodypartType.Hand_R).Rig;
    rig2.transform.position = rig1.transform.TransformPoint(relativePos);
    this.character.data.grabJoint = rig2.gameObject.AddComponent<FixedJoint>();
    this.character.data.grabJoint.connectedBody = rig1;
    this.character.data.grabbedPlayer = component;
    component.data.grabbingPlayer = this.character;
    Debug.Log((object) $"Grab Attaching {component} to {rig1}");
  }

  [PunRPC]
  public void RPCA_GrabUnattach()
  {
    if ((bool) (UnityEngine.Object) this.character.data.grabbedPlayer)
      this.character.data.grabbedPlayer.data.grabbingPlayer = (Character) null;
    this.character.data.grabbedPlayer = (Character) null;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.character.data.grabJoint);
    this.character.data.sinceLetGoOfFriend = 0.0f;
    Debug.Log((object) "Grab unattaching");
  }

  private void Update()
  {
    if (!this.character.refs.view.IsMine)
      return;
    if ((bool) (UnityEngine.Object) this.character.data.grabbingPlayer && this.character.input.jumpWasPressed && !this.character.data.grabbingPlayer.isBot)
      this.character.data.grabbingPlayer.refs.view.RPC("RPCA_GrabUnattach", RpcTarget.All);
    if (!this.CanGrab())
    {
      if (!(bool) (UnityEngine.Object) this.character.data.grabJoint && !this.character.data.isReaching)
        return;
      this.character.refs.view.RPC("RPCA_StopReaching", RpcTarget.All);
    }
    else
    {
      if ((double) this.character.data.sincePressReach < 0.20000000298023224)
      {
        if (!this.character.data.isReaching)
          this.character.refs.view.RPC("RPCA_StartReaching", RpcTarget.All);
      }
      else if (this.character.data.isReaching)
        this.character.refs.view.RPC("RPCA_StopReaching", RpcTarget.All);
      if (!(bool) (UnityEngine.Object) this.character.data.grabJoint)
        return;
      if ((bool) (UnityEngine.Object) this.character.data.grabbedPlayer)
        this.character.data.grabbedPlayer.LimitFalling();
      if (this.character.data.isReaching)
        return;
      this.character.refs.view.RPC("RPCA_GrabUnattach", RpcTarget.All);
    }
  }

  private void FixedUpdate()
  {
    this.character.data.grabFriendDistance = 1000f;
    if (!this.character.data.isReaching)
      return;
    this.Reach();
  }

  [PunRPC]
  private void RPCA_StopReaching()
  {
    this.character.data.isReaching = false;
    if (!(bool) (UnityEngine.Object) this.character.data.grabJoint)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.character.data.grabJoint);
  }

  [PunRPC]
  private void RPCA_StartGrabbing() => this.character.data.isReaching = false;

  [PunRPC]
  private void RPCA_StartReaching() => this.character.data.isReaching = true;

  private void Reach()
  {
    foreach (Character allCharacter in Character.AllCharacters)
    {
      float num = Vector3.Distance(this.character.Center, allCharacter.Center);
      if ((double) num <= 4.0 && (double) Vector3.Angle(this.character.data.lookDirection, allCharacter.Center - this.character.Center) <= 60.0 && this.TargetCanBeHelped(allCharacter))
      {
        if (allCharacter.IsStuck() && allCharacter.IsLocal)
          allCharacter.UnStick();
        if ((double) num < (double) this.character.data.grabFriendDistance)
        {
          this.character.data.grabFriendDistance = num;
          this.character.data.sinceGrabFriend = 0.0f;
        }
        if (this.character.refs.view.IsMine)
          GUIManager.instance.Grasp();
        if (allCharacter.refs.view.IsMine)
        {
          allCharacter.DragTowards(this.character.Center, 70f);
          allCharacter.LimitFalling();
          GUIManager.instance.Grasp();
        }
      }
    }
  }

  private bool TargetCanBeHelped(Character item)
  {
    return item.IsStuck() || (double) item.data.sinceUnstuck < 1.0 || item.data.isClimbing && (double) item.Center.y < (double) this.character.Center.y + 1.0;
  }

  private bool CanGrab()
  {
    return !((UnityEngine.Object) this.character.data.currentItem != (UnityEngine.Object) null) && (double) Time.time - (double) this.character.data.lastConsumedItem >= 0.5 && !this.character.data.isClimbing && !this.character.data.isRopeClimbing && !this.character.data.isVineClimbing;
  }

  internal void Throw(Vector3 force, float fallSeconds)
  {
    this.character.data.grabbedPlayer.RPCA_Fall(1f);
    this.character.data.grabbedPlayer.AddForce(force, 0.7f);
    this.RPCA_GrabUnattach();
  }
}
