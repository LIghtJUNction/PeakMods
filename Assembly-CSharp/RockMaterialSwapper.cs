// Decompiled with JetBrains decompiler
// Type: RockMaterialSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RockMaterialSwapper : MonoBehaviour
{
  public Transform[] parents;
  public Material mat;

  private void Start()
  {
    foreach (Component parent in this.parents)
    {
      foreach (Renderer componentsInChild in parent.GetComponentsInChildren<MeshRenderer>(true))
        componentsInChild.sharedMaterial = this.mat;
    }
  }
}
