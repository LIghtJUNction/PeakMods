// Decompiled with JetBrains decompiler
// Type: Knot.KnotTieText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using pworld.Scripts.PPhys;
using UnityEngine;

#nullable disable
namespace Knot;

public class KnotTieText : MonoBehaviour
{
  public float velocity;
  public PPhysSpringBase spring;
  public float lifeTime;
  private float timeAlive;

  private void Awake() => this.spring = this.GetComponent<PPhysSpringBase>();

  private void Start()
  {
  }

  private void Update()
  {
    this.timeAlive += Time.deltaTime;
    if ((double) this.timeAlive > (double) this.lifeTime)
      Object.Destroy((Object) this.gameObject);
    if ((double) this.timeAlive > (double) this.lifeTime - 1.0)
      this.spring.Target = 0.ToVec();
    this.transform.position += Vector3.up * (Time.deltaTime * this.velocity);
  }
}
