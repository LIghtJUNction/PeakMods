// Decompiled with JetBrains decompiler
// Type: LazyGizmo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LazyGizmo : MonoBehaviour
{
  public bool onSelected = true;
  public bool useTop;
  public Color color;
  public float radius;
  public bool showArrows;

  private void DrawGizmos()
  {
    Gizmos.color = this.color;
    if (this.useTop)
      Gizmos.DrawSphere(this.transform.position - Vector3.up * this.radius, this.radius);
    else
      Gizmos.DrawSphere(this.transform.position, this.radius);
  }

  private void OnDrawGizmos()
  {
    if (this.onSelected)
      return;
    this.DrawGizmos();
  }

  private void OnDrawGizmosSelected()
  {
    if (this.onSelected)
      this.DrawGizmos();
    if (!this.showArrows)
      return;
    Gizmos.color = Color.green;
    Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.up * (this.radius + 1f));
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * (this.radius + 1f));
    Gizmos.color = Color.red;
    Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.right * (this.radius + 1f));
  }
}
