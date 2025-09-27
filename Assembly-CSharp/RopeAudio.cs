// Decompiled with JetBrains decompiler
// Type: RopeAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RopeAudio : MonoBehaviour
{
  public RopeSpool ropeSpool;
  public AudioSource loop1;
  public AudioSource loop2;
  private float prev;
  public SFX_Instance[] min;
  public SFX_Instance[] max;
  private bool t;
  private float startT = 0.5f;

  private void Start() => this.prev = this.ropeSpool.segments;

  private void Update()
  {
    this.startT -= Time.deltaTime;
    this.prev = Mathf.Lerp(this.prev, this.ropeSpool.segments, Time.deltaTime * 20f);
    if ((double) this.startT > 0.0)
      return;
    this.loop1.volume = Mathf.Lerp(this.loop1.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 6f, 20f * Time.deltaTime);
    this.loop1.pitch = Mathf.Lerp(this.loop1.pitch, (float) (1.0 + (double) Mathf.Abs(this.prev - this.ropeSpool.segments) / 2.0), 20f * Time.deltaTime);
    this.loop2.volume = Mathf.Lerp(this.loop2.volume, Mathf.Abs(this.prev - this.ropeSpool.segments) / 3f, 10f * Time.deltaTime);
    this.loop2.pitch = Mathf.Lerp(this.loop2.pitch, (float) (0.25 + (double) Mathf.Abs(this.prev - this.ropeSpool.segments) / 2.0), 10f * Time.deltaTime);
    if ((double) this.loop1.volume > 0.075000002980232239)
      this.loop1.volume = 0.075f;
    if ((double) this.loop2.volume > 0.075000002980232239)
      this.loop2.volume = 0.075f;
    if (!this.t && (double) this.ropeSpool.segments == 40.0)
    {
      for (int index = 0; index < this.min.Length; ++index)
        this.min[index].Play(this.transform.position);
      this.t = true;
    }
    if (!this.t || (double) this.ropeSpool.segments != 3.0)
      return;
    for (int index = 0; index < this.max.Length; ++index)
      this.max[index].Play(this.transform.position);
    this.t = false;
  }
}
