// Decompiled with JetBrains decompiler
// Type: AntlionSandRumbleSFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class AntlionSandRumbleSFX : MonoBehaviour
{
  public GameObject refObj;
  public float vol = 0.3f;
  private AudioSource source;

  private void Start() => this.source = this.GetComponent<AudioSource>();

  private void Update()
  {
    if (!(bool) (Object) this.refObj)
      return;
    if (this.refObj.active)
      this.source.volume = Mathf.Lerp(this.source.volume, this.vol, 5f * Time.deltaTime);
    else
      this.source.volume = Mathf.Lerp(this.source.volume, 0.0f, 5f * Time.deltaTime);
  }
}
