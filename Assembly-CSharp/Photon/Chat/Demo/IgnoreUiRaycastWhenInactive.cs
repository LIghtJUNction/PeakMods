// Decompiled with JetBrains decompiler
// Type: Photon.Chat.Demo.IgnoreUiRaycastWhenInactive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Photon.Chat.Demo;

public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
{
  public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
  {
    return this.gameObject.activeInHierarchy;
  }
}
