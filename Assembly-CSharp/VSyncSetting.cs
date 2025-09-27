// Decompiled with JetBrains decompiler
// Type: VSyncSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class VSyncSetting : CustomLocalizedEnumSetting<VSyncSetting.VSyncMode>, IExposedSetting
{
  public override void ApplyValue() => QualitySettings.vSyncCount = (int) this.Value;

  protected override VSyncSetting.VSyncMode GetDefaultValue()
  {
    return (VSyncSetting.VSyncMode) QualitySettings.vSyncCount;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Vsync";

  public string GetCategory() => "Graphics";

  public override List<string> GetUnlocalizedChoices()
  {
    return new List<string>() { "OFF", "ON" };
  }

  public enum VSyncMode
  {
    None,
    Enabled,
  }
}
