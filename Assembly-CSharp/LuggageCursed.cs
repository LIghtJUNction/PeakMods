// Decompiled with JetBrains decompiler
// Type: LuggageCursed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LuggageCursed : Luggage
{
  public int minCurse;
  public int maxCurse;
  public float injuryAmt;

  public override void Interact_CastFinished(Character interactor)
  {
    if (!interactor.IsLocal)
      return;
    float amount = (float) Random.Range(this.minCurse, this.maxCurse + 1) * 0.025f;
    if ((double) amount > 0.0)
      interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, amount);
    interactor.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, this.injuryAmt);
    base.Interact_CastFinished(interactor);
  }
}
