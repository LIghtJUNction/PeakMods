// Decompiled with JetBrains decompiler
// Type: Action_ApplyAffliction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using UnityEngine;

#nullable disable
public class Action_ApplyAffliction : ItemAction
{
  [SerializeReference]
  public Affliction affliction;
  [SerializeReference]
  public Affliction[] extraAfflictions;

  public override void RunAction()
  {
    if (this.affliction == null)
    {
      Debug.LogError((object) "Your affliction is null bro");
    }
    else
    {
      this.character.refs.afflictions.AddAffliction(this.affliction);
      if (this.extraAfflictions == null)
        return;
      foreach (Affliction extraAffliction in this.extraAfflictions)
        this.character.refs.afflictions.AddAffliction(extraAffliction);
    }
  }
}
