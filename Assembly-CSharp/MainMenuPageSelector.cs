// Decompiled with JetBrains decompiler
// Type: MainMenuPageSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.UI;

#nullable disable
public class MainMenuPageSelector : UIPageHandlerStartPageSelector
{
  public MainMenuMainPage mainPage;
  public MainMenuFirstTimeSetupPage firstTimeSetupPage;

  public override UIPage GetStartPage()
  {
    string key = "FirstTimeStartup2";
    if (PlayerPrefs.HasKey(key))
      return (UIPage) this.mainPage;
    PlayerPrefs.SetInt(key, 1);
    PlayerPrefs.Save();
    return (UIPage) this.firstTimeSetupPage;
  }
}
