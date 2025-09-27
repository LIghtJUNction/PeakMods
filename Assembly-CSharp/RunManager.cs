// Decompiled with JetBrains decompiler
// Type: RunManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class RunManager : MonoBehaviourPunCallbacks
{
  public float timeSinceRunStarted;
  private bool runStarted;
  private bool timerActive;
  public static RunManager Instance;

  private void Awake() => RunManager.Instance = this;

  private IEnumerator Start()
  {
    RunManager runManager = this;
    runManager.runStarted = false;
    runManager.timeSinceRunStarted = 0.0f;
    while (!PhotonNetwork.InRoom || !(bool) (Object) Character.localCharacter || LoadingScreenHandler.loading)
      yield return (object) null;
    Debug.Log((object) "RUN STARTED");
    runManager.StartRun();
    yield return (object) new WaitForSeconds(2f);
    if (PhotonNetwork.IsMasterClient)
      runManager.photonView.RPC("RPC_SyncTime", RpcTarget.All, (object) 0.0f, (object) true);
  }

  private void Update()
  {
    if (!this.timerActive)
      return;
    this.timeSinceRunStarted += Time.deltaTime;
  }

  public void StartRun()
  {
    this.runStarted = true;
    Singleton<AchievementManager>.Instance.InitRunBasedValues();
  }

  private void DebugCurrentTime() => Debug.Log((object) this.timeSinceRunStarted);

  internal void SyncTimeMaster()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.photonView.RPC("RPC_SyncTime", RpcTarget.All, (object) this.timeSinceRunStarted, (object) this.timerActive);
  }

  internal void EndGame()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.photonView.RPC("RPC_SyncTime", RpcTarget.All, (object) this.timeSinceRunStarted, (object) false);
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.photonView.RPC("RPC_SyncTime", newPlayer, (object) this.timeSinceRunStarted, (object) this.timerActive);
  }

  [PunRPC]
  private void RPC_SyncTime(float time, bool timerActive)
  {
    Debug.Log((object) $"Time synced: {time} timer active: {timerActive}");
    this.timeSinceRunStarted = time;
    this.timerActive = timerActive;
  }
}
