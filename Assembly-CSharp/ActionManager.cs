// Decompiled with JetBrains decompiler
// Type: ActionManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ActionManager : MonoBehaviour
{
  public float actionTimer;
  public float edgeCaseTimer;
  public Animator anim;
  public bool fallCancel = true;
  public bool jumpCancel = true;
  public bool attackCancel = true;
  public bool dashCancel = true;
  public bool crouchCancel = true;
  public bool continuable;
  public bool specialState;

  private void Start()
  {
    if (!(bool) (Object) this.GetComponent<Animator>())
      return;
    this.anim = this.GetComponent<Animator>();
  }

  private void Update()
  {
    if ((bool) (Object) this.anim)
    {
      this.anim.SetBool("Jump Cancel", this.jumpCancel);
      this.anim.SetBool("Attack Cancel", this.attackCancel);
      this.anim.SetBool("Continuable", this.continuable);
      this.anim.SetBool("Fall Cancel", this.fallCancel);
      this.anim.SetBool("Dash Cancel", this.dashCancel);
      this.anim.SetBool("Crouch Cancel", this.crouchCancel);
      this.anim.SetBool("Special State", this.specialState);
      if ((double) this.actionTimer <= 0.0)
        this.anim.SetBool("Action", false);
      if ((double) this.actionTimer > 0.0)
        this.anim.SetBool("Action", true);
      if ((double) this.edgeCaseTimer <= 0.0)
        this.anim.SetBool("Edge Case", false);
      if ((double) this.edgeCaseTimer > 0.0)
        this.anim.SetBool("Edge Case", true);
    }
    this.actionTimer -= Time.deltaTime;
    this.edgeCaseTimer -= Time.deltaTime;
    if ((double) this.actionTimer <= 0.0)
      this.actionTimer = 0.0f;
    if ((double) this.edgeCaseTimer > 0.0)
      return;
    this.edgeCaseTimer = 0.0f;
  }
}
