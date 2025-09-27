// Decompiled with JetBrains decompiler
// Type: SFXOnImage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class SFXOnImage : MonoBehaviour
{
  public RawImage image;
  private Texture tex;
  public SFX_Instance[] equipSound;

  private void Update()
  {
    if ((Object) this.image.texture != (Object) this.tex)
    {
      for (int index = 0; index < this.equipSound.Length; ++index)
        this.equipSound[index].Play();
    }
    this.tex = this.image.texture;
  }
}
