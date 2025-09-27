// Decompiled with JetBrains decompiler
// Type: PauseMenuRebindButton
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
public class PauseMenuRebindButton : MonoBehaviour
{
  private UnityEngine.InputSystem.InputAction inputAction;
  public string inputActionName;
  public LocalizedText inputDescriptionText;
  public string currentBindingPath;
  public Button rebindButton;
  public Button resetButton;
  [SerializeField]
  private UIPageHandler pageHandler;
  public Color defaultTextColor;
  public Color overriddenTextColor;
  public GameObject warning;
  public bool allowAxisBinding;
  private bool initialized;

  private void Awake()
  {
    this.inputAction = UnityEngine.InputSystem.InputSystem.actions.FindAction(this.inputActionName, false);
    this.rebindButton.onClick.AddListener(new UnityAction(this.OnRebindClicked));
    this.resetButton.onClick.AddListener(new UnityAction(this.OnResetClicked));
  }

  internal void UpdateBindingVisuals(PauseMenuRebindButton[] allButtons, InputScheme scheme)
  {
    bool hasOverride;
    this.currentBindingPath = InputSpriteData.GetBindingPath(this.inputActionName, scheme, out hasOverride);
    bool flag = false;
    foreach (PauseMenuRebindButton allButton in allButtons)
    {
      if (!((UnityEngine.Object) allButton == (UnityEngine.Object) this) && allButton.gameObject.activeInHierarchy && InputSpriteData.GetBindingPath(allButton.inputActionName, scheme, out bool _) == this.currentBindingPath)
        flag = true;
    }
    this.warning.SetActive(flag);
    if (hasOverride)
      this.inputDescriptionText.tmp.color = this.overriddenTextColor;
    else
      this.inputDescriptionText.tmp.color = this.defaultTextColor;
  }

  public void Init(UIPageHandler pageHandler) => this.pageHandler = pageHandler;

  private void OnRebindClicked()
  {
    PauseMenuRebindKeyPage.inputAction = this.inputAction;
    PauseMenuRebindKeyPage.inputLocIndex = this.inputDescriptionText.index;
    PauseMenuRebindKeyPage.forcedInputScheme = InputHandler.GetCurrentUsedInputScheme();
    this.pageHandler.TransistionToPage<PauseMenuRebindKeyPage>();
  }

  private void OnResetClicked()
  {
    this.inputAction.RemoveAllBindingOverrides();
    Rebinding.SaveRebindingsToFile();
    Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
    if (inputSchemeChanged == null)
      return;
    inputSchemeChanged(InputHandler.GetCurrentUsedInputScheme());
  }

  public UnityEngine.InputSystem.InputAction GetInputAction()
  {
    if (this.inputAction == null)
      this.inputAction = UnityEngine.InputSystem.InputSystem.actions.FindAction(this.inputActionName, false);
    return this.inputAction;
  }
}
