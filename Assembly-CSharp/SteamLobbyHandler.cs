// Decompiled with JetBrains decompiler
// Type: SteamLobbyHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Steamworks;
using System;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.Serizalization;
using Zorro.UI.Modal;

#nullable disable
public class SteamLobbyHandler : GameService
{
  private const string PHOTON_REGION_KEY = "PhotonRegion";
  private const string GAME_VERSION_KEY = "PeakVersion";
  private const string CURRENT_SCENE_KEY = "CurrentScene";
  private bool m_isHosting;
  private CSteamID m_currentLobby;
  private Optionable<CSteamID> m_currentlyFetchingGameVersion;
  private Optionable<CSteamID> m_currentlyRequestingRoomID;
  private Optionable<(CSteamID, string, string)> m_currentlyWaitingForRoomID;
  private Optionable<int> tryingToFetchLobbyDataAttempts = Optionable<int>.None;

  public SteamLobbyHandler()
  {
    Debug.Log((object) "Steam Lobby Handler initialized");
    Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
    Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnLobbyJoinRequested));
    Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(this.OnLobbyEnter));
    Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyDataUpdate));
    Callback<LobbyChatMsg_t>.Create(new Callback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChat));
    this.m_currentLobby = CSteamID.Nil;
  }

  private void OnLobbyEnter(LobbyEnter_t param)
  {
    if (this.m_isHosting)
      this.m_isHosting = false;
    else if (param.m_EChatRoomEnterResponse == 1U)
    {
      this.m_currentLobby = new CSteamID(param.m_ulSteamIDLobby);
      Debug.Log((object) ("Entered Steam Lobby: " + this.m_currentLobby.ToString()));
      string lobbyData1 = SteamMatchmaking.GetLobbyData(this.m_currentLobby, "PhotonRegion");
      string lobbyData2 = SteamMatchmaking.GetLobbyData(this.m_currentLobby, "CurrentScene");
      if (string.IsNullOrEmpty(lobbyData1))
      {
        this.tryingToFetchLobbyDataAttempts = !this.tryingToFetchLobbyDataAttempts.IsNone ? Optionable<int>.Some(this.tryingToFetchLobbyDataAttempts.Value + 1) : Optionable<int>.Some(1);
        Debug.LogError((object) $"Failed to get lobby region, attempts: {this.tryingToFetchLobbyDataAttempts.Value}");
        if (this.tryingToFetchLobbyDataAttempts.Value < 5)
        {
          this.LeaveLobby();
          this.TryJoinLobby(new CSteamID(param.m_ulSteamIDLobby));
        }
        else
        {
          Debug.LogError((object) "Failed to fetch steam lobby");
          this.LeaveLobby();
          Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTJOINLOBBY_TITLE"), LocalizedText.GetText("MODAL_INVALIDLOBBY_BODY")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
          {
            new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
          }));
        }
      }
      else
      {
        this.tryingToFetchLobbyDataAttempts = Optionable<int>.None;
        this.m_currentlyWaitingForRoomID = Optionable<(CSteamID, string, string)>.Some((this.m_currentLobby, lobbyData2, lobbyData1));
      }
    }
    else
      this.m_currentLobby = CSteamID.Nil;
  }

  public override void Update()
  {
    base.Update();
    if (!this.m_currentlyWaitingForRoomID.IsSome || !this.m_currentlyRequestingRoomID.IsNone)
      return;
    this.RequestPhotonRoomID();
  }

  private void RequestPhotonRoomID()
  {
    if (this.m_currentLobby == CSteamID.Nil)
    {
      Debug.LogError((object) "Not in a lobby");
    }
    else
    {
      this.m_currentlyRequestingRoomID = Optionable<CSteamID>.Some(this.m_currentLobby);
      BinarySerializer binarySerializer = new BinarySerializer(256 /*0x0100*/, Allocator.Temp);
      binarySerializer.WriteByte((byte) 1);
      byte[] byteArray = binarySerializer.buffer.ToByteArray();
      binarySerializer.Dispose();
      if (!SteamMatchmaking.SendLobbyChatMsg(this.m_currentLobby, byteArray, byteArray.Length))
      {
        this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
        Debug.LogError((object) "Failed to request Room ID");
      }
      else
        Debug.Log((object) "Requested Room ID");
    }
  }

  private void SendRoomID()
  {
    if (this.m_currentLobby == CSteamID.Nil)
    {
      Debug.LogError((object) "Not in a lobby");
    }
    else
    {
      BinarySerializer binarySerializer = new BinarySerializer(256 /*0x0100*/, Allocator.Temp);
      binarySerializer.WriteByte((byte) 2);
      binarySerializer.WriteString(PhotonNetwork.CurrentRoom.Name, Encoding.ASCII);
      byte[] byteArray = binarySerializer.buffer.ToByteArray();
      binarySerializer.Dispose();
      if (!SteamMatchmaking.SendLobbyChatMsg(this.m_currentLobby, byteArray, byteArray.Length))
      {
        this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
        Debug.LogError((object) "Failed to send Room ID...");
      }
      else
        Debug.Log((object) ("Lobby has been requested. Sending " + PhotonNetwork.CurrentRoom.Name));
    }
  }

  private void OnLobbyChat(LobbyChatMsg_t param)
  {
    if ((long) param.m_ulSteamIDLobby != (long) this.m_currentLobby.m_SteamID)
      Debug.LogError((object) $"Received Chat Message from another lobby: {param.m_ulSteamIDLobby}");
    else if ((long) param.m_ulSteamIDUser == (long) SteamUser.GetSteamID().m_SteamID)
    {
      Debug.Log((object) "Ignoring local chat message");
    }
    else
    {
      byte[] numArray = new byte[1024 /*0x0400*/];
      if (SteamMatchmaking.GetLobbyChatEntry(this.m_currentLobby, (int) param.m_iChatID, out CSteamID _, numArray, numArray.Length, out EChatEntryType _) <= 0)
      {
        Debug.LogError((object) "Failed to get chat message, no bytes written");
      }
      else
      {
        BinaryDeserializer deserializer = new BinaryDeserializer(numArray.ToNativeArray<byte>(Allocator.Temp));
        SteamLobbyHandler.MessageType messageType = (SteamLobbyHandler.MessageType) deserializer.ReadByte();
        if (messageType == SteamLobbyHandler.MessageType.INVALID)
          Debug.LogError((object) "Invalid message type");
        else
          this.HandleMessage(messageType, deserializer, new CSteamID(param.m_ulSteamIDLobby));
        deserializer.Dispose();
      }
    }
  }

  private void HandleMessage(
    SteamLobbyHandler.MessageType messageType,
    BinaryDeserializer deserializer,
    CSteamID lobbyID)
  {
    switch (messageType)
    {
      case SteamLobbyHandler.MessageType.RequestRoomID:
        if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.InRoom)
          break;
        this.SendRoomID();
        break;
      case SteamLobbyHandler.MessageType.RoomID:
        if (this.m_currentlyRequestingRoomID.IsNone)
        {
          Debug.LogError((object) "Not requesting room id, ignoring...");
          break;
        }
        if (this.m_currentlyRequestingRoomID.IsSome && this.m_currentlyRequestingRoomID.Value != lobbyID)
        {
          Debug.LogError((object) "Got room id for wrong lobby");
          break;
        }
        string str1 = deserializer.ReadString(Encoding.ASCII);
        (CSteamID _, string sceneName, string str2) = this.m_currentlyWaitingForRoomID.Value;
        this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
        this.m_currentlyWaitingForRoomID = Optionable<(CSteamID, string, string)>.None;
        if (string.IsNullOrEmpty(sceneName))
        {
          sceneName = "Airport";
          Debug.LogError((object) "Failed to get scene to load, defaulting to airport");
        }
        JoinSpecificRoomState specificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>();
        specificRoomState.RoomName = str1;
        specificRoomState.RegionToJoin = str2;
        RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(sceneName, false, true));
        break;
    }
  }

  private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param)
  {
    Debug.Log((object) $"On Lobby Join Requested: {param.m_steamIDLobby} by {param.m_steamIDFriend}");
    if (SteamMatchmaking.RequestLobbyData(param.m_steamIDLobby))
      this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(param.m_steamIDLobby);
    else
      Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTFINDLOBBY_TITLE"), LocalizedText.GetText("MODAL_FAILEDTOFETCH_BODY")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
      {
        new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
      }));
  }

  private void OnLobbyDataUpdate(LobbyDataUpdate_t param)
  {
    if (param.m_bSuccess == (byte) 1)
    {
      if (this.m_currentlyFetchingGameVersion.IsSome && (long) this.m_currentlyFetchingGameVersion.Value.m_SteamID == (long) param.m_ulSteamIDLobby)
      {
        string lobbyData = SteamMatchmaking.GetLobbyData(this.m_currentlyFetchingGameVersion.Value, "PeakVersion");
        string str1 = lobbyData;
        BuildVersion buildVersion = new BuildVersion(Application.version);
        string matchmaking1 = buildVersion.ToMatchmaking();
        if (str1 == matchmaking1)
        {
          if (PhotonNetwork.InRoom)
          {
            Debug.LogError((object) "Not joining invite because your already in a room...");
            return;
          }
          this.JoinLobby(this.m_currentlyFetchingGameVersion.Value);
        }
        else
        {
          Debug.LogError((object) ("Game version mismatch: " + lobbyData));
          string str2 = LocalizedText.GetText("MODAL_MISMATCH_BODY").Replace("#", lobbyData);
          buildVersion = new BuildVersion(Application.version);
          string matchmaking2 = buildVersion.ToMatchmaking();
          string subheader = str2.Replace("&", matchmaking2);
          Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_MISMATCH_TITLE"), subheader), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
          {
            new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
          }));
        }
      }
    }
    else
    {
      Debug.LogError((object) "Failed to fetch lobby data");
      Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTFINDLOBBY_TITLE"), LocalizedText.GetText("MODAL_CANTFINDLOBBY_BODY")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
      {
        new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
      }));
    }
    if (!this.m_currentlyFetchingGameVersion.IsSome)
      return;
    this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.None;
  }

  private void JoinLobby(CSteamID lobbyID)
  {
    this.LeaveLobby();
    Debug.Log((object) $"Joining lobby: {lobbyID}");
    SteamMatchmaking.JoinLobby(lobbyID);
  }

  public void TryJoinLobby(CSteamID lobbyID)
  {
    if (SteamMatchmaking.RequestLobbyData(lobbyID))
      this.m_currentlyFetchingGameVersion = Optionable<CSteamID>.Some(lobbyID);
    else
      Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTJOINLOBBY_TITLE"), LocalizedText.GetText("MODAL_FAILEDTOFETCH_BODY")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
      {
        new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
      }));
  }

  private void OnLobbyCreated(LobbyCreated_t param)
  {
    this.m_isHosting = true;
    if (param.m_eResult != EResult.k_EResultOK)
    {
      Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_CANTCREATELOBBY_TITLE"), $"{param.m_eResult}"), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
      {
        new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
      }));
    }
    else
    {
      Debug.Log((object) $"Lobby Created: {param.m_ulSteamIDLobby}");
      this.m_currentLobby = new CSteamID(param.m_ulSteamIDLobby);
      if (!SteamMatchmaking.SetLobbyData(this.m_currentLobby, "PeakVersion", new BuildVersion(Application.version).ToMatchmaking()))
        Debug.LogError((object) "Failed to assign game version to lobby");
      GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>().RoomName = Guid.NewGuid().ToString();
      RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true));
    }
  }

  public void SetLobbyData()
  {
    if (this.m_currentLobby == CSteamID.Nil)
      Debug.LogError((object) "Failed to set lobby data, no lobby joined...");
    else if (!PhotonNetwork.InRoom)
    {
      Debug.LogError((object) "Failed to set Lobby data. not in a photon room");
    }
    else
    {
      if (SteamMatchmaking.SetLobbyData(this.m_currentLobby, "PhotonRegion", PhotonNetwork.CloudRegion))
        Debug.Log((object) ("Set Photon Region to steam lobby data: " + PhotonNetwork.CloudRegion));
      else
        Debug.LogError((object) "Failed to set lobby data, returned not okay...");
      string name = SceneManager.GetActiveScene().name;
      if (SteamMatchmaking.SetLobbyData(this.m_currentLobby, "CurrentScene", name))
        Debug.Log((object) ("Set current scene to: " + name));
      else
        Debug.LogError((object) "Failed to set lobby data, returned not okay...");
    }
  }

  public void LeaveLobby()
  {
    this.m_currentlyWaitingForRoomID = Optionable<(CSteamID, string, string)>.None;
    this.m_currentlyRequestingRoomID = Optionable<CSteamID>.None;
    if (this.m_currentLobby != CSteamID.Nil)
    {
      Debug.Log((object) ("Leaving current lobby: " + this.m_currentLobby.ToString()));
      SteamMatchmaking.LeaveLobby(this.m_currentLobby);
      this.m_currentLobby = CSteamID.Nil;
    }
    else
      Debug.Log((object) "Can't leave current lobby because not in a lobby");
  }

  public bool InSteamLobby() => this.m_currentLobby != CSteamID.Nil;

  public bool InSteamLobby(out CSteamID lobbyID)
  {
    lobbyID = this.m_currentLobby;
    return this.m_currentLobby != CSteamID.Nil;
  }

  public enum MessageType : byte
  {
    INVALID,
    RequestRoomID,
    RoomID,
  }
}
