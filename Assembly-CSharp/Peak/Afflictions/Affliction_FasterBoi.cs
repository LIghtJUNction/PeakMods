// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_FasterBoi
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_FasterBoi : Affliction
{
  public float moveSpeedMod = 1f;
  public float climbSpeedMod = 1f;
  public float drowsyOnEnd;
  private float cachedDrowsy;
  public float climbDelay;

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.FasterBoi;
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime = Mathf.Max(this.totalTime, incomingAffliction.totalTime);
  }

  protected override void UpdateEffect()
  {
    if (!this.character.data.isClimbing)
      return;
    this.climbDelay = 0.0f;
    this.bonusTime = 0.0f;
  }

  public override void OnApplied()
  {
    base.OnApplied();
    this.character.refs.movement.movementModifier += this.moveSpeedMod;
    this.character.refs.climbing.climbSpeedMod += this.climbSpeedMod;
    this.character.refs.ropeHandling.climbSpeedMod += this.climbSpeedMod;
    this.character.refs.vineClimbing.climbSpeedMod += this.climbSpeedMod;
    if (this.character.IsLocal)
      GUIManager.instance.StartEnergyDrink();
    this.cachedDrowsy = this.character.refs.afflictions.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy);
    this.character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 2f);
    this.bonusTime = this.climbDelay;
  }

  public override void OnRemoved()
  {
    base.OnRemoved();
    this.character.refs.movement.movementModifier -= this.moveSpeedMod;
    this.character.refs.climbing.climbSpeedMod -= this.climbSpeedMod;
    this.character.refs.ropeHandling.climbSpeedMod -= this.climbSpeedMod;
    this.character.refs.vineClimbing.climbSpeedMod -= this.climbSpeedMod;
    this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.cachedDrowsy + this.drowsyOnEnd);
    if (!this.character.IsLocal)
      return;
    GUIManager.instance.EndEnergyDrink();
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
    serializer.WriteFloat(this.moveSpeedMod);
    serializer.WriteFloat(this.climbSpeedMod);
    serializer.WriteFloat(this.drowsyOnEnd);
    serializer.WriteFloat(this.cachedDrowsy);
    serializer.WriteFloat(this.climbDelay);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
    this.moveSpeedMod = serializer.ReadFloat();
    this.climbSpeedMod = serializer.ReadFloat();
    this.drowsyOnEnd = serializer.ReadFloat();
    this.cachedDrowsy = serializer.ReadFloat();
    this.climbDelay = serializer.ReadFloat();
    this.bonusTime = this.climbDelay;
    Debug.Log((object) ("Bonus time set to " + this.bonusTime.ToString()));
  }
}
