// Decompiled with JetBrains decompiler
// Type: GiantTreeAchievementZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class GiantTreeAchievementZone : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.layer != LayerMask.NameToLayer("Character") || !other.GetComponentInParent<Character>().IsLocal)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ArboristBadge);
  }
}
