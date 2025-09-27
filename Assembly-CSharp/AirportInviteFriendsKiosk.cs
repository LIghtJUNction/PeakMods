// Decompiled with JetBrains decompiler
// Type: AirportInviteFriendsKiosk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using UnityEngine;

#nullable disable
public class AirportInviteFriendsKiosk : MonoBehaviour, IInteractible
{
  private MaterialPropertyBlock mpb;
  private MeshRenderer[] _mr;

  public bool IsInteractible(Character interactor) => true;

  private MeshRenderer[] meshRenderers
  {
    get
    {
      if (this._mr == null)
      {
        this._mr = this.GetComponentsInChildren<MeshRenderer>();
        MonoBehaviour.print((object) this._mr.Length);
      }
      return this._mr;
    }
    set => this._mr = value;
  }

  public void Awake() => this.mpb = new MaterialPropertyBlock();

  public void Interact(Character interactor)
  {
    CSteamID lobbyID;
    if (!GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out lobbyID))
      return;
    Debug.Log((object) "Open Invite Friends UI...");
    SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
  }

  public void HoverEnter()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
    {
      if ((Object) this.meshRenderers[index] != (Object) null)
        this.meshRenderers[index].SetPropertyBlock(this.mpb);
    }
  }

  public void HoverExit()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
      this.meshRenderers[index].SetPropertyBlock(this.mpb);
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("INVITEFRIENDS");

  public string GetName() => LocalizedText.GetText("INVITEKIOSK");
}
