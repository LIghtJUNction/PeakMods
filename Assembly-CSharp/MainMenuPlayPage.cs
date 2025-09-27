// Decompiled with JetBrains decompiler
// Type: MainMenuPlayPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.UI;

#nullable disable
public class MainMenuPlayPage : UIPage, IHaveParentPage
{
  [SerializeField]
  private Button m_playButton;
  [SerializeField]
  private TMP_InputField m_usernameField;
  [SerializeField]
  private TMP_InputField m_roomField;

  private void Start() => this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));

  public (UIPage, PageTransistion) GetParentPage()
  {
    return (this.pageHandler.GetPage<MainMenuMainPage>(), (PageTransistion) new SetActivePageTransistion());
  }

  public void PlayClicked()
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
      RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true));
    }
  }
}
