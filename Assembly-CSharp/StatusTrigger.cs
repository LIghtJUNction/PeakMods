// Decompiled with JetBrains decompiler
// Type: StatusTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using UnityEngine;

#nullable disable
public class StatusTrigger : MonoBehaviour
{
  public float cooldown = 1f;
  public bool addStatus;
  public CharacterAfflictions.STATUSTYPE statusType;
  public float statusAmount = 0.05f;
  public bool poisonOverTime;
  public float poisonOverTimeDuration = 5f;
  public float poisonOverTimeDelay = 1f;
  public float poisonOverTimeAmountPerSecond = 0.01f;
  private float counter;

  private void Update() => this.counter += Time.deltaTime;

  private void OnTriggerEnter(Collider other)
  {
    Character componentInParent = other.GetComponentInParent<Character>();
    if ((Object) componentInParent == (Object) null || !componentInParent.IsLocal || (double) this.counter < (double) this.cooldown)
      return;
    this.counter = 0.0f;
    if (this.addStatus)
      componentInParent.refs.afflictions.AddStatus(this.statusType, this.statusAmount);
    if (!this.poisonOverTime)
      return;
    componentInParent.refs.afflictions.AddAffliction((Affliction) new Affliction_PoisonOverTime(this.poisonOverTimeDuration, this.poisonOverTimeDelay, this.poisonOverTimeAmountPerSecond));
  }
}
