// Decompiled with JetBrains decompiler
// Type: LanternLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LanternLight : MonoBehaviour
{
  public Light light;
  public float flickerSpeed;
  public float flickerAmount;
  public AnimationCurve flickerCurve;
  private float startIntensity;

  private void Start() => this.startIntensity = this.light.intensity;

  private void Update()
  {
    if (!this.light.enabled)
      return;
    this.light.intensity = this.startIntensity + this.flickerCurve.Evaluate(Time.deltaTime * this.flickerSpeed) * this.flickerCurve.Evaluate((float) ((double) Time.deltaTime * (double) this.flickerSpeed * 0.38374000787734985)) * this.flickerAmount;
  }
}
