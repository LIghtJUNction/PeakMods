// Decompiled with JetBrains decompiler
// Type: NetworkConnector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.UI.Modal;

#nullable disable
public class NetworkConnector : MonoBehaviourPunCallbacks
{
  public static int MAX_PLAYERS = 4;
  private static NetworkConnector _instance;
  private Coroutine keepSettingLobbyDataCoroutine;
  private bool yieldingForCaelan;
  private static string rejoinScene = "Title";

  private void Awake() => NetworkConnector._instance = this;

  private async void Start()
  {
    NetworkConnector networkConnector = this;
    Debug.Log((object) ("Network Connector is starting in scene: " + SceneManager.GetActiveScene().name));
    ConnectionState state = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
    switch (state)
    {
      case InRoomState _:
        foreach (Player allPlayer in PlayerHandler.GetAllPlayers())
          allPlayer.hasClosedEndScreen = false;
        if (PhotonNetwork.IsMasterClient)
          GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
        if (networkConnector.keepSettingLobbyDataCoroutine != null)
        {
          state = (ConnectionState) null;
          return;
        }
        networkConnector.keepSettingLobbyDataCoroutine = networkConnector.StartCoroutine(networkConnector.KeepSettingLobbyData());
        state = (ConnectionState) null;
        return;
      case DefaultConnectionState _:
        string[] mppmTag = CurrentPlayer.ReadOnlyTags();
        if (((IEnumerable<string>) mppmTag).Contains<string>("Client") && !((IEnumerable<string>) mppmTag).Contains<string>("CaelansShitComputer"))
          await Awaitable.WaitForSecondsAsync(10f);
        if (((IEnumerable<string>) mppmTag).Contains<string>("CaelansShitComputer"))
        {
          networkConnector.yieldingForCaelan = true;
          Debug.LogError((object) "Waiting for button in NetworkConnector to be clicked...");
          while (networkConnector.yieldingForCaelan)
            await Task.Delay(100);
          Debug.LogError((object) "Button clicked.");
        }
        mppmTag = (string[]) null;
        break;
    }
    PhotonNetwork.SerializationRate = 30;
    PhotonNetwork.SendRate = 30;
    if (state is DefaultConnectionState)
    {
      PhotonNetwork.NickName = NetworkConnector.GetUsername();
      PhotonNetwork.AuthValues = NetworkConnector.LoadUserID();
      Debug.Log((object) ("Initialized with name: " + PhotonNetwork.NickName));
      PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = new BuildVersion(Application.version).ToMatchmaking();
      if (((IEnumerable<string>) CurrentPlayer.ReadOnlyTags()).Contains<string>("Client"))
      {
        JoinSpecificRoomState specificRoomState = state.stateMachine.SwitchState<JoinSpecificRoomState>();
        specificRoomState.RoomName = Environment.MachineName;
        state = (ConnectionState) specificRoomState;
      }
      else
      {
        HostState hostState = state.stateMachine.SwitchState<HostState>();
        hostState.RoomName = Environment.MachineName;
        state = (ConnectionState) hostState;
      }
      if (!PhotonNetwork.OfflineMode)
      {
        NetworkConnector.ConnectToPhoton();
        state = (ConnectionState) null;
      }
      else
      {
        PhotonNetwork.NickName = NetworkConnector.GetUsername();
        PhotonNetwork.AuthValues = NetworkConnector.LoadUserID();
        RoomOptions roomOptions = NetworkConnector.HostRoomOptions();
        PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions);
        state = (ConnectionState) null;
      }
    }
    else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
    {
      networkConnector.HandleConnectionState(state);
      state = (ConnectionState) null;
    }
    else
    {
      PhotonNetwork.NickName = NetworkConnector.GetUsername();
      PhotonNetwork.AuthValues = NetworkConnector.LoadUserID();
      NetworkConnector.ConnectToPhoton();
      state = (ConnectionState) null;
    }
  }

  public static AuthenticationValues LoadUserID()
  {
    string str;
    if (PlayerPrefs.HasKey("UserID"))
    {
      str = PlayerPrefs.GetString("UserID");
    }
    else
    {
      str = Guid.NewGuid().ToString();
      PlayerPrefs.SetString("UserID", str);
    }
    return new AuthenticationValues()
    {
      AuthType = CustomAuthenticationType.None,
      UserId = str
    };
  }

  public static string GetUsername() => SteamFriends.GetPersonaName();

  private IEnumerator KeepSettingLobbyData()
  {
    int index = 100;
    while (PhotonNetwork.InRoom)
    {
      if (GameHandler.GetService<SteamLobbyHandler>().InSteamLobby() && PhotonNetwork.InRoom)
      {
        string name = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
          GameHandler.GetService<SteamLobbyHandler>().SetLobbyData();
          Debug.Log((object) "IS master, is updating lobby data");
        }
      }
      ++index;
      yield return (object) new WaitForSecondsRealtime(100f);
    }
  }

  private void EndConnectionYield() => this.yieldingForCaelan = false;

  private void HandleConnectionState(ConnectionState state)
  {
    if (state is HostState hostState)
    {
      RoomOptions roomOptions = NetworkConnector.HostRoomOptions();
      PhotonNetwork.CreateRoom(hostState.RoomName, roomOptions);
    }
    JoinSpecificRoomState joinState = state as JoinSpecificRoomState;
    if (joinState == null)
      return;
    Debug.Log((object) $"$Connecting to specific region: {joinState.RegionToJoin} with app ID {PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime}. Is currently connected to: {PhotonNetwork.CloudRegion}");
    if (PhotonNetwork.CloudRegion != joinState.RegionToJoin && !string.IsNullOrEmpty(joinState.RegionToJoin))
    {
      Debug.Log((object) ("Disconnecting and reconnecting to specfic region: " + joinState.RegionToJoin));
      PhotonNetwork.Disconnect();
      NetworkConnector.PrepareSteamAuthTicket((Action) (() => PhotonNetwork.ConnectToRegion(joinState.RegionToJoin)));
    }
    else
    {
      Debug.Log((object) ("Joining specific room: " + joinState.RoomName));
      PhotonNetwork.JoinRoom(joinState.RoomName);
    }
  }

  public override void OnConnectedToMaster()
  {
    ConnectionState currentState = GameHandler.GetService<ConnectionService>().StateMachine.CurrentState;
    Debug.Log((object) ("Connected to Photon Master Server... region: " + PhotonNetwork.CloudRegion));
    this.HandleConnectionState(currentState);
  }

  public override void OnLeftRoom()
  {
    base.OnLeftRoom();
    SceneManager.LoadScene(NetworkConnector.rejoinScene);
    NetworkConnector.rejoinScene = "Title";
  }

  public override void OnCreatedRoom()
  {
    base.OnCreatedRoom();
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>();
    SteamLobbyHandler service = GameHandler.GetService<SteamLobbyHandler>();
    if (!service.InSteamLobby())
      return;
    if (PhotonNetwork.IsMasterClient)
      service.SetLobbyData();
    if (this.keepSettingLobbyDataCoroutine != null)
      return;
    this.keepSettingLobbyDataCoroutine = this.StartCoroutine(this.KeepSettingLobbyData());
  }

  public override void OnCreateRoomFailed(short returnCode, string message)
  {
    base.OnCreateRoomFailed(returnCode, message);
    Debug.LogError((object) $"Failed to create Photon Room, code: {returnCode}, message: {message}");
  }

  public override void OnDisconnected(DisconnectCause cause)
  {
    base.OnDisconnected(cause);
    if (PhotonNetwork.OfflineMode || cause == DisconnectCause.DisconnectByClientLogic)
      return;
    Debug.LogError((object) $"Disconnected from Photon Server: {cause}");
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>();
    Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_DISCONNECTEDPHOTON_TITLE"), LocalizedText.GetText("MODAL_DISCONNECTEDPHOTON_BODY").Replace("#", cause.ToString())), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
    {
      new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) (() => SceneManager.LoadScene("Title")))
    }));
  }

  public override void OnJoinRoomFailed(short returnCode, string message)
  {
    base.OnJoinRoomFailed(returnCode, message);
    Debug.LogError((object) $"Failed to join Photon Room, code: {returnCode}, message: {message}");
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>();
    Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_FAILEDPHOTON_TITLE"), $"[{returnCode}] {message}"), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
    {
      new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) (() => SceneManager.LoadScene("Title")))
    }));
  }

  public override void OnJoinRandomFailed(short returnCode, string message)
  {
    base.OnJoinRandomFailed(returnCode, message);
    Debug.LogError((object) $"Failed to join Random Photon Room, code: {returnCode}, message: {message}");
  }

  public override void OnJoinedRoom()
  {
    if ((UnityEngine.Object) Character.localCharacter != (UnityEngine.Object) null)
      Debug.Log((object) $"On Joined Photon Room. UserId:{Character.localCharacter.photonView.Owner.UserId}, rejoined: {Character.localCharacter.photonView.Owner.HasRejoined}");
    else
      Debug.Log((object) "On Joined Photon Room. No Character");
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<InRoomState>();
  }

  public static void ConnectToPhoton()
  {
    BuildVersion version = new BuildVersion(Application.version);
    PhotonNetwork.AutomaticallySyncScene = true;
    PhotonNetwork.GameVersion = version.ToString();
    PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = version.ToMatchmaking();
    NetworkConnector.PrepareSteamAuthTicket((Action) (() =>
    {
      PhotonNetwork.ConnectUsingSettings();
      Debug.Log((object) $"Photon Start{PhotonNetwork.NetworkClientState.ToString()} using app version: {version.ToMatchmaking()}");
    }));
  }

  private static void PrepareSteamAuthTicket(Action readyToConnect)
  {
    Action action = readyToConnect;
    if (action == null)
      return;
    action();
  }

  public static RoomOptions HostRoomOptions()
  {
    return new RoomOptions()
    {
      IsVisible = false,
      MaxPlayers = NetworkConnector.MAX_PLAYERS,
      PublishUserId = true
    };
  }
}
