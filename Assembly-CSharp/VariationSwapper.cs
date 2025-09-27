// Decompiled with JetBrains decompiler
// Type: VariationSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class VariationSwapper : MonoBehaviour, IGenConfigStep
{
  public VariationSwapper.Variation[] Variations;

  public void EnableRandom()
  {
    float num1 = UnityEngine.Random.Range(0.0f, ((IEnumerable<VariationSwapper.Variation>) this.Variations).Sum<VariationSwapper.Variation>((Func<VariationSwapper.Variation, float>) (variation => variation.chance)));
    GameObject parent = ((IEnumerable<VariationSwapper.Variation>) this.Variations).First<VariationSwapper.Variation>().parent;
    float num2 = 0.0f;
    foreach (VariationSwapper.Variation variation in this.Variations)
    {
      num2 += variation.chance;
      if ((double) num1 < (double) num2)
      {
        Debug.Log((object) $"Found new: {variation.parent}");
        parent = variation.parent;
        break;
      }
    }
    if (!((UnityEngine.Object) parent != (UnityEngine.Object) null))
      return;
    foreach (VariationSwapper.Variation variation in this.Variations)
      variation.parent.SetActive(false);
    parent.SetActive(true);
  }

  public void RunStep() => this.EnableRandom();

  [Serializable]
  public class Variation
  {
    public GameObject parent;
    public float chance = 1f;
  }
}
