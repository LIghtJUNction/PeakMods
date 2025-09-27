// Decompiled with JetBrains decompiler
// Type: Photon.Realtime.Demo.ConnectAndJoinRandomLb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Photon.Realtime.Demo;

public class ConnectAndJoinRandomLb : 
  MonoBehaviour,
  IConnectionCallbacks,
  IMatchmakingCallbacks,
  ILobbyCallbacks
{
  [SerializeField]
  private AppSettings appSettings = new AppSettings();
  private LoadBalancingClient lbc;
  private ConnectionHandler ch;
  public Text StateUiText;

  public void Start()
  {
    this.lbc = new LoadBalancingClient();
    this.lbc.AddCallbackTarget((object) this);
    if (!this.lbc.ConnectUsingSettings(this.appSettings))
      Debug.LogError((object) "Error while connecting");
    this.ch = this.gameObject.GetComponent<ConnectionHandler>();
    if (!((UnityEngine.Object) this.ch != (UnityEngine.Object) null))
      return;
    this.ch.Client = this.lbc;
    this.ch.StartFallbackSendAckThread();
  }

  public void Update()
  {
    LoadBalancingClient lbc = this.lbc;
    if (lbc == null)
      return;
    lbc.Service();
    Text stateUiText = this.StateUiText;
    string str = lbc.State.ToString();
    if (!((UnityEngine.Object) stateUiText != (UnityEngine.Object) null) || stateUiText.text.Equals(str))
      return;
    stateUiText.text = "State: " + str;
  }

  public void OnConnected()
  {
  }

  public void OnConnectedToMaster()
  {
    Debug.Log((object) nameof (OnConnectedToMaster));
    this.lbc.OpJoinRandomRoom();
  }

  public void OnDisconnected(DisconnectCause cause)
  {
    Debug.Log((object) $"OnDisconnected({cause.ToString()})");
  }

  public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
  {
  }

  public void OnCustomAuthenticationFailed(string debugMessage)
  {
  }

  public void OnRegionListReceived(RegionHandler regionHandler)
  {
    Debug.Log((object) nameof (OnRegionListReceived));
    regionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), (string) null);
  }

  public void OnRoomListUpdate(List<RoomInfo> roomList)
  {
  }

  public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
  {
  }

  public void OnJoinedLobby()
  {
  }

  public void OnLeftLobby()
  {
  }

  public void OnFriendListUpdate(List<FriendInfo> friendList)
  {
  }

  public void OnCreatedRoom()
  {
  }

  public void OnCreateRoomFailed(short returnCode, string message)
  {
  }

  public void OnJoinedRoom() => Debug.Log((object) nameof (OnJoinedRoom));

  public void OnJoinRoomFailed(short returnCode, string message)
  {
  }

  public void OnJoinRandomFailed(short returnCode, string message)
  {
    Debug.Log((object) nameof (OnJoinRandomFailed));
    this.lbc.OpCreateRoom(new EnterRoomParams());
  }

  public void OnLeftRoom()
  {
  }

  private void OnRegionPingCompleted(RegionHandler regionHandler)
  {
    Debug.Log((object) ("OnRegionPingCompleted " + regionHandler.BestRegion?.ToString()));
    Debug.Log((object) ("RegionPingSummary: " + regionHandler.SummaryToCache));
    this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
  }
}
