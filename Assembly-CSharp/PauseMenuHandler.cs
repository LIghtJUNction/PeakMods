// Decompiled with JetBrains decompiler
// Type: PauseMenuHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.UI;

#nullable disable
public class PauseMenuHandler : UIPageHandler
{
  public InputActionReference backButton;

  private void OnEnable()
  {
    if (PhotonNetwork.OfflineMode)
      Time.timeScale = 0.0f;
    if (!(this.currentPage is PauseMenuMainPage))
    {
      this.TransistionToPage<PauseMenuMainPage>();
    }
    else
    {
      this.currentPage.gameObject.SetActive(true);
      this.currentPage.OnPageEnter();
    }
  }

  private void OnDisable()
  {
    if (!PhotonNetwork.OfflineMode)
      return;
    Time.timeScale = 1f;
  }

  private void Update()
  {
    if (!(bool) (Object) Character.localCharacter || !Character.localCharacter.input.pauseWasPressed && !this.backButton.action.WasPressedThisFrame())
      return;
    if (this.currentPage is IHaveParentPage currentPage)
    {
      (UIPage, PageTransistion) parentPage = currentPage.GetParentPage();
      this.TransistionToPage(parentPage.Item1, parentPage.Item2);
    }
    else if (!PreventUnpause.UnpausePreventionActive)
      this.gameObject.SetActive(false);
    Character.localCharacter.input.pauseWasPressed = false;
  }
}