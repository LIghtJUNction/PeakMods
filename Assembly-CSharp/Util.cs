// Decompiled with JetBrains decompiler
// Type: Util
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public static class Util
{
  private static System.Random r;

  public static System.Random random
  {
    get
    {
      if (Util.r == null)
        Util.r = new System.Random();
      return Util.r;
    }
  }

  public static float RangeLerp(
    float min,
    float max,
    float minParam,
    float maxParam,
    float param,
    bool clamp = true,
    AnimationCurve curve = null)
  {
    if ((double) maxParam - (double) minParam == 0.0)
      return min;
    float time = Mathf.Clamp((float) (((double) param - (double) minParam) / ((double) maxParam - (double) minParam)), 0.0f, 1f);
    if (curve != null && curve.keys.Length != 0)
      time = curve.Evaluate(time);
    float num = max - min;
    return min + num * time;
  }

  public static T RandomSelection<T>(this IEnumerable<T> enumerable, Func<T, int> weightFunc)
  {
    int num1 = 0;
    T obj1 = default (T);
    foreach (T obj2 in enumerable)
    {
      int num2 = weightFunc(obj2);
      if (Util.random.Next(num1 + num2) >= num1)
        obj1 = obj2;
      num1 += num2;
    }
    // ISSUE: variable of a boxed type
    __Boxed<T> local = (object) obj1;
    return obj1;
  }

  public static Vector2 FlattenVector3(Vector3 original) => new Vector2(original.x, original.z);

  public static float GenerateNormalDistribution(float mean, float stdDev)
  {
    double d = 1.0 - (double) UnityEngine.Random.value;
    double num1 = 1.0 - (double) UnityEngine.Random.value;
    double num2 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Cos(2.0 * Math.PI * num1);
    Debug.Log((object) $"Created random distribution result:{num2.ToString()} mean: {mean.ToString()} stdDev: {stdDev.ToString()}");
    float num3 = (float) num2;
    return mean + num3 * stdDev;
  }
}
