// Decompiled with JetBrains decompiler
// Type: CharacterInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;

#nullable disable
public class CharacterInput : MonoBehaviour
{
  public InputActionAsset actions;
  public static UnityEngine.InputSystem.InputAction action_move;
  public static UnityEngine.InputSystem.InputAction action_moveForward;
  public static UnityEngine.InputSystem.InputAction action_moveBackward;
  public static UnityEngine.InputSystem.InputAction action_moveLeft;
  public static UnityEngine.InputSystem.InputAction action_moveRight;
  public static UnityEngine.InputSystem.InputAction action_look;
  public static UnityEngine.InputSystem.InputAction action_jump;
  public static UnityEngine.InputSystem.InputAction action_sprint;
  public static UnityEngine.InputSystem.InputAction action_sprintToggle;
  public static UnityEngine.InputSystem.InputAction action_interact;
  public static UnityEngine.InputSystem.InputAction action_drop;
  public static UnityEngine.InputSystem.InputAction action_crouch;
  public static UnityEngine.InputSystem.InputAction action_crouchToggle;
  public static UnityEngine.InputSystem.InputAction action_usePrimary;
  public static UnityEngine.InputSystem.InputAction action_useSecondary;
  public static UnityEngine.InputSystem.InputAction action_scroll;
  public static UnityEngine.InputSystem.InputAction action_emote;
  public static UnityEngine.InputSystem.InputAction action_ping;
  public static UnityEngine.InputSystem.InputAction action_pause;
  public static UnityEngine.InputSystem.InputAction action_scrollBackward;
  public static UnityEngine.InputSystem.InputAction action_scrollForward;
  public static UnityEngine.InputSystem.InputAction action_selectSlotForward;
  public static UnityEngine.InputSystem.InputAction action_selectSlotBackward;
  public static UnityEngine.InputSystem.InputAction action_unselectSlot;
  public static UnityEngine.InputSystem.InputAction action_selectBackpack;
  public static UnityEngine.InputSystem.InputAction[] hotbarActions = new UnityEngine.InputSystem.InputAction[9];
  public static UnityEngine.InputSystem.InputAction push_to_talk;
  public Vector2 movementInput;
  public Vector2 lookInput;
  public float scrollInput;
  public bool crouchIsPressed;
  public bool crouchWasPressed;
  public bool crouchToggleWasPressed;
  public bool sprintIsPressed;
  public bool sprintToggleIsPressed;
  public bool sprintWasPressed;
  public bool sprintToggleWasPressed;
  public bool pauseWasPressed;
  public bool jumpWasPressed;
  public bool jumpIsPressed;
  public bool interactWasPressed;
  public bool interactIsPressed;
  public bool interactWasReleased;
  public bool dropWasPressed;
  public bool dropIsPressed;
  public bool dropWasReleased;
  public bool usePrimaryWasPressed;
  public bool usePrimaryIsPressed;
  public bool usePrimaryWasReleased;
  public bool useSecondaryWasPressed;
  public bool useSecondaryIsPressed;
  public bool useSecondaryWasReleased;
  public bool pingWasPressed;
  public bool selectSlotForwardWasPressed;
  public bool selectSlotBackwardWasPressed;
  public bool unselectSlotWasPressed;
  public bool selectBackpackWasPressed;
  public bool scrollBackwardWasPressed;
  public bool scrollForwardWasPressed;
  public bool scrollBackwardIsPressed;
  public bool scrollForwardIsPressed;
  public bool emoteIsPressed;
  public bool spectateLeftWasPressed;
  public bool spectateRightWasPressed;
  public bool pushToTalkPressed;

