// Decompiled with JetBrains decompiler
// Type: AutoLocalizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
public class AutoLocalizer : MonoBehaviour
{
  public string index;
  public bool toUpper;

  public void AutoLoc()
  {
    string str = this.GetComponent<TMP_Text>().text;
    if (this.toUpper)
      str = str.ToUpper();
    if (str.Contains(',') || str.Contains('.'))
      str = $"\"{str}\"";
    LocalizedText.AppendCSVLine($"{this.index},{str},,,,,,,,,,,,,ENDLINE", "Localization/Unlocalized_Text", "Assets/Resources/Localization/Unlocalized_Text.csv");
    LocalizedText localizedText = this.gameObject.AddComponent<LocalizedText>();
    localizedText.index = this.index;
    localizedText.DebugReload();
    Object.DestroyImmediate((Object) this);
  }
}
