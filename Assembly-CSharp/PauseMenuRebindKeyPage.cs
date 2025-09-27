// Decompiled with JetBrains decompiler
// Type: PauseMenuRebindKeyPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.UI;

#nullable disable
public class PauseMenuRebindKeyPage : UIPage, INavigationPage
{
  private InputActionRebindingExtensions.RebindingOperation rebindOperation;
  public GameObject dummyButton;
  internal static UnityEngine.InputSystem.InputAction inputAction;
  internal static string inputLocIndex;
  public TextMeshProUGUI promptText;
  internal static InputScheme forcedInputScheme;
  private UnityEngine.InputSystem.InputAction action_pause;

  private void Awake() => this.action_pause = UnityEngine.InputSystem.InputSystem.actions.FindAction("Pause", false);

  private void Update()
  {
    if (!this.action_pause.WasPressedThisFrame() || this.rebindOperation == null || !this.rebindOperation.started || this.rebindOperation.completed)
      return;
    Debug.Log((object) ("CANCEL REBINDING " + PauseMenuRebindKeyPage.inputAction.name));
    this.rebindOperation.Cancel();
    this.rebindOperation.Dispose();
  }

  public GameObject GetFirstSelectedGameObject() => this.dummyButton;

  public override void OnPageEnter()
  {
    base.OnPageEnter();
    if (PauseMenuRebindKeyPage.inputAction == null || PauseMenuRebindKeyPage.inputAction.name == "Pause")
      return;
    int bindingIndex = -1;
    int index = 0;
    while (true)
    {
      int num = index;
      ReadOnlyArray<InputBinding> bindings = PauseMenuRebindKeyPage.inputAction.bindings;
      int count = bindings.Count;
      if (num < count)
      {
        bindings = PauseMenuRebindKeyPage.inputAction.bindings;
        if (!bindings[index].groups.Contains("Keyboard&Mouse") || PauseMenuRebindKeyPage.forcedInputScheme != InputScheme.KeyboardMouse)
        {
          bindings = PauseMenuRebindKeyPage.inputAction.bindings;
          if (!bindings[index].groups.Contains("Gamepad") || PauseMenuRebindKeyPage.forcedInputScheme != InputScheme.Gamepad)
            ++index;
          else
            goto label_5;
        }
        else
          break;
      }
      else
        goto label_8;
    }
    bindingIndex = index;
    goto label_8;
label_5:
    bindingIndex = index;
label_8:
    PauseMenuRebindKeyPage.inputAction.Disable();
    this.rebindOperation = PauseMenuRebindKeyPage.inputAction.PerformInteractiveRebinding(bindingIndex).WithControlsExcluding("<Mouse>/position").WithControlsExcluding("<Mouse>/delta").WithControlsExcluding("<Gamepad>/Start").WithControlsExcluding("<Gamepad>/leftStick/left").WithControlsExcluding("<Gamepad>/leftStick/right").WithControlsExcluding("<Gamepad>/leftStick/up").WithControlsExcluding("<Gamepad>/leftStick/down").WithControlsExcluding("<Gamepad>/rightStick/left").WithControlsExcluding("<Gamepad>/rightStick/right").WithControlsExcluding("<Gamepad>/rightStick/up").WithControlsExcluding("<Gamepad>/rightStick/down").WithControlsExcluding("<Keyboard>/leftMeta").WithControlsExcluding("<Keyboard>/rightMeta").WithControlsExcluding("<Keyboard>/contextMenu").WithControlsExcluding("<Keyboard>/anyKey").WithCancelingThrough("<Keyboard>/escape").WithCancelingThrough("<Gamepad>/Start").WithControlsExcluding("<Keyboard>/escape").OnComplete((Action<InputActionRebindingExtensions.RebindingOperation>) (operation => this.Completed())).OnCancel((Action<InputActionRebindingExtensions.RebindingOperation>) (operation => this.Cancelled()));
    switch (PauseMenuRebindKeyPage.forcedInputScheme)
    {
      case InputScheme.KeyboardMouse:
        this.rebindOperation = this.rebindOperation.WithControlsExcluding("<Gamepad>");
        break;
      case InputScheme.Gamepad:
        this.rebindOperation = this.rebindOperation.WithControlsExcluding("<Keyboard>").WithControlsExcluding("<Mouse>");
        break;
    }
    this.rebindOperation.Start();
    this.promptText.text = LocalizedText.GetText("PROMPT_REBIND").Replace("@", LocalizedText.GetText(PauseMenuRebindKeyPage.inputLocIndex));
  }

  private void Completed()
  {
    Debug.Log((object) $"FINISHED REBINDING {PauseMenuRebindKeyPage.inputAction.name} to {this.rebindOperation.selectedControl}");
    foreach (InputBinding binding in PauseMenuRebindKeyPage.inputAction.bindings)
    {
      Debug.Log((object) ("Checking against " + binding.path));
      if (InputSpriteData.GetPathEnd(binding.path) == InputSpriteData.GetPathEnd(this.rebindOperation.selectedControl.path))
        this.rebindOperation.action.RemoveAllBindingOverrides();
    }
    this.rebindOperation.Dispose();
    PauseMenuRebindKeyPage.inputAction.Enable();
    Rebinding.SaveRebindingsToFile();
    Action<InputScheme> inputSchemeChanged = RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged;
    if (inputSchemeChanged != null)
      inputSchemeChanged(InputHandler.GetCurrentUsedInputScheme());
    this.StartCoroutine(this.ReturnRoutine());
  }

  private void Cancelled()
  {
    this.rebindOperation.Dispose();
    PauseMenuRebindKeyPage.inputAction.Enable();
    this.StartCoroutine(this.ReturnRoutine());
  }

  private IEnumerator ReturnRoutine()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PauseMenuRebindKeyPage menuRebindKeyPage = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      menuRebindKeyPage.pageHandler.TransistionToPage<PauseMenuControlsPage>();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override void OnPageExit()
  {
    if (this.rebindOperation == null || !this.rebindOperation.started || this.rebindOperation.completed)
      return;
    Debug.Log((object) ("CANCEL REBINDING " + PauseMenuRebindKeyPage.inputAction.name));
    this.rebindOperation.Cancel();
    this.rebindOperation.Dispose();
  }
}
