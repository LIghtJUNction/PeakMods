// Decompiled with JetBrains decompiler
// Type: Action_ModifyStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core;

#nullable disable
public class Action_ModifyStatus : ItemAction
{
  public CharacterAfflictions.STATUSTYPE statusType;
  public float changeAmount;

  public override void RunAction()
  {
    int num = this.character.data.passedOut ? 1 : 0;
    if ((double) this.changeAmount < 0.0)
    {
      if (this.statusType == CharacterAfflictions.STATUSTYPE.Poison)
      {
        this.character.refs.afflictions.ClearPoisonAfflictions();
        int amt = Mathf.RoundToInt(Mathf.Min(this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison), Mathf.Abs(this.changeAmount)) * 100f);
        Character feeder;
        if (this.item.TryGetFeeder(out feeder))
          GameUtils.instance.IncrementFriendPoisonHealing(amt, feeder.photonView.Owner);
        else
          Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.PoisonHealed, amt);
      }
      Character feeder1;
      if (this.statusType == CharacterAfflictions.STATUSTYPE.Injury && this.item.TryGetFeeder(out feeder1))
      {
        int amt = Mathf.RoundToInt(Mathf.Min(this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Injury), Mathf.Abs(this.changeAmount)) * 100f);
        GameUtils.instance.IncrementFriendHealing(amt, feeder1.photonView.Owner);
      }
      this.character.refs.afflictions.SubtractStatus(this.statusType, Mathf.Abs(this.changeAmount));
    }
    else
      this.character.refs.afflictions.AddStatus(this.statusType, Mathf.Abs(this.changeAmount));
    float statusSum = this.character.refs.afflictions.statusSum;
    if (num == 0 || (double) statusSum > 1.0)
      return;
    Debug.Log((object) "LIFE WAS SAVED");
    Character feeder2;
    if (!this.item.TryGetFeeder(out feeder2))
      return;
    GameUtils.instance.ThrowEmergencyPreparednessAchievement(feeder2.photonView.Owner);
  }
}
