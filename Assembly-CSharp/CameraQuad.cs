// Decompiled with JetBrains decompiler
// Type: CameraQuad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[DefaultExecutionOrder(100000)]
public class CameraQuad : MonoBehaviour
{
  public float distance = 0.01f;
  private Camera cam;

  private void LateUpdate()
  {
    if (!(bool) (Object) this.cam)
      this.cam = Camera.main;
    float z = this.cam.nearClipPlane + this.distance;
    Vector3 worldPoint1 = this.cam.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, z));
    Vector3 worldPoint2 = this.cam.ViewportToWorldPoint(new Vector3(0.0f, 1f, z));
    Vector3 worldPoint3 = this.cam.ViewportToWorldPoint(new Vector3(1f, 0.0f, z));
    this.cam.ViewportToWorldPoint(new Vector3(1f, 1f, z));
    float y = Vector3.Distance(worldPoint1, worldPoint2);
    this.transform.localScale = new Vector3(Vector3.Distance(worldPoint1, worldPoint3), y, 1f);
    this.transform.position = this.cam.transform.position + this.cam.transform.forward * z;
    this.transform.rotation = this.cam.transform.rotation;
  }
}
