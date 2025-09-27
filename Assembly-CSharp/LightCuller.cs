// Decompiled with JetBrains decompiler
// Type: LightCuller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class LightCuller : MonoBehaviour
{
  private Light lightToCull;
  public float cullDistance = 50f;
  public float defaultRange;

  private void Start()
  {
    this.lightToCull = this.GetComponent<Light>();
    this.defaultRange = this.lightToCull.range;
  }

  private void OnEnable() => this.StartCoroutine(this.LightCullRoutine());

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.cullDistance);
  }

  private IEnumerator LightCullRoutine()
  {
    LightCuller lightCuller = this;
    yield return (object) new WaitForSeconds(Random.value);
    while (true)
    {
      if ((bool) (Object) Character.localCharacter)
      {
        bool shouldEnable = (double) Vector3.Distance(MainCamera.instance.transform.position, lightCuller.transform.position) < (double) lightCuller.cullDistance;
        float t;
        if (!lightCuller.lightToCull.enabled & shouldEnable)
        {
          lightCuller.lightToCull.enabled = true;
          t = 0.0f;
          while ((double) t < 1.0)
          {
            t += Time.deltaTime;
            lightCuller.lightToCull.range = lightCuller.defaultRange * t;
            yield return (object) null;
          }
        }
        if (lightCuller.lightToCull.enabled && !shouldEnable)
        {
          t = 0.0f;
          while ((double) t < 1.0)
          {
            t += Time.deltaTime;
            lightCuller.lightToCull.range = lightCuller.defaultRange * (1f - t);
            yield return (object) null;
          }
          lightCuller.lightToCull.enabled = false;
        }
      }
      yield return (object) new WaitForSeconds(1f);
    }
  }
}
