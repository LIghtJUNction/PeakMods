// Decompiled with JetBrains decompiler
// Type: StatusEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class StatusEmitter : MonoBehaviour
{
  public CharacterAfflictions.STATUSTYPE statusType;
  public float amount;
  public float radius = 1f;
  public float outerFade;
  private float timeSinceLastTick;
  private float tickTime = 0.5f;

  public void Update()
  {
    this.timeSinceLastTick += Time.deltaTime;
    if ((double) this.timeSinceLastTick < (double) this.tickTime)
      return;
    foreach (CharacterAfflictions characterAfflictions in new HashSet<CharacterAfflictions>(((IEnumerable<Collider>) Physics.OverlapSphere(this.transform.position, this.radius + this.outerFade)).Select<Collider, CharacterAfflictions>((Func<Collider, CharacterAfflictions>) (hit => hit.GetComponentInParent<CharacterAfflictions>()))).Where<CharacterAfflictions>((Func<CharacterAfflictions, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) null)).Where<CharacterAfflictions>((Func<CharacterAfflictions, bool>) (c => c.character.photonView.IsMine)))
    {
      float amount = this.amount;
      if ((double) this.outerFade > 0.0099999997764825821)
      {
        float b = Vector3.Distance(characterAfflictions.character.Center, this.transform.position);
        amount *= Mathf.InverseLerp(this.radius + this.outerFade, b, b);
      }
      if ((double) amount > 0.0)
        characterAfflictions.AddStatus(this.statusType, this.amount * this.timeSinceLastTick);
      if ((double) amount < 0.0)
        characterAfflictions.SubtractStatus(this.statusType, Mathf.Abs(this.amount * this.timeSinceLastTick));
    }
    this.timeSinceLastTick = 0.0f;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, this.radius);
    Gizmos.color = new Color(1f, 0.0f, 0.0f, 0.2f);
    Gizmos.DrawWireSphere(this.transform.position, this.radius + this.outerFade);
  }

  private void Start()
  {
  }
}
