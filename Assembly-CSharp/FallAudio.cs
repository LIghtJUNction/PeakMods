// Decompiled with JetBrains decompiler
// Type: FallAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FallAudio : MonoBehaviour
{
  public AudioSource au;
  private float yVel;
  private float prevY;

  private void Update()
  {
    this.yVel = this.transform.position.y - this.prevY;
    this.prevY = this.transform.position.y;
    this.au.volume = Mathf.Lerp(this.au.volume, Mathf.Abs(this.yVel) / 10f, Time.deltaTime * 10f);
    if ((double) this.au.volume <= 0.5)
      return;
    this.au.volume = 0.5f;
  }
}
