// Decompiled with JetBrains decompiler
// Type: DebugAddForce
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DebugAddForce : ItemComponent
{
  public float force;

  public override void OnInstanceDataSet()
  {
  }

  private void FixedUpdate()
  {
    if (this.item.itemState != ItemState.Ground || !this.item.photonView.IsMine || this.item.rig.isKinematic)
      return;
    this.item.rig.linearVelocity = Vector3.right * this.force;
  }
}
