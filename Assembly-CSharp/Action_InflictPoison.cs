// Decompiled with JetBrains decompiler
// Type: Action_InflictPoison
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;

#nullable disable
public class Action_InflictPoison : ItemAction
{
  public float inflictionTime;
  public float poisonPerSecond;
  public float delay;

  public override void RunAction()
  {
    this.character.refs.afflictions.AddAffliction((Affliction) new Affliction_PoisonOverTime(this.inflictionTime, this.delay, this.poisonPerSecond));
  }
}
