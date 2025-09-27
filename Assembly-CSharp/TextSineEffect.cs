// Decompiled with JetBrains decompiler
// Type: TextSineEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TextSineEffect : DialogueEffect
{
  public bool abs;
  public float amplitude = 3f;
  public float period = 0.15f;
  public float offset = 0.1f;

  public override void UpdateCharacter(int index)
  {
    Vector3 offset = Vector3.up * (Mathf.Sin((Time.time + this.offset * (float) index) / this.period) * this.amplitude);
    if (this.abs)
      offset = new Vector3(offset.x, Mathf.Abs(offset.y), offset.z);
    this.DTanimator.SetCharOffset(index, offset);
  }
}
