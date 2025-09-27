// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using Unity.Collections;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

[Serializable]
public abstract class Affliction
{
  public float timeElapsed;
  public float totalTime;
  protected float bonusTime;
  [HideInInspector]
  public Character character;

  public Affliction()
  {
  }

  public Affliction(float totalTime) => this.totalTime = totalTime;

  public static Affliction CreateBlankAffliction(Affliction.AfflictionType afflictionType)
  {
    switch (afflictionType)
    {
      case Affliction.AfflictionType.PoisonOverTime:
        return (Affliction) new Affliction_PoisonOverTime();
      case Affliction.AfflictionType.InfiniteStamina:
        return (Affliction) new Affliction_InfiniteStamina();
      case Affliction.AfflictionType.FasterBoi:
        return (Affliction) new Affliction_FasterBoi();
      case Affliction.AfflictionType.Exhausted:
        return (Affliction) new Affliction_Exhaustion();
      case Affliction.AfflictionType.Glowing:
        return (Affliction) new Affliction_Glowing();
      case Affliction.AfflictionType.ColdOverTime:
        return (Affliction) new Affliction_AdjustColdOverTime();
      case Affliction.AfflictionType.Chaos:
        return (Affliction) new Affliction_Chaos();
      case Affliction.AfflictionType.AdjustStatus:
        return (Affliction) new Affliction_AdjustStatus();
      case Affliction.AfflictionType.ClearAllStatus:
        return (Affliction) new Affliction_ClearAllStatus();
      case Affliction.AfflictionType.PreventPoisonHealing:
        return (Affliction) new Affliction_PreventPoisonHealing();
      case Affliction.AfflictionType.AddBonusStamina:
        return (Affliction) new Affliction_AddBonusStamina();
      case Affliction.AfflictionType.DrowsyOverTime:
        return (Affliction) new Affliction_AdjustDrowsyOverTime();
      case Affliction.AfflictionType.AdjustStatusOverTime:
        return (Affliction) new Affliction_AdjustStatusOverTime();
      case Affliction.AfflictionType.Sunscreen:
        return (Affliction) new Affliction_Sunscreen();
      case Affliction.AfflictionType.BingBongShield:
        return (Affliction) new Affliction_BingBongShield();
      default:
        return (Affliction) null;
    }
  }

  public abstract Affliction.AfflictionType GetAfflictionType();

  public virtual void OnApplied()
  {
  }

  public virtual void OnRemoved()
  {
  }

  public abstract void Stack(Affliction incomingAffliction);

  public virtual bool Tick()
  {
    if ((double) this.bonusTime > 0.0)
      this.bonusTime -= Time.deltaTime;
    else
      this.timeElapsed += Time.deltaTime;
    if ((double) this.timeElapsed >= (double) this.totalTime)
      return true;
    this.UpdateEffect();
    return false;
  }

  protected virtual void UpdateEffect()
  {
  }

  public abstract void Serialize(BinarySerializer serializer);

  public abstract void Deserialize(BinaryDeserializer serializer);

  public Affliction Copy()
  {
    BinarySerializer serializer1 = new BinarySerializer(100, Allocator.Temp);
    Affliction blankAffliction = Affliction.CreateBlankAffliction(this.GetAfflictionType());
    this.Serialize(serializer1);
    BinaryDeserializer serializer2 = new BinaryDeserializer(serializer1);
    blankAffliction.Deserialize(serializer2);
    serializer1.Dispose();
    serializer2.Dispose();
    return blankAffliction;
  }

  public enum AfflictionType
  {
    PoisonOverTime,
    InfiniteStamina,
    FasterBoi,
    Exhausted,
    Glowing,
    ColdOverTime,
    Chaos,
    AdjustStatus,
    ClearAllStatus,
    PreventPoisonHealing,
    AddBonusStamina,
    DrowsyOverTime,
    AdjustStatusOverTime,
    Sunscreen,
    BingBongShield,
  }
}
