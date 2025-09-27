// Decompiled with JetBrains decompiler
// Type: UIWheelSlice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class UIWheelSlice : MonoBehaviour
{
  public Button button;
  private float offsetRotation = 22.5f;

  public Vector3 GetUpVector()
  {
    return Quaternion.Euler(0.0f, 0.0f, this.offsetRotation) * this.transform.up;
  }
}
