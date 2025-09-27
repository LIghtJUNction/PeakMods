// Decompiled with JetBrains decompiler
// Type: RopeRender
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class RopeRender
{
  public float wobble = 1f;
  public float scrollSpeed = 1f;
  public float scale = 0.3f;
  public AnimationCurve wobbleCurve;
  public AnimationCurve wobbleOverLineCurve;

  public void DisplayRope(Vector3 from, Vector3 to, float time, LineRenderer line)
  {
    line.enabled = true;
    float num1 = Mathf.Lerp(1f, 0.0f, time);
    for (int index = 0; index < line.positionCount; ++index)
    {
      float num2 = (float) index / ((float) line.positionCount - 1f);
      Vector3 position = Vector3.Lerp(from, to, num2) + Mathf.Cos((float) index * this.scale + time * this.scrollSpeed) * Vector3.up * num1 * this.wobbleCurve.Evaluate(time) * this.wobbleOverLineCurve.Evaluate(num2);
      line.SetPosition(index, position);
    }
  }

  internal void StopRend(LineRenderer line) => line.enabled = false;
}
