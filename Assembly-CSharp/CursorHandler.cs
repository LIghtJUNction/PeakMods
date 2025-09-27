// Decompiled with JetBrains decompiler
// Type: CursorHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
public class CursorHandler : Singleton<CursorHandler>
{
  public bool isMenuScene;

  private void Update()
  {
    if ((InputHandler.GetCurrentUsedInputScheme() != InputScheme.KeyboardMouse ? 0 : (this.isMenuScene || DebugUIHandler.IsOpen || (Object) GUIManager.instance != (Object) null && (GUIManager.instance.windowShowingCursor || GUIManager.instance.wheelActive) ? 1 : (Zorro.UI.Modal.Modal.IsOpen ? 1 : 0))) == 0)
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }
    else
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
  }
}
