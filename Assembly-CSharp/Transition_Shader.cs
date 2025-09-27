// Decompiled with JetBrains decompiler
// Type: Transition_Shader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class Transition_Shader : Transition
{
  private MeshRenderer rend;
  private Material mat;
  public float inSpeed = 1f;
  public AnimationCurve inCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  public float outSpeed = 1f;
  public AnimationCurve outCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);

  private void Awake()
  {
    this.rend = this.GetComponent<MeshRenderer>();
    this.mat = Object.Instantiate<Material>(this.rend.sharedMaterial);
    this.rend.sharedMaterial = this.mat;
  }

  public override IEnumerator TransitionIn(float speed = 1f)
  {
    float c = 0.0f;
    float t = this.inCurve.keys[this.inCurve.keys.Length - 1].time;
    while ((double) c < (double) t)
    {
      c += Time.unscaledDeltaTime * speed * this.inSpeed;
      this.mat.SetFloat("_Progress", c);
      this.mat.SetInt("_In", 1);
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
      this.mat.SetFloat("_Progress", c);
      this.mat.SetInt("_In", 0);
      yield return (object) null;
    }
  }
}
