// Decompiled with JetBrains decompiler
// Type: ShelfShroom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PhotonView))]
public class ShelfShroom : MonoBehaviour
{
  private Item item;
  public bool breakOnCollision;
  public float minBreakVelocity;
  public GameObject instantiateOnBreak;
  public Transform instantiatePoint;
  public bool stickToNormal;
  private bool alreadyBroke;

  private void Awake() => this.item = this.GetComponent<Item>();

  private void OnCollisionEnter(Collision collision)
  {
    if (!this.item.photonView.IsMine || this.item.itemState != ItemState.Ground || !this.breakOnCollision || !(bool) (Object) this.item.rig || (double) collision.relativeVelocity.magnitude <= (double) this.minBreakVelocity)
      return;
    this.Break(collision);
  }

  public void Break(Collision coll)
  {
    if (this.alreadyBroke)
      return;
    this.alreadyBroke = true;
    string prefabName = "0_Items/" + this.instantiateOnBreak.name;
    Quaternion quaternion = Quaternion.Euler(0.0f, (float) Random.Range(0, 360), 0.0f);
    if (this.stickToNormal)
      quaternion = Quaternion.LookRotation(Vector3.forward, coll.contacts[0].normal);
    Vector3 point = coll.contacts[0].point;
    Quaternion rotation = quaternion;
    PhotonNetwork.Instantiate(prefabName, point, rotation);
    PhotonNetwork.Destroy(this.gameObject);
  }
}
