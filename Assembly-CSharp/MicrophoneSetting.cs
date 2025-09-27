// Decompiled with JetBrains decompiler
// Type: MicrophoneSetting
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
public class MicrophoneSetting : Setting, IEnumSetting, IExposedSetting
{
  public MicrophoneSetting.MicrophoneInfo Value;

  public List<MicrophoneSetting.MicrophoneInfo> GetChoices()
  {
    string[] devices = Microphone.devices;
    List<MicrophoneSetting.MicrophoneInfo> choices = new List<MicrophoneSetting.MicrophoneInfo>();
    foreach (string str in devices)
      choices.Add(new MicrophoneSetting.MicrophoneInfo()
      {
        id = str,
        name = str
      });
    return choices;
  }

  public override void Load(ISettingsSaveLoad loader)
  {
    string value;
    if (loader.TryLoadString(this.GetType(), out value))
    {
      this.Value = this.GetChoices().Find((Predicate<MicrophoneSetting.MicrophoneInfo>) (x => x.id == value));
      if (!string.IsNullOrEmpty(this.Value.id))
        return;
      Debug.LogWarning((object) $"Failed to load setting of type {this.GetType().FullName} from PlayerPrefs. Value not found in choices.");
      this.Value = this.GetDefaultValue();
    }
    else
    {
      Debug.LogWarning((object) $"Failed to load setting of type {this.GetType().FullName} from PlayerPrefs.");
      this.Value = this.GetDefaultValue();
    }
  }

  private MicrophoneSetting.MicrophoneInfo GetDefaultValue()
  {
    if (this.GetChoices().Count != 0)
      return this.GetChoices().First<MicrophoneSetting.MicrophoneInfo>();
    Debug.LogError((object) "No voice devices found.");
    return new MicrophoneSetting.MicrophoneInfo();
  }

  public override void Save(ISettingsSaveLoad saver)
  {
    saver.SaveString(this.GetType(), this.Value.id);
  }

  public override void ApplyValue()
  {
    Debug.Log((object) ("Voice setting applied: " + this.Value.ToString()));
  }

  public override SettingUI GetDebugUI(ISettingHandler settingHandler)
  {
    return (SettingUI) new EnumSettingsUI((IEnumSetting) this, settingHandler);
  }

  public override GameObject GetSettingUICell()
  {
    return SingletonAsset<InputCellMapper>.Instance.EnumSettingCell;
  }

  public List<string> GetUnlocalizedChoices()
  {
    return this.GetChoices().Select<MicrophoneSetting.MicrophoneInfo, string>((Func<MicrophoneSetting.MicrophoneInfo, string>) (info => info.name)).ToList<string>();
  }

  public int GetValue()
  {
    return this.GetChoices().Select<MicrophoneSetting.MicrophoneInfo, string>((Func<MicrophoneSetting.MicrophoneInfo, string>) (info => info.id)).ToList<string>().IndexOf(this.Value.id);
  }

  public void SetValue(int v, ISettingHandler settingHandler, bool fromUI)
  {
    this.Value = this.GetChoices()[v];
    this.ApplyValue();
    settingHandler.SaveSetting((Setting) this);
  }

  public string GetDisplayName() => "Microphone";

  public string GetCategory() => "Audio";

  public struct MicrophoneInfo
  {
    public string id;
    public string name;

    public override string ToString() => $"{this.id} ({this.name})";
  }
}
