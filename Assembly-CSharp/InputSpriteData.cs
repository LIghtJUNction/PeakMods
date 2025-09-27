// Decompiled with JetBrains decompiler
// Type: InputSpriteData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.ControllerSupport;
using Zorro.Core;

#nullable disable
[CreateAssetMenu(fileName = "InputSpriteData", menuName = "Scouts/Input Sprite Data")]
public class InputSpriteData : SingletonAsset<InputSpriteData>
{
  public TMP_SpriteAsset keyboardSprites;
  public TMP_SpriteAsset xboxSprites;
  public TMP_SpriteAsset switchSprites;
  public TMP_SpriteAsset ps5Sprites;
  public TMP_SpriteAsset ps4Sprites;
  public static Dictionary<InputSpriteData.InputAction, string> ActionToHardcodedSpriteKeyboard = new Dictionary<InputSpriteData.InputAction, string>()
  {
    {
      InputSpriteData.InputAction.Aim,
      "<sprite=108 tint=1>"
    },
    {
      InputSpriteData.InputAction.Move,
      "<sprite=115 tint=1>"
    },
    {
      InputSpriteData.InputAction.Scroll,
      "<sprite=112 tint=1>"
    }
  };
  public static Dictionary<InputSpriteData.InputAction, string> ActionToBackendNameKeyboard = new Dictionary<InputSpriteData.InputAction, string>()
  {
    {
      InputSpriteData.InputAction.Interact,
      "Interact"
    },
    {
      InputSpriteData.InputAction.HoldInteract,
      "Interact"
    },
    {
      InputSpriteData.InputAction.UsePrimary,
      "UsePrimary"
    },
    {
      InputSpriteData.InputAction.UseSecondary,
      "UseSecondary"
    },
    {
      InputSpriteData.InputAction.Scroll,
      "Scroll"
    },
    {
      InputSpriteData.InputAction.Throw,
      "Drop"
    },
    {
      InputSpriteData.InputAction.Drop,
      "Drop"
    },
    {
      InputSpriteData.InputAction.Slot1,
      "Hotbar1"
    },
    {
      InputSpriteData.InputAction.Slot2,
      "Hotbar2"
    },
    {
      InputSpriteData.InputAction.Slot3,
      "Hotbar3"
    },
    {
      InputSpriteData.InputAction.Slot4,
      "Hotbar4"
    },
    {
      InputSpriteData.InputAction.SpectateLeft,
      "Hotbar1"
    },
    {
      InputSpriteData.InputAction.SpectateRight,
      "Hotbar2"
    },
    {
      InputSpriteData.InputAction.Move,
      "Move"
    },
    {
      InputSpriteData.InputAction.Aim,
      "Aim"
    },
    {
      InputSpriteData.InputAction.Sprint,
      "Sprint"
    },
    {
      InputSpriteData.InputAction.Jump,
      "Jump"
    },
    {
      InputSpriteData.InputAction.Crouch,
      "Crouch"
    },
    {
      InputSpriteData.InputAction.Ping,
      "Ping"
    },
    {
      InputSpriteData.InputAction.SlotLeft,
      "SelectSlotBackward"
    },
    {
      InputSpriteData.InputAction.SlotRight,
      "SelectSlotForward"
    },
    {
      InputSpriteData.InputAction.DeselectSlot,
      "UnselectSlot"
    },
    {
      InputSpriteData.InputAction.Emote,
      "Emote"
    },
    {
      InputSpriteData.InputAction.PushToTalk,
      "PushToTalk"
    },
    {
      InputSpriteData.InputAction.TabLeft,
      "TabLeft"
    },
    {
      InputSpriteData.InputAction.TabRight,
      "TabRight"
    },
    {
      InputSpriteData.InputAction.MoveForward,
      "MoveForward"
    },
    {
      InputSpriteData.InputAction.MoveBackward,
      "MoveBackward"
    },
    {
      InputSpriteData.InputAction.MoveLeft,
      "MoveLeft"
    },
    {
      InputSpriteData.InputAction.MoveRight,
      "MoveRight"
    },
    {
      InputSpriteData.InputAction.ScrollForward,
      "ScrollForward"
    },
    {
      InputSpriteData.InputAction.ScrollBackward,
      "ScrollBackward"
    },
    {
      InputSpriteData.InputAction.Pause,
      "Pause"
    }
  };
  public static Dictionary<InputSpriteData.InputAction, string> ActionToBackendNameGamepad = new Dictionary<InputSpriteData.InputAction, string>()
  {
    {
      InputSpriteData.InputAction.Interact,
      "Interact"
    },
    {
      InputSpriteData.InputAction.HoldInteract,
      "Interact"
    },
    {
      InputSpriteData.InputAction.UsePrimary,
      "UsePrimary"
    },
    {
      InputSpriteData.InputAction.UseSecondary,
      "UseSecondary"
    },
    {
      InputSpriteData.InputAction.Scroll,
      "Scroll"
    },
    {
      InputSpriteData.InputAction.Throw,
      "Drop"
    },
    {
      InputSpriteData.InputAction.Drop,
      "Drop"
    },
    {
      InputSpriteData.InputAction.Slot1,
      "Hotbar1"
    },
    {
      InputSpriteData.InputAction.Slot2,
      "Hotbar2"
    },
    {
      InputSpriteData.InputAction.Slot3,
      "Hotbar3"
    },
    {
      InputSpriteData.InputAction.Slot4,
      "Hotbar4"
    },
    {
      InputSpriteData.InputAction.SpectateLeft,
      "SelectSlotBackward"
    },
    {
      InputSpriteData.InputAction.SpectateRight,
      "SelectSlotForward"
    },
    {
      InputSpriteData.InputAction.Move,
      "Move"
    },
    {
      InputSpriteData.InputAction.Aim,
      "Aim"
    },
    {
      InputSpriteData.InputAction.Sprint,
      "SprintToggle"
    },
    {
      InputSpriteData.InputAction.Jump,
      "Jump"
    },
    {
      InputSpriteData.InputAction.Crouch,
      "CrouchToggle"
    },
    {
      InputSpriteData.InputAction.Ping,
      "Ping"
    },
    {
      InputSpriteData.InputAction.SlotLeft,
      "SelectSlotBackward"
    },
    {
      InputSpriteData.InputAction.SlotRight,
      "SelectSlotForward"
    },
    {
      InputSpriteData.InputAction.DeselectSlot,
      "UnselectSlot"
    },
    {
      InputSpriteData.InputAction.Emote,
      "Emote"
    },
    {
      InputSpriteData.InputAction.PushToTalk,
      "PushToTalk"
    },
    {
      InputSpriteData.InputAction.TabLeft,
      "TabLeft"
    },
    {
      InputSpriteData.InputAction.TabRight,
      "TabRight"
    },
    {
      InputSpriteData.InputAction.MoveForward,
      "MoveForward"
    },
    {
      InputSpriteData.InputAction.MoveBackward,
      "MoveBackward"
    },
    {
      InputSpriteData.InputAction.MoveLeft,
      "MoveLeft"
    },
    {
      InputSpriteData.InputAction.MoveRight,
      "MoveRight"
    },
    {
      InputSpriteData.InputAction.ScrollForward,
      "ScrollForward"
    },
    {
      InputSpriteData.InputAction.ScrollBackward,
      "ScrollBackward"
    },
    {
      InputSpriteData.InputAction.Pause,
      "Pause"
    }
  };
  private Dictionary<string, string> inputPathToSpriteTagGamepad = new Dictionary<string, string>()
  {
    {
      "leftShoulder",
      "<sprite=4 tint=1>"
    },
    {
      "rightShoulder",
      "<sprite=5 tint=1>"
    },
    {
      "leftTrigger",
      "<sprite=6 tint=1>"
    },
    {
      "rightTrigger",
      "<sprite=7 tint=1>"
    },
    {
      "buttonNorth",
      "<sprite=3 tint=1>"
    },
    {
      "buttonSouth",
      "<sprite=0 tint=1>"
    },
    {
      "buttonWest",
      "<sprite=2 tint=1>"
    },
    {
      "buttonEast",
      "<sprite=1 tint=1>"
    },
    {
      "up",
      "<sprite=12 tint=1>"
    },
    {
      "down",
      "<sprite=13 tint=1>"
    },
    {
      "left",
      "<sprite=14 tint=1>"
    },
    {
      "right",
      "<sprite=15 tint=1>"
    },
    {
      "start",
      "<sprite=8 tint=1>"
    },
    {
      "select",
      "<sprite=9 tint=1>"
    },
    {
      "leftStickPress",
      "<sprite=10 tint=1>"
    },
    {
      "rightStickPress",
      "<sprite=11 tint=1>"
    }
  };
  private Dictionary<string, string> inputPathToSpriteTagKeyboard = new Dictionary<string, string>()
  {
    {
      "0",
      "<sprite=0 tint=1>"
    },
    {
      "1",
      "<sprite=1 tint=1>"
    },
    {
      "2",
      "<sprite=2 tint=1>"
    },
    {
      "3",
      "<sprite=3 tint=1>"
    },
    {
      "4",
      "<sprite=4 tint=1>"
    },
    {
      "5",
      "<sprite=5 tint=1>"
    },
    {
      "6",
      "<sprite=6 tint=1>"
    },
    {
      "7",
      "<sprite=7 tint=1>"
    },
    {
      "8",
      "<sprite=8 tint=1>"
    },
    {
      "9",
      "<sprite=9 tint=1>"
    },
    {
      "a",
      "<sprite=10 tint=1>"
    },
    {
      "b",
      "<sprite=11 tint=1>"
    },
    {
      "c",
      "<sprite=12 tint=1>"
    },
    {
      "d",
      "<sprite=13 tint=1>"
    },
    {
      "e",
      "<sprite=14 tint=1>"
    },
    {
      "f",
      "<sprite=15 tint=1>"
    },
    {
      "g",
      "<sprite=16 tint=1>"
    },
    {
      "h",
      "<sprite=17 tint=1>"
    },
    {
      "i",
      "<sprite=18 tint=1>"
    },
    {
      "j",
      "<sprite=19 tint=1>"
    },
    {
      "k",
      "<sprite=20 tint=1>"
    },
    {
      "l",
      "<sprite=21 tint=1>"
    },
    {
      "m",
      "<sprite=22 tint=1>"
    },
    {
      "n",
      "<sprite=23 tint=1>"
    },
    {
      "o",
      "<sprite=24 tint=1>"
    },
    {
      "p",
      "<sprite=25 tint=1>"
    },
    {
      "q",
      "<sprite=26 tint=1>"
    },
    {
      "r",
      "<sprite=27 tint=1>"
    },
    {
      "s",
      "<sprite=28 tint=1>"
    },
    {
      "t",
      "<sprite=29 tint=1>"
    },
    {
      "u",
      "<sprite=30 tint=1>"
    },
    {
      "v",
      "<sprite=31 tint=1>"
    },
    {
      "w",
      "<sprite=32 tint=1>"
    },
    {
      "x",
      "<sprite=33 tint=1>"
    },
    {
      "y",
      "<sprite=34 tint=1>"
    },
    {
      "z",
      "<sprite=35 tint=1>"
    },
    {
      "f1",
      "<sprite=36 tint=1>"
    },
    {
      "f2",
      "<sprite=37 tint=1>"
    },
    {
      "f3",
      "<sprite=38 tint=1>"
    },
    {
      "f4",
      "<sprite=39 tint=1>"
    },
    {
      "f5",
      "<sprite=40 tint=1>"
    },
    {
      "f6",
      "<sprite=41 tint=1>"
    },
    {
      "f7",
      "<sprite=42 tint=1>"
    },
    {
      "f8",
      "<sprite=43 tint=1>"
    },
    {
      "f9",
      "<sprite=44 tint=1>"
    },
    {
      "f10",
      "<sprite=45 tint=1>"
    },
    {
      "f11",
      "<sprite=46 tint=1>"
    },
    {
      "f12",
      "<sprite=47 tint=1>"
    },
    {
      "minus",
      "<sprite=78 tint=1>"
    },
    {
      "equals",
      "<sprite=80 tint=1>"
    },
    {
      "leftBracket",
      "<sprite=82 tint=1>"
    },
    {
      "rightBracket",
      "<sprite=83 tint=1>"
    },
    {
      "backquote",
      "<sprite=81 tint=1>"
    },
    {
      "tab",
      "<sprite=53 tint=1>"
    },
    {
      "leftShift",
      "<sprite=51 tint=1>"
    },
    {
      "rightShift",
      "<sprite=51 tint=1>"
    },
    {
      "shift",
      "<sprite=51 tint=1>"
    },
    {
      "leftCtrl",
      "<sprite=49 tint=1>"
    },
    {
      "rightCtrl",
      "<sprite=49 tint=1>"
    },
    {
      "ctrl",
      "<sprite=49 tint=1>"
    },
    {
      "leftAlt",
      "<sprite=50 tint=1>"
    },
    {
      "rightAlt",
      "<sprite=50 tint=1>"
    },
    {
      "alt",
      "<sprite=50 tint=1>"
    },
    {
      "space",
      "<sprite=69 tint=1>"
    },
    {
      "semicolon",
      "<sprite=85 tint=1>"
    },
    {
      "quote",
      "<sprite=100 tint=1>"
    },
    {
      "comma",
      "<sprite=87 tint=1>"
    },
    {
      "period",
      "<sprite=88 tint=1>"
    },
    {
      "slash",
      "<sprite=76 tint=1>"
    },
    {
      "backslash",
      "<sprite=84 tint=1>"
    },
    {
      "insert",
      "<sprite=70 tint=1>"
    },
    {
      "delete",
      "<sprite=71 tint=1>"
    },
    {
      "home",
      "<sprite=72 tint=1>"
    },
    {
      "end",
      "<sprite=73 tint=1>"
    },
    {
      "pageUp",
      "<sprite=74 tint=1>"
    },
    {
      "pageDown",
      "<sprite=75 tint=1>"
    },
    {
      "upArrow",
      "<sprite=56 tint=1>"
    },
    {
      "downArrow",
      "<sprite=58 tint=1>"
    },
    {
      "leftArrow",
      "<sprite=59 tint=1>"
    },
    {
      "rightArrow",
      "<sprite=57 tint=1>"
    },
    {
      "numpad0",
      "<sprite=127 tint=1>"
    },
    {
      "numpad1",
      "<sprite=128 tint=1>"
    },
    {
      "numpad2",
      "<sprite=129 tint=1>"
    },
    {
      "numpad3",
      "<sprite=130 tint=1>"
    },
    {
      "numpad4",
      "<sprite=131 tint=1>"
    },
    {
      "numpad5",
      "<sprite=132 tint=1>"
    },
    {
      "numpad6",
      "<sprite=133 tint=1>"
    },
    {
      "numpad7",
      "<sprite=134 tint=1>"
    },
    {
      "numpad8",
      "<sprite=135 tint=1>"
    },
    {
      "numpad9",
      "<sprite=136 tint=1>"
    },
    {
      "numpadPlus",
      "<sprite=119 tint=1>"
    },
    {
      "numpadMinus",
      "<sprite=118 tint=1>"
    },
    {
      "numpadDivide",
      "<sprite=120 tint=1>"
    },
    {
      "numpadMultiply",
      "<sprite=121 tint=1>"
    },
    {
      "numpadEnter",
      "<sprite=122 tint=1>"
    },
    {
      "numpadPeriod",
      "<sprite=123 tint=1>"
    },
    {
      "capsLock",
      "<sprite=52 tint=1>"
    },
    {
      "backspace",
      "<sprite=67 tint=1>"
    },
    {
      "enter",
      "<sprite=68 tint=1>"
    },
    {
      "esc",
      "<sprite=54 tint=1>"
    }
  };
  private Dictionary<string, string> inputPathToSpriteTagMouse = new Dictionary<string, string>()
  {
    {
      "down",
      "<sprite=112 tint=1>"
    },
    {
      "up",
      "<sprite=112 tint=1>"
    },
    {
      "scroll",
      "<sprite=112 tint=1>"
    },
    {
      "leftButton",
      "<sprite=109 tint=1>"
    },
    {
      "rightButton",
      "<sprite=110 tint=1>"
    },
    {
      "middleButton",
      "<sprite=111 tint=1>"
    }
  };

