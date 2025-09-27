// Decompiled with JetBrains decompiler
// Type: SFX_PlayOneShot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
public class SFX_PlayOneShot : MonoBehaviour
{
  public Action beforePlayAction;
  public Action afterPlayAction;
  public bool playOnStart;
  public bool playOnEnable;
  public bool followTransform = true;
  public SFX_Instance sfx;
  public SFX_Instance[] sfxs;

  public void Start()
  {
    if (!this.playOnStart)
      return;
    this.Play();
  }

  public void OnEnable()
  {
    if (!this.playOnEnable)
      return;
    this.StartCoroutine(PlayAfterAnim());

    IEnumerator PlayAfterAnim()
    {
      yield return (object) new WaitForEndOfFrame();
      this.Play();
    }
  }

  public void Play() => this.PlayOneShot();

  public void PlayOneShot()
  {
    Action beforePlayAction = this.beforePlayAction;
    if (beforePlayAction != null)
      beforePlayAction();
    if ((UnityEngine.Object) this.sfx != (UnityEngine.Object) null)
      SFX_Player.instance.PlaySFX(this.sfx, this.transform.position, this.followTransform ? this.transform : (Transform) null);
    for (int index = 0; index < this.sfxs.Length; ++index)
      SFX_Player.instance.PlaySFX(this.sfxs[index], this.transform.position, this.followTransform ? this.transform : (Transform) null);
    Action afterPlayAction = this.afterPlayAction;
    if (afterPlayAction == null)
      return;
    afterPlayAction();
  }
}
