// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_AdjustDrowsyOverTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_AdjustDrowsyOverTime : Affliction
{
  public float statusPerSecond;

  public Affliction_AdjustDrowsyOverTime()
  {
  }

  public Affliction_AdjustDrowsyOverTime(float statusPerSecond, float totalTime)
    : base(totalTime)
  {
    this.statusPerSecond = statusPerSecond;
    this.totalTime = totalTime;
  }

  protected override void UpdateEffect()
  {
    this.character.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.statusPerSecond * Time.deltaTime);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.statusPerSecond);
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.statusPerSecond = serializer.ReadFloat();
    this.totalTime = serializer.ReadFloat();
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.DrowsyOverTime;
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime += incomingAffliction.totalTime;
    if (!(incomingAffliction is Affliction_AdjustDrowsyOverTime adjustDrowsyOverTime))
      return;
    this.statusPerSecond = Mathf.Max(adjustDrowsyOverTime.statusPerSecond, this.statusPerSecond);
  }
}
