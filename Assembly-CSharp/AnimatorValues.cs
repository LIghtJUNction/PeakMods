// Decompiled with JetBrains decompiler
// Type: AnimatorValues
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class AnimatorValues : MonoBehaviour
{
  private Animator anim;
  private CharacterData cD;
  private CharacterInput cI;

  private void Start()
  {
    this.anim = this.GetComponent<Animator>();
    this.cD = this.GetComponentInParent<CharacterData>();
    this.cI = this.GetComponentInParent<CharacterInput>();
  }

  private void Update()
  {
    this.anim.SetFloat("Input X", this.cI.movementInput.x);
    this.anim.SetFloat("Input Y", this.cI.movementInput.y);
    this.anim.SetBool("Is Grounded", this.cD.isGrounded);
    if (this.cI.sprintIsPressed)
      this.anim.SetFloat("Sprint", 1f, 0.125f, Time.deltaTime);
    if (!this.cI.sprintIsPressed)
      this.anim.SetFloat("Sprint", 0.0f, 0.125f, Time.deltaTime);
    this.anim.SetFloat("Velocity Y", this.cD.avarageVelocity.y);
  }
}
