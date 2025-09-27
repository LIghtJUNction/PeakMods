// Decompiled with JetBrains decompiler
// Type: SandStoneClimb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class SandStoneClimb : MonoBehaviour
{
  private void Start()
  {
    this.GetComponent<ClimbModifierSurface>().onClimbAction += new Action<Character>(this.OnClimb);
    this.GetComponent<CollisionModifier>().onCollide += new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnCollide);
  }

  private void OnCollide(
    Character character,
    CollisionModifier modifier,
    Collision collision,
    Bodypart bodypart)
  {
    this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, Vector3.zero, Time.deltaTime * 0.05f);
  }

  private void OnClimb(Character character)
  {
    this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, Vector3.zero, Time.deltaTime * 0.1f);
  }
}
