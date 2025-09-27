// Decompiled with JetBrains decompiler
// Type: SettingsHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Settings;

#nullable disable
public class SettingsHandler : ISettingHandler
{
  private List<Setting> settings;
  private ISettingsSaveLoad _settingsSaveLoad;
  public static SettingsHandler Instance;

  public SettingsHandler()
  {
    this.settings = new List<Setting>(30);
    this._settingsSaveLoad = (ISettingsSaveLoad) new DefaultSettingsSaveLoad();
    this.AddSetting((Setting) new LanguageSetting());
    this.AddSetting((Setting) new FovSetting());
    this.AddSetting((Setting) new ExtraFovSetting());
    this.AddSetting((Setting) new FullscreenEnumSetting());
    this.AddSetting((Setting) new ResolutionSetting());
    this.AddSetting((Setting) new FPSCapSetting());
    this.AddSetting((Setting) new VSyncSetting());
    this.AddSetting((Setting) new MicrophoneSetting());
    this.AddSetting((Setting) new RenderScaleSetting());
    this.AddSetting((Setting) new ShadowDistanceSettings());
    this.AddSetting((Setting) new TextureQualitySetting());
    this.AddSetting((Setting) new PushToTalkSetting());
    this.AddSetting((Setting) new MasterVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
    this.AddSetting((Setting) new SFXVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
    this.AddSetting((Setting) new MusicVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
    this.AddSetting((Setting) new VoiceVolumeSetting(SingletonAsset<StaticReferences>.Instance.masterMixerGroup));
    this.AddSetting((Setting) new MouseSensitivitySetting());
    this.AddSetting((Setting) new ControllerSensitivitySetting());
    this.AddSetting((Setting) new LodQuality());
    this.AddSetting((Setting) new AOSetting());
    this.AddSetting((Setting) new ControllerIconSetting());
    this.AddSetting((Setting) new InvertXSetting());
    this.AddSetting((Setting) new InvertYSetting());
    this.AddSetting((Setting) new JumpToClimbSetting());
    this.AddSetting((Setting) new LobbyTypeSetting());
    this.AddSetting((Setting) new HeadBobSetting());
    this.AddSetting((Setting) new CannibalismSetting());
    this.AddSetting((Setting) new BugPhobiaSetting());
    this.AddSetting((Setting) new PhotosensitiveSetting());
    this.AddSetting((Setting) new ColorblindSetting());
    this.AddSetting((Setting) new LookerSetting());
    Singleton<DebugUIHandler>.Instance?.RegisterPage("Settings", (Func<DebugPage>) (() => (DebugPage) new SettingsPage(this.settings, (ISettingHandler) this)));
    SettingsHandler.Instance = this;
    Debug.Log((object) "Settings Initlaized");
  }

  public void AddSetting(Setting setting)
  {
    this.settings.Add(setting);
    setting.Load(this._settingsSaveLoad);
    setting.ApplyValue();
  }

  public void SaveSetting(Setting setting)
  {
    setting.Save(this._settingsSaveLoad);
    this._settingsSaveLoad.WriteToDisk();
  }

  public T GetSetting<T>() where T : Setting
  {
    foreach (Setting setting1 in this.settings)
    {
      if (setting1 is T setting2)
        return setting2;
    }
    return default (T);
  }

  public IEnumerable<Setting> GetAllSettings() => (IEnumerable<Setting>) this.settings;

  public void Update()
  {
    foreach (Setting setting in this.settings)
      setting.Update();
  }
}
