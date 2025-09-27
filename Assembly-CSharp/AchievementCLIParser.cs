// Decompiled with JetBrains decompiler
// Type: AchievementCLIParser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Zorro.Core.CLI;

#nullable disable
[TypeParser(typeof (ACHIEVEMENTTYPE))]
public class AchievementCLIParser : CLITypeParser
{
  public override object Parse(string str)
  {
    ACHIEVEMENTTYPE result;
    return Enum.TryParse<ACHIEVEMENTTYPE>(str, out result) ? (object) result : (object) ACHIEVEMENTTYPE.NONE;
  }

  public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
  {
    List<ParameterAutocomplete> autocomplete = new List<ParameterAutocomplete>();
    foreach (ACHIEVEMENTTYPE achievementtype in (ACHIEVEMENTTYPE[]) Enum.GetValues(typeof (ACHIEVEMENTTYPE)))
      autocomplete.Add(new ParameterAutocomplete(achievementtype.ToString()));
    return autocomplete;
  }
}
