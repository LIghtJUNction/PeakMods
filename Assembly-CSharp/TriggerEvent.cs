// Decompiled with JetBrains decompiler
// Type: TriggerEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
public class TriggerEvent : MonoBehaviour
{
  [Range(0.0f, 1f)]
  public float triggerChance = 1f;
  public bool onlyOnce;
  public bool waitForRenderFrame = true;
  public UnityEvent triggerEvent;
  private PhotonView view;
  private bool hasActivated;
  private List<Character> hits = new List<Character>();

  private void Start() => this.view = this.GetComponentInParent<PhotonView>();

  private void OnTriggerEnter(Collider other)
  {
    if (other.isTrigger)
      return;
    Character player = other.GetComponentInParent<Character>();
    if (!(bool) (Object) player || this.hits.Contains(player))
      return;
    this.StartCoroutine(IHoldHit());
    this.TriggerEntered();

    IEnumerator IHoldHit()
    {
      this.hits.Add(player);
      yield return (object) new WaitForSeconds(1f);
      this.hits.Remove(player);
    }
  }

  public void TriggerEntered()
  {
    if (this.onlyOnce && this.hasActivated || !this.view.IsMine || (double) this.triggerChance < (double) Random.value)
      return;
    this.view.RPC("RPCA_Trigger", RpcTarget.All, (object) this.transform.GetSiblingIndex());
  }

  public void Trigger() => GameUtils.instance.StartCoroutine(this.TriggerRoutine());

  private IEnumerator TriggerRoutine()
  {
    if (!this.onlyOnce || !this.hasActivated)
    {
      if (this.waitForRenderFrame)
        yield return (object) new WaitForEndOfFrame();
      this.triggerEvent.Invoke();
      this.hasActivated = true;
    }
  }
}
