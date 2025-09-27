// Decompiled with JetBrains decompiler
// Type: PushToTalkSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class PushToTalkSetting : 
  CustomLocalizedEnumSetting<PushToTalkSetting.PushToTalkType>,
  IExposedSetting
{
  public override void ApplyValue()
  {
  }

  protected override PushToTalkSetting.PushToTalkType GetDefaultValue()
  {
    return PushToTalkSetting.PushToTalkType.VoiceActivation;
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Microphone mode";

  public string GetCategory() => "Audio";

  public override List<string> GetUnlocalizedChoices()
  {
    return new List<string>()
    {
      "Voice Activation",
      "Push To Talk",
      "Push To Mute"
    };
  }

  public enum PushToTalkType
  {
    VoiceActivation,
    PushToTalk,
    PushToMute,
  }
}
