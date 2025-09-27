// Decompiled with JetBrains decompiler
// Type: PersistentPlayerDataService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.PhotonUtility;

#nullable disable
public class PersistentPlayerDataService : GameService, IDisposable
{
  private Dictionary<int, PersistentPlayerData> PersistentPlayerDatas = new Dictionary<int, PersistentPlayerData>();
  private Dictionary<int, Action<PersistentPlayerData>> OnChangeActions = new Dictionary<int, Action<PersistentPlayerData>>();
  private ListenerHandle syncPersistentPlayerDataHandle;

  public PersistentPlayerDataService()
  {
    this.syncPersistentPlayerDataHandle = CustomCommands<CustomCommandType>.RegisterListener<SyncPersistentPlayerDataPackage>(new Action<SyncPersistentPlayerDataPackage>(this.OnSyncReceived));
  }

  public void Dispose()
  {
    CustomCommands<CustomCommandType>.UnregisterListener(this.syncPersistentPlayerDataHandle);
  }

  private void OnSyncReceived(SyncPersistentPlayerDataPackage package)
  {
    Debug.Log((object) "On Sync Received!");
    this.PersistentPlayerDatas[package.ActorNumber] = package.Data;
    if (!this.OnChangeActions.ContainsKey(package.ActorNumber))
      return;
    this.OnChangeActions[package.ActorNumber](package.Data);
  }

  public PersistentPlayerData GetPlayerData(Photon.Realtime.Player player)
  {
    return this.GetPlayerData(player.ActorNumber);
  }

  public PersistentPlayerData GetPlayerData(int actorNumber)
  {
    if (!this.PersistentPlayerDatas.ContainsKey(actorNumber))
    {
      this.PersistentPlayerDatas[actorNumber] = new PersistentPlayerData();
      Debug.Log((object) $"Initializing player data for player: {actorNumber}");
    }
    return this.PersistentPlayerDatas[actorNumber];
  }

  public void SetPlayerData(Photon.Realtime.Player player, PersistentPlayerData playerData)
  {
    this.PersistentPlayerDatas[player.ActorNumber] = playerData;
    Debug.Log((object) ("Setting Player Data for: " + player.NickName));
    if (this.OnChangeActions.ContainsKey(player.ActorNumber))
    {
      Action<PersistentPlayerData> onChangeAction = this.OnChangeActions[player.ActorNumber];
      if (onChangeAction != null)
        onChangeAction(playerData);
    }
    CustomCommands<CustomCommandType>.SendPackage((CustomCommandPackage<CustomCommandType>) new SyncPersistentPlayerDataPackage()
    {
      Data = playerData,
      ActorNumber = player.ActorNumber
    }, ReceiverGroup.Others);
  }

  public void SubscribeToPlayerDataChange(Photon.Realtime.Player player, Action<PersistentPlayerData> onChange)
  {
    if (!this.OnChangeActions.ContainsKey(player.ActorNumber))
      this.OnChangeActions[player.ActorNumber] = onChange;
    else
      this.OnChangeActions[player.ActorNumber] += onChange;
  }

  public void UnsubscribeToPlayerDataChange(Photon.Realtime.Player player, Action<PersistentPlayerData> onChange)
  {
    if (!this.OnChangeActions.ContainsKey(player.ActorNumber))
      return;
    this.OnChangeActions[player.ActorNumber] -= onChange;
  }

  public void SyncToPlayer(Photon.Realtime.Player newPlayer)
  {
    foreach (KeyValuePair<int, PersistentPlayerData> persistentPlayerData1 in this.PersistentPlayerDatas)
    {
      int num;
      PersistentPlayerData persistentPlayerData2;
      persistentPlayerData1.Deconstruct(ref num, ref persistentPlayerData2);
      int actorNumber = num;
      PersistentPlayerData persistentPlayerData3 = persistentPlayerData2;
      Photon.Realtime.Player player;
      if (PhotonNetwork.TryGetPlayer(actorNumber, out player) && !player.IsInactive)
        CustomCommands<CustomCommandType>.SendPackage((CustomCommandPackage<CustomCommandType>) new SyncPersistentPlayerDataPackage()
        {
          Data = persistentPlayerData3,
          ActorNumber = actorNumber
        }, new RaiseEventOptions()
        {
          TargetActors = new int[1]{ newPlayer.ActorNumber }
        });
    }
  }
}
