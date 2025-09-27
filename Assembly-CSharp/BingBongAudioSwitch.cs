// Decompiled with JetBrains decompiler
// Type: BingBongAudioSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BingBongAudioSwitch : MonoBehaviour
{
  public GameObject refObject;
  public SFX_PlayOneShot[] clipOriginal;
  public SFX_PlayOneShot[] clipReplace;
  public GameObject[] enableLoop;
  public GameObject[] disableLoop;

  public void Init()
  {
    if (this.refObject.activeSelf)
    {
      for (int index = 0; index < this.enableLoop.Length; ++index)
        this.enableLoop[index].SetActive(true);
      for (int index = 0; index < this.disableLoop.Length; ++index)
        this.disableLoop[index].SetActive(false);
      for (int index = 0; index < this.clipOriginal.Length; ++index)
        this.clipOriginal[index].enabled = false;
      for (int index = 0; index < this.clipReplace.Length; ++index)
        this.clipReplace[index].enabled = true;
    }
    if (this.refObject.activeSelf)
      return;
    for (int index = 0; index < this.enableLoop.Length; ++index)
      this.enableLoop[index].SetActive(false);
    for (int index = 0; index < this.disableLoop.Length; ++index)
      this.disableLoop[index].SetActive(true);
    for (int index = 0; index < this.clipOriginal.Length; ++index)
      this.clipOriginal[index].enabled = true;
    for (int index = 0; index < this.clipReplace.Length; ++index)
      this.clipReplace[index].enabled = false;
  }
}
