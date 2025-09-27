// Decompiled with JetBrains decompiler
// Type: DispelFogField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class DispelFogField : MonoBehaviour
{
  public float innerRadius = 7.5f;
  public float outerRadius = 12.5f;
  private float lastEnteredTime;
  private bool inflicting;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.innerRadius);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(this.transform.position, this.outerRadius);
  }

  public void OnDisable() => Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0.0f;

  public void Update()
  {
    float num = Vector3.Distance(Character.observedCharacter.Center, this.transform.position);
    if ((bool) (Object) Character.observedCharacter && (double) num <= (double) this.outerRadius)
      Singleton<OrbFogHandler>.Instance.dispelFogAmount = Mathf.InverseLerp(this.outerRadius, this.innerRadius, num);
    else
      Singleton<OrbFogHandler>.Instance.dispelFogAmount = 0.0f;
  }
}
