// Decompiled with JetBrains decompiler
// Type: ItemParticles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ItemParticles : MonoBehaviour
{
  public ParticleSystem smoke;

  public void EnableSmoke(bool active)
  {
    if (!(bool) (Object) this.smoke)
      return;
    if (active)
      this.smoke.Play();
    else
      this.smoke.Stop();
  }
}
