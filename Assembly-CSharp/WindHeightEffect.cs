// Decompiled with JetBrains decompiler
// Type: WindHeightEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WindHeightEffect : MonoBehaviour
{
  public float from;
  public float to;
  public float fromHeight;
  public float toHeight;
  private WindChillZone zone;

  private void Start() => this.zone = this.GetComponent<WindChillZone>();

  private void Update()
  {
    if ((Object) Character.observedCharacter == (Object) null)
      return;
    this.zone.lightVolumeSampleThreshold_lower = Mathf.Lerp(this.from, this.to, Mathf.InverseLerp(this.fromHeight, this.toHeight, Character.observedCharacter.Center.y));
  }
}