  public string GetSpriteTag(InputSpriteData.InputAction action, InputScheme scheme)
  {
    switch (scheme)
    {
      case InputScheme.KeyboardMouse:
        if (InputSpriteData.ActionToHardcodedSpriteKeyboard.ContainsKey(action))
          return InputSpriteData.ActionToHardcodedSpriteKeyboard[action];
        if (InputSpriteData.ActionToBackendNameKeyboard.ContainsKey(action))
          return this.GetSpriteTagFromInputPathKeyboard(InputSpriteData.GetBindingPath(InputSpriteData.ActionToBackendNameKeyboard[action], scheme, out bool _));
        Debug.Log((object) $"Failed to find backend name for {action}");
        break;
      case InputScheme.Gamepad:
        if (action == InputSpriteData.InputAction.Aim)
          return "<sprite=17 tint=1>";
        if (action == InputSpriteData.InputAction.Move)
          return "<sprite=16 tint=1>";
        if (InputSpriteData.ActionToBackendNameGamepad.ContainsKey(action))
          return this.GetSpriteTagFromInputPathGamepad(InputSpriteData.GetBindingPath(InputSpriteData.ActionToBackendNameGamepad[action], scheme, out bool _));
        Debug.Log((object) $"Failed to find backend name for {action}");
        break;
    }
    return "";
  }

