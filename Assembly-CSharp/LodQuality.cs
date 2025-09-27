// Decompiled with JetBrains decompiler
// Type: LodQuality
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class LodQuality : CustomLocalizedEnumSetting<LodQuality.Quality>, IExposedSetting
{
  public override void ApplyValue() => QualitySettings.lodBias = this.GetBias(this.Value);

  private float GetBias(LodQuality.Quality value)
  {
    if (value == LodQuality.Quality.High)
      return 1f;
    return value == LodQuality.Quality.Medium ? 0.85f : 0.7f;
  }

  protected override LodQuality.Quality GetDefaultValue()
  {
    return SteamUtils.IsSteamRunningOnSteamDeck() ? LodQuality.Quality.Low : LodQuality.Quality.Medium;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "World Quality";

  public string GetCategory() => "Graphics";

  public enum Quality
  {
    Low,
    Medium,
    High,
  }
}
