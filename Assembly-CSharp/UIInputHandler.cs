// Decompiled with JetBrains decompiler
// Type: UIInputHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zorro.ControllerSupport;
using Zorro.Core;

#nullable disable
[DefaultExecutionOrder(-1000)]
public class UIInputHandler : Singleton<UIInputHandler>
{
  public static UnityEngine.InputSystem.InputAction action_confirm;
  public static UnityEngine.InputSystem.InputAction action_cancel;
  public static UnityEngine.InputSystem.InputAction action_tabLeft;
  public static UnityEngine.InputSystem.InputAction action_tabRight;
  public static UnityEngine.InputSystem.InputAction action_navigateWheel;
  public bool confirmWasPressed;
  public bool cancelWasPressed;
  public bool tabLeftWasPressed;
  public bool tabRightWasPressed;
  internal static GameObject previouslySelectedControllerElement;

  public Vector2 wheelNavigationVector { get; private set; }

  public void Initialize()
  {
    UIInputHandler.action_confirm = UnityEngine.InputSystem.InputSystem.actions.FindAction("UIConfirm", false);
    UIInputHandler.action_cancel = UnityEngine.InputSystem.InputSystem.actions.FindAction("UICancel", false);
    UIInputHandler.action_tabLeft = UnityEngine.InputSystem.InputSystem.actions.FindAction("UITabLeft", false);
    UIInputHandler.action_tabRight = UnityEngine.InputSystem.InputSystem.actions.FindAction("UITabRight", false);
    UIInputHandler.action_navigateWheel = UnityEngine.InputSystem.InputSystem.actions.FindAction("NavigateWheel", false);
    RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged += new Action<InputScheme>(this.OnInputSchemeChanged);
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    RetrievableResourceSingleton<InputHandler>.Instance.InputSchemeChanged -= new Action<InputScheme>(this.OnInputSchemeChanged);
  }

  private void Update() => this.Sample();

  private void Sample()
  {
    this.confirmWasPressed = UIInputHandler.action_confirm.WasPressedThisFrame();
    this.cancelWasPressed = UIInputHandler.action_cancel.WasPressedThisFrame();
    this.tabLeftWasPressed = UIInputHandler.action_tabLeft.WasPressedThisFrame();
    this.tabRightWasPressed = UIInputHandler.action_tabRight.WasPressedThisFrame();
    this.wheelNavigationVector = UIInputHandler.action_navigateWheel.ReadValue<Vector2>();
  }

  private void OnInputSchemeChanged(InputScheme scheme)
  {
  }

  public static void SetSelectedObject(GameObject obj)
  {
    if (InputHandler.GetCurrentUsedInputScheme() != InputScheme.Gamepad)
      return;
    EventSystem.current.SetSelectedGameObject(obj);
  }

  private void Deselect() => EventSystem.current.SetSelectedGameObject((GameObject) null);

  private void SelectPrevious()
  {
    EventSystem.current.SetSelectedGameObject(UIInputHandler.previouslySelectedControllerElement);
  }
}
