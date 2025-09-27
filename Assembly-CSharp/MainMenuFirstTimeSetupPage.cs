// Decompiled with JetBrains decompiler
// Type: MainMenuFirstTimeSetupPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Settings;
using Zorro.Settings.UI;
using Zorro.UI;

#nullable disable
public class MainMenuFirstTimeSetupPage : UIPage, INavigationPage
{
  public EnumSettingUI MicSettingUI;
  public Button ContinueButton;

  public void Start()
  {
    SettingsHandler instance = SettingsHandler.Instance;
    this.MicSettingUI.Setup((Setting) instance.GetSetting<MicrophoneSetting>(), (ISettingHandler) instance);
    this.ContinueButton.onClick.AddListener(new UnityAction(this.ContinueClicked));
  }

  private void ContinueClicked() => this.pageHandler.TransistionToPage<MainMenuMainPage>();

  public GameObject GetFirstSelectedGameObject() => this.MicSettingUI.dropdown.gameObject;
}
