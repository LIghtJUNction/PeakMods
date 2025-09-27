// Decompiled with JetBrains decompiler
// Type: TombCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TombCheck : MonoBehaviour
{
  private Character character;
  private Animator anim;

  private void Start() => this.anim = this.GetComponent<Animator>();

  private void Update()
  {
    if (!(bool) (Object) this.character)
      this.character = Character.localCharacter;
    if (!(bool) (Object) this.character || !(bool) (Object) this.character.GetComponent<CharacterAnimations>() || !(bool) (Object) this.character.GetComponent<CharacterAnimations>().ambienceAudio)
      return;
    if (this.character.GetComponent<CharacterAnimations>().ambienceAudio.inTomb)
      this.anim.SetBool("Tomb", true);
    else
      this.anim.SetBool("Tomb", false);
  }
}
