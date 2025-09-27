// Decompiled with JetBrains decompiler
// Type: FallPost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
public class FallPost : MonoBehaviour
{
  private Volume vol;

  private void Start() => this.vol = this.GetComponent<Volume>();

  private void Update()
  {
    if ((Object) Character.localCharacter == (Object) null)
      return;
    this.vol.enabled = (double) this.vol.weight > 9.9999997473787516E-05;
    if ((double) Character.localCharacter.data.fallSeconds > 0.0)
      this.vol.weight = Mathf.Lerp(this.vol.weight, 1f, Time.deltaTime);
    else
      this.vol.weight = Mathf.Lerp(this.vol.weight, 0.0f, Time.deltaTime);
  }
}
