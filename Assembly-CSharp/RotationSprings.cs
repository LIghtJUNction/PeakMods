// Decompiled with JetBrains decompiler
// Type: RotationSprings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RotationSprings : MonoBehaviour
{
  public List<RotationSprings.RotationSpringInstance> springs = new List<RotationSprings.RotationSpringInstance>();

  private void Update()
  {
    Transform parent = this.transform.parent;
    Vector3 forward = parent.forward;
    Vector3 up = parent.up;
    Vector3 zero = Vector3.zero;
    for (int index = 0; index < this.springs.Count; ++index)
      this.springs[index].DoUpdate(forward, up);
  }

  public void AddForce(Vector3 force, float spring, float drag)
  {
    RotationSprings.RotationSpringInstance rotationSpringInstance = new RotationSprings.RotationSpringInstance()
    {
      spring = spring,
      drag = drag,
      forward = this.transform.parent.forward,
      up = this.transform.parent.up
    };
  }

  [Serializable]
  public class RotationSpringInstance
  {
    public float spring;
    public float drag;
    public Vector3 vel;
    public Vector3 forward;
    public Vector3 up;

    public void DoUpdate(Vector3 targetForward, Vector3 targetUp)
    {
      this.vel = FRILerp.Lerp(this.vel, (Vector3.Cross(this.forward, targetForward) * Vector3.Angle(this.forward, targetForward) + Vector3.Cross(this.up, targetUp) * Vector3.Angle(this.up, targetUp)) * this.spring, this.drag);
      this.forward = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.forward;
      this.up = Quaternion.AngleAxis(Time.deltaTime * this.vel.magnitude, this.vel) * this.up;
    }
  }
}
