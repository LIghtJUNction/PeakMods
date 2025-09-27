// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_ClearAllStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_ClearAllStatus : Affliction
{
  public bool excludeCurse;

  public Affliction_ClearAllStatus()
  {
  }

  public Affliction_ClearAllStatus(bool excludeCurse, float totalTime)
    : base(totalTime)
  {
    this.excludeCurse = excludeCurse;
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.ClearAllStatus;
  }

  public override void Stack(Affliction incomingAffliction) => this.OnApplied();

  public override void OnApplied()
  {
    if (!this.character.IsLocal)
      return;
    this.character.refs.afflictions.ClearAllStatus(this.excludeCurse);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteBool(this.excludeCurse);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.excludeCurse = serializer.ReadBool();
  }
}
