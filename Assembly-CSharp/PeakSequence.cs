// Decompiled with JetBrains decompiler
// Type: PeakSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PeakSequence : MonoBehaviour
{
  private PhotonView view;
  public GameObject ropeAnchorWithRopePref;
  public Transform ropeSpawnPoint;
  private float waitTime;
  public float timeToWait = 5f;
  public int totalSeconds = 30;
  public int totalWinningSeconds = 5;
  public float lengthOfASecond = 1.5f;
  private bool spawnedRope;
  public RopeAnchorWithRope ropeAnchorInstance;
  public Rope ropeInstance;
  private float timerElapsed;
  private int secondsElapsed;
  private bool endingGame;

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void OnDisable()
  {
    if (PhotonNetwork.IsMasterClient)
    {
      Debug.Log((object) "Destroying ropes");
      if ((Object) this.ropeAnchorInstance != (Object) null)
        PhotonNetwork.Destroy(this.ropeAnchorInstance.photonView);
      if (!((Object) this.ropeInstance != (Object) null))
        return;
      PhotonNetwork.Destroy(this.ropeInstance.photonView);
    }
    else
    {
      if ((Object) this.ropeAnchorInstance != (Object) null)
        this.ropeAnchorInstance.gameObject.SetActive(false);
      if (!((Object) this.ropeInstance != (Object) null))
        return;
      this.ropeInstance.gameObject.SetActive(false);
    }
  }

  private void Update()
  {
    if ((double) this.waitTime > (double) this.timeToWait)
    {
      if (!this.spawnedRope)
      {
        if (PhotonNetwork.IsMasterClient)
        {
          this.spawnedRope = true;
          this.ropeAnchorInstance = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.ropeSpawnPoint.position, Quaternion.identity).GetComponent<RopeAnchorWithRope>();
          this.ropeAnchorInstance.ropeSegmentLength = 40f;
          this.view.RPC("SetRopeToClients", RpcTarget.All, (object) this.ropeAnchorInstance.SpawnRope().GetComponent<PhotonView>());
        }
      }
      else
        this.CheckGameComplete();
    }
    this.waitTime += Time.deltaTime;
  }

  private void CheckGameComplete()
  {
    if (this.endingGame || !PhotonNetwork.IsMasterClient)
      return;
    int num = 0;
    List<Character> playerCharacters = PlayerHandler.GetAllPlayerCharacters();
    for (int index = playerCharacters.Count - 1; index >= 0; --index)
    {
      if (playerCharacters[index].data.dead)
        playerCharacters.RemoveAt(index);
    }
    List<Character> characterList = new List<Character>();
    foreach (Character character in playerCharacters)
    {
      if (character.data.fullyConscious)
        characterList.Add(character);
    }
    for (int index = 0; index < playerCharacters.Count; ++index)
    {
      if (Character.CheckWinCondition(playerCharacters[index]))
        ++num;
    }
    if (num > 0)
    {
      this.timerElapsed += Time.deltaTime;
      if ((double) this.timerElapsed < (double) this.lengthOfASecond)
        return;
      if (num >= characterList.Count && this.secondsElapsed < this.totalSeconds - this.totalWinningSeconds)
        this.secondsElapsed = this.totalSeconds - this.totalWinningSeconds;
      this.timerElapsed = 0.0f;
      this.view.RPC("RPCUpdateTimer", RpcTarget.All, (object) this.secondsElapsed);
      ++this.secondsElapsed;
      if (this.secondsElapsed <= this.totalSeconds)
        return;
      this.endingGame = true;
      Character.localCharacter.EndGame();
    }
    else
    {
      this.secondsElapsed = 0;
      this.timerElapsed = 0.0f;
      this.view.RPC("RPCUpdateTimer", RpcTarget.All, (object) -1);
    }
  }

  [PunRPC]
  public void SetRopeToClients(PhotonView v)
  {
    this.ropeInstance = v.GetComponent<Rope>();
    Debug.Log((object) $"ROPE AS BEEN SET TO {this.ropeInstance}");
  }

  [PunRPC]
  private void RPCUpdateTimer(int seconds)
  {
    if (seconds == -1)
      GUIManager.instance.endgame.Disable();
    else
      GUIManager.instance.endgame.UpdateCounter(this.totalSeconds - seconds);
  }
}
