// Decompiled with JetBrains decompiler
// Type: FakeCursor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.InputSystem;

#nullable disable
public class FakeCursor : MonoBehaviour
{
  public Transform target;

  private void Update()
  {
    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(this.target.parent as RectTransform, Mouse.current.position.ReadValue(), (Camera) null, out localPoint);
    this.target.localPosition = (Vector3) localPoint;
  }
}
