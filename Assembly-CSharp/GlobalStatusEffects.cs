// Decompiled with JetBrains decompiler
// Type: GlobalStatusEffects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class GlobalStatusEffects : MonoBehaviour
{
  public List<GlobalStatusEffects.Effect> effects = new List<GlobalStatusEffects.Effect>();

  private void Start()
  {
  }

  private void Update()
  {
    foreach (GlobalStatusEffects.Effect effect in this.effects)
    {
      foreach (Character allPlayerCharacter in PlayerHandler.GetAllPlayerCharacters())
        allPlayerCharacter.refs.afflictions.AddStatus(effect.type, effect.amount / effect.inTime * Time.deltaTime);
    }
  }

  [Serializable]
  public class Effect
  {
    public CharacterAfflictions.STATUSTYPE type;
    public float amount;
    public float inTime = 60f;
  }
}
