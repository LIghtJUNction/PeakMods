// Decompiled with JetBrains decompiler
// Type: PerlinSampler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class PerlinSampler
{
  public float scale = 1f;
  public int iterations = 2;
  public float scaleIncrease = 3f;
  public float roughness = 0.3f;
  public float pow = 1f;
  public Vector2 minMax = new Vector2(0.0f, 1f);

  public bool Sample(Vector2 pos, int seed = 0)
  {
    float num = this.SampleValue(pos, seed);
    return (double) num > (double) this.minMax.x && (double) num < (double) this.minMax.y;
  }

  public float SampleValue(Vector2 pos, int seed = 0)
  {
    float num1 = 0.0f;
    for (int p = 0; p < this.iterations; ++p)
    {
      float num2 = this.scale * Mathf.Pow(this.roughness, (float) p);
      float b = Mathf.PerlinNoise((float) (12345 + seed) + (float) ((double) pos.x * (double) num2 * 0.10000000149011612), (float) (12345 + seed) + (float) ((double) pos.y * (double) num2 * 0.10000000149011612));
      if (p == 0)
      {
        num1 = b;
      }
      else
      {
        float t = Mathf.Pow(this.roughness, (float) p);
        num1 = Mathf.Lerp(num1, b, t);
      }
    }
    if (!Mathf.Approximately(this.pow, 1f))
      num1 = Mathf.Pow(num1, this.pow);
    return num1;
  }
}
