// Decompiled with JetBrains decompiler
// Type: ItemMoveZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ItemMoveZone : MonoBehaviour
{
  public float forceMultiplier = 1f;

  private void OnTriggerStay(Collider other)
  {
    if (!((Object) other.attachedRigidbody.GetComponent<Item>() != (Object) null))
      return;
    other.attachedRigidbody.MovePosition(other.attachedRigidbody.position + this.transform.forward * this.forceMultiplier * Time.fixedDeltaTime);
  }
}
