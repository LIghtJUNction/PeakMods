// Decompiled with JetBrains decompiler
// Type: TextureQualitySetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using Zorro.Settings;

#nullable disable
public class TextureQualitySetting : 
  CustomLocalizedEnumSetting<TextureQualitySetting.TextureQuality>,
  IExposedSetting
{
  public override void ApplyValue()
  {
    RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
    switch (this.Value)
    {
      case TextureQualitySetting.TextureQuality.Native:
        QualitySettings.globalTextureMipmapLimit = 0;
        break;
      case TextureQualitySetting.TextureQuality.High:
        QualitySettings.globalTextureMipmapLimit = 1;
        break;
      case TextureQualitySetting.TextureQuality.Medium:
        QualitySettings.globalTextureMipmapLimit = 2;
        break;
      case TextureQualitySetting.TextureQuality.Low:
        QualitySettings.globalTextureMipmapLimit = 3;
        break;
    }
  }

  protected override TextureQualitySetting.TextureQuality GetDefaultValue()
  {
    return SteamUtils.IsSteamRunningOnSteamDeck() ? TextureQualitySetting.TextureQuality.High : TextureQualitySetting.TextureQuality.Native;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Texture Quality";

  public string GetCategory() => "Graphics";

  public enum TextureQuality
  {
    Native,
    High,
    Medium,
    Low,
  }
}
