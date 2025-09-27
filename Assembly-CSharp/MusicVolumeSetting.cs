// Decompiled with JetBrains decompiler
// Type: MusicVolumeSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine.Audio;
using Zorro.Settings;

#nullable disable
public class MusicVolumeSetting(AudioMixerGroup mixerGroup) : VolumeSetting(mixerGroup), IExposedSetting
{
  public override string GetParameterName() => "MusicVolume";

  public string GetDisplayName() => "Music Volume";

  public string GetCategory() => "Audio";
}
