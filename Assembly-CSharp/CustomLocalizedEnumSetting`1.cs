// Decompiled with JetBrains decompiler
// Type: CustomLocalizedEnumSetting`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Settings;

#nullable disable
public abstract class CustomLocalizedEnumSetting<T> : EnumSetting<T>, ICustomLocalizedEnumSetting where T : unmanaged, Enum
{
  public virtual List<string> GetCustomLocalizedChoices()
  {
    return this.GetUnlocalizedChoices().Select<string, string>((Func<string, string>) (s => LocalizedText.GetText(s))).ToList<string>();
  }

  public void DeregisterCustomLocalized(Action action)
  {
    LocalizedText.OnLangugageChanged -= action;
  }

  public void RegisterCustomLocalized(Action action) => LocalizedText.OnLangugageChanged += action;
}
