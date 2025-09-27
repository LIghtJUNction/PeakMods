// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CrossPlatformInput.PlatformSpecific.StandaloneInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

public class StandaloneInput : VirtualInput
{
  public override float GetAxis(string name, bool raw)
  {
    return !raw ? Input.GetAxis(name) : Input.GetAxisRaw(name);
  }

  public override bool GetButton(string name) => Input.GetButton(name);

  public override bool GetButtonDown(string name) => Input.GetButtonDown(name);

  public override bool GetButtonUp(string name) => Input.GetButtonUp(name);

  public override void SetButtonDown(string name)
  {
    throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
  }

  public override void SetButtonUp(string name)
  {
    throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
  }

  public override void SetAxisPositive(string name)
  {
    throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
  }

  public override void SetAxisNegative(string name)
  {
    throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
  }

  public override void SetAxisZero(string name)
  {
    throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
  }

  public override void SetAxis(string name, float value)
  {
    throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
  }

  public override Vector3 MousePosition() => Input.mousePosition;
}
