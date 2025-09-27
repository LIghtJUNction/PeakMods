// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CrossPlatformInput.InputAxisScrollbar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.CrossPlatformInput;

public class InputAxisScrollbar : MonoBehaviour
{
  public string axis;

  private void Update()
  {
  }

  public void HandleInput(float value)
  {
    CrossPlatformInputManager.SetAxis(this.axis, (float) ((double) value * 2.0 - 1.0));
  }
}
