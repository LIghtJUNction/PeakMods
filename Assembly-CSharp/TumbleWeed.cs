// Decompiled with JetBrains decompiler
// Type: TumbleWeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class TumbleWeed : MonoBehaviour
{
  private Rigidbody rig;
  public float rollForce;
  public float collisionForce;
  private float maxAngle;
  private PhotonView photonView;
  private float originalScale = 1f;
  public float powerMultiplier = 0.035f;
  public bool testFullPower;
  private List<Character> ignored = new List<Character>();

  private void Start()
  {
    this.photonView = this.GetComponent<PhotonView>();
    this.rig = this.GetComponent<Rigidbody>();
    this.maxAngle = Mathf.Lerp(50f, 180f, Mathf.Pow(Random.value, 5f));
    this.rollForce *= Mathf.Lerp(0.5f, 1f, Mathf.Pow(Random.value, 2f));
    this.originalScale = this.transform.localScale.x;
  }

  private void FixedUpdate()
  {
    if (!this.photonView.IsMine)
      return;
    Vector3 vector3 = -Vector3.right;
    Character target = this.GetTarget();
    if ((bool) (Object) target)
      vector3 = (target.Center - this.transform.position).normalized;
    this.rig.AddForce(vector3 * this.rollForce, ForceMode.Acceleration);
  }

  private Character GetTarget()
  {
    float num1 = 300f;
    Character target = (Character) null;
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if ((double) Vector3.Angle(-Vector3.right, allCharacter.Center - this.transform.position) <= (double) this.maxAngle)
      {
        float num2 = Vector3.Distance(allCharacter.Center, this.transform.position);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          target = allCharacter;
        }
      }
    }
    return target;
  }

  public void OnCollisionEnter(Collision collision)
  {
    Character componentInParent = collision.gameObject.GetComponentInParent<Character>();
    if (!(bool) (Object) componentInParent || !componentInParent.IsLocal || this.ignored.Contains(componentInParent))
      return;
    this.StartCoroutine(this.IgnoreTarget(componentInParent));
    float num1 = this.transform.localScale.x / this.originalScale;
    if ((double) this.originalScale == 0.0)
      num1 = 1f;
    float num2 = Mathf.Clamp01(num1);
    Vector3 linearVelocity = this.rig.linearVelocity;
    float num3 = Mathf.Clamp01(linearVelocity.magnitude * num2 * this.powerMultiplier);
    if (this.testFullPower)
      num3 = 1f;
    if ((double) num3 < 0.20000000298023224)
      return;
    componentInParent.Fall(2f * num3);
    Character character = componentInParent;
    linearVelocity = this.rig.linearVelocity;
    Vector3 force = linearVelocity.normalized * this.collisionForce * num3;
    Vector3 point = collision.contacts[0].point;
    character.AddForceAtPosition(force, point, 2f);
    componentInParent.refs.afflictions.AddThorn(collision.contacts[0].point);
    if ((double) num3 <= 0.60000002384185791)
      return;
    componentInParent.refs.afflictions.AddThorn(collision.contacts[0].point);
  }

  public IEnumerator IgnoreTarget(Character target)
  {
    this.ignored.Add(target);
    yield return (object) new WaitForSeconds(1f);
    this.ignored.Remove(target);
  }
}
