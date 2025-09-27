// Decompiled with JetBrains decompiler
// Type: BackpackStabilizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class BackpackStabilizer : MonoBehaviour
{
  public Backpack backpack;
  private Vector3 startingPos;
  private Quaternion startingRot;
  private float timeSinceSpawned;
  private const float failsafeTime = 0.5f;
  private const float failsafeDistance = 5f;
  public float torqueStrength = 10f;

  private void Start()
  {
    this.startingPos = this.transform.position;
    this.startingRot = this.transform.rotation;
  }

  private void FixedUpdate()
  {
    if (this.backpack.photonView.IsMine && this.backpack.itemState == ItemState.Ground && (double) this.backpack.lastThrownAmount < 0.15000000596046448 && (double) this.timeSinceSpawned < 0.5)
    {
      this.timeSinceSpawned += Time.fixedDeltaTime;
      if ((double) this.timeSinceSpawned >= 0.5)
        Debug.Log((object) ("Distance moved: " + Vector3.Distance(this.transform.position, this.startingPos).ToString()));
      if ((double) Vector3.Distance(this.transform.position, this.startingPos) > 5.0)
        this.ResetPosition();
    }
    if (this.backpack.itemState != ItemState.Ground)
      return;
    Vector3 up = this.transform.up;
    this.backpack.rig.AddTorque(Vector3.Cross(up, Vector3.up).normalized * Vector3.Angle(up, Vector3.up) * this.torqueStrength, ForceMode.Acceleration);
  }

  private void ResetPosition()
  {
    this.backpack.photonView.RPC("SetKinematicAndResetSyncData", RpcTarget.All, (object) true, (object) this.startingPos, (object) this.startingRot);
  }

  private void Teleport() => this.transform.position += Vector3.up * 6f;
}
