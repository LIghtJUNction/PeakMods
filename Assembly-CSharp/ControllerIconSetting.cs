// Decompiled with JetBrains decompiler
// Type: ControllerIconSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class ControllerIconSetting : 
  CustomLocalizedEnumSetting<ControllerIconSetting.IconMode>,
  IExposedSetting
{
  public override void ApplyValue()
  {
  }

  protected override ControllerIconSetting.IconMode GetDefaultValue()
  {
    return ControllerIconSetting.IconMode.Auto;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "INPUTICONS";

  public string GetCategory() => "General";

  public enum IconMode
  {
    Auto,
    Style1,
    Style2,
    KBM,
  }
}
