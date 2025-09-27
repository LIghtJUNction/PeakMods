// Decompiled with JetBrains decompiler
// Type: Peak.Afflictions.Affliction_Sunscreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
namespace Peak.Afflictions;

public class Affliction_Sunscreen : Affliction
{
  public Affliction_Sunscreen()
  {
  }

  public Affliction_Sunscreen(float totalTime)
    : base(totalTime)
  {
    this.totalTime = totalTime;
  }

  protected override void UpdateEffect()
  {
    this.character.refs.customization.PulseStatus(Color.white);
    Debug.Log((object) ("time elapsed: " + this.timeElapsed.ToString()));
  }

  public override void Serialize(BinarySerializer serializer)
  {
    serializer.WriteFloat(this.totalTime);
  }

  public override void Deserialize(BinaryDeserializer serializer)
  {
    this.totalTime = serializer.ReadFloat();
  }

  public override Affliction.AfflictionType GetAfflictionType()
  {
    return Affliction.AfflictionType.Sunscreen;
  }

  public override void OnApplied()
  {
    if (!this.character.IsLocal)
      return;
    this.character.data.wearingSunscreen = true;
    GUIManager.instance.StartSunscreen();
  }

  public override void OnRemoved()
  {
    if (!this.character.IsLocal)
      return;
    this.character.data.wearingSunscreen = false;
    GUIManager.instance.EndSunscreen();
  }

  public override void Stack(Affliction incomingAffliction) => this.timeElapsed = 0.0f;
}
