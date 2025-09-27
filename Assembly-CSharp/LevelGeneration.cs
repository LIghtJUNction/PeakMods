// Decompiled with JetBrains decompiler
// Type: LevelGeneration
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LevelGeneration : MonoBehaviour
{
  public int seed;
  public bool updateLightmap = true;

  public void Generate()
  {
  }

  private void RandomizeBiomeVariants()
  {
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      BiomeVariant[] componentsInChildren = this.transform.GetChild(index).GetComponentsInChildren<BiomeVariant>(true);
      foreach (Component component in componentsInChildren)
        component.gameObject.SetActive(false);
      if (componentsInChildren.Length != 0)
        componentsInChildren[Random.Range(0, componentsInChildren.Length)].gameObject.SetActive(true);
    }
  }

  private void Clear()
  {
    Object.FindFirstObjectByType<LightVolume>().SetSize();
    this.GetComponent<PropGrouper>().ClearAll();
  }
}
