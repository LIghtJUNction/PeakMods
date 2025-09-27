// Decompiled with JetBrains decompiler
// Type: CharacterSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

#nullable disable
public class CharacterSpawner : MonoBehaviourPunCallbacks
{
  public Item[] itemsToSpawnWith;
  private bool hasSpawnedPlayer;
  private Dictionary<int, float> attemptedSpawns = new Dictionary<int, float>();

  private void Update()
  {
    if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
      return;
    foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
    {
      if (!PlayerHandler.TryGetCharacter(player.ActorNumber, out Character _) && (!this.attemptedSpawns.ContainsKey(player.ActorNumber) ? 0 : ((double) Time.realtimeSinceStartup - (double) this.attemptedSpawns[player.ActorNumber] < 1.5 ? 1 : 0)) == 0)
      {
        Debug.Log((object) $"Attempting to spawn character for: {player}");
        this.attemptedSpawns[player.ActorNumber] = Time.realtimeSinceStartup;
        if (player.IsLocal)
        {
          this.StartCoroutine(this.SpawnLocalPlayer(ReconnectData.Invalid, false));
        }
        else
        {
          ReconnectData reconnectData = Singleton<ReconnectHandler>.Instance.GetReconnectData(player);
          bool flag = reconnectData.isValid && reconnectData.mapSegment != Singleton<MapHandler>.Instance.GetCurrentSegment();
          if ((UnityEngine.Object) Singleton<OrbFogHandler>.Instance != (UnityEngine.Object) null && !reconnectData.isValid && Singleton<OrbFogHandler>.Instance.isMoving)
            flag = true;
          this.photonView.RPC("SpawnPlayerRPC", player, (object) reconnectData, (object) flag);
        }
      }
    }
  }

  [PunRPC]
  public void SpawnPlayerRPC(ReconnectData reconnectData, bool dead)
  {
    this.StartCoroutine(this.SpawnLocalPlayer(reconnectData, dead));
  }

  private IEnumerator SpawnLocalPlayer(ReconnectData reconnectData, bool dead)
  {
    yield return (object) new WaitForEndOfFrame();
    if (this.hasSpawnedPlayer)
    {
      Debug.LogError((object) "Already spawned local character and player");
    }
    else
    {
      Vector3 position = Vector3.zero;
      Quaternion rotation = Quaternion.identity;
      int index = PhotonNetwork.LocalPlayer.ActorNumber % SpawnPoint.allSpawnPoints.Count;
      SpawnPoint spawnPoint = SpawnPoint.allSpawnPoints.FirstOrDefault<SpawnPoint>((Func<SpawnPoint, bool>) (s => s.index == index));
      if ((UnityEngine.Object) spawnPoint == (UnityEngine.Object) null)
        spawnPoint = SpawnPoint.allSpawnPoints[0];
      this.hasSpawnedPlayer = true;
      if ((UnityEngine.Object) spawnPoint != (UnityEngine.Object) null)
      {
        position = spawnPoint.transform.position;
        rotation = spawnPoint.transform.rotation;
        Debug.Log((object) $"Setting player{PhotonNetwork.LocalPlayer.ActorNumber} to spawn point {spawnPoint.index}");
      }
      else
        Debug.LogError((object) "No Spawn Point, make on in the scene!");
      bool flag = SceneManager.GetActiveScene().name == "Airport";
      if (!GameHandler.TryGetStatus<SceneSwitchingStatus>(out SceneSwitchingStatus _) && !flag)
      {
        if (reconnectData.isValid)
          position = reconnectData.position;
      }
      else
        GameHandler.ClearStatus<SceneSwitchingStatus>();
      string[] source = CurrentPlayer.ReadOnlyTags();
      if ((UnityEngine.Object) Singleton<MapHandler>.Instance != (UnityEngine.Object) null && Singleton<MapHandler>.Instance.GetCurrentSegment() != Segment.Beach)
        position = Singleton<MapHandler>.Instance.segments[(int) Singleton<MapHandler>.Instance.GetCurrentSegment()].reconnectSpawnPos.position;
      Character character = (Character) null;
      if (!((IEnumerable<string>) source).Contains<string>("NoCharacter"))
      {
        if ((UnityEngine.Object) Character.localCharacter == (UnityEngine.Object) null)
        {
          Debug.Log((object) "Spawning local character.");
          Character component = PhotonNetwork.Instantiate("Character", position, rotation).GetComponent<Character>();
          character = component;
          component.data.spawnPoint = spawnPoint.transform;
          if (spawnPoint.startPassedOut)
            component.StartPassedOutOnTheBeach();
        }
        else
        {
          Debug.Log((object) "Moving local character to warp point.");
          Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, (object) position, (object) false);
          Character.localCharacter.data.spawnPoint = spawnPoint.transform;
          character = Character.localCharacter;
        }
      }
      if ((UnityEngine.Object) Player.localPlayer == (UnityEngine.Object) null)
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
      if (!flag && reconnectData.isValid)
        Singleton<ReconnectHandler>.Instance.SetCharacterState(character, reconnectData, !dead);
      if (dead && (UnityEngine.Object) character != (UnityEngine.Object) null)
        character.photonView.RPC("RPCA_Die", RpcTarget.All, (object) Vector3.zero);
    }
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    GameHandler.GetService<RichPresenceService>().Dirty();
  }

  public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
  {
    base.OnPlayerLeftRoom(otherPlayer);
    GameHandler.GetService<RichPresenceService>().Dirty();
  }
}
