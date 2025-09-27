// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_Exhaustion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_Exhaustion : Affliction
{
  public float drainAmount;

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.Exhausted;
  }

  protected override void UpdateEffect()
  {
    base.UpdateEffect();
    float usage = this.drainAmount / this.totalTime * Time.deltaTime;
    this.character.UseStamina(usage);
    Debug.Log((object) $"Exhausterd: {usage}");
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.totalTime = Mathf.Max(this.timeElapsed, incomingAffliction.totalTime);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
    serializer.WriteFloat(this.drainAmount);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
    this.drainAmount = serializer.ReadFloat();
  }
}
