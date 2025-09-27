// Decompiled with JetBrains decompiler
// Type: GameHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
[DefaultExecutionOrder(-100)]
public class GameHandler : MonoBehaviour
{
  private static GameHandler _instance;
  private Dictionary<System.Type, GameService> m_gameServices;
  private bool m_initialized;
  private Dictionary<System.Type, GameStatus> m_gameStatus;

  public static GameHandler Instance => GameHandler._instance;

  public SettingsHandler SettingsHandler { get; private set; }

  public static bool Initialized
  {
    get => (UnityEngine.Object) GameHandler.Instance != (UnityEngine.Object) null && GameHandler.Instance.m_initialized;
  }

  public void Initialize()
  {
    Debug.Log((object) "Game Handler Initialized");
    GameHandler._instance = this;
    UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
  }

  private void OnDestroy()
  {
    Debug.Log((object) "Game Handler Destroying...");
    foreach (GameService gameService in this.m_gameServices.Values)
      gameService.OnDestroy();
  }

  private async void Awake()
  {
    GameHandler gameHandler = this;
    gameHandler.m_gameStatus = new Dictionary<System.Type, GameStatus>();
    gameHandler.m_gameServices = new Dictionary<System.Type, GameService>();
    List<ConsoleCommand> consoleCommands = ConsoleHandler.ScanForConsoleCommands();
    Dictionary<System.Type, CLITypeParser> dictionary = ConsoleHandler.ScanForTypeParsers();
    CustomTypeRPCSerialization.Initialize();
    Dictionary<System.Type, CLITypeParser> typeParsers = dictionary;
    ConsoleHandler.Initialize(consoleCommands, typeParsers);
    RetrievableResourceSingleton<InputHandler>.Instance.Initialize((Func<bool>) (() => false), (Func<bool>) (() => !DebugUIHandler.IsOpen));
    NetworkStats networkStats = gameHandler.gameObject.AddComponent<NetworkStats>();
    gameHandler.RegisterService<PlayerHandler>(new PlayerHandler());
    gameHandler.RegisterService<ConnectionService>(new ConnectionService());
    gameHandler.RegisterService<SteamLobbyHandler>(new SteamLobbyHandler());
    gameHandler.RegisterService<PersistentPlayerDataService>(new PersistentPlayerDataService());
    gameHandler.RegisterService<NextLevelService>(new NextLevelService());
    gameHandler.RegisterService<SteamAuthTicketService>(new SteamAuthTicketService());
    gameHandler.RegisterService<RichPresenceService>(new RichPresenceService());
    Singleton<DebugUIHandler>.Instance.RegisterPage("Network Stats", (Func<DebugPage>) (() => (DebugPage) new NetworkStatsPage(networkStats)));
    Singleton<DebugUIHandler>.Instance.RegisterPage("Item Instance Datas", (Func<DebugPage>) (() => (DebugPage) new ItemInstanceDataDebugPage()));
    Singleton<DebugUIHandler>.Instance.RegisterPage("Reconnect Data", (Func<DebugPage>) (() => (DebugPage) new ReconnectDataDebugPage()));
    gameHandler.gameObject.AddComponent<SteamManager>();
    Debug.Log((object) "Added SteamManager");
    gameHandler.SettingsHandler = new SettingsHandler();
    gameHandler.m_initialized = true;
  }

  private void RegisterService<T>(T service) where T : GameService
  {
    this.m_gameServices[service.GetType()] = (GameService) service;
  }

  public static T GetService<T>() where T : GameService
  {
    return GameHandler.Instance.m_gameServices[typeof (T)] as T;
  }

  public static async Awaitable WaitForInitialization()
  {
    while (!GameHandler.Instance.m_initialized)
      await Awaitable.NextFrameAsync();
  }

  public static T RestartService<T>(T service) where T : GameService, IDisposable
  {
    Debug.Log((object) ("Restarting Service of type: " + typeof (T).Name));
    System.Type type = service.GetType();
    if (GameHandler.Instance.m_gameServices.ContainsKey(type))
      ((T) GameHandler.Instance.m_gameServices[type]).Dispose();
    GameHandler.Instance.m_gameServices[type] = (GameService) service;
    return service;
  }

  public static void AddStatus<T>(GameStatus status) where T : GameStatus
  {
    System.Type type = status.GetType();
    GameHandler.Instance.m_gameStatus[type] = status;
    Debug.Log((object) $"Add status: {type}");
  }

  public static bool TryGetStatus<T>(out T status) where T : GameStatus
  {
    GameStatus gameStatus;
    int num = GameHandler.Instance.m_gameStatus.TryGetValue(typeof (T), out gameStatus) ? 1 : 0;
    status = default (T);
    if (num == 0)
      return num != 0;
    status = gameStatus as T;
    return num != 0;
  }

  public static void ClearStatus<T>() where T : GameStatus
  {
    System.Type key = typeof (T);
    if (!GameHandler.Instance.m_gameStatus.ContainsKey(key))
      return;
    GameHandler.Instance.m_gameStatus.Remove(key);
    Debug.Log((object) $"Clear status: {key}");
  }

  public static void ClearAllStatuses()
  {
    GameHandler.Instance.m_gameStatus.Clear();
    Debug.Log((object) "Clearing all statuses!");
  }

  private void Update()
  {
    this.SettingsHandler.Update();
    foreach (GameService gameService in this.m_gameServices.Values)
      gameService.Update();
    Debug.ClearDeveloperConsole();
  }
}
