// Decompiled with JetBrains decompiler
// Type: SFXOnChildCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SFXOnChildCount : MonoBehaviour
{
  public SFX_Instance[] sfx;
  private int index;

  private void Start() => this.index = this.transform.childCount;

  private void Update()
  {
    if (this.index != this.transform.childCount)
    {
      for (int index = 0; index < this.sfx.Length; ++index)
        this.sfx[index].Play();
    }
    this.index = this.transform.childCount;
  }
}
