// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_PoisonOverTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_PoisonOverTime : Affliction
{
  public float delayBeforeEffect;
  public float statusPerSecond;

  public override void OnApplied()
  {
    Debug.Log((object) $"Added poison to character {this.character.gameObject.name} total time: {this.totalTime} delay: {this.delayBeforeEffect} status per second: {this.statusPerSecond}");
  }

  public Affliction_PoisonOverTime(float totalTime, float delay, float statusPerSecond)
    : base(totalTime)
  {
    this.totalTime = totalTime + delay;
    this.delayBeforeEffect = delay;
    this.statusPerSecond = statusPerSecond;
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
    serializer.WriteFloat(this.delayBeforeEffect);
    serializer.WriteFloat(this.statusPerSecond);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
    this.delayBeforeEffect = serializer.ReadFloat();
    this.statusPerSecond = serializer.ReadFloat();
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime += incomingAffliction.totalTime;
  }

  public Affliction_PoisonOverTime()
  {
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.PoisonOverTime;
  }

  protected override void UpdateEffect()
  {
    if ((double) this.timeElapsed <= (double) this.delayBeforeEffect)
      return;
    this.character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, this.statusPerSecond * Time.deltaTime);
  }
}
