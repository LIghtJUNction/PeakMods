// Decompiled with JetBrains decompiler
// Type: ClimbModifierSurface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class ClimbModifierSurface : MonoBehaviour
{
  public bool onlySlideDown;
  public float speedMultiplier = 1f;
  public float staminaUsageMultiplier = 1f;
  public bool applyStatus;
  public CharacterAfflictions.STATUSTYPE statusType;
  public float statusAmount = 0.5f;
  public float statusCooldown = 0.5f;
  private float lastTriggerTime;
  public bool staticClimbCost;
  public Action<Character> onClimbAction;
  internal bool hasAlwaysClimbableRange;
  internal float alwaysClimbableRange;

  public void OnClimb(Character character)
  {
    Action<Character> onClimbAction = this.onClimbAction;
    if (onClimbAction != null)
      onClimbAction(character);
    if (!this.applyStatus || !character.IsLocal || (double) Time.time < (double) this.lastTriggerTime + (double) this.statusCooldown)
      return;
    character.refs.afflictions.AddStatus(this.statusType, this.statusAmount);
    this.lastTriggerTime = Time.time;
  }

  public void OnClimbEnter()
  {
  }

  public void OnClimbExit()
  {
  }

  internal float OverrideClimbAngle(Character character, float climbAngle)
  {
    return this.hasAlwaysClimbableRange && (double) Vector3.Distance(this.transform.position, character.Center) < (double) this.alwaysClimbableRange ? 90f : climbAngle;
  }
}
