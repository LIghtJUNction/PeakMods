// Decompiled with JetBrains decompiler
// Type: AscentData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
[CreateAssetMenu(fileName = "AscentData", menuName = "Scriptable Objects/AscentData")]
public class AscentData : SingletonAsset<AscentData>
{
  public List<AscentData.AscentInstanceData> ascents;

  public void AddAllToCSV()
  {
    for (int index = 0; index < this.ascents.Count; ++index)
      LocalizedText.AppendCSVLine($"{this.ascents[index].title.ToUpperInvariant()},{this.ascents[index].title.ToUpperInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
    for (int index = 0; index < this.ascents.Count; ++index)
      LocalizedText.AppendCSVLine($"{this.ascents[index].titleReward.ToUpperInvariant()},{this.ascents[index].titleReward.ToUpperInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
    for (int index = 0; index < this.ascents.Count; ++index)
      LocalizedText.AppendCSVLine($"DESC_{this.ascents[index].title.ToUpperInvariant()},{this.ascents[index].description.ToUpperInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
  }

  [Serializable]
  public class AscentInstanceData
  {
    public string title;
    public string titleReward;
    public string description;
    public Color color;
    public Sprite sashSprite;

    public string localizedTitle => LocalizedText.GetText(this.title);

    public string localizedReward => LocalizedText.GetText(this.titleReward);

    public string localizedDescription
    {
      get => LocalizedText.GetText(LocalizedText.GetDescriptionIndex(this.title));
    }
  }
}
