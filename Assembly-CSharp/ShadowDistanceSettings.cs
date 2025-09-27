// Decompiled with JetBrains decompiler
// Type: ShadowDistanceSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

#nullable disable
public class ShadowDistanceSettings : 
  CustomLocalizedEnumSetting<ShadowDistanceSettings.ShadowDistanceQuality>,
  IExposedSetting
{
  public override void ApplyValue()
  {
    if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset currentRenderPipeline))
      return;
    switch (this.Value)
    {
      case ShadowDistanceSettings.ShadowDistanceQuality.High:
        currentRenderPipeline.shadowDistance = 200f;
        currentRenderPipeline.shadowCascadeCount = 2;
        break;
      case ShadowDistanceSettings.ShadowDistanceQuality.Medium:
        currentRenderPipeline.shadowDistance = 150f;
        currentRenderPipeline.shadowCascadeCount = 2;
        break;
      case ShadowDistanceSettings.ShadowDistanceQuality.Low:
        currentRenderPipeline.shadowDistance = 75f;
        currentRenderPipeline.shadowCascadeCount = 1;
        break;
      case ShadowDistanceSettings.ShadowDistanceQuality.Off:
        currentRenderPipeline.shadowDistance = 0.0f;
        currentRenderPipeline.shadowCascadeCount = 1;
        break;
    }
  }

  protected override ShadowDistanceSettings.ShadowDistanceQuality GetDefaultValue()
  {
    return SteamUtils.IsSteamRunningOnSteamDeck() ? ShadowDistanceSettings.ShadowDistanceQuality.Low : ShadowDistanceSettings.ShadowDistanceQuality.Medium;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Shadow Distance";

  public string GetCategory() => "Graphics";

  public enum ShadowDistanceQuality
  {
    High,
    Medium,
    Low,
    Off,
  }
}
