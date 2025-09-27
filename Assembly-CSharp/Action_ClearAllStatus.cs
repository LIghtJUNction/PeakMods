// Decompiled with JetBrains decompiler
// Type: Action_ClearAllStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Action_ClearAllStatus : ItemAction
{
  public bool excludeCurse = true;
  public List<CharacterAfflictions.STATUSTYPE> otherExclusions = new List<CharacterAfflictions.STATUSTYPE>();

  public override void RunAction()
  {
    int length = Enum.GetNames(typeof (CharacterAfflictions.STATUSTYPE)).Length;
    for (int index = 0; index < length; ++index)
    {
      CharacterAfflictions.STATUSTYPE statusType = (CharacterAfflictions.STATUSTYPE) index;
      if ((!this.excludeCurse || statusType != CharacterAfflictions.STATUSTYPE.Curse) && !this.otherExclusions.Contains(statusType))
        this.character.refs.afflictions.SubtractStatus(statusType, (float) Mathf.Abs(5));
    }
  }
}
