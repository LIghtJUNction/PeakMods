// Decompiled with JetBrains decompiler
// Type: ChildMaterialSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ChildMaterialSwapper : MonoBehaviour, IGenConfigStep
{
  public List<ThemeWithRarity> themes = new List<ThemeWithRarity>();
  private Dictionary<Material, int> _materialToSlot;
  public ThemeWithRarity chosenTheme;

  public void RunStep() => this.chosenTheme = this.GetRandomTheme();

  private void Start()
  {
    if (this.chosenTheme == null)
      return;
    this.ApplyRandomTheme();
  }

  public void ApplyRandomTheme()
  {
    if (this.themes == null || this.themes.Count == 0)
      Debug.LogError((object) "No themes configured.");
    else if (this.chosenTheme == null)
    {
      Debug.LogError((object) "Failed to choose a theme.");
    }
    else
    {
      this.BuildMaterialSlotLookup();
      foreach (MeshRenderer componentsInChild in this.GetComponentsInChildren<MeshRenderer>(true))
        this.ReplaceMaterialsForRenderer(componentsInChild, this.chosenTheme);
    }
  }

  private void ReplaceMaterialsForRenderer(MeshRenderer rend, ThemeWithRarity chosenTheme)
  {
    if ((Object) rend == (Object) null || chosenTheme == null || chosenTheme.mats == null)
      return;
    Material[] sharedMaterials = rend.sharedMaterials;
    bool flag = false;
    for (int index1 = 0; index1 < sharedMaterials.Length; ++index1)
    {
      Material key = sharedMaterials[index1];
      int index2;
      if (!((Object) key == (Object) null) && this._materialToSlot != null && this._materialToSlot.TryGetValue(key, out index2))
      {
        if (index2 >= 0 && index2 < chosenTheme.mats.Length && (Object) chosenTheme.mats[index2] != (Object) null)
        {
          if ((Object) sharedMaterials[index1] != (Object) chosenTheme.mats[index2])
          {
            sharedMaterials[index1] = chosenTheme.mats[index2];
            flag = true;
          }
        }
        else
          Debug.LogWarning((object) $"Chosen theme \"{chosenTheme.name}\" doesn't have a material at slot {index2}.");
      }
    }
    if (!flag)
      return;
    rend.sharedMaterials = sharedMaterials;
  }

  private void BuildMaterialSlotLookup()
  {
    this._materialToSlot = new Dictionary<Material, int>();
    for (int index1 = 0; index1 < this.themes.Count; ++index1)
    {
      ThemeWithRarity theme = this.themes[index1];
      if (theme != null && theme.mats != null)
      {
        for (int index2 = 0; index2 < theme.mats.Length; ++index2)
        {
          Material mat = theme.mats[index2];
          if (!((Object) mat == (Object) null) && !this._materialToSlot.ContainsKey(mat))
            this._materialToSlot.Add(mat, index2);
        }
      }
    }
  }

  private ThemeWithRarity GetRandomTheme()
  {
    float maxInclusive = 0.0f;
    foreach (ThemeWithRarity theme in this.themes)
    {
      if (theme != null)
        maxInclusive += Mathf.Max(0.0f, theme.rarity);
    }
    if ((double) maxInclusive <= 0.0)
      return (ThemeWithRarity) null;
    float num1 = Random.Range(0.0f, maxInclusive);
    float num2 = 0.0f;
    foreach (ThemeWithRarity theme in this.themes)
    {
      if (theme != null)
      {
        float num3 = Mathf.Max(0.0f, theme.rarity);
        num2 += num3;
        if ((double) num1 <= (double) num2)
          return theme;
      }
    }
    return (ThemeWithRarity) null;
  }
}
