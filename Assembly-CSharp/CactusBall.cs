// Decompiled with JetBrains decompiler
// Type: CactusBall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CactusBall : StickyItemComponent
{
  private int framesSinceSpawned;

  private void Start()
  {
    this.physicalCollider.GetComponent<CollisionModifier>().onCollide += new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide);
  }

  private void OnCollide(
    Character character,
    CollisionModifier modifier,
    Collision collision,
    Bodypart bodypart)
  {
    if (!character.IsLocal || this.stuck || character.data.isInvincible || this.item.itemState != ItemState.Ground || (UnityEngine.Object) this.item.lastThrownCharacter == (UnityEngine.Object) character && (double) Time.time - (double) this.item.lastThrownTime < 0.5)
      return;
    this.StickToCharacterLocal(character, bodypart, this.transform.position - bodypart.transform.position);
  }

  internal override void StickToCharacterLocal(
    Character character,
    Bodypart bodypart,
    Vector3 worldOffset)
  {
    base.StickToCharacterLocal(character, bodypart, worldOffset);
    this.TestNeedlepointAchievement(character);
  }

  public void TestNeedlepointAchievement(Character character)
  {
    if (!character.IsLocal)
      return;
    int num = 0;
    foreach (StickyItemComponent stickyItemComponent in StickyItemComponent.ALL_STUCK_ITEMS)
    {
      if (stickyItemComponent.stuckToCharacter.IsLocal && stickyItemComponent is CactusBall)
        ++num;
    }
    if (num < 5)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.NeedlepointBadge);
  }
}
