// Decompiled with JetBrains decompiler
// Type: ReadyUpToTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ReadyUpToTrigger : MonoBehaviourPunCallbacks
{
  public Dictionary<Photon.Realtime.Player, bool> readyUpStatusDict = new Dictionary<Photon.Realtime.Player, bool>();

  public override void OnJoinedRoom()
  {
    this.readyUpStatusDict.Clear();
    this.PopulatePlayerDict();
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    this.PopulatePlayerDict();
  }

  public override void OnPlayerLeftRoom(Photon.Realtime.Player leavingPlayer)
  {
    this.readyUpStatusDict.Remove(leavingPlayer);
    Debug.Log((object) ("Removing player from ready-up list: " + leavingPlayer.NickName));
  }

  private void PopulatePlayerDict()
  {
    foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
    {
      if (!this.readyUpStatusDict.ContainsKey(player))
      {
        this.readyUpStatusDict.Add(player, false);
        Debug.Log((object) ("Adding player to ready-up list: " + player.NickName));
      }
    }
  }
}
