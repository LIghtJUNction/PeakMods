// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_AdjustStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_AdjustStatus : Affliction
{
  public CharacterAfflictions.STATUSTYPE statusType;
  public float statusAmount;

  public Affliction_AdjustStatus()
  {
  }

  public Affliction_AdjustStatus(
    CharacterAfflictions.STATUSTYPE statusType,
    float statusAmount,
    float totalTime)
    : base(totalTime)
  {
    this.statusType = statusType;
    this.statusAmount = statusAmount;
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.AdjustStatus;
  }

  public override void Stack(Affliction incomingAffliction) => this.OnApplied();

  public override void OnApplied()
  {
    if (!this.character.IsLocal)
      return;
    this.character.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    Debug.Log((object) "Serializing int");
    serializer.WriteInt((int) this.statusType);
    Debug.Log((object) "Serializing float");
    serializer.WriteFloat(this.statusAmount);
    Debug.Log((object) "Serializing float");
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    Debug.Log((object) "Deserializing int");
    this.statusType = (CharacterAfflictions.STATUSTYPE) serializer.ReadInt();
    Debug.Log((object) "Deserializing float");
    this.statusAmount = serializer.ReadFloat();
    Debug.Log((object) "Deserializing float");
    this.totalTime = serializer.ReadFloat();
  }
}
