// Decompiled with JetBrains decompiler
// Type: PauseMenuMainPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

#nullable disable
public class PauseMenuMainPage : UIPage, INavigationPage
{
  [SerializeField]
  private Button m_quitButton;
  [SerializeField]
  private Button m_settingsButton;
  [SerializeField]
  private Button m_resumeButton;
  [SerializeField]
  private Button m_accoladesButton;
  [SerializeField]
  private Button m_controllsButton;
  [SerializeField]
  private Button m_inviteButton;

  public Button resumeButton => this.m_resumeButton;

  private void Start()
  {
    this.m_quitButton.onClick.AddListener(new UnityAction(this.OnQuitClicked));
    this.m_settingsButton.onClick.AddListener(new UnityAction(this.OnSettingsClicked));
    this.m_resumeButton.onClick.AddListener(new UnityAction(this.OnResumeClicked));
    this.m_accoladesButton.onClick.AddListener(new UnityAction(this.OnAccoladesClicked));
    this.m_controllsButton.onClick.AddListener(new UnityAction(this.OnControlsClicked));
    this.m_inviteButton.onClick.AddListener(new UnityAction(this.InviteFriendsClicked));
  }

  private void OnEnable() => this.m_inviteButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);

  private void InviteFriendsClicked()
  {
    CSteamID lobbyID;
    if (!GameHandler.GetService<SteamLobbyHandler>().InSteamLobby(out lobbyID))
      return;
    SteamFriends.ActivateGameOverlayInviteDialog(lobbyID);
  }

  private void OnControlsClicked() => this.pageHandler.TransistionToPage<PauseMenuControlsPage>();

  private void OnAccoladesClicked() => this.pageHandler.TransistionToPage<PauseMenuAccoladesPage>();

  private void OnResumeClicked() => this.pageHandler.gameObject.SetActive(false);

  private void OnSettingsClicked()
  {
    this.pageHandler.TransistionToPage<PauseMenuSettingsMenuPage>();
  }

  private void OnQuitClicked()
  {
    this.pageHandler.gameObject.SetActive(false);
    GameHandler.GetService<SteamLobbyHandler>().LeaveLobby();
    PhotonNetwork.Disconnect();
    Debug.Log((object) "Leaving Photon room and returning to main menu");
  }

  public GameObject GetFirstSelectedGameObject() => this.m_resumeButton.gameObject;
}
