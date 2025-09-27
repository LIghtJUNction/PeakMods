// Decompiled with JetBrains decompiler
// Type: ReconnectHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

#nullable disable
public class ReconnectHandler : Singleton<ReconnectHandler>, IInRoomCallbacks
{
  private Dictionary<string, Optionable<ReconnectData>> reconnectData = new Dictionary<string, Optionable<ReconnectData>>();
  private Optionable<float> lastReconnectDataRecordedTime = Optionable<float>.None;
  private PhotonView photonView;

  public Dictionary<string, Optionable<ReconnectData>> Data => this.reconnectData;

  protected override void Awake()
  {
    base.Awake();
    this.photonView = this.GetComponent<PhotonView>();
    PhotonNetwork.AddCallbackTarget((object) this);
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    PhotonNetwork.RemoveCallbackTarget((object) this);
  }

  private void Update()
  {
    Character character;
    if (!PhotonNetwork.InRoom || !PlayerHandler.TryGetCharacter(PhotonNetwork.LocalPlayer.ActorNumber, out character))
      return;
    if (this.lastReconnectDataRecordedTime.IsNone)
    {
      this.RegisterLocalReconnectData(character);
    }
    else
    {
      if ((double) Time.realtimeSinceStartup - (double) this.lastReconnectDataRecordedTime.Value <= 10.0)
        return;
      this.RegisterLocalReconnectData(character);
    }
  }

  public void RegisterLocalReconnectData(Character character)
  {
    if ((UnityEngine.Object) Singleton<MapHandler>.Instance == (UnityEngine.Object) null)
      return;
    this.lastReconnectDataRecordedTime = Optionable<float>.Some(Time.realtimeSinceStartup);
    Debug.Log((object) "Registering Local ReconnectData");
    this.photonView.RPC("SetReconnectDataRPC", RpcTarget.All, (object) PhotonNetwork.LocalPlayer.ActorNumber, (object) ReconnectData.CreateFromCharacter(character, Singleton<MapHandler>.Instance.GetCurrentSegment()));
  }

  [PunRPC]
  public void SetReconnectDataRPC(int actorNumber, ReconnectData data)
  {
    Photon.Realtime.Player player1 = ((IEnumerable<Photon.Realtime.Player>) PhotonNetwork.PlayerList).FirstOrDefault<Photon.Realtime.Player>((Func<Photon.Realtime.Player, bool>) (player => player.ActorNumber == actorNumber));
    if (player1 == null)
    {
      Debug.LogError((object) $"Failed to find player with ID: {actorNumber}");
    }
    else
    {
      this.reconnectData[player1.UserId] = Optionable<ReconnectData>.Some(data);
      Debug.Log((object) ("Set reconnect data with key: " + player1.UserId));
    }
  }

  [PunRPC]
  public void SetReconnectDataWithKeyRPC(string key, ReconnectData data)
  {
    this.reconnectData[key] = Optionable<ReconnectData>.Some(data);
    Debug.Log((object) ("Set reconnect data with key: " + key));
  }

  public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    if (!PhotonNetwork.IsMasterClient || newPlayer.IsMasterClient)
      return;
    this.reconnectData.ToArray<KeyValuePair<string, Optionable<ReconnectData>>>();
    foreach (KeyValuePair<string, Optionable<ReconnectData>> keyValuePair in this.reconnectData)
    {
      string str1;
      Optionable<ReconnectData> optionable1;
      keyValuePair.Deconstruct(ref str1, ref optionable1);
      string str2 = str1;
      Optionable<ReconnectData> optionable2 = optionable1;
      if (optionable2.IsSome)
        this.photonView.RPC("SetReconnectDataWithKeyRPC", newPlayer, (object) str2, (object) optionable2.Value);
    }
  }

  public ReconnectData GetReconnectData(Photon.Realtime.Player player)
  {
    if (this.reconnectData.ContainsKey(player.UserId))
    {
      Optionable<ReconnectData> optionable = this.reconnectData[player.UserId];
      if (optionable.IsSome)
      {
        optionable = this.reconnectData[player.UserId];
        if (optionable.Value.isValid)
        {
          optionable = this.reconnectData[player.UserId];
          return optionable.Value;
        }
      }
    }
    return ReconnectData.Invalid;
  }

  public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
  {
  }

  public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
  {
  }

  public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
  {
  }

  public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
  {
  }

  public void SetCharacterState(Character character, ReconnectData data, bool teleport)
  {
    Debug.Log((object) "Warping because reconnect...");
    this.StartCoroutine(Warp());

    IEnumerator Warp()
    {
      if (teleport)
      {
        Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, (object) data.position, (object) true);
        int timeWarping = 0;
        while (Character.localCharacter.warping || timeWarping > 6)
        {
          ++timeWarping;
          yield return (object) null;
        }
      }
      Character.localCharacter.photonView.RPC("ApplyStatusesFromFloatArrayRPC", RpcTarget.All, (object) data.currentStatuses);
      Player.localPlayer.photonView.RPC("SyncInventoryRPC", RpcTarget.All, (object) IBinarySerializable.ToManagedArray<InventorySyncData>(data.inventorySyncData), (object) true);
      Singleton<MountainProgressHandler>.Instance.CheckProgress(false);
      if (data.inventorySyncData.tempSlot.ItemID != ushort.MaxValue)
        Character.localCharacter.refs.items.EquipSlot(Optionable<byte>.Some((byte) 250));
      if (data.fullyPassedOut && !data.dead)
      {
        Character.localCharacter.photonView.RPC("RPCA_PassOut", RpcTarget.All);
        Character.localCharacter.data.deathTimer = data.deathTimer;
      }
      else if (data.dead)
        Character.Die();
    }
  }
}
