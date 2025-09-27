// Decompiled with JetBrains decompiler
// Type: UIWheel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;

#nullable disable
public class UIWheel : MonoBehaviour
{
  public float maxCursorDistance;

  protected virtual Vector2 GetCursorOrigin()
  {
    return new Vector2(this.transform.position.x, this.transform.position.y);
  }

  protected virtual void Update()
  {
    if (InputHandler.GetCurrentUsedInputScheme() != InputScheme.Gamepad)
      return;
    this.TestGamepadInput();
  }

  protected void TestGamepadInput()
  {
    this.TestSelectSliceGamepad(Singleton<UIInputHandler>.Instance.wheelNavigationVector);
  }

  protected virtual void TestSelectSliceGamepad(Vector2 gamepadVector)
  {
  }
}
