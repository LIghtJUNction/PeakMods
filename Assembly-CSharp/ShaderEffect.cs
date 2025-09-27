// Decompiled with JetBrains decompiler
// Type: ShaderEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ShaderEffect : MonoBehaviour
{
  public Renderer[] renderers;
  private List<Material> currentEffects = new List<Material>();
  private MaterialPropertyBlock prop;

  private void Start() => this.prop = new MaterialPropertyBlock();

  private void Update()
  {
    foreach (Renderer renderer in this.renderers)
      this.PerRendere(renderer);
  }

  private void PerRendere(Renderer item)
  {
  }

  internal void SetEffect(Material mat, string key, float value)
  {
    if (!this.currentEffects.Contains(mat))
      this.AddEffect(mat);
    foreach (Renderer renderer in this.renderers)
    {
      this.prop.SetFloat(key, value);
      MaterialPropertyBlock prop = this.prop;
      renderer.SetPropertyBlock(prop);
    }
  }

  private void AddEffect(Material mat)
  {
    foreach (Renderer renderer in this.renderers)
    {
      List<Material> materialList = new List<Material>();
      materialList.AddRange((IEnumerable<Material>) renderer.sharedMaterials);
      materialList.Add(mat);
      renderer.sharedMaterials = materialList.ToArray();
    }
    this.currentEffects.Add(mat);
  }

  internal void ClearEffect(Material mat)
  {
    if (this.currentEffects.Count == 0 || !this.currentEffects.Contains(mat))
      return;
    this.RemoveEffect(mat);
  }

  private void RemoveEffect(Material mat)
  {
    foreach (Renderer renderer in this.renderers)
    {
      List<Material> materialList = new List<Material>();
      materialList.AddRange((IEnumerable<Material>) renderer.sharedMaterials);
      materialList.Remove(mat);
      renderer.sharedMaterials = materialList.ToArray();
    }
    this.currentEffects.Remove(mat);
  }
}
