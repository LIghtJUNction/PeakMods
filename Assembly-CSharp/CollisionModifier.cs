// Decompiled with JetBrains decompiler
// Type: CollisionModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class CollisionModifier : MonoBehaviour
{
  private List<Character> characterList = new List<Character>();
  public bool applyEffects = true;
  public CharacterAfflictions.STATUSTYPE statusType;
  public float damage = 0.15f;
  public float cooldown = 1f;
  public float knockback = 20f;
  public float knockbackTowardsFwdVector;
  public List<CollisionMod> additionalMods = new List<CollisionMod>();
  public Action<Character, CollisionModifier, Collision, Bodypart> onCollide;
  public bool useBounceCoroutine;
  public bool standable = true;
  internal bool hasStandableRange;
  internal float standableRange;

  public void Collide(
    Character character,
    ContactPoint contactPoint,
    Collision collision,
    Bodypart bodypart)
  {
    Action<Character, CollisionModifier, Collision, Bodypart> onCollide = this.onCollide;
    if (onCollide != null)
      onCollide(character, this, collision, bodypart);
    if (!this.applyEffects || this.characterList.Contains(character))
      return;
    foreach (CollisionMod additionalMod in this.additionalMods)
    {
      character.refs.afflictions.AddStatus(additionalMod.statusType, additionalMod.amount);
      character.AddForce((character.Center - contactPoint.point).normalized * additionalMod.knockback);
    }
    character.refs.afflictions.AddStatus(this.statusType, this.damage);
    if ((double) this.knockback > 0.0)
    {
      if (this.useBounceCoroutine)
        this.StartCoroutine(BounceRoutine(Vector3.Lerp((character.Center - contactPoint.point).normalized, this.transform.forward, this.knockbackTowardsFwdVector) * this.knockback));
      else
        character.AddForce(Vector3.Lerp((character.Center - contactPoint.point).normalized, this.transform.forward, this.knockbackTowardsFwdVector) * this.knockback);
    }
    this.StartCoroutine(IHoldPlayer());

    IEnumerator IHoldPlayer()
    {
      this.characterList.Add(character);
      yield return (object) new WaitForSeconds(this.cooldown);
      this.characterList.Remove(character);
    }

    IEnumerator BounceRoutine(Vector3 kb)
    {
      float t = 0.0f;
      while ((double) t < 1.0)
      {
        character.AddForce(kb * (1f - t) * Time.fixedDeltaTime);
        character.data.sinceGrounded = Mathf.Clamp(character.data.sinceGrounded, 0.0f, 0.5f);
        t += Time.fixedDeltaTime;
        yield return (object) new WaitForFixedUpdate();
      }
    }
  }

  internal bool CanStand(Character character)
  {
    return !this.hasStandableRange || (double) Vector3.Distance(this.transform.position, character.Center) >= (double) this.standableRange;
  }
}
