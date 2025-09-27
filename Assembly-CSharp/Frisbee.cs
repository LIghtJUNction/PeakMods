// Decompiled with JetBrains decompiler
// Type: Frisbee
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Frisbee : MonoBehaviour
{
  public Item item;
  public float liftForce = 10f;
  public float velocityForLift = 10f;
  private Vector3 startedThrowPosition;
  private bool throwValidForAchievement;

  private void OnEnable()
  {
    GlobalEvents.OnItemThrown += new Action<Item>(this.OnItemThrown);
    GlobalEvents.OnItemRequested += new Action<Item, Character>(this.TestRequestedItem);
  }

  private void OnDisable()
  {
    GlobalEvents.OnItemThrown -= new Action<Item>(this.OnItemThrown);
    GlobalEvents.OnItemRequested -= new Action<Item, Character>(this.TestRequestedItem);
  }

  private void OnItemThrown(Item obj)
  {
    if (!((UnityEngine.Object) obj == (UnityEngine.Object) this.item))
      return;
    this.startedThrowPosition = obj.Center();
    this.throwValidForAchievement = true;
  }

  private void FixedUpdate()
  {
    if (!((UnityEngine.Object) this.item.holderCharacter == (UnityEngine.Object) null))
      return;
    float num = Mathf.InverseLerp(0.0f, this.velocityForLift, this.item.rig.linearVelocity.sqrMagnitude);
    this.item.rig.AddForce(this.transform.up * Mathf.Clamp01(Vector3.Dot(this.transform.up, Vector3.up)) * this.liftForce * num);
  }

  private void OnCollisionEnter(Collision collision)
  {
    this.item.rig.linearVelocity *= 0.5f;
    if (!this.throwValidForAchievement)
      return;
    int layer = collision.gameObject.layer;
    if (((int) HelperFunctions.terrainMapMask & 1 << layer) == 0)
      return;
    this.throwValidForAchievement = false;
  }

  private float throwDistance
  {
    get
    {
      return Vector3.Distance(this.startedThrowPosition, this.item.Center()) * CharacterStats.unitsToMeters;
    }
  }

  private void TestRequestedItem(Item requestedItem, Character character)
  {
    if (!this.throwValidForAchievement || !character.IsLocal || !((UnityEngine.Object) requestedItem == (UnityEngine.Object) this.item))
      return;
    Debug.Log((object) ("Frisbee grabbed at distance " + this.throwDistance.ToString()));
    if ((double) this.throwDistance < 100.0)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.UltimateBadge);
  }
}
