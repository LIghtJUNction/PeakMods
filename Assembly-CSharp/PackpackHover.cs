// Decompiled with JetBrains decompiler
// Type: PackpackHover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PackpackHover : MonoBehaviour
{
  private Rigidbody rig;
  private RaycastHit hit;
  private Item item;
  private Vector3 forward;
  private Vector3 up;

  private void Start()
  {
    this.forward = this.transform.forward;
    this.up = this.transform.up;
    this.item = this.GetComponent<Item>();
    this.rig = this.GetComponent<Rigidbody>();
    this.hit = HelperFunctions.LineCheck(this.transform.position, this.transform.position + Vector3.down * 2f, HelperFunctions.LayerType.TerrainMap);
  }

  private void FixedUpdate()
  {
    if ((Object) this.rig == (Object) null || !(bool) (Object) this.hit.transform || this.item.itemState != ItemState.Ground || !this.item.photonView.IsMine)
      return;
    this.rig.AddForce((this.hit.point + this.hit.normal * 1f - this.transform.position) * 60f, ForceMode.Acceleration);
    this.rig.AddTorque((Vector3.Cross(this.transform.forward, this.forward).normalized * Vector3.Angle(this.transform.forward, this.forward) + Vector3.Cross(this.transform.up, this.up).normalized * Vector3.Angle(this.transform.up, this.up)) * 100f, ForceMode.Acceleration);
    this.rig.linearVelocity *= 0.8f;
    this.rig.angularVelocity *= 0.8f;
  }
}
