// Decompiled with JetBrains decompiler
// Type: ScoutCannonAchievementZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
[ExecuteAlways]
public class ScoutCannonAchievementZone : MonoBehaviour
{
  public Bounds bounds;
  private bool playerWasLaunched;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
  }

  private void Update()
  {
    this.bounds.center = this.transform.position;
    this.bounds.size = this.transform.localScale;
  }

  private void FixedUpdate()
  {
    if (!Application.isPlaying)
      return;
    this.DetectPlayerWasLaunched();
    this.TestAchievement();
  }

  private void DetectPlayerWasLaunched()
  {
    if (!(bool) (Object) Character.localCharacter || !this.bounds.Contains(Character.localCharacter.Center) || !Character.localCharacter.data.launchedByCannon)
      return;
    Debug.Log((object) "Player launched by Cannon");
    this.playerWasLaunched = true;
  }

  private void TestAchievement()
  {
    if (!this.playerWasLaunched || (double) Character.localCharacter.data.fallSeconds > 0.0 || !Character.localCharacter.data.isGrounded)
      return;
    this.playerWasLaunched = false;
    if ((double) Character.localCharacter.Center.y < (double) this.bounds.min.y)
      return;
    Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.DaredevilBadge);
  }
}
