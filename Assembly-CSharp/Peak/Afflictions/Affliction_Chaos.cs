// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_Chaos
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_Chaos : Affliction
{
  public float statusAmountAverage;
  public float statusAmountStandardDeviation;
  public float averageBonusStamina;
  public float standardDeviationBonusStamina;

  public Affliction_Chaos()
  {
  }

  public Affliction_Chaos(
    float statusAmountAverage,
    float statusAmountStandardDeviation,
    float averageBonusStamina,
    float standardDeviationBonusStamina)
  {
    this.statusAmountAverage = statusAmountAverage;
    this.statusAmountStandardDeviation = statusAmountStandardDeviation;
    this.averageBonusStamina = averageBonusStamina;
    this.standardDeviationBonusStamina = standardDeviationBonusStamina;
  }

  public override void OnApplied()
  {
    if (!this.character.IsLocal)
      return;
    List<CharacterAfflictions.STATUSTYPE> statustypeList = new List<CharacterAfflictions.STATUSTYPE>()
    {
      CharacterAfflictions.STATUSTYPE.Cold,
      CharacterAfflictions.STATUSTYPE.Hot,
      CharacterAfflictions.STATUSTYPE.Poison,
      CharacterAfflictions.STATUSTYPE.Drowsy,
      CharacterAfflictions.STATUSTYPE.Injury,
      CharacterAfflictions.STATUSTYPE.Hunger
    };
    this.character.refs.afflictions.ClearAllStatus(false);
    float num = Mathf.Clamp(Util.GenerateNormalDistribution(this.statusAmountAverage, this.statusAmountStandardDeviation), 0.0f, 1f);
    Debug.Log((object) $"total status: {num}");
    float b = num;
    while ((double) b > 0.05000000074505806)
    {
      if (statustypeList.Count != 0)
      {
        float a = statustypeList.Count != 1 ? num * Util.GenerateNormalDistribution(0.3f, 0.5f) : b;
        Debug.Log((object) $"Next status: {a}");
        float amount = Mathf.Min(a, b);
        if ((double) amount >= 0.02500000037252903)
        {
          int index = Random.Range(0, statustypeList.Count);
          CharacterAfflictions.STATUSTYPE statusType = statustypeList[index];
          this.character.refs.afflictions.AddStatus(statusType, amount);
          statustypeList.RemoveAt(index);
          switch (statusType)
          {
            case CharacterAfflictions.STATUSTYPE.Cold:
              statustypeList.Remove(CharacterAfflictions.STATUSTYPE.Hot);
              break;
            case CharacterAfflictions.STATUSTYPE.Hot:
              statustypeList.Remove(CharacterAfflictions.STATUSTYPE.Cold);
              break;
          }
          b -= amount;
        }
      }
      else
        break;
    }
    this.character.SetExtraStamina(Mathf.Clamp(Util.GenerateNormalDistribution(this.averageBonusStamina, this.standardDeviationBonusStamina), 0.0f, 1f));
    this.character.refs.afflictions.RemoveAffliction((Affliction) this);
  }

  public override Affliction.AfflictionType GetAfflictionType() => Affliction.AfflictionType.Chaos;

  public override void Stack(Affliction incomingAffliction)
  {
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.statusAmountAverage);
    serializer.WriteFloat(this.statusAmountStandardDeviation);
    serializer.WriteFloat(this.averageBonusStamina);
    serializer.WriteFloat(this.standardDeviationBonusStamina);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.statusAmountAverage = serializer.ReadFloat();
    this.statusAmountStandardDeviation = serializer.ReadFloat();
    this.averageBonusStamina = serializer.ReadFloat();
    this.standardDeviationBonusStamina = serializer.ReadFloat();
  }
}
