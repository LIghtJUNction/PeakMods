// Decompiled with JetBrains decompiler
// Type: RotationSpring
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RotationSpring : MonoBehaviour
{
  public float spring;
  public float drag;
  private Vector3 vel;

  private void Update()
  {
    Transform parent = this.transform.parent;
    Vector3 forward = parent.forward;
    Vector3 up = parent.up;
    this.vel = FRILerp.Lerp(this.vel, (Vector3.Cross(this.transform.forward, forward).normalized * Vector3.Angle(this.transform.forward, forward) + Vector3.Cross(this.transform.up, up).normalized * Vector3.Angle(this.transform.up, up)) * this.spring, this.drag);
    this.transform.Rotate(this.vel * Time.deltaTime, Space.World);
  }

  public void AddForce(Vector3 force) => this.vel += force;
}
