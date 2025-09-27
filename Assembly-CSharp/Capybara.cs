// Decompiled with JetBrains decompiler
// Type: Capybara
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Capybara : MonoBehaviour
{
  public float serenadeDistance;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.serenadeDistance);
  }

  private void OnEnable() => GlobalEvents.OnBugleTooted += new Action<Item>(this.TestBugleTooted);

  private void OnDisable() => GlobalEvents.OnBugleTooted -= new Action<Item>(this.TestBugleTooted);

  private void TestBugleTooted(Item bugle)
  {
    if ((double) Vector3.Distance(this.transform.position, bugle.transform.position) >= (double) this.serenadeDistance || !(bool) (UnityEngine.Object) bugle.holderCharacter || !bugle.holderCharacter.IsLocal)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AnimalSerenadingBadge);
  }
}
