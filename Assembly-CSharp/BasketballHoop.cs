// Decompiled with JetBrains decompiler
// Type: BasketballHoop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BasketballHoop : MonoBehaviour
{
  public Animator anim;
  public ParticleSystem confetti;
  public SFX_PlayOneShot success;
  private float lastScoredTime;
  private Rigidbody ballRb;

  private void OnTriggerEnter(Collider other)
  {
    if (!((Object) other.attachedRigidbody != (Object) null))
      return;
    Item component = other.attachedRigidbody.GetComponent<Item>();
    if (!((Object) component != (Object) null) || (double) component.transform.position.y <= (double) this.transform.position.y)
      return;
    this.ballRb = other.attachedRigidbody;
  }

  private void OnTriggerExit(Collider other)
  {
    if (!((Object) other.attachedRigidbody != (Object) null) || !((Object) other.attachedRigidbody == (Object) this.ballRb) || (double) other.attachedRigidbody.linearVelocity.y >= 0.0 || (double) this.ballRb.transform.position.y >= (double) this.transform.position.y || !((Object) other.attachedRigidbody.GetComponent<Item>() != (Object) null) || (double) Time.time <= (double) this.lastScoredTime + 2.0)
      return;
    this.ballRb = (Rigidbody) null;
    this.confetti.Play();
    this.success.Play();
    this.anim.SetTrigger("Score");
    this.lastScoredTime = Time.time;
  }
}
