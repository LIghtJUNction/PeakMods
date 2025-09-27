// Decompiled with JetBrains decompiler
// Type: Action_LightLantern
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
public class Action_LightLantern : ItemAction
{
  private Lantern lantern;

  private void Awake() => this.lantern = this.GetComponent<Lantern>();

  public override void RunAction() => this.lantern.ToggleLantern();
}
