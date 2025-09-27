// Decompiled with JetBrains decompiler
// Type: ChessStabilizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ChessStabilizer : MonoBehaviour
{
  public bool startKinematic;
  public Item item;
  private Vector3 startingPos;
  private Quaternion startingRot;
  private float groundTimer;
  public float torqueStrength = 10f;
  public float dampingStrength = 5f;

  private void Start()
  {
    this.startingPos = this.transform.position;
    this.startingRot = this.transform.rotation;
    if (!this.startKinematic)
      return;
    this.item.rig.isKinematic = true;
  }

  private void FixedUpdate()
  {
    if (this.item.itemState != ItemState.Ground || this.item.rig.isKinematic)
      return;
    Vector3 up = this.transform.up;
    this.item.rig.AddTorque(Vector3.Cross(up, Vector3.up).normalized * Vector3.Angle(up, Vector3.up) * this.torqueStrength + -this.item.rig.angularVelocity * this.dampingStrength, ForceMode.Acceleration);
    this.groundTimer += Time.fixedDeltaTime;
    if ((double) this.groundTimer <= 2.0 || (double) this.item.rig.linearVelocity.sqrMagnitude >= 0.5 || (double) this.item.rig.angularVelocity.sqrMagnitude >= 0.5 || (double) Vector3.Angle(this.transform.up, Vector3.up) >= 2.0)
      return;
    this.item.rig.isKinematic = true;
  }
}
