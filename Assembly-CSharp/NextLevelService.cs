// Decompiled with JetBrains decompiler
// Type: NextLevelService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using Zorro.Core;
using Zorro.UI.Modal;

#nullable disable
public class NextLevelService : GameService
{
  public Optionable<NextLevelService.NextLevelData> Data;

  public void NewData(LoginResponse response)
  {
    this.Data = Optionable<NextLevelService.NextLevelData>.Some(new NextLevelService.NextLevelData(response));
    Debug.Log((object) ("Setting new NextLevelData: " + this.Data.Value.ToString()));
  }

  public struct NextLevelData
  {
    public int CurrentLevelIndex;
    public float StartupTimeWhenQueried;
    public float SecondsLeftFromQueryTime;
    public string DevMessage;

    public int SecondsLeft
    {
      get
      {
        double f = (double) this.SecondsLeftFromQueryTime - (double) (Time.realtimeSinceStartup - this.StartupTimeWhenQueried);
        if (f < 0.0 && !GameHandler.TryGetStatus<QueryingGameTimeStatus>(out QueryingGameTimeStatus _))
          CloudAPI.CheckVersion((Action<LoginResponse>) (response =>
          {
            GameHandler.GetService<NextLevelService>().NewData(response);
            if (response.VersionOkay)
              return;
            Zorro.UI.Modal.Modal.OpenModal((HeaderModalOption) new DefaultHeaderModalOption(LocalizedText.GetText("MODAL_OUTOFDATE_TITLE"), LocalizedText.GetText("MODAL_OUTOFDATE_BODY")), (ModalContentOption) new ModalButtonsOption(new ModalButtonsOption.Option[1]
            {
              new ModalButtonsOption.Option(LocalizedText.GetText("OK"), (Action) null)
            }), new Action(Application.Quit));
          }));
        return Mathf.RoundToInt((float) f);
      }
    }

    public NextLevelData(LoginResponse login)
    {
      this.CurrentLevelIndex = login.LevelIndex;
      this.StartupTimeWhenQueried = Time.realtimeSinceStartup;
      this.SecondsLeftFromQueryTime = (float) (login.HoursUntilLevel * 60 * 60 + login.MinutesUntilLevel * 60 + login.SecondsUntilLevel);
      this.DevMessage = login.Message;
    }

    public override string ToString()
    {
      return $"CurrentIndex: {this.CurrentLevelIndex}, seconds left {this.SecondsLeft}";
    }
  }
}
