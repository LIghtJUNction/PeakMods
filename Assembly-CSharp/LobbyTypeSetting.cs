// Decompiled with JetBrains decompiler
// Type: LobbyTypeSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class LobbyTypeSetting : 
  CustomLocalizedEnumSetting<LobbyTypeSetting.LobbyType>,
  IExposedSetting,
  IConditionalSetting
{
  public override void ApplyValue()
  {
  }

  public override void Load(ISettingsSaveLoad loader)
  {
    base.Load(loader);
    if (PlayerPrefs.HasKey("DEFAULT_CHANGE_LOBBY_TYPE"))
      return;
    this.Value = this.GetDefaultValue();
  }

  public override void Save(ISettingsSaveLoad saver)
  {
    base.Save(saver);
    PlayerPrefs.SetInt("DEFAULT_CHANGE_LOBBY_TYPE", 1);
  }

  protected override LobbyTypeSetting.LobbyType GetDefaultValue()
  {
    return LobbyTypeSetting.LobbyType.InviteOnly;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public override List<string> GetUnlocalizedChoices()
  {
    return new List<string>() { "Friends", "Invite Only" };
  }

  public string GetDisplayName() => "Lobby Mode";

  public string GetCategory() => "General";

  public bool ShouldShow() => !PhotonNetwork.InRoom;

  public enum LobbyType
  {
    Friends,
    InviteOnly,
  }
}
