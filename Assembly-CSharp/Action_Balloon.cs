// Decompiled with JetBrains decompiler
// Type: Action_Balloon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Action_Balloon : ItemAction
{
  public Balloon balloon;

  public override void RunAction()
  {
    if (!(bool) (Object) this.character)
      return;
    if (this.balloon.isBunch)
    {
      this.character.refs.balloons.TieNewBalloon(0);
      this.character.refs.balloons.TieNewBalloon(2);
      this.character.refs.balloons.TieNewBalloon(4);
    }
    else
      this.character.refs.balloons.TieNewBalloon(this.balloon.colorIndex);
  }
}
