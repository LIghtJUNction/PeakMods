// Decompiled with JetBrains decompiler
// Type: Transition_CanvasGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class Transition_CanvasGroup : Transition
{
  private CanvasGroup gr;
  public float inSpeed = 1f;
  public AnimationCurve inCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  public float outSpeed = 1f;
  public AnimationCurve outCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);

  private void Awake() => this.gr = this.GetComponent<CanvasGroup>();

  public override IEnumerator TransitionIn(float speed = 1f)
  {
    float c = 0.0f;
    float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
    while ((double) c < (double) t)
    {
      c += Time.unscaledDeltaTime * speed * this.inSpeed;
      this.gr.alpha = this.inCurve.Evaluate(c);
      yield return (object) null;
    }
  }

  public override IEnumerator TransitionOut(float speed = 1f)
  {
    float c = 0.0f;
    float t = this.outCurve.keys[this.outCurve.keys.Length - 1].time;
    while ((double) c < (double) t)
    {
      c += Time.unscaledDeltaTime * speed * this.outSpeed;
      this.gr.alpha = this.outCurve.Evaluate(c);
      yield return (object) null;
    }
  }
}
