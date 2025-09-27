// Decompiled with JetBrains decompiler
// Type: BingBongShieldWhileHolding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using UnityEngine;

#nullable disable
public class BingBongShieldWhileHolding : ItemComponent
{
  private bool wasHeldByLocal;
  private float tick;

  private void Start() => this.TryApplyInvincibility();

  private void Update()
  {
    this.tick += Time.deltaTime;
    if ((double) this.tick >= 1.5)
    {
      this.TryApplyInvincibility();
      this.tick = 0.0f;
    }
    if (this.wasHeldByLocal || !((Object) Character.localCharacter.data.currentItem == (Object) this.item))
      return;
    this.wasHeldByLocal = true;
  }

  private void TryApplyInvincibility()
  {
    if (!(bool) (Object) Character.localCharacter || !((Object) Character.localCharacter.data.currentItem == (Object) this.item))
      return;
    CharacterAfflictions afflictions = Character.localCharacter.refs.afflictions;
    Affliction_BingBongShield afflictionBingBongShield = new Affliction_BingBongShield();
    afflictionBingBongShield.totalTime = 2f;
    afflictions.AddAffliction((Affliction) afflictionBingBongShield);
  }

  private void OnDestroy()
  {
    if (!this.wasHeldByLocal)
      return;
    Character.localCharacter.refs.afflictions.RemoveAffliction(Affliction.AfflictionType.BingBongShield);
  }

  public override void OnInstanceDataSet()
  {
  }
}
