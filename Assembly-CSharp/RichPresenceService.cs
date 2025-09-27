// Decompiled with JetBrains decompiler
// Type: RichPresenceService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Steamworks;

#nullable disable
public class RichPresenceService : GameService
{
  private RichPresenceState m_currentState;

  public void SetState(RichPresenceState state)
  {
    this.m_currentState = state;
    int num1 = 1;
    int num2 = 4;
    if (state == RichPresenceState.Status_MainMenu)
      SteamFriends.ClearRichPresence();
    else if (PhotonNetwork.OfflineMode)
    {
      num1 = 1;
      num2 = 1;
    }
    else if (PhotonNetwork.InRoom)
    {
      num1 = PhotonNetwork.PlayerList.Length;
      SteamFriends.SetRichPresence("steam_player_group", PhotonNetwork.CurrentRoom.Name);
      num2 = PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    SteamFriends.SetRichPresence("steam_display", "#" + state.ToString());
    SteamFriends.SetRichPresence("players", num1.ToString());
    SteamFriends.SetRichPresence("steam_player_group_size", num1.ToString());
    SteamFriends.SetRichPresence("max_players", num2.ToString());
  }

  public void Dirty() => this.SetState(this.m_currentState);
}
