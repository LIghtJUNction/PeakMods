// Decompiled with JetBrains decompiler
// Type: MyresAmbience
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MyresAmbience : MonoBehaviour
{
  public Animator anim;
  public AudioSource fearMusic;

  private void Update()
  {
    if (!(bool) (Object) this.anim)
      return;
    if ((double) this.anim.GetFloat("Myers Distance") > 60.0)
      this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.0f, 1f * Time.deltaTime);
    if ((double) this.anim.GetFloat("Myers Distance") < 50.0)
      this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.25f, 1f * Time.deltaTime);
    if ((double) this.anim.GetFloat("Myers Distance") < 25.0)
      this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.75f, 1f * Time.deltaTime);
    if ((double) this.anim.GetFloat("Myers Distance") != 0.0)
      return;
    this.fearMusic.volume = Mathf.Lerp(this.fearMusic.volume, 0.0f, 1f * Time.deltaTime);
  }
}
