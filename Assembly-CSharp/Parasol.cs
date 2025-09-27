// Decompiled with JetBrains decompiler
// Type: Parasol
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PhotonView))]
public class Parasol : MonoBehaviour
{
  public Item item;
  public float extraYDrag = 0.8f;
  public float extraXZDrag = 0.8f;
  public float sinceGroundedOnClose = 2f;
  public GameObject openParasol;
  public GameObject closedParasol;
  public Animator anim;
  public bool isOpen;

  internal void ToggleOpen()
  {
    if (!this.item.photonView.IsMine)
      return;
    this.item.photonView.RPC("ToggleOpenRPC", RpcTarget.All, (object) !this.isOpen);
  }

  private void FixedUpdate()
  {
    if (!(bool) (Object) this.item.holderCharacter || this.item.holderCharacter.data.isGrounded || !this.isOpen)
      return;
    this.item.holderCharacter.refs.movement.ApplyParasolDrag(this.extraYDrag, this.extraXZDrag);
  }

  private void OnDisable()
  {
    if (!this.isOpen)
      return;
    this.OnClose();
  }

  private void OnClose()
  {
    if (!(bool) (Object) this.item.holderCharacter)
      return;
    this.item.holderCharacter.data.sinceGrounded = this.sinceGroundedOnClose;
  }

  [PunRPC]
  private void ToggleOpenRPC(bool open)
  {
    this.isOpen = open;
    this.anim.SetBool("Open", open);
    if (this.isOpen)
      return;
    this.OnClose();
  }
}
