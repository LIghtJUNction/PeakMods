// Decompiled with JetBrains decompiler
// Type: MainMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class MainMenu : MonoBehaviour
{
  public GameObject credits;
  public Transform mainGuysHolder;
  public Button playSoloButton;
  public Button creditsButton;
  public Button quitButton;
  public Button discordButton;
  public Button landfallButton;
  public Button aggrocrabButton;

  private void Start()
  {
    AudioLevels.ResetSliders();
    this.playSoloButton.onClick.AddListener(new UnityAction(this.PlaySoloClicked));
    this.creditsButton.onClick.AddListener(new UnityAction(this.ToggleCredits));
    this.quitButton.onClick.AddListener(new UnityAction(this.Quit));
    this.discordButton.onClick.AddListener(new UnityAction(this.OpenDiscord));
    this.landfallButton.onClick.AddListener(new UnityAction(this.OpenLandfallWebsite));
    this.aggrocrabButton.onClick.AddListener(new UnityAction(this.OpenAggrocrabWebsite));
    Time.timeScale = 1f;
  }

  public void ToggleCredits()
  {
    this.credits.SetActive(!this.credits.activeSelf);
    if (!this.credits.activeSelf)
      return;
    this.RandomizeMainGuys();
  }

  public void OpenDiscord() => Application.OpenURL("https://discord.gg/peakgame");

  public void OpenLandfallWebsite() => Application.OpenURL("https://landfall.se/");

  public void OpenAggrocrabWebsite() => Application.OpenURL("https://aggrocrab.com/");

  public void RandomizeMainGuys()
  {
    Transform mainGuysHolder = this.mainGuysHolder;
    List<Transform> transformList1 = new List<Transform>();
    for (int index = 0; index < mainGuysHolder.childCount; ++index)
      transformList1.Add(mainGuysHolder.GetChild(index));
    for (int index1 = transformList1.Count - 1; index1 > 0; --index1)
    {
      int index2 = UnityEngine.Random.Range(0, index1 + 1);
      List<Transform> transformList2 = transformList1;
      int num = index1;
      List<Transform> transformList3 = transformList1;
      int index3 = index2;
      Transform transform1 = transformList1[index2];
      Transform transform2 = transformList1[index1];
      int index4 = num;
      Transform transform3;
      Transform transform4 = transform3 = transform1;
      transformList2[index4] = transform3;
      transformList3[index3] = transform4 = transform2;
    }
    for (int index = 0; index < transformList1.Count; ++index)
      transformList1[index].SetSiblingIndex(index);
  }

  public void Quit() => Application.Quit();

  private void PlaySoloClicked()
  {
    RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, this.StartOfflineModeRoutine());
  }

  private IEnumerator StartOfflineModeRoutine()
  {
    PhotonNetwork.IsMessageQueueRunning = true;
    GameHandler.AddStatus<IsDisconnectingForOfflineMode>((GameStatus) new IsDisconnectingForOfflineMode());
    PhotonNetwork.Disconnect();
    while (PhotonNetwork.IsConnected)
    {
      Debug.Log((object) "We are still connected.. waiting for disconnect");
      yield return (object) null;
    }
    PhotonNetwork.OfflineMode = true;
    GameHandler.ClearStatus<IsDisconnectingForOfflineMode>();
    yield return (object) RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true);
  }
}
