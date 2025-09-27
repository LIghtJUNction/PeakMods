// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_AddBonusStamina
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_AddBonusStamina : Affliction
{
  public float staminaAmount;

  public Affliction_AddBonusStamina()
  {
  }

  public Affliction_AddBonusStamina(float staminaAmount, float totalTime)
    : base(totalTime)
  {
    this.staminaAmount = staminaAmount;
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.AddBonusStamina;
  }

  public override void Stack(Affliction incomingAffliction) => this.OnApplied();

  public override void OnApplied()
  {
    if (!this.character.IsLocal)
      return;
    Debug.Log((object) ("Adding extra stamina: " + this.staminaAmount.ToString()));
    this.character.AddExtraStamina(this.staminaAmount);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.staminaAmount);
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.staminaAmount = serializer.ReadFloat();
    this.totalTime = serializer.ReadFloat();
  }
}
