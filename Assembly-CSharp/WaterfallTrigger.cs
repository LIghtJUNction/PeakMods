// Decompiled with JetBrains decompiler
// Type: WaterfallTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WaterfallTrigger : MonoBehaviour
{
  public float force = 5f;
  public float drag = 0.9f;

  private void OnTriggerStay(Collider other)
  {
    if (!(bool) (Object) other.gameObject.GetComponentInParent<Character>())
      return;
    Rigidbody attachedRigidbody = other.attachedRigidbody;
    if (!(bool) (Object) attachedRigidbody)
      return;
    attachedRigidbody.AddForce(this.transform.forward * this.force, ForceMode.Acceleration);
    attachedRigidbody.linearVelocity *= this.drag;
  }
}
