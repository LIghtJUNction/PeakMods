// Decompiled with JetBrains decompiler
// Type: EventOnItemCollision
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
public class EventOnItemCollision : MonoBehaviour
{
  public bool onlyWhenImKinematic;
  public UnityEvent eventOnCollided;
  private Rigidbody rb;
  public float minCollisionVelocity;
  public bool onlyOnce;
  private bool triggered;

  private void Awake() => this.rb = this.GetComponent<Rigidbody>();

  private void OnCollisionEnter(Collision collision)
  {
    if (this.onlyOnce && this.triggered || this.onlyWhenImKinematic && (Object) this.rb != (Object) null && !this.rb.isKinematic)
      return;
    Item componentInParent = collision.gameObject.GetComponentInParent<Item>();
    if ((Object) componentInParent == (Object) null || componentInParent.itemState != ItemState.Ground)
      return;
    Debug.Log((object) $"{this.gameObject.name} collided with {componentInParent.gameObject.name} at velocity {collision.relativeVelocity.magnitude}");
    if ((double) collision.relativeVelocity.magnitude <= (double) this.minCollisionVelocity)
      return;
    this.TriggerEvent();
  }

  internal void TriggerEvent()
  {
    if (this.onlyOnce && this.triggered)
      return;
    this.triggered = true;
    this.eventOnCollided?.Invoke();
  }
}
