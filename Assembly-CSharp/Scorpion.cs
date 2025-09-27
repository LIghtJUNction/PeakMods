// Decompiled with JetBrains decompiler
// Type: Scorpion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using UnityEngine;

#nullable disable
public class Scorpion : Mob
{
  public float totalPoisonTime = 10f;

  protected override void InflictAttack(Character character)
  {
    if (!character.IsLocal)
      return;
    float num = Mathf.Max(0.5f, 1f - character.refs.afflictions.statusSum + 0.05f);
    character.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.025f);
    character.refs.afflictions.AddAffliction((Affliction) new Affliction_PoisonOverTime(this.totalPoisonTime, 0.0f, num / this.totalPoisonTime));
    character.AddForceAtPosition(500f * this.mesh.forward, this.transform.position, 2f);
  }
}
