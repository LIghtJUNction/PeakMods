// Decompiled with JetBrains decompiler
// Type: CoverageToVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CoverageToVolume : MonoBehaviour
{
  public float mod;
  public AudioSource sound;
  public AmbienceAudio aM;
  public float max = 0.1f;
  public float mid = 0.05f;
  public float min = 0.025f;
  private float vol;

  private void Update()
  {
    if (!(bool) (Object) this.aM || !(bool) (Object) this.sound)
      return;
    if ((double) this.aM.obstruction <= 0.60000002384185791)
      this.vol = this.max;
    if ((double) this.aM.obstruction > 0.60000002384185791)
      this.vol = this.mid;
    if ((double) this.aM.obstruction >= 0.800000011920929)
      this.vol = this.min;
    this.sound.volume = Mathf.Lerp(this.sound.volume, this.vol * this.mod, 0.5f * Time.deltaTime);
  }
}
