// Decompiled with JetBrains decompiler
// Type: ClimbHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
public class ClimbHandle : MonoBehaviour, IInteractible
{
  public Character hanger;
  internal PhotonView view;
  public bool isPickaxe;
  public Action<Character> onHangStart;
  public Action onHangStop;

  private void Start() => this.view = this.GetComponent<PhotonView>();

  public Vector3 Center() => this.transform.position;

  public string GetInteractionText() => LocalizedText.GetText("GRAB");

  public string GetName()
  {
    return this.isPickaxe ? LocalizedText.GetText("PICKAXE") : LocalizedText.GetText("PITONPROMPT");
  }

  public Transform GetTransform() => this.transform;

  public void HoverEnter()
  {
  }

  public void HoverExit()
  {
  }

  public void Interact(Character interactor)
  {
    if ((bool) (UnityEngine.Object) this.hanger)
      return;
    this.view.RPC("RPCA_Hang", RpcTarget.All, (object) interactor.photonView);
  }

  [PunRPC]
  public void RPCA_Hang(PhotonView view)
  {
    if ((UnityEngine.Object) view == (UnityEngine.Object) null)
      return;
    Character component = view.GetComponent<Character>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    this.hanger = component;
    component.refs.climbing.StartHang(this);
    Action<Character> onHangStart = this.onHangStart;
    if (onHangStart == null)
      return;
    onHangStart(component);
  }

  [PunRPC]
  public void RPCA_UnHang(PhotonView view)
  {
    this.hanger = (Character) null;
    if ((UnityEngine.Object) view == (UnityEngine.Object) null)
      return;
    Character component = view.GetComponent<Character>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.data.currentClimbHandle = (ClimbHandle) null;
    Action onHangStop = this.onHangStop;
    if (onHangStop == null)
      return;
    onHangStop();
  }

  public bool IsInteractible(Character interactor) => (UnityEngine.Object) this.hanger == (UnityEngine.Object) null;

  internal void Break()
  {
    if ((UnityEngine.Object) this.hanger != (UnityEngine.Object) null)
      this.hanger.refs.climbing.CancelHandle();
    foreach (Component component in this.transform)
      component.gameObject.SetActive(false);
  }
}
