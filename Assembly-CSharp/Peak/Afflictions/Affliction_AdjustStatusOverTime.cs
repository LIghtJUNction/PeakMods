// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_AdjustStatusOverTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_AdjustStatusOverTime : Affliction
{
  public float statusPerSecond;

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.AdjustStatusOverTime;
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime += incomingAffliction.totalTime;
    if (!(incomingAffliction is Affliction_AdjustStatusOverTime adjustStatusOverTime))
      return;
    this.statusPerSecond = Mathf.Max(adjustStatusOverTime.statusPerSecond, this.statusPerSecond);
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
}
