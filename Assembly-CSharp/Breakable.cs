// Decompiled with JetBrains decompiler
// Type: Breakable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PhotonView))]
public class Breakable : MonoBehaviour
{
  private Item item;
  public bool breakOnCollision;
  public float minBreakVelocity;
  public List<GameObject> instantiateOnBreak;
  public float alternateChance;
  public List<GameObject> alternateInstantiateOnBreak;
  public List<SFX_Instance> breakSFX;
  public List<GameObject> instantiateNonItemOnBreak;
  public List<Transform> instantiatePoints;
  public bool spawnsItemsKinematic;
  public bool playAnimationOnInstantiatedObject;
  public string animString;
  public bool ragdollCharacterOnBreak;
  private Rigidbody rig;
  private bool alreadyBroke;
  private Vector3 lastVelocity;
  public float pushForce = 2f;
  public float wholeBodyPushForce = 1f;

  private void Awake()
  {
    this.item = this.GetComponent<Item>();
    this.rig = this.GetComponent<Rigidbody>();
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!this.item.photonView.IsMine || this.item.itemState != ItemState.Ground || !this.breakOnCollision || !(bool) (Object) this.item.rig || (double) collision.relativeVelocity.magnitude <= (double) this.minBreakVelocity)
      return;
    this.Break(collision);
  }

  private void FixedUpdate()
  {
    if ((Object) this.rig == (Object) null || this.rig.isKinematic)
      return;
    this.lastVelocity = this.rig.linearVelocity;
  }

  public virtual void Break(Collision coll)
  {
    if (this.alreadyBroke)
      return;
    this.alreadyBroke = true;
    for (int index = 0; index < this.breakSFX.Count; ++index)
      this.breakSFX[index].Play(this.transform.position);
    if (this.ragdollCharacterOnBreak)
    {
      Character componentInParent1 = coll.transform.GetComponentInParent<Character>();
      if ((bool) (Object) componentInParent1)
      {
        Rigidbody componentInParent2 = coll.transform.GetComponentInParent<Rigidbody>();
        Vector3 vector3 = this.lastVelocity.normalized * this.pushForce;
        componentInParent1.AddForceToBodyPart(componentInParent2, vector3 * this.pushForce, vector3 * this.wholeBodyPushForce);
        componentInParent1.Fall(2f, 15f);
      }
    }
    if ((double) this.alternateChance > 0.0 && (double) Random.value < (double) this.alternateChance)
    {
      this.instantiateOnBreak = this.alternateInstantiateOnBreak;
      this.spawnsItemsKinematic = false;
    }
    for (int index = 0; index < this.instantiateOnBreak.Count; ++index)
    {
      Item component = PhotonNetwork.Instantiate("0_Items/" + this.instantiateOnBreak[index].name, this.instantiatePoints[index].position, this.instantiatePoints[index].rotation).GetComponent<Item>();
      if ((bool) (Object) component)
      {
        IntItemData intItemData;
        if (this.item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
          component.photonView.RPC("SetCookedAmountRPC", RpcTarget.All, (object) intItemData.Value);
        if (this.spawnsItemsKinematic)
        {
          component.rig.isKinematic = true;
          component.transform.position = coll.contacts[0].point;
          component.transform.up = coll.contacts[0].normal;
        }
        else
        {
          component.rig.linearVelocity = this.item.rig.linearVelocity;
          component.rig.angularVelocity = this.item.rig.angularVelocity;
        }
        if (this.playAnimationOnInstantiatedObject)
        {
          Animator componentInChildren = component.GetComponentInChildren<Animator>();
          if ((bool) (Object) componentInChildren)
            componentInChildren.Play(this.animString, 0, 0.0f);
        }
      }
    }
    for (int index = 0; index < this.instantiateNonItemOnBreak.Count; ++index)
    {
      Rigidbody component = Object.Instantiate<GameObject>(this.instantiateNonItemOnBreak[index], this.transform.position, this.transform.rotation).GetComponent<Rigidbody>();
      if ((bool) (Object) component)
      {
        component.linearVelocity = this.item.rig.linearVelocity;
        component.angularVelocity = this.item.rig.angularVelocity;
      }
    }
    PhotonNetwork.Destroy(this.gameObject);
  }
}
