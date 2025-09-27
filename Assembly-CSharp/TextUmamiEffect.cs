// Decompiled with JetBrains decompiler
// Type: TextUmamiEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TextUmamiEffect : DialogueEffect
{
  public bool abs;
  public float amplitude = 0.2f;
  public float period = 0.5f;
  public float offset = 0.1f;
  public float charOffset = 10f;
  public Gradient colorGradient;

  public virtual float colorSpeedMult => 1f;

  public override void UpdateCharacter(int index)
  {
    float num1 = this.offset * (float) index;
    float num2 = (float) (1.0 + (double) Mathf.Sin((Time.time + num1) / this.period) * (double) this.amplitude);
    Vector3 scale = Vector3.one * num2;
    this.DTanimator.SetCharScale(index, scale);
    this.DTanimator.SetCharOffset(index, Vector3.up * num2 * this.charOffset);
    float time = (float) (((double) Mathf.Sin((float) (((double) Time.time + (double) num1) / ((double) this.period / (double) this.colorSpeedMult))) + 1.0) * 0.5);
    this.DTanimator.SetCharColor(index, (Color32) this.colorGradient.Evaluate(time));
  }
}
