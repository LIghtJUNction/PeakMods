// Decompiled with JetBrains decompiler
// Type: FogCutoutZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FogCutoutZone : MonoBehaviour
{
  public float min = 10f;
  public float max = 100f;
  public float amount = 1f;
  public float transitionPoint;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(1f, 1f, 1f, this.amount);
    Gizmos.DrawWireSphere(this.transform.position, this.min);
    Gizmos.DrawWireSphere(this.transform.position, this.max);
    Gizmos.color = new Color(1f, 0.0f, 0.0f, 0.5f);
    Gizmos.DrawCube(this.transform.position + Vector3.forward * this.transitionPoint, new Vector3(300f, 9999f, 0.1f));
  }
}
