// Decompiled with JetBrains decompiler
// Type: MirageLuggage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MirageLuggage : MonoBehaviour
{
  private Renderer[] renderers;

  private void OnEnable() => this.setMirageState(1f);

  private void setMirageState(float mirageState)
  {
    this.renderers = this.GetComponentsInChildren<Renderer>();
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      foreach (Material material in this.renderers[index].materials)
        material.SetFloat("_DoMirage", mirageState);
    }
  }

  private void Update()
  {
  }

  private void OnDisable() => this.setMirageState(0.0f);
}
