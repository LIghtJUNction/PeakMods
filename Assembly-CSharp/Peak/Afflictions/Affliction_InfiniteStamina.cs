// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_InfiniteStamina
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_InfiniteStamina : Affliction
{
  [SerializeReference]
  public Affliction drowsyAffliction;
  public float climbDelay;

  public Affliction_InfiniteStamina(float totalTime)
    : base(totalTime)
  {
  }

  public Affliction_InfiniteStamina()
  {
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.InfiniteStamina;
  }

  public override void Stack(Affliction incomingAffliction)
  {
    if (!(incomingAffliction is Affliction_InfiniteStamina afflictionInfiniteStamina))
      return;
    this.totalTime = incomingAffliction.totalTime;
    this.timeElapsed = 0.0f;
    if (this.drowsyAffliction == null)
      return;
    this.drowsyAffliction.totalTime += afflictionInfiniteStamina.drowsyAffliction.totalTime;
  }

  public override void OnApplied()
  {
    if (!this.character.IsLocal)
      return;
    GUIManager.instance.StartSugarRush();
  }

  public override void OnRemoved()
  {
    if (!this.character.IsLocal)
      return;
    GUIManager.instance.EndSugarRush();
    if (this.drowsyAffliction == null)
      return;
    this.character.refs.afflictions.AddAffliction(this.drowsyAffliction);
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
    serializer.WriteFloat(this.climbDelay);
    bool flag = this.drowsyAffliction != null;
    serializer.WriteBool(flag);
    if (!flag)
      return;
    this.drowsyAffliction.Serialize(serializer);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
    this.climbDelay = serializer.ReadFloat();
    this.bonusTime = this.climbDelay;
    if (!serializer.ReadBool())
      return;
    this.drowsyAffliction = (Affliction) new Affliction_AdjustDrowsyOverTime();
    this.drowsyAffliction.Deserialize(serializer);
  }

  protected override void UpdateEffect()
  {
    this.character.AddStamina(1f);
    if (!this.character.data.isClimbing)
      return;
    this.climbDelay = 0.0f;
    this.bonusTime = 0.0f;
  }
}
