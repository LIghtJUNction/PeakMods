// Decompiled with JetBrains decompiler
// Type: AudioLoop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class AudioLoop : MonoBehaviour
{
  public AudioSource loop;
  public float volume;
  public float pitch = 1f;

  private void Update()
  {
    this.loop.volume = Mathf.Lerp(this.loop.volume, this.volume, 2f * Time.deltaTime);
    this.loop.pitch = Mathf.Lerp(this.loop.pitch, this.pitch, 2f * Time.deltaTime);
  }
}
