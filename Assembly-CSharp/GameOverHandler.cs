// Decompiled with JetBrains decompiler
// Type: GameOverHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class GameOverHandler : Singleton<GameOverHandler>
{
  private PhotonView view;

  protected override void Awake()
  {
    base.Awake();
    this.view = this.GetComponent<PhotonView>();
  }

  public void LocalPlayerHasClosedEndScreen()
  {
    this.view.RPC("PlayerHasClosedEndScreen", RpcTarget.All, (object) PhotonNetwork.LocalPlayer.ActorNumber);
  }

  [PunRPC]
  public void PlayerHasClosedEndScreen(int actorNumber)
  {
    Player player;
    if (!PlayerHandler.TryGetPlayer(actorNumber, out player))
    {
      Debug.LogError((object) $"Player not found: {actorNumber}");
    }
    else
    {
      player.hasClosedEndScreen = true;
      Debug.Log((object) $"{player} Player has closed end screen");
    }
  }

  public void LoadAirport() => this.view.RPC("LoadAirportMaster", RpcTarget.MasterClient);

  [PunRPC]
  public void LoadAirportMaster() => this.view.RPC("BeginIslandLoadRPC", RpcTarget.All);

  [PunRPC]
  public void BeginIslandLoadRPC()
  {
    Debug.Log((object) "Load Island RPC..");
    if (GameHandler.TryGetStatus<SceneSwitchingStatus>(out SceneSwitchingStatus _))
    {
      Debug.Log((object) "Already loading... ");
    }
    else
    {
      GameHandler.AddStatus<SceneSwitchingStatus>((GameStatus) new SceneSwitchingStatus());
      RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 0.0f));
    }
  }

  public void ForceEveryPlayerDoneWithEndScreen()
  {
    this.view.RPC("ForceEveryPlayerDoneWithEndScreenRPC", RpcTarget.All);
  }

  [PunRPC]
  public void ForceEveryPlayerDoneWithEndScreenRPC()
  {
    Debug.Log((object) "Force every player closed end screen");
    foreach (Player allPlayer in PlayerHandler.GetAllPlayers())
      allPlayer.hasClosedEndScreen = true;
  }
}
