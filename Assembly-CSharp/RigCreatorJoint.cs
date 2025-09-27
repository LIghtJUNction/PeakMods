// Decompiled with JetBrains decompiler
// Type: RigCreatorJoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class RigCreatorJoint : MonoBehaviour
{
  public float spring;
  internal ConfigurableJoint joint;
  internal Rigidbody rig;
  internal global::RigCreator rigCreator;

  private void Awake()
  {
    if (Application.isEditor && !Application.isPlaying)
      return;
    Object.Destroy((Object) this);
  }

  private ConfigurableJoint Joint()
  {
    if (!(bool) (Object) this.joint)
      this.joint = this.GetComponentInParent<ConfigurableJoint>();
    return this.joint;
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
    if ((double) this.spring == (double) this.CurrentSpring())
      return;
    this.SetSpring(this.spring);
    this.RigCreator().JointChanged(this, this.CurrentSpring());
  }

  private float CurrentSpring()
  {
    return this.Joint().angularXDrive.positionSpring / (this.Rig().mass * this.RigCreator().springMultiplier);
  }

  internal void SetSpring(float spring)
  {
    JointDrive angularXdrive = this.Joint().angularXDrive with
    {
      positionSpring = this.Rig().mass * spring * this.RigCreator().springMultiplier,
      positionDamper = (float) ((double) this.Rig().mass * (double) spring * 0.10000000149011612) * this.RigCreator().springMultiplier
    };
    this.Joint().angularXDrive = angularXdrive;
    this.Joint().angularYZDrive = angularXdrive;
  }
}
