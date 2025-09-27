// Decompiled with JetBrains decompiler
// Type: StatusField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class StatusField : MonoBehaviour
{
  public CharacterAfflictions.STATUSTYPE statusType;
  public float statusAmountPerSecond;
  public float statusAmountOnEntry;
  public float radius;
  private float lastEnteredTime;
  public float entryCooldown = 1f;
  public bool doNotApplyIfStatusesMaxed;
  public List<StatusField.StatusFieldStatus> additionalStatuses;
  private bool inflicting;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, this.radius);
  }

  public void Update()
  {
    if ((bool) (UnityEngine.Object) Character.localCharacter && (double) Vector3.Distance(Character.localCharacter.Center, this.transform.position) <= (double) this.radius)
    {
      if (this.doNotApplyIfStatusesMaxed && (double) Character.localCharacter.refs.afflictions.statusSum >= 1.0)
      {
        this.inflicting = false;
      }
      else
      {
        Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountPerSecond * Time.deltaTime);
        foreach (StatusField.StatusFieldStatus additionalStatuse in this.additionalStatuses)
          Character.localCharacter.refs.afflictions.AdjustStatus(additionalStatuse.statusType, additionalStatuse.statusAmountPerSecond * Time.deltaTime);
        if (!this.inflicting && (double) this.statusAmountOnEntry != 0.0 && (double) Time.time - (double) this.lastEnteredTime > (double) this.entryCooldown)
        {
          Character.localCharacter.refs.afflictions.AdjustStatus(this.statusType, this.statusAmountOnEntry);
          this.lastEnteredTime = Time.time;
        }
        this.inflicting = true;
      }
    }
    else
      this.inflicting = false;
  }

  [Serializable]
  public class StatusFieldStatus
  {
    public CharacterAfflictions.STATUSTYPE statusType;
    public float statusAmountPerSecond;
  }
}
