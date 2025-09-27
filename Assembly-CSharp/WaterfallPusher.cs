// Decompiled with JetBrains decompiler
// Type: WaterfallPusher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WaterfallPusher : MonoBehaviour
{
  public float fallTime = 0.5f;
  public float knockback = 25f;
  public float downwardKnockback = 25f;
  public float cooldown = 1f;
  private float cooldownTimer;
  public SFX_PlayOneShot sfx;

  private void OnTriggerEnter(Collider other)
  {
    Character componentInParent = other.GetComponentInParent<Character>();
    if (!(bool) (Object) componentInParent || !componentInParent.IsLocal || (double) Time.time <= (double) this.cooldown + (double) this.cooldownTimer)
      return;
    if ((double) this.fallTime > 0.0)
      componentInParent.Fall(this.fallTime);
    this.cooldownTimer = Time.time;
    this.sfx.Play();
    GamefeelHandler.instance.AddPerlinShake(30f, 0.8f, 20f);
    Vector3 vector3_1 = (componentInParent.Center - this.transform.position).normalized with
    {
      y = 0.0f
    } * this.knockback;
    Vector3 vector3_2 = Vector3.down * this.downwardKnockback;
    componentInParent.AddForce(vector3_1 + vector3_2, 0.7f, 1.3f);
  }
}
