// Decompiled with JetBrains decompiler
// Type: CursorAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CursorAnimation : MonoBehaviour
{
  public Texture2D cursorOpen;
  public Texture2D curserClosed;
  private Vector2 cursorHotspot = new Vector2(32f, 32f);

  private void Start() => Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, CursorMode.Auto);

  private void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Cursor.SetCursor(this.curserClosed, this.cursorHotspot, CursorMode.Auto);
    }
    else
    {
      if (!Input.GetMouseButtonUp(0))
        return;
      Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, CursorMode.Auto);
    }
  }
}
