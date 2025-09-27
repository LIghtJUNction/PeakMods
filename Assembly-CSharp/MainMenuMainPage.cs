// Decompiled with JetBrains decompiler
// Type: MainMenuMainPage
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
public class MainMenuMainPage : UIPage, INavigationPage
{
  [SerializeField]
  private Button m_playButton;
  [SerializeField]
  private Button m_playSoloButton;
  [SerializeField]
  private Button m_settingsButton;

  private void Start()
  {
    this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
    this.m_settingsButton.onClick.AddListener(new UnityAction(this.SettingsClicked));
    NetworkConnector.ConnectToPhoton();
    PhotonNetwork.AddCallbackTarget((object) this);
    PhotonNetwork.NickName = NetworkConnector.GetUsername();
    PhotonNetwork.AuthValues = NetworkConnector.LoadUserID();
    Debug.Log((object) ("Initialized with name: " + PhotonNetwork.NickName));
    GameHandler.RestartService<PlayerHandler>(new PlayerHandler());
    Debug.Log((object) "Restarting Player Handler Service...");
    GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_MainMenu);
  }

  private void SettingsClicked()
  {
    this.pageHandler.TransistionToPage<MainMenuSettingsPage>((PageTransistion) new SetActivePageTransistion());
  }

  private void OnDestroy() => PhotonNetwork.RemoveCallbackTarget((object) this);

  private void PlayClicked()
  {
    SteamMatchmaking.CreateLobby(GameHandler.Instance.SettingsHandler.GetSetting<LobbyTypeSetting>().Value == LobbyTypeSetting.LobbyType.Friends ? ELobbyType.k_ELobbyTypeFriendsOnly : ELobbyType.k_ELobbyTypePrivate, NetworkConnector.MAX_PLAYERS);
  }

  private void Update() => this.m_playButton.gameObject.SetActive(!PhotonNetwork.OfflineMode);

  public GameObject GetFirstSelectedGameObject() => this.m_playButton.gameObject;
}
