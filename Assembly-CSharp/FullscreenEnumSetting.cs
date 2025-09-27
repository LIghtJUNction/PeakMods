// Decompiled with JetBrains decompiler
// Type: FullscreenEnumSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

#nullable disable
public class FullscreenEnumSetting : 
  Setting,
  IEnumSetting,
  IExposedSetting,
  ICustomLocalizedEnumSetting
{
  public override void ApplyValue()
  {
  }

  public override SettingUI GetDebugUI(ISettingHandler settingHandler)
  {
    return (SettingUI) new EnumSettingsUI((IEnumSetting) this, settingHandler);
  }

  public override GameObject GetSettingUICell()
  {
    return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
  }

  public override void Load(ISettingsSaveLoad loader)
  {
  }

  public override void Save(ISettingsSaveLoad saver)
  {
  }

  public string GetDisplayName() => "Window Mode";

  public string GetCategory() => "Graphics";

  public List<string> GetUnlocalizedChoices()
  {
    return new List<string>()
    {
      "Windowed",
      "Fullscreen",
      "Windowed Fullscreen"
    };
  }

  public int GetValue()
  {
    switch (UnityEngine.Device.Screen.fullScreenMode)
    {
      case FullScreenMode.ExclusiveFullScreen:
        return 1;
      case FullScreenMode.FullScreenWindow:
        return 2;
      case FullScreenMode.Windowed:
        return 0;
      default:
        return 0;
    }
  }

  public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
  {
    switch (v)
    {
      case 0:
        UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.Windowed;
        break;
      case 1:
        UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        break;
      case 2:
        UnityEngine.Device.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        break;
    }
  }

  public List<string> GetCustomLocalizedChoices()
  {
    return this.GetUnlocalizedChoices().Select<string, string>((Func<string, string>) (s => LocalizedText.GetText(s))).ToList<string>();
  }

  public void DeregisterCustomLocalized(Action action)
  {
    LocalizedText.OnLangugageChanged -= action;
  }

  public void RegisterCustomLocalized(Action action) => LocalizedText.OnLangugageChanged += action;
}
