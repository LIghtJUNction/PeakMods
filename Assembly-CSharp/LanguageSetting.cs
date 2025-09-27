// Decompiled with JetBrains decompiler
// Type: LanguageSetting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

#nullable disable
public class LanguageSetting : CustomLocalizedEnumSetting<LanguageSetting.Language>, IExposedSetting
{
  public LocalizedText.Language ValueToLanguage(int val)
  {
    switch (val)
    {
      case 0:
        return LocalizedText.Language.English;
      case 1:
        return LocalizedText.Language.French;
      case 2:
        return LocalizedText.Language.Italian;
      case 3:
        return LocalizedText.Language.German;
      case 4:
        return LocalizedText.Language.SpanishSpain;
      case 5:
        return LocalizedText.Language.SpanishLatam;
      case 6:
        return LocalizedText.Language.BRPortuguese;
      case 7:
        return LocalizedText.Language.Russian;
      case 8:
        return LocalizedText.Language.Ukrainian;
      case 9:
        return LocalizedText.Language.SimplifiedChinese;
      case 10:
        return LocalizedText.Language.Japanese;
      case 11:
        return LocalizedText.Language.Korean;
      case 12:
        return LocalizedText.Language.Polish;
      case 13:
        return LocalizedText.Language.Turkish;
      default:
        return LocalizedText.Language.English;
    }
  }

  public override void ApplyValue()
  {
    LocalizedText.SetLanguage((int) this.ValueToLanguage((int) this.Value));
  }

  protected override LanguageSetting.Language GetDefaultValue()
  {
    return (LanguageSetting.Language) LocalizedText.GetSystemLanguage();
  }

  public override List<LocalizedString> GetLocalizedChoices() => (List<LocalizedString>) null;

  public string GetDisplayName() => "Language";

  public override List<string> GetCustomLocalizedChoices()
  {
    List<string> localizedChoices = new List<string>();
    int length = this.Value.GetType().GetEnumNames().Length;
    for (int val = 0; val < length; ++val)
      localizedChoices.Add(LocalizedText.GetText("CURRENT_LANGUAGE", this.ValueToLanguage(val)));
    return localizedChoices;
  }

  public string GetCategory() => "General";

  public enum Language
  {
    English = 0,
    French = 1,
    Italian = 2,
    German = 3,
    SpanishSpain = 4,
    SpanishLatam = 5,
    BRPortuguese = 6,
    Russian = 7,
    Ukrainian = 8,
    SimplifiedChinese = 9,
    Japanese = 11, // 0x0000000B
    Korean = 12, // 0x0000000C
    Polish = 13, // 0x0000000D
    Turkish = 14, // 0x0000000E
  }
}