  public void Init()
  {
    Rebinding.LoadRebindingsFromFile();
    CharacterInput.action_pause = UnityEngine.InputSystem.InputSystem.actions.FindAction("Pause", false);
    CharacterInput.action_move = UnityEngine.InputSystem.InputSystem.actions.FindAction("Move", false);
    CharacterInput.action_moveForward = UnityEngine.InputSystem.InputSystem.actions.FindAction("MoveForward", false);
    CharacterInput.action_moveBackward = UnityEngine.InputSystem.InputSystem.actions.FindAction("MoveBackward", false);
    CharacterInput.action_moveLeft = UnityEngine.InputSystem.InputSystem.actions.FindAction("MoveLeft", false);
    CharacterInput.action_moveRight = UnityEngine.InputSystem.InputSystem.actions.FindAction("MoveRight", false);
    CharacterInput.action_look = UnityEngine.InputSystem.InputSystem.actions.FindAction("Look", false);
    CharacterInput.action_jump = UnityEngine.InputSystem.InputSystem.actions.FindAction("Jump", false);
    CharacterInput.action_sprint = UnityEngine.InputSystem.InputSystem.actions.FindAction("Sprint", false);
    CharacterInput.action_sprintToggle = UnityEngine.InputSystem.InputSystem.actions.FindAction("SprintToggle", false);
    CharacterInput.action_interact = UnityEngine.InputSystem.InputSystem.actions.FindAction("Interact", false);
    CharacterInput.action_drop = UnityEngine.InputSystem.InputSystem.actions.FindAction("Drop", false);
    CharacterInput.action_crouch = UnityEngine.InputSystem.InputSystem.actions.FindAction("Crouch", false);
    CharacterInput.action_crouchToggle = UnityEngine.InputSystem.InputSystem.actions.FindAction("CrouchToggle", false);
    CharacterInput.action_usePrimary = UnityEngine.InputSystem.InputSystem.actions.FindAction("UsePrimary", false);
    CharacterInput.action_useSecondary = UnityEngine.InputSystem.InputSystem.actions.FindAction("UseSecondary", false);
    CharacterInput.action_scroll = UnityEngine.InputSystem.InputSystem.actions.FindAction("Scroll", false);
    CharacterInput.push_to_talk = UnityEngine.InputSystem.InputSystem.actions.FindAction("PushToTalk", false);
    CharacterInput.action_emote = UnityEngine.InputSystem.InputSystem.actions.FindAction("Emote", false);
    CharacterInput.action_ping = UnityEngine.InputSystem.InputSystem.actions.FindAction("Ping", false);
    for (int index = 0; index < CharacterInput.hotbarActions.Length; ++index)
      CharacterInput.hotbarActions[index] = UnityEngine.InputSystem.InputSystem.actions.FindAction($"Hotbar{index + 1}", false);
    CharacterInput.action_selectSlotForward = UnityEngine.InputSystem.InputSystem.actions.FindAction("SelectSlotForward", false);
    CharacterInput.action_selectSlotBackward = UnityEngine.InputSystem.InputSystem.actions.FindAction("SelectSlotBackward", false);
    CharacterInput.action_unselectSlot = UnityEngine.InputSystem.InputSystem.actions.FindAction("UnselectSlot", false);
    CharacterInput.action_selectBackpack = UnityEngine.InputSystem.InputSystem.actions.FindAction("SelectBackpack", false);
    CharacterInput.action_scrollBackward = UnityEngine.InputSystem.InputSystem.actions.FindAction("ScrollBackward", false);
    CharacterInput.action_scrollForward = UnityEngine.InputSystem.InputSystem.actions.FindAction("ScrollForward", false);
  }

  public bool SelectSlotWasPressed(int key) => this.HotbarKeyWasPressed(key);

  public bool SelectSlotIsPressed(int key) => this.HotbarKeyIsPressed(key);

  public bool HotbarKeyWasPressed(int key)
  {
    return key >= 0 && key < CharacterInput.hotbarActions.Length && CharacterInput.hotbarActions[key].WasPressedThisFrame();
  }

  public bool HotbarKeyIsPressed(int key)
  {
    return key >= 0 && key < CharacterInput.hotbarActions.Length && CharacterInput.hotbarActions[key].IsPressed();
  }

