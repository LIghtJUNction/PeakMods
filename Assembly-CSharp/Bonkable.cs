// Decompiled with JetBrains decompiler
// Type: Bonkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PhotonView))]
public class Bonkable : MonoBehaviour
{
  private Item item;
  public float minBonkVelocity = 5f;
  public float ragdollTime = 1f;
  public float bonkForce = 1000f;
  public float bonkRange = 3f;
  public SFX_Instance[] bonk;
  public float lastBonkedTime;
  private float bonkCooldown = 1f;

  private void Awake() => this.item = this.GetComponent<Item>();

  private void OnCollisionEnter(Collision coll)
  {
    if (!this.item.photonView.IsMine || this.item.itemState != ItemState.Ground || !(bool) (Object) this.item.rig || (double) coll.relativeVelocity.magnitude <= (double) this.minBonkVelocity)
      return;
    this.Bonk(coll);
  }

  private void Bonk(Collision coll)
  {
    Character componentInParent = coll.gameObject.GetComponentInParent<Character>();
    if (!(bool) (Object) componentInParent || (double) Time.time <= (double) this.lastBonkedTime + (double) this.bonkCooldown)
      return;
    componentInParent.Fall(this.ragdollTime);
    for (int index = 0; index < this.bonk.Length; ++index)
      this.bonk[index].Play(this.transform.position);
    this.lastBonkedTime = Time.time;
    componentInParent.AddForceAtPosition(-coll.relativeVelocity.normalized * this.bonkForce, coll.contacts[0].point, this.bonkRange);
  }
}
