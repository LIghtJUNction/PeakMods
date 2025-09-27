// Decompiled with JetBrains decompiler
// Type: CharacterCarrying
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterCarrying : MonoBehaviour
{
  private Character character;

  private void Start() => this.character = this.GetComponent<Character>();

  private void FixedUpdate()
  {
    if (this.character.data.isCarried && (Object) this.character.data.carrier == (Object) null)
      this.CarrierGone();
    if (!(bool) (Object) this.character.data.carrier)
      return;
    this.GetCarried();
  }

  private void Update()
  {
    if (!(bool) (Object) this.character.data.carriedPlayer || !this.character.data.carriedPlayer.data.dead && this.character.data.carriedPlayer.data.fullyPassedOut && !this.character.data.fullyPassedOut && !this.character.data.dead || !this.character.refs.view.IsMine)
      return;
    this.Drop(this.character.data.carriedPlayer);
  }

  private void ToggleCarryPhysics(bool setCarried)
  {
    this.character.refs.ragdoll.ToggleCollision(!setCarried);
    this.character.refs.animations.SetBool("IsCarried", setCarried);
    Debug.Log((object) ("SetIsCarried: " + setCarried.ToString()));
  }

  private void GetCarried()
  {
    this.character.AddForce(Vector3.ClampMagnitude(this.character.data.carrier.refs.carryPosRef.position + this.character.data.carrier.data.avarageVelocity * 0.06f - this.character.Center, 1f) * 500f);
    this.character.refs.movement.ApplyExtraDrag(0.5f, true);
    this.character.data.sinceGrounded = 0.0f;
  }

  internal void StartCarry(Character target)
  {
    this.character.refs.items.EquipSlot(Optionable<byte>.None);
    this.character.photonView.RPC("RPCA_StartCarry", RpcTarget.All, (object) target.photonView);
  }

  [PunRPC]
  public void RPCA_StartCarry(PhotonView targetView)
  {
    Character component = targetView.GetComponent<Character>();
    BackpackSlot backpackSlot = this.character.player.backpackSlot;
    if (!backpackSlot.IsEmpty())
    {
      if (PhotonNetwork.IsMasterClient)
      {
        Debug.Log((object) $"{this.character} is starting to carry {component} but has backpack, dropping backpack");
        PhotonNetwork.InstantiateItemRoom(backpackSlot.GetPrefabName(), component.GetBodypart(BodypartType.Torso).transform.position, Quaternion.identity).GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, (object) backpackSlot.data);
      }
      backpackSlot.EmptyOut();
    }
    else if ((Object) this.character.data.carriedPlayer != (Object) null)
    {
      this.character.refs.carriying.Drop(this.character.data.carriedPlayer);
      return;
    }
    component.refs.carriying.ToggleCarryPhysics(true);
    component.data.isCarried = true;
    this.character.data.carriedPlayer = component;
    component.data.carrier = this.character;
    List<Character> playerCharacters = PlayerHandler.GetAllPlayerCharacters();
    for (int index = 0; index < playerCharacters.Count; ++index)
    {
      Debug.Log((object) $"Updating weight for {playerCharacters[index].gameObject.name}...");
      playerCharacters[index].refs.afflictions.UpdateWeight();
    }
  }

  internal void Drop(Character target)
  {
    this.character.photonView.RPC("RPCA_Drop", RpcTarget.All, (object) target.photonView);
  }

  [PunRPC]
  public void RPCA_Drop(PhotonView targetView)
  {
    Character component = targetView.GetComponent<Character>();
    component.refs.carriying.ToggleCarryPhysics(false);
    component.data.isCarried = false;
    component.data.carrier = (Character) null;
    this.character.data.carriedPlayer = (Character) null;
    List<Character> playerCharacters = PlayerHandler.GetAllPlayerCharacters();
    for (int index = 0; index < playerCharacters.Count; ++index)
    {
      Debug.Log((object) $"Updating weight for {playerCharacters[index].gameObject.name}...");
      playerCharacters[index].refs.afflictions.UpdateWeight();
    }
  }

  private void CarrierGone() => this.character.refs.carriying.ToggleCarryPhysics(false);
}
