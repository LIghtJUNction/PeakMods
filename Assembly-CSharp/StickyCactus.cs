// Decompiled with JetBrains decompiler
// Type: StickyCactus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class StickyCactus : MonoBehaviour
{
  public bool applyThorn = true;

  private void Start()
  {
    this.GetComponent<CollisionModifier>().onCollide += new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide);
  }

  private void OnCollide(
    Character character,
    CollisionModifier modifier,
    Collision collision,
    Bodypart bodypart)
  {
    if (!character.IsLocal || character.data.isInvincible || bodypart.partType == BodypartType.Head || bodypart.partType == BodypartType.Torso || bodypart.partType == BodypartType.Hip || !character.TryStickBodypart(bodypart, collision.contacts[0].point, CharacterAfflictions.STATUSTYPE.Thorns, 0.0f) || !this.applyThorn)
      return;
    character.refs.afflictions.AddThorn(collision.contacts[0].point);
  }
}
