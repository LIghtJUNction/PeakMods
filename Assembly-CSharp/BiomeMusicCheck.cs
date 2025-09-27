// Decompiled with JetBrains decompiler
// Type: BiomeMusicCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BiomeMusicCheck : MonoBehaviour
{
  public GameObject tornado;
  public GameObject regularMusic;
  public GameObject mesaMusic;

  private void Update()
  {
    if ((bool) (Object) this.tornado)
    {
      if (this.tornado.active)
      {
        this.regularMusic.SetActive(false);
        this.mesaMusic.SetActive(true);
      }
      else
      {
        this.regularMusic.SetActive(true);
        this.mesaMusic.SetActive(false);
      }
    }
    else
    {
      this.regularMusic.SetActive(true);
      this.mesaMusic.SetActive(false);
    }
  }
}
