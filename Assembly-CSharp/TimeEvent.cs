// Decompiled with JetBrains decompiler
// Type: TimeEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
public class TimeEvent : MonoBehaviour
{
  private float counter;
  public float rate = 2f;
  public bool repeating;
  public UnityEvent timeEvent;

  private void Update()
  {
    this.counter += Time.deltaTime;
    if ((double) this.counter <= (double) this.rate)
      return;
    if (!this.repeating)
      this.enabled = false;
    this.timeEvent.Invoke();
    this.counter = 0.0f;
  }

  private void OnEnable() => this.counter = 0.0f;
}
