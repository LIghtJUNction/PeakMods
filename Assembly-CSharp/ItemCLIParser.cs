// Decompiled with JetBrains decompiler
// Type: ItemCLIParser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
[TypeParser(typeof (Item))]
public class ItemCLIParser : CLITypeParser
{
  public override object Parse(string str)
  {
    return (object) ObjectDatabaseAsset<ItemDatabase, Item>.GetObjectFromString(str);
  }

  public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
  {
    return SingletonAsset<ItemDatabase>.Instance.GetAutocompleteOptions(parameterText);
  }
}
