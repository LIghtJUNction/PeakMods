// Decompiled with JetBrains decompiler
// Type: AnimationJuice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class AnimationJuice : MonoBehaviour
{
  public Transform overrideGameFeelTransform;
  public ParticleSystem[] particles;

  public void Screenshake(float amount)
  {
    Vector3 position = this.transform.position;
    if ((bool) (Object) this.overrideGameFeelTransform)
      position = this.overrideGameFeelTransform.position;
    GamefeelHandler.instance.AddPerlinShakeProximity(position, amount, 0.3f, maxProximity: 5f);
  }

  public void PlayParticle(int index)
  {
    if (this.particles.WithinRange<ParticleSystem>(index))
    {
      ParticleSystem particle = this.particles[index];
      if ((Object) particle != (Object) null)
        particle.Play();
      else
        Debug.LogError((object) "Particle could not be played, is null");
    }
    else
      Debug.LogError((object) "PlayParticle index out of range");
  }
}
