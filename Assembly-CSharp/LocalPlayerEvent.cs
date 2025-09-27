// Decompiled with JetBrains decompiler
// Type: LocalPlayerEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
public class LocalPlayerEvent : MonoBehaviour
{
  public UnityEvent isLocalEvent;

  public void Start()
  {
    if (!this.GetComponentInParent<Character>().IsLocal)
      return;
    this.isLocalEvent.Invoke();
  }
}
