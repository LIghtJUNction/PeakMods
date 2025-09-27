// Decompiled with JetBrains decompiler
// Type: PauseBadgesMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class PauseBadgesMenu : MonoBehaviour
{
  public Image badgeSashImage;
  public TMP_Text scoutTitleText;
  public AscentData ascentData;
  public TMP_Text peaksSummitedText;

  private void OnEnable()
  {
    int num1 = 0;
    int num2;
    if (Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.TimesPeaked, out num2))
      num1 = num2;
    this.peaksSummitedText.text = LocalizedText.GetText("PEAKSSUMMITTTED").Replace("#", num1.ToString() ?? "");
    this.scoutTitleText.text = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent()].localizedReward;
    this.badgeSashImage.color = this.ascentData.ascents[Singleton<AchievementManager>.Instance.GetMaxAscent()].color;
  }
}