  public static string GetBindingPath(string actionName, InputScheme scheme, out bool hasOverride)
  {
    hasOverride = false;
    UnityEngine.InputSystem.InputAction action = UnityEngine.InputSystem.InputSystem.actions.FindAction(actionName, false);
    if (action != null)
    {
      foreach (InputBinding binding in action.bindings)
      {
        if (scheme == InputScheme.KeyboardMouse && (binding.effectivePath.Contains("<Keyboard>") || binding.effectivePath.Contains("<Mouse>")))
        {
          hasOverride = !string.IsNullOrEmpty(binding.overridePath);
          return binding.effectivePath;
        }
        if ((scheme == InputScheme.Gamepad || scheme == InputScheme.Unknown) && binding.effectivePath.Contains("<Gamepad>"))
        {
          hasOverride = !string.IsNullOrEmpty(binding.overridePath);
          return binding.effectivePath;
        }
      }
    }
    return "";
  }

  public static string GetPathEnd(string inputPath)
  {
    if (string.IsNullOrEmpty(inputPath))
      return "";
    string[] strArray = inputPath.Split("/", StringSplitOptions.None);
    return strArray[strArray.Length - 1];
  }

  public string GetSpriteTagFromInputPathGamepad(string inputPath)
  {
    if (string.IsNullOrEmpty(inputPath))
      return "";
    string[] strArray = inputPath.Split("/", StringSplitOptions.None);
    string str;
    return this.inputPathToSpriteTagGamepad.TryGetValue(strArray[strArray.Length - 1], out str) ? str : (string) null;
  }

  public string GetSpriteTagFromInputPathKeyboard(string inputPath)
  {
    if (string.IsNullOrEmpty(inputPath))
      return "<sprite=124 tint=1>";
    string[] strArray = inputPath.Split("/", StringSplitOptions.None);
    string key = strArray[strArray.Length - 1];
    string inputPathKeyboard;
    if (strArray[0] == "<Mouse>" && this.inputPathToSpriteTagMouse.TryGetValue(key, out inputPathKeyboard))
      return inputPathKeyboard;
    string str;
    return this.inputPathToSpriteTagKeyboard.TryGetValue(key, out str) ? str : "<sprite=124 tint=1>";
  }

  public enum InputAction
  {
    Interact,
    HoldInteract,
    UsePrimary,
    UseSecondary,
    Scroll,
    Throw,
    Drop,
    Slot1,
    Slot2,
    Slot3,
    Slot4,
    SpectateLeft,
    SpectateRight,
    Move,
    Aim,
    Sprint,
    Jump,
    Crouch,
    Ping,
    SlotLeft,
    SlotRight,
    DeselectSlot,
    Emote,
    PushToTalk,
    TabLeft,
    TabRight,
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    ScrollForward,
    ScrollBackward,
    Pause,
  }
}
