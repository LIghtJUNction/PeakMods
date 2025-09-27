// Decompiled with JetBrains decompiler
// Type: MainMenuPageHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.UI;
using Zorro.UI.Modal;

#nullable disable
public class MainMenuPageHandler : UIPageHandler
{
  private static readonly int Connected = Animator.StringToHash(nameof (Connected));
  public InputActionReference BackReference;
  public Animator IntroAnimation;
  public TextMeshProUGUI ConnectingInfoText;
  private bool disconnected;

  protected override void Start()
  {
    base.Start();
    GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>();
    CloudAPI.CheckVersion((Action<LoginResponse>) (response =>
    {
      GameHandler.GetService<NextLevelService>().NewData(response);
      if (!response.VersionOkay)
      {
        Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("VERSIONOUTOFDATE"), LocalizedText.GetText("CLOSETHEGAME")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
        {
          new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
        }), new Action(Application.Quit));
      }
      else
      {
        string[] commandLineArgs = Environment.GetCommandLineArgs();
        if (commandLineArgs.Length < 2)
          return;
        for (int index = 0; index < commandLineArgs.Length - 1; ++index)
        {
          Debug.Log((object) ("Parsing arg: " + commandLineArgs[index]));
          if (commandLineArgs[index].ToLower() == "+connect_lobby")
          {
            ulong lobbyID;
            if (!ulong.TryParse(commandLineArgs[index + 1], out lobbyID) || lobbyID <= 0UL)
              break;
            this.StartCoroutine(ConnectSoon());
            break;

            IEnumerator ConnectSoon()
            {
              yield return (object) new WaitForSecondsRealtime(0.1f);
              GameHandler.GetService<SteamLobbyHandler>().TryJoinLobby(new CSteamID(lobbyID));
            }
          }
        }
      }
    }));
  }

  private void Update()
  {
    if (this.BackReference.action.WasPerformedThisFrame() && this.currentPage is IHaveParentPage currentPage)
    {
      (UIPage, PageTransistion) parentPage = currentPage.GetParentPage();
      this.TransistionToPage(parentPage.Item1, parentPage.Item2);
    }
    if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer && !(this.currentPage is MainMenuFirstTimeSetupPage) || PhotonNetwork.OfflineMode)
      this.IntroAnimation.SetBool(MainMenuPageHandler.Connected, true);
    if (!this.disconnected && PhotonNetwork.NetworkClientState == ClientState.Disconnected && !PhotonNetwork.OfflineMode && !GameHandler.TryGetStatus<IsDisconnectingForOfflineMode>(out IsDisconnectingForOfflineMode _))
    {
      this.disconnected = true;
      Debug.Log((object) "Opening disconnected modal");
      Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("FAILEDTOCONNECT"), LocalizedText.GetText("TRYCONNECTAGAIN")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[2]
      {
        new ModalButtonsOption.Option(LocalizedText.GetText("TRYAGAIN"), (Action) (() =>
        {
          PhotonNetwork.OfflineMode = false;
          NetworkConnector.ConnectToPhoton();
          this.StartCoroutine(Timeout());
        })),
        new ModalButtonsOption.Option(LocalizedText.GetText("PLAYOFFLINE"), (Action) (() => PhotonNetwork.OfflineMode = true))
      }));
    }
    this.ConnectingInfoText.text = this.GetPrettyStateName();

    IEnumerator Timeout()
    {
      yield return (object) new WaitForSecondsRealtime(5f);
      this.disconnected = false;
    }
  }

  private string GetPrettyStateName()
  {
    ClientState networkClientState = PhotonNetwork.NetworkClientState;
    switch (networkClientState)
    {
      case ClientState.Authenticating:
        return LocalizedText.GetText("AUTHENTICATING");
      case ClientState.ConnectingToMasterServer:
      case ClientState.ConnectingToNameServer:
      case ClientState.ConnectedToNameServer:
        return LocalizedText.GetText("CONNECTING");
      case ClientState.ConnectedToMasterServer:
        return "";
      default:
        return networkClientState.ToString();
    }
  }
}
