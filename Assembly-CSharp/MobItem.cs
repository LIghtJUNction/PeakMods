// Decompiled with JetBrains decompiler
// Type: MobItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MobItem : Item
{
  protected Mob mob;
  private MobItemPhysicsSyncer syncer;
  public float sleepDistance = 50f;
  public Animator anim;
  public GameObject rend;
  private bool sleeping;

  protected override void Awake()
  {
    base.Awake();
    this.mob = this.GetComponent<Mob>();
    this.syncer = this.GetComponent<MobItemPhysicsSyncer>();
  }

  protected override void Start()
  {
    base.Start();
    this.mob.forceNoMovement = this.itemState != ItemState.Ground || !this.photonView.IsMine;
    if (this.cooking.timesCookedLocal > 0)
      this.mob.anim.Play("ScorpionCooked", 0, 1f);
    if (this.itemState != ItemState.Held || !((Object) Character.localCharacter.data.currentItem == (Object) this))
      return;
    this.mob.SetForcedTarget(Character.localCharacter);
  }

  protected override void Update()
  {
    this.TestSleepMode();
    this.syncer.shouldSync = !this.sleeping;
    this.mob.enabled = !this.sleeping;
    this.rend.gameObject.SetActive(!this.sleeping || this.mob.mobState != Mob.MobState.Walking);
    if (this.anim.enabled && this.sleeping)
      this.anim.enabled = false;
    else if (!this.anim.enabled && !this.sleeping)
      this.anim.enabled = true;
    this.UIData.hasMainInteract = this.cooking.timesCookedLocal > 0;
    this.canUseOnFriend = this.cooking.timesCookedLocal > 0;
    if (this.cooking.timesCookedLocal > 0)
    {
      this.mob.mobState = Mob.MobState.Dead;
    }
    else
    {
      if (this.sleeping)
        return;
      this.mob.forceNoMovement = this.itemState != ItemState.Ground || !this.photonView.IsMine;
      if (!this.photonView.IsMine || this.itemState != ItemState.Ground)
        return;
      this.ForceSyncForFrames();
    }
  }

  public override bool CanUsePrimary() => this.cooking.timesCookedLocal > 0;

  public bool IsNearCharacter()
  {
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if ((Object) allCharacter != (Object) null && (double) Vector3.Distance(this.Center(), allCharacter.Center) < (double) this.sleepDistance)
        return true;
    }
    return false;
  }

  private void TestSleepMode()
  {
    if (this.sleeping)
    {
      if (!this.IsNearCharacter())
        return;
      this.sleeping = false;
    }
    else
    {
      if (this.mob.mobState != Mob.MobState.Walking || this.itemState != ItemState.Ground || (double) this.rig.linearVelocity.magnitude >= 1.0 || this.IsNearCharacter())
        return;
      this.sleeping = true;
    }
  }
}
