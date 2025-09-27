// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.CrossPlatformInput.MobileControlRig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace UnityStandardAssets.CrossPlatformInput;

[ExecuteInEditMode]
public class MobileControlRig : MonoBehaviour
{
  private void OnEnable() => this.CheckEnableControlRig();

  private void Start()
  {
    if (!((UnityEngine.Object) UnityEngine.Object.FindObjectOfType<EventSystem>() == (UnityEngine.Object) null))
      return;
    GameObject gameObject = new GameObject("EventSystem");
    gameObject.AddComponent<EventSystem>();
    gameObject.AddComponent<StandaloneInputModule>();
  }

  private void CheckEnableControlRig() => this.EnableControlRig(false);

  private void EnableControlRig(bool enabled)
  {
    try
    {
      foreach (Component component in this.transform)
        component.gameObject.SetActive(enabled);
    }
    catch (Exception ex)
    {
    }
  }
}