  internal void Sample(bool playerMovementActive)
  {
    this.ResetInput();
    this.pauseWasPressed = CharacterInput.action_pause.WasPressedThisFrame();
    this.interactWasPressed = CharacterInput.action_interact.WasPressedThisFrame();
    this.interactIsPressed = CharacterInput.action_interact.IsPressed();
    this.interactWasReleased = CharacterInput.action_interact.WasReleasedThisFrame();
    this.emoteIsPressed = CharacterInput.action_emote.IsPressed();
    if (playerMovementActive)
    {
      this.movementInput = this.GetMovementInput();
      this.sprintWasPressed = CharacterInput.action_sprint.WasPressedThisFrame();
      this.sprintIsPressed = CharacterInput.action_sprint.IsPressed();
      this.sprintToggleIsPressed = CharacterInput.action_sprintToggle.IsPressed();
      this.sprintToggleWasPressed = CharacterInput.action_sprintToggle.WasPressedThisFrame();
      this.jumpWasPressed = CharacterInput.action_jump.WasPressedThisFrame();
      this.jumpIsPressed = CharacterInput.action_jump.IsPressed();
      this.dropWasPressed = CharacterInput.action_drop.WasPressedThisFrame();
      this.dropIsPressed = CharacterInput.action_drop.IsPressed();
      this.dropWasReleased = CharacterInput.action_drop.WasReleasedThisFrame();
      this.lookInput = CharacterInput.action_look.ReadValue<Vector2>();
      this.scrollBackwardWasPressed = CharacterInput.action_scrollBackward.WasPressedThisFrame();
      this.scrollForwardWasPressed = CharacterInput.action_scrollForward.WasPressedThisFrame();
      this.scrollBackwardIsPressed = CharacterInput.action_scrollBackward.IsPressed();
      this.scrollForwardIsPressed = CharacterInput.action_scrollForward.IsPressed();
      this.scrollInput = CharacterInput.action_scroll.ReadValue<float>();
      this.usePrimaryWasPressed = CharacterInput.action_usePrimary.WasPressedThisFrame();
      this.usePrimaryIsPressed = CharacterInput.action_usePrimary.IsPressed();
      this.usePrimaryWasReleased = CharacterInput.action_usePrimary.WasReleasedThisFrame();
      this.useSecondaryWasPressed = CharacterInput.action_useSecondary.WasPressedThisFrame();
      this.useSecondaryIsPressed = CharacterInput.action_useSecondary.IsPressed();
      this.useSecondaryWasReleased = CharacterInput.action_useSecondary.WasReleasedThisFrame();
      this.crouchWasPressed = CharacterInput.action_crouch.WasPressedThisFrame();
      this.crouchIsPressed = CharacterInput.action_crouch.IsPressed();
      this.crouchToggleWasPressed = CharacterInput.action_crouchToggle.WasPressedThisFrame();
      if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.KeyboardMouse)
      {
        this.spectateLeftWasPressed = this.HotbarKeyWasPressed(0);
        this.spectateRightWasPressed = this.HotbarKeyWasPressed(1);
      }
      else
      {
        this.spectateLeftWasPressed = CharacterInput.action_selectSlotBackward.WasPressedThisFrame();
        this.spectateRightWasPressed = CharacterInput.action_selectSlotForward.WasPressedThisFrame();
      }
      this.selectBackpackWasPressed = CharacterInput.action_selectBackpack.WasPerformedThisFrame();
      this.pingWasPressed = CharacterInput.action_ping.WasPressedThisFrame();
      this.pushToTalkPressed = CharacterInput.push_to_talk.IsPressed();
      this.unselectSlotWasPressed = CharacterInput.action_unselectSlot.WasPressedThisFrame();
    }
    this.selectSlotForwardWasPressed = CharacterInput.action_selectSlotForward.WasPressedThisFrame();
    this.selectSlotBackwardWasPressed = CharacterInput.action_selectSlotBackward.WasPressedThisFrame();
    this.unselectSlotWasPressed = CharacterInput.action_unselectSlot.WasPressedThisFrame();
  }

  private Vector2 GetMovementInput()
  {
    if (InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad)
      return CharacterInput.action_move.ReadValue<Vector2>();
    Vector2 zero = Vector2.zero;
    if (CharacterInput.action_moveForward.IsPressed())
      zero += Vector2.up;
    if (CharacterInput.action_moveBackward.IsPressed())
      zero -= Vector2.up;
    if (CharacterInput.action_moveRight.IsPressed())
      zero += Vector2.right;
    if (CharacterInput.action_moveLeft.IsPressed())
      zero -= Vector2.right;
    return zero.normalized;
  }

  internal void SampleAlways()
  {
  }

  internal void ResetInput()
  {
    this.lookInput = Vector2.zero;
    this.movementInput = Vector2.zero;
    this.sprintIsPressed = false;
    this.jumpWasPressed = false;
    this.jumpIsPressed = false;
    this.useSecondaryIsPressed = false;
    this.useSecondaryWasPressed = false;
    this.useSecondaryWasReleased = false;
    this.usePrimaryWasPressed = false;
    this.usePrimaryIsPressed = false;
    this.usePrimaryWasReleased = false;
    this.interactWasPressed = false;
    this.interactIsPressed = false;
    this.interactWasReleased = false;
    this.dropWasPressed = false;
    this.dropIsPressed = false;
    this.dropWasReleased = false;
    this.sprintWasPressed = false;
    this.sprintToggleWasPressed = false;
    this.crouchWasPressed = false;
    this.crouchToggleWasPressed = false;
    this.crouchIsPressed = false;
    this.emoteIsPressed = false;
  }
}
