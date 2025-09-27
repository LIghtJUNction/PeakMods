// Decompiled with JetBrains decompiler
// Type: BingBongMouth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BingBongMouth : MonoBehaviour
{
  public AudioSource audioSource;
  public Animator animator;
  public string animValue;
  public AnimationCurve curveMap;
  public float maxMouthOpen;
  public bool isPlaying;
  public bool canPlay;
  public Action_AskBingBong action;
  public float timeOffset = 0.25f;
  private float maxTime;
  private float time;
  private float lerpedMouthVal;
  public float timeMultiplier = 1.5f;

  private void Start()
  {
    this.action.OnAsk += new Action_AskBingBong.AskEvent(this.SampleAudioClip);
  }

  public void SampleAudioClip(AudioClip clip)
  {
    float[] data = new float[clip.samples * clip.channels];
    clip.GetData(data, 0);
    List<float> samples = new List<float>();
    int num1 = clip.frequency / 30;
    for (int index1 = 0; index1 < data.Length; index1 += num1)
    {
      float num2 = 0.0f;
      for (int index2 = 0; index2 < num1 && index1 + index2 < data.Length; ++index2)
        num2 += Mathf.Abs(data[index1 + index2]);
      samples.Add(num2 / (float) num1);
    }
    this.CreateCurveMap(samples);
  }

  public void CreateCurveMap(List<float> samples)
  {
    this.curveMap = new AnimationCurve();
    for (int index = 0; index < samples.Count; ++index)
      this.curveMap.AddKey((float) index / 30f, samples[index]);
    this.maxTime = (float) (samples.Count - 1) / 30f;
    this.canPlay = true;
    this.time = this.timeOffset;
  }

  private void Update()
  {
    if (!this.canPlay)
      return;
    this.time += Time.deltaTime * this.timeMultiplier;
    this.lerpedMouthVal = Mathf.Lerp(this.lerpedMouthVal, this.curveMap.Evaluate(this.time) * this.maxMouthOpen, Time.deltaTime * 25f);
    this.animator.SetFloat(this.animValue, this.lerpedMouthVal);
    if ((double) this.time <= (double) this.maxTime)
      return;
    this.canPlay = false;
    this.time = this.timeOffset;
    this.lerpedMouthVal = 0.0f;
  }
}
