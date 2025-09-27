// Decompiled with JetBrains decompiler
// Type: AOE
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class AOE : MonoBehaviour
{
  public HelperFunctions.LayerType mask;
  public bool auto = true;
  public bool onEnable;
  public float range = 5f;
  public float fallTime = 0.5f;
  public float knockback = 25f;
  public float minFactor = 0.2f;
  public float factorPow = 1f;
  public bool requireLineOfSigh;
  public bool canLaunchItems;
  public float itemKnockbackMultiplier = 1f;
  public float statusAmount;
  public CharacterAfflictions.STATUSTYPE statusType;
  public string illegalStatus = "";
  public bool useSingleDirection;
  public Transform singleDirectionForwardTF;
  public bool hasAffliction;
  [SerializeReference]
  public Affliction affliction;
  public bool procCollisionEvents;
  public bool cooksItems;

  private bool hasStatus => (double) Mathf.Abs(this.statusAmount) > 0.0;

  private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(this.transform.position, this.range);

  private void Start()
  {
    if (!this.auto)
      return;
    this.Explode();
  }

  private void OnEnable()
  {
    if (!this.onEnable)
      return;
    this.Explode();
  }

  public void Explode()
  {
    if ((double) this.range == 0.0)
      return;
    Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.range, (int) HelperFunctions.GetMask(this.mask));
    List<Character> characterList = new List<Character>();
    for (int index = 0; index < colliderArray.Length; ++index)
    {
      Character componentInParent1 = colliderArray[index].GetComponentInParent<Character>();
      RaycastHit raycastHit;
      Vector3 vector3_1;
      if ((UnityEngine.Object) componentInParent1 != (UnityEngine.Object) null && !characterList.Contains(componentInParent1))
      {
        float dist = Vector3.Distance(this.transform.position, componentInParent1.Center);
        if ((double) dist <= (double) this.range)
        {
          float factor = this.GetFactor(dist);
          if ((double) factor >= (double) this.minFactor)
          {
            if (this.requireLineOfSigh)
            {
              raycastHit = HelperFunctions.LineCheck(this.transform.position, componentInParent1.Center, HelperFunctions.LayerType.TerrainMap);
              if ((bool) (UnityEngine.Object) raycastHit.transform)
                continue;
            }
            characterList.Add(componentInParent1);
            Vector3 zero = Vector3.zero;
            Vector3 vector3_2;
            if (this.useSingleDirection)
            {
              vector3_2 = this.singleDirectionForwardTF.forward;
            }
            else
            {
              vector3_1 = componentInParent1.Center - this.transform.position;
              vector3_2 = vector3_1.normalized;
            }
            if ((double) Mathf.Abs(this.statusAmount) > 0.0)
            {
              if (this.illegalStatus != "")
              {
                componentInParent1.AddIllegalStatus(this.illegalStatus, this.statusAmount * factor);
              }
              else
              {
                Debug.Log((object) $"Adding status {this.statusType} with amount {(ValueType) (float) ((double) this.statusAmount * (double) factor)} to player {componentInParent1.name}");
                componentInParent1.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount * factor);
              }
            }
            if (this.hasAffliction)
              componentInParent1.refs.afflictions.AddAffliction(this.affliction);
            componentInParent1.AddForce(vector3_2 * factor * this.knockback, 0.7f, 1.3f);
            if ((double) this.fallTime > 0.0 && componentInParent1.IsLocal)
              componentInParent1.Fall(factor * this.fallTime);
          }
        }
      }
      else if (this.canLaunchItems)
      {
        Item componentInParent2 = colliderArray[index].GetComponentInParent<Item>();
        if ((UnityEngine.Object) componentInParent2 != (UnityEngine.Object) null && componentInParent2.photonView.IsMine && componentInParent2.itemState == ItemState.Ground)
        {
          float dist = Vector3.Distance(this.transform.position, componentInParent2.Center());
          if ((double) dist <= (double) this.range)
          {
            float factor = this.GetFactor(dist);
            if ((double) factor >= (double) this.minFactor)
            {
              if (this.requireLineOfSigh)
              {
                raycastHit = HelperFunctions.LineCheck(this.transform.position, componentInParent2.Center(), HelperFunctions.LayerType.TerrainMap);
                if ((bool) (UnityEngine.Object) raycastHit.transform)
                  continue;
              }
              EventOnItemCollision component;
              if (this.procCollisionEvents && componentInParent2.TryGetComponent<EventOnItemCollision>(out component))
                component.TriggerEvent();
              if (this.cooksItems)
                componentInParent2.cooking.FinishCooking();
              vector3_1 = componentInParent2.Center() - this.transform.position;
              Vector3 normalized = vector3_1.normalized;
              componentInParent2.rig.AddForce(normalized * factor * this.knockback * this.itemKnockbackMultiplier, ForceMode.Impulse);
            }
          }
        }
      }
    }
  }

  private float GetFactor(float dist)
  {
    return Mathf.Pow((float) (1.0 - (double) dist / (double) this.range), this.factorPow);
  }
}
