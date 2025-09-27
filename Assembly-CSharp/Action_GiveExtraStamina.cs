// Decompiled with JetBrains decompiler
// Type: Action_GiveExtraStamina
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class Action_GiveExtraStamina : ItemAction
{
  public float amount;

  public override void RunAction() => this.character.AddExtraStamina(this.amount);
}
