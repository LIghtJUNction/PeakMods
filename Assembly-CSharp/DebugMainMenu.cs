// Decompiled with JetBrains decompiler
// Type: DebugMainMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable disable
public class DebugMainMenu : MonoBehaviour
{
  [SerializeField]
  private Button m_matchmakeButton;
  [SerializeField]
  private Button m_debugJoinButton;
  [SerializeField]
  private Button m_debugCreateButton;
  [SerializeField]
  private Button m_debugRejoinButton;
  [SerializeField]
  private TMP_InputField m_usernameField;
  [SerializeField]
  private TMP_InputField m_roomField;
  public bool debugJoinOnAwake = true;
  private static bool first = true;

  private void Start()
  {
    this.m_matchmakeButton.onClick.AddListener(new UnityAction(this.MatchmakeClicked));
    this.m_debugJoinButton.onClick.AddListener(new UnityAction(this.DebugJoinClicked));
    this.m_debugCreateButton.onClick.AddListener(new UnityAction(this.DebugCreateClicked));
    this.m_debugRejoinButton.onClick.AddListener(new UnityAction(this.DebugRejoinClicked));
    if (!this.debugJoinOnAwake)
      return;
    this.DebugHaxxClicked();
  }

  private void DebugRejoinClicked()
  {
    Debug.Log((object) "Rejoining...");
    GameHandler.GetService<ConnectionService>();
    SceneManager.LoadScene("WilIsland");
  }

  private void DebugCreateClicked()
  {
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<HostState>().RoomName = "THEPETHEN";
    SceneManager.LoadScene("WilIsland");
  }

  private void DebugJoinClicked()
  {
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>().RoomName = "THEPETHEN";
    SceneManager.LoadScene("WilIsland");
  }

  private void DebugHaxxClicked()
  {
    ConnectionService service = GameHandler.GetService<ConnectionService>();
    if (((IEnumerable<string>) CurrentPlayer.ReadOnlyTags()).Contains<string>("Client") || !DebugMainMenu.first)
      service.StateMachine.SwitchState<JoinSpecificRoomState>().RoomName = "THEPETHEN";
    else
      service.StateMachine.SwitchState<HostState>().RoomName = "THEPETHEN";
    DebugMainMenu.first = false;
    SceneManager.LoadScene("WilIsland");
  }

  private void MatchmakeClicked()
  {
    if (string.IsNullOrEmpty(this.m_usernameField.text))
      Debug.LogError((object) "Failed to get username field...");
    else if (string.IsNullOrEmpty(this.m_roomField.text))
    {
      Debug.LogError((object) "Failed to get room name field...");
    }
    else
    {
      JoinSpecificRoomState specificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>();
      specificRoomState.RoomName = this.m_roomField.text.ToLower();
      specificRoomState.RegionToJoin = "eu";
      SceneManager.LoadScene("WilIsland");
    }
  }
}
