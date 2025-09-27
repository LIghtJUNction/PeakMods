// Decompiled with JetBrains decompiler
// Type: EmoteWheelData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "EmoteWheelData", menuName = "Scriptable Objects/EmoteWheelData")]
public class EmoteWheelData : ScriptableObject
{
  public string emoteName;
  public Sprite emoteSprite;
  public string anim;

  public void AddNameToCSV()
  {
    LocalizedText.AppendCSVLine($"{this.emoteName.ToUpperInvariant()},{this.emoteName.ToLowerInvariant()},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
  }
}
