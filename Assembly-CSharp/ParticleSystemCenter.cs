// Decompiled with JetBrains decompiler
// Type: ParticleSystemCenter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (ParticleSystem))]
public class ParticleSystemCenter : MonoBehaviour
{
  private static readonly int Center = Shader.PropertyToID("_Center");
  private Vector3 pos;
  public Material material;
  private ParticleSystemRenderer psr;

  private void Start()
  {
  }

  private void Update() => this.setPosition();

  public void setPosition()
  {
    if ((Object) this.psr == (Object) null)
    {
      this.psr = this.GetComponent<ParticleSystemRenderer>();
      this.material = this.psr.material;
    }
    this.pos = this.transform.position;
    this.material.SetVector(ParticleSystemCenter.Center, (Vector4) this.pos);
  }
}
