// Decompiled with JetBrains decompiler
// Type: NextLevelUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
public class NextLevelUI : MonoBehaviour
{
  public TextMeshProUGUI timer;
  private NextLevelService nextLevelService;

  private void Start() => this.nextLevelService = GameHandler.GetService<NextLevelService>();

  private void Update()
  {
    if (this.nextLevelService.Data.IsSome)
      this.timer.text = this.ParseSeconds(this.nextLevelService.Data.Value.SecondsLeft);
    else
      this.timer.text = "NO DATA";
  }

  public string ParseSeconds(int seconds)
  {
    if (seconds < 0)
      return "-- -- --";
    int num1 = Mathf.FloorToInt((float) seconds / 3600f);
    int num2 = Mathf.FloorToInt((float) (seconds - num1 * 3600) / 60f);
    float num3 = (float) (seconds - (num1 * 3600 + num2 * 60));
    return $"{num1}h {num2}m {num3}s";
  }
}
