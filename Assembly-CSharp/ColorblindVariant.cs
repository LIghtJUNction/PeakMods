// Decompiled with JetBrains decompiler
// Type: ColorblindVariant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ColorblindVariant : MonoBehaviour
{
  public Renderer rend;
  public Material colorblindMaterial;
  public int matIndex;

  private void Awake()
  {
    if (!(bool) (Object) GUIManager.instance || !GUIManager.instance.colorblindness)
      return;
    if (this.matIndex == 0)
    {
      this.rend.material = this.colorblindMaterial;
    }
    else
    {
      Material[] materials = this.rend.materials;
      materials[this.matIndex] = this.colorblindMaterial;
      this.rend.materials = materials;
    }
    Item component;
    if (!this.TryGetComponent<Item>(out component))
      return;
    component.UIData.icon = component.UIData.altIcon;
  }
}
