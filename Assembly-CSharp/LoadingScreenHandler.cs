// Decompiled with JetBrains decompiler
// Type: LoadingScreenHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;

#nullable disable
public class LoadingScreenHandler : RetrievableResourceSingleton<LoadingScreenHandler>
{
  public LoadingScreen loadingScreenPrefabBasic;
  public LoadingScreen loadingScreenPrefabPlane;
  private Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen> loadingScreens;

  public static bool loading { get; private set; }

  private void Awake()
  {
    this.loadingScreens = new Dictionary<LoadingScreen.LoadingScreenType, LoadingScreen>()
    {
      {
        LoadingScreen.LoadingScreenType.Basic,
        this.loadingScreenPrefabBasic
      },
      {
        LoadingScreen.LoadingScreenType.Plane,
        this.loadingScreenPrefabPlane
      }
    };
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this);
  }

  public LoadingScreen GetLoadingScreenPrefab(LoadingScreen.LoadingScreenType type)
  {
    return this.loadingScreens[type];
  }

  public void Load(
    LoadingScreen.LoadingScreenType type,
    Action runAfter,
    params IEnumerator[] processes)
  {
    GameHandler.ClearStatus<EndScreenStatus>();
    if (!LoadingScreenHandler.loading)
      this.StartCoroutine(this.LoadingRoutine(type, runAfter, processes));
    else
      Debug.LogError((object) "Tried to load while already loading! If this happens a lot it's likely an issue!");
  }

  private IEnumerator LoadingRoutine(
    LoadingScreen.LoadingScreenType type,
    Action runAfter,
    params IEnumerator[] processes)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LoadingScreenHandler loadingScreenHandler = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      LoadingScreenHandler.loading = false;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    LoadingScreen loadingScreen = UnityEngine.Object.Instantiate<LoadingScreen>(loadingScreenHandler.GetLoadingScreenPrefab(type), Vector3.zero, Quaternion.identity);
    LoadingScreenHandler.loading = true;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) loadingScreenHandler.StartCoroutine(loadingScreen.LoadingRoutine(runAfter, processes));
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  internal IEnumerator LoadSceneProcess(
    string sceneName,
    bool networked,
    bool yieldForCharacterSpawn = false,
    float extraYieldTimeOnEnd = 3f)
  {
    if (networked)
      yield return (object) this.LoadSceneProcessNetworked(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
    else
      yield return (object) this.LoadSceneProcessOffline(sceneName, yieldForCharacterSpawn, extraYieldTimeOnEnd);
  }

  private IEnumerator LoadSceneProcessNetworked(
    string sceneName,
    bool yieldForCharacterSpawn,
    float extraYieldTimeOnEnd)
  {
    PhotonNetwork.LoadLevel(sceneName);
    float timeout = 5f;
    while ((double) timeout > 0.0 && (double) PhotonNetwork.LevelLoadingProgress == 0.0 || (double) PhotonNetwork.LevelLoadingProgress >= 1.0)
    {
      timeout -= Time.unscaledDeltaTime;
      yield return (object) null;
    }
    if ((UnityEngine.Object) DayNightManager.instance != (UnityEngine.Object) null)
    {
      DayNightManager.instance.specialDaySunBlend = 0.0f;
      DayNightManager.instance.specialDaySkyBlend = 0.0f;
    }
    while ((double) PhotonNetwork.LevelLoadingProgress < 1.0)
      yield return (object) null;
    while (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
      yield return (object) null;
    if (yieldForCharacterSpawn)
    {
      while (!(bool) (UnityEngine.Object) Character.localCharacter && PhotonNetwork.InRoom)
      {
        Debug.Log((object) "Connected and waiting for player to be spawned");
        yield return (object) null;
      }
    }
    yield return (object) new WaitForSecondsRealtime(extraYieldTimeOnEnd);
  }

  private IEnumerator LoadSceneProcessOffline(
    string sceneName,
    bool yieldForCharacterSpawn,
    float extraYieldTimeOnEnd)
  {
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    while (!operation.isDone)
    {
      Debug.Log((object) "Waiting for scene loading...");
      yield return (object) null;
    }
    while (PhotonNetwork.NetworkClientState == ClientState.ConnectingToGameServer)
    {
      Debug.Log((object) "Waiting while connecting...");
      yield return (object) null;
    }
    if (yieldForCharacterSpawn)
    {
      while (!(bool) (UnityEngine.Object) Character.localCharacter && PhotonNetwork.InRoom)
      {
        Debug.Log((object) "Connected and waiting for player to be spawned");
        yield return (object) null;
      }
    }
    yield return (object) new WaitForSecondsRealtime(extraYieldTimeOnEnd);
  }
}
