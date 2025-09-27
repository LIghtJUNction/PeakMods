// Decompiled with JetBrains decompiler
// Type: RenderScaleSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zorro.Settings;

#nullable disable
public class RenderScaleSetting : 
  CustomLocalizedEnumSetting<RenderScaleSetting.RenderScaleQuality>,
  IExposedSetting
{
  public override void ApplyValue()
  {
    if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset currentRenderPipeline))
      return;
    currentRenderPipeline.renderScale = this.GetRenderScale(this.Value);
    Debug.Log((object) $"Set Render Scale: {currentRenderPipeline.renderScale}");
    if (this.Value == RenderScaleSetting.RenderScaleQuality.Native)
      currentRenderPipeline.upscalingFilter = UpscalingFilterSelection.Linear;
    else
      currentRenderPipeline.upscalingFilter = UpscalingFilterSelection.STP;
  }

  public float GetRenderScale(RenderScaleSetting.RenderScaleQuality quality)
  {
    switch (quality)
    {
      case RenderScaleSetting.RenderScaleQuality.Native:
        return 1f;
      case RenderScaleSetting.RenderScaleQuality.High:
        return 0.8f;
      case RenderScaleSetting.RenderScaleQuality.Medium:
        return 0.4f;
      case RenderScaleSetting.RenderScaleQuality.Low:
        return 0.2f;
      default:
        throw new ArgumentOutOfRangeException(nameof (quality), (object) quality, (string) null);
    }
  }

  protected override RenderScaleSetting.RenderScaleQuality GetDefaultValue()
  {
    return SteamUtils.IsSteamRunningOnSteamDeck() ? RenderScaleSetting.RenderScaleQuality.Medium : RenderScaleSetting.RenderScaleQuality.High;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Render Scale";

  public string GetCategory() => "Graphics";

  public enum RenderScaleQuality
  {
    Native,
    High,
    Medium,
    Low,
  }
}
