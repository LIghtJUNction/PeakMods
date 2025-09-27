// Decompiled with JetBrains decompiler
// Type: PlayerHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class PlayerHandler : GameService, IDisposable
{
  private Dictionary<int, Player> m_playerLookup = new Dictionary<int, Player>();
  private Dictionary<int, Character> m_playerCharacterLookup = new Dictionary<int, Character>();
  private Dictionary<byte, Character> m_assignedVoiceGroups = new Dictionary<byte, Character>();
  public static Action<Character> OnCharacterRegistered;

  protected static PlayerHandler Instance => GameHandler.GetService<PlayerHandler>();

  public static List<Character> GetAllPlayerCharacters()
  {
    List<int> intList = new List<int>();
    foreach (KeyValuePair<int, Character> keyValuePair in PlayerHandler.Instance.m_playerCharacterLookup)
    {
      Photon.Realtime.Player player;
      if (!PhotonNetwork.TryGetPlayer(keyValuePair.Key, out player))
        intList.Add(keyValuePair.Key);
      else if (player.IsInactive)
        intList.Add(keyValuePair.Key);
    }
    foreach (int key in intList)
    {
      PlayerHandler.Instance.m_playerCharacterLookup.Remove(key);
      Debug.Log((object) $"Removing {key} character from list..");
    }
    return PlayerHandler.Instance.m_playerCharacterLookup.Values.ToList<Character>();
  }

  public static void RegisterPlayer(Player player)
  {
    PhotonView component = player.GetComponent<PhotonView>();
    if (PlayerHandler.Instance.m_playerLookup.ContainsKey(component.Owner.ActorNumber))
    {
      PlayerHandler.Instance.m_playerLookup.Remove(component.Owner.ActorNumber);
      Debug.Log((object) $"Overwriting player for {component.Owner.ActorNumber}");
    }
    PlayerHandler.Instance.m_playerLookup.Add(component.Owner.ActorNumber, player);
    Debug.Log((object) $"Registering Player object for {component.Owner.NickName} : {component.Owner.ActorNumber}");
  }

  public static void RegisterCharacter(Character character)
  {
    PhotonView component = character.GetComponent<PhotonView>();
    if (PlayerHandler.Instance.m_playerCharacterLookup.ContainsKey(component.Owner.ActorNumber))
    {
      Debug.Log((object) $"Overwriting character for {component.Owner.ActorNumber}");
      Character character1 = PlayerHandler.Instance.m_playerCharacterLookup[component.Owner.ActorNumber];
      if ((UnityEngine.Object) character1 != (UnityEngine.Object) null)
      {
        character1.gameObject.SetActive(false);
        Debug.LogError((object) "Disabled Old Player....");
      }
      PlayerHandler.Instance.m_playerCharacterLookup.Remove(component.Owner.ActorNumber);
    }
    PlayerHandler.Instance.m_playerCharacterLookup.Add(component.Owner.ActorNumber, character);
    Debug.Log((object) $"Registering Character object for {component.Owner.NickName} : {component.Owner.ActorNumber}");
    Action<Character> characterRegistered = PlayerHandler.OnCharacterRegistered;
    if (characterRegistered == null)
      return;
    characterRegistered(character);
  }

  public static Player GetPlayer(Photon.Realtime.Player photonPlayer)
  {
    return CollectionExtensions.GetValueOrDefault<int, Player>((IReadOnlyDictionary<int, Player>) PlayerHandler.Instance.m_playerLookup, photonPlayer.ActorNumber);
  }

  public static Player GetPlayer(int actorNumber)
  {
    return CollectionExtensions.GetValueOrDefault<int, Player>((IReadOnlyDictionary<int, Player>) PlayerHandler.Instance.m_playerLookup, actorNumber);
  }

  public static bool TryGetPlayer(int actorNumber, out Player player)
  {
    player = PlayerHandler.GetPlayer(actorNumber);
    return (UnityEngine.Object) player != (UnityEngine.Object) null;
  }

  public static Character GetPlayerCharacter(Photon.Realtime.Player photonPlayer)
  {
    return CollectionExtensions.GetValueOrDefault<int, Character>((IReadOnlyDictionary<int, Character>) PlayerHandler.Instance.m_playerCharacterLookup, photonPlayer.ActorNumber);
  }

  public static bool HasHadPlayerCharacter(Photon.Realtime.Player photonPlayer)
  {
    return PlayerHandler.Instance.m_playerCharacterLookup.ContainsKey(photonPlayer.ActorNumber);
  }

  public static byte AssignMixerGroup(Character character)
  {
    for (byte key = 0; key < (byte) 4; ++key)
    {
      if (!PlayerHandler.Instance.m_assignedVoiceGroups.ContainsKey(key) || !PlayerHandler.Instance.m_assignedVoiceGroups[key].UnityObjectExists<Character>())
      {
        PlayerHandler.Instance.m_assignedVoiceGroups[key] = character;
        return key;
      }
    }
    return byte.MaxValue;
  }

  public void Dispose() => Debug.Log((object) "Disposing PlayerHandler");

  public static List<Player> GetAllPlayers()
  {
    List<Player> allPlayers = new List<Player>();
    foreach (Photon.Realtime.Player player1 in PhotonNetwork.PlayerList)
    {
      Player player2;
      if (!player1.IsInactive && PlayerHandler.TryGetPlayer(player1.ActorNumber, out player2))
        allPlayers.Add(player2);
    }
    return allPlayers;
  }

  public static bool TryGetCharacter(int actorID, out Character character)
  {
    Player player;
    if (PlayerHandler.TryGetPlayer(actorID, out player))
    {
      character = player.character;
      return true;
    }
    character = (Character) null;
    return false;
  }
}
