// Decompiled with JetBrains decompiler
// Type: BiomeSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class BiomeSelector : MonoBehaviour
{
  public List<BiomeSelector.Biome> Biomes;

  public char Select(char lastID)
  {
    foreach (BiomeSelector.Biome biome in this.Biomes)
    {
      if ((int) biome.ID == (int) lastID && this.Biomes.Count > 1)
      {
        biome.Weight *= 0.0f;
        Debug.LogError((object) $"SAME AS LAST BIOME: {biome.ID} {lastID}");
      }
      else
      {
        biome.Weight *= 1f;
        Debug.LogError((object) $"DIFF FROM LAST BIOME: {biome.ID} {lastID}");
      }
    }
    BiomeSelector.Biome biome1 = this.Biomes.SelectRandomWeighted<BiomeSelector.Biome>((Func<BiomeSelector.Biome, float>) (biome => biome.Weight));
    foreach (BiomeSelector.Biome biome2 in this.Biomes)
      biome2.Parent.SetActive(false);
    biome1.Parent.SetActive(true);
    return biome1.ID;
  }

  [Serializable]
  public class Biome
  {
    public GameObject Parent;
    public float Weight;
    public char ID;
  }
}
