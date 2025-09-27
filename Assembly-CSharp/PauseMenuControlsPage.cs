// Decompiled with JetBrains decompiler
// Type: PauseMenuControlsPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

#nullable disable
public class PauseMenuControlsPage : UIPage, IHaveParentPage, INavigationPage
{
  public Button backButton;
  public Button restoreAllButton;
  private PauseMenuRebindButton[] controlsMenuButtons;
  public Transform controlsMenuButtonsParent;
  private bool initializedButtons;
  public GameObject[] keyboardOnlyObjects;
  public GameObject[] controllerOnlyObjects;

  private void Awake()
  {
    Rebinding.LoadRebindingsFromFile();
    this.restoreAllButton.onClick.AddListener(new UnityAction(this.OnResetClicked));
  }

  private void OnResetClicked()
  {
    UnityEngine.InputSystem.InputSystem.actions.RemoveAllBindingOverrides();
    Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
    if (inputSchemeChanged != null)
      inputSchemeChanged(InputHandler.GetCurrentUsedInputScheme());
    Rebinding.SaveRebindingsToFile();
  }

  private void OnEnable()
  {
    this.InitButtons();
    RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged += new Action<InputScheme>(this.OnDeviceChange);
    this.OnDeviceChange(InputHandler.GetCurrentUsedInputScheme());
  }

  private void OnDisable()
  {
    RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged -= new Action<InputScheme>(this.OnDeviceChange);
  }

  private void OnDeviceChange(InputScheme scheme)
  {
    switch (scheme)
    {
      case InputScheme.KeyboardMouse:
        foreach (GameObject keyboardOnlyObject in this.keyboardOnlyObjects)
          keyboardOnlyObject.SetActive(true);
        foreach (GameObject controllerOnlyObject in this.controllerOnlyObjects)
          controllerOnlyObject.SetActive(false);
        break;
      case InputScheme.Gamepad:
        foreach (GameObject keyboardOnlyObject in this.keyboardOnlyObjects)
          keyboardOnlyObject.SetActive(false);
        foreach (GameObject controllerOnlyObject in this.controllerOnlyObjects)
          controllerOnlyObject.SetActive(true);
        break;
    }
    this.InitButtonBindingVisuals(scheme);
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
  }

  private void InitButtons()
  {
    if ((UnityEngine.Object) this.pageHandler == (UnityEngine.Object) null)
      this.pageHandler = this.GetComponentInParent<UIPageHandler>();
    if (this.initializedButtons)
      return;
    this.controlsMenuButtons = this.controlsMenuButtonsParent.GetComponentsInChildren<PauseMenuRebindButton>(true);
    this.initializedButtons = true;
  }

  private void InitButtonBindingVisuals(InputScheme scheme)
  {
    for (int index = 0; index < this.controlsMenuButtons.Length; ++index)
    {
      this.controlsMenuButtons[index].Init(this.pageHandler);
      this.controlsMenuButtons[index].UpdateBindingVisuals(this.controlsMenuButtons, scheme);
    }
  }

  private void Start() => this.backButton.onClick.AddListener(new UnityAction(this.BackClicked));

  private void BackClicked() => this.pageHandler.TransistionToPage<PauseMenuMainPage>();

  public (UIPage, PageTransistion) GetParentPage()
  {
    return (this.pageHandler.GetPage<PauseMenuMainPage>(), (PageTransistion) new SetActivePageTransistion());
  }

  public GameObject GetFirstSelectedGameObject()
  {
    if (this.controlsMenuButtons.Length != 0)
    {
      for (int index = 0; index < this.controlsMenuButtons.Length; ++index)
      {
        if (this.controlsMenuButtons[index].gameObject.activeInHierarchy)
          return this.controlsMenuButtons[index].gameObject;
      }
    }
    return this.backButton.gameObject;
  }
}
