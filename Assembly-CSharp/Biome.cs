// Decompiled with JetBrains decompiler
// Type: Biome
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Biome : MonoBehaviour
{
  public MapHandler mapHandler;
  public Biome.BiomeType biomeType;

  public enum BiomeType
  {
    Shore = 0,
    Tropics = 1,
    Alpine = 2,
    Volcano = 3,
    Peak = 5,
    Mesa = 6,
    Colony = 7,
  }
}
