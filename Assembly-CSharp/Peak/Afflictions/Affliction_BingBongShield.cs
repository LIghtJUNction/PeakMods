// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_BingBongShield
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_BingBongShield : Affliction
{
  private Color gold = new Color(0.9f, 0.59f, 0.035f);

  protected override void UpdateEffect()
  {
    this.character.refs.customization.PulseStatus(this.gold);
    Debug.Log((object) ("time elapsed: " + this.timeElapsed.ToString()));
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.BingBongShield;
  }

  public override void Stack(Affliction incomingAffliction)
  {
    this.timeElapsed = 0.0f;
    this.character.data.RecalculateInvincibility();
  }

  public override void OnApplied() => this.character.data.RecalculateInvincibility();

  public override void OnRemoved() => this.character.data.RecalculateInvincibility();

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
  }
}
