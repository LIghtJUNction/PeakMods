// Decompiled with JetBrains decompiler
// Type: ParticleCuller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class ParticleCuller : MonoBehaviour
{
  public ParticleSystem[] systems;
  public float cullDistance = 50f;

  private void OnEnable() => this.StartCoroutine(this.CullRoutine());

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.cullDistance);
  }

  private IEnumerator CullRoutine()
  {
    ParticleCuller particleCuller = this;
    yield return (object) new WaitForSeconds(Random.value);
    while (true)
    {
      if ((bool) (Object) Character.localCharacter)
      {
        bool flag = (double) Vector3.Distance(MainCamera.instance.transform.position, particleCuller.transform.position) < (double) particleCuller.cullDistance;
        for (int index = 0; index < particleCuller.systems.Length; ++index)
        {
          if (flag && !particleCuller.systems[index].isPlaying)
            particleCuller.systems[index].Play();
          if (!flag && particleCuller.systems[index].isPlaying)
            particleCuller.systems[index].Stop();
        }
      }
      yield return (object) new WaitForSeconds(1f);
    }
  }
}
