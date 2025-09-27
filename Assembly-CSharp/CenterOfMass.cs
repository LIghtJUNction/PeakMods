// Decompiled with JetBrains decompiler
// Type: CenterOfMass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CenterOfMass : MonoBehaviour
{
  public bool onlyOnGround;
  private Item item;
  public Transform centerOfMassTransform;
  public Vector3 localCenterOfMass;
  public float angularDamping = 3f;
  private Rigidbody rb;

  private void Start()
  {
    if (this.onlyOnGround)
    {
      this.item = this.GetComponent<Item>();
      if (this.item.itemState != ItemState.Ground)
        return;
    }
    this.rb = this.GetComponent<Rigidbody>();
    this.rb.centerOfMass = this.localCenterOfMass;
    this.rb.angularDamping = this.angularDamping;
    if (!(bool) (Object) this.centerOfMassTransform)
      return;
    this.rb.centerOfMass = this.centerOfMassTransform.localPosition;
  }

  private void FixedUpdate()
  {
    if (this.onlyOnGround && this.item.itemState != ItemState.Ground)
      return;
    this.rb.centerOfMass = !(bool) (Object) this.centerOfMassTransform ? this.localCenterOfMass : this.centerOfMassTransform.localPosition;
    this.rb.angularDamping = this.angularDamping;
  }

  private void OnDrawGizmosSelected()
  {
    if (!(bool) (Object) this.rb)
      return;
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(this.rb.worldCenterOfMass, 0.5f);
  }
}
