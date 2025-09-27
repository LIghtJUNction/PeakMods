// Decompiled with JetBrains decompiler
// Type: BadgeData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
[CreateAssetMenu(fileName = "BadgeData", menuName = "Scriptable Objects/BadgeData")]
public class BadgeData : ScriptableObject
{
  public Texture icon;
  public string displayName;
  public string description;
  public ACHIEVEMENTTYPE linkedAchievement;
  public bool testLocked;
  public int visualID;

  public bool IsLocked
  {
    get => !Singleton<AchievementManager>.Instance.IsAchievementUnlocked(this.linkedAchievement);
  }

  public void AddToCSV()
  {
    string line1 = $"NAME_{this.displayName.ToUpperInvariant()},{this.displayName.ToUpperInvariant()} BADGE,,,,,,,,,,,,,ENDLINE";
    string line2 = $"DESC_{this.displayName.ToUpperInvariant()},\"{this.description}\",,,,,,,,,,,,,,ENDLINE";
    LocalizedText.AppendCSVLine(line1, "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
    LocalizedText.AppendCSVLine(line2, "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
  }
}
