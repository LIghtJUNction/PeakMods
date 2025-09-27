// Decompiled with JetBrains decompiler
// Type: ControllerManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine.InputSystem;

#nullable disable
public class ControllerManager
{
  public bool gamepadAttached;

  public void Init()
  {
    UnityEngine.InputSystem.InputSystem.onDeviceChange += new Action<InputDevice, InputDeviceChange>(this.OnDeviceChange);
    this.UpdateGamepadUsage();
  }

  public void Destroy()
  {
    UnityEngine.InputSystem.InputSystem.onDeviceChange -= new Action<InputDevice, InputDeviceChange>(this.OnDeviceChange);
  }

  private void OnDeviceChange(InputDevice device, InputDeviceChange change)
  {
    this.UpdateGamepadUsage();
  }

  private void UpdateGamepadUsage()
  {
    foreach (InputDevice device in UnityEngine.InputSystem.InputSystem.devices)
    {
      if (device is Gamepad)
      {
        this.gamepadAttached = true;
        return;
      }
    }
    this.gamepadAttached = false;
  }
}
