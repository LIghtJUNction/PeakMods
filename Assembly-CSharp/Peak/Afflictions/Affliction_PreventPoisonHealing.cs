// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_PreventPoisonHealing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_PreventPoisonHealing : Affliction
{
  public Affliction_PreventPoisonHealing()
  {
  }

  public Affliction_PreventPoisonHealing(float totalTime)
    : base(totalTime)
  {
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.PreventPoisonHealing;
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime = incomingAffliction.totalTime;
  }
}
