// Decompiled with JetBrains decompiler
// Type: PauseMenuSettingsMenuPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.UI;

#nullable disable
public class PauseMenuSettingsMenuPage : UIPage, IHaveParentPage, INavigationPage
{
  public Button backButton;
  public SharedSettingsMenu sharedSettingsMenu;

  private void Start() => this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));

  private void BackClicked() => this.pageHandler.TransistionToPage<PauseMenuMainPage>();

  public (UIPage, PageTransistion) GetParentPage()
  {
    return (this.pageHandler.GetPage<PauseMenuMainPage>(), (PageTransistion) new SetActivePageTransistion());
  }

  public GameObject GetFirstSelectedGameObject()
  {
    GameObject defaultSelection = this.sharedSettingsMenu.GetDefaultSelection();
    return (Object) defaultSelection == (Object) null ? this.backButton.gameObject : defaultSelection;
  }
}
