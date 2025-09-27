// Decompiled with JetBrains decompiler
// Type: RigCreatorRigidbody
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class RigCreatorRigidbody : MonoBehaviour
{
  internal float mass;
  internal Rigidbody rig;
  internal global::RigCreator rigCreator;

  private void Awake()
  {
    if (!Application.isEditor || Application.isPlaying)
      Object.Destroy((Object) this);
    else
      this.SetValues();
  }

  private Rigidbody Rig()
  {
    if (!(bool) (Object) this.rig)
      this.rig = this.GetComponentInParent<Rigidbody>();
    return this.rig;
  }

  private global::RigCreator RigCreator()
  {
    if (!(bool) (Object) this.rigCreator)
      this.rigCreator = this.GetComponentInParent<global::RigCreator>();
    return this.rigCreator;
  }

  private void Update()
  {
    if ((double) this.mass == (double) this.Rig().mass)
      return;
    this.RigCreator().RigidbodyChanged(this, this.Rig().mass);
    this.SetValues();
  }

  private void SetValues() => this.mass = this.Rig().mass;
}
