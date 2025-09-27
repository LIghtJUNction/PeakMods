// Decompiled with JetBrains decompiler
// Type: PlaySFXOnChange
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PlaySFXOnChange : MonoBehaviour
{
  public SFX_Instance[] sfxOn;
  public SFX_Instance[] sfxOff;
  private bool t;
  public GameObject refObj;

  private void Update()
  {
    if (this.refObj.active && !this.t)
    {
      this.t = true;
      for (int index = 0; index < this.sfxOn.Length; ++index)
        this.sfxOn[index].Play();
    }
    if (this.refObj.active || !this.t)
      return;
    this.t = false;
    for (int index = 0; index < this.sfxOff.Length; ++index)
      this.sfxOff[index].Play();
  }
}
