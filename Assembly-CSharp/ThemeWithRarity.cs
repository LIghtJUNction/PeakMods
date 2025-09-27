// Decompiled with JetBrains decompiler
// Type: ThemeWithRarity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class ThemeWithRarity
{
  [Tooltip("Parallel slot array. Index i should correspond to the same logical slot across all themes.")]
  public Material[] mats;
  [Tooltip("Weighted chance to pick this theme.")]
  public float rarity = 1f;
  public string name;
}
