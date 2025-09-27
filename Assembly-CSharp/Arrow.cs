// Decompiled with JetBrains decompiler
// Type: Arrow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Arrow : MonoBehaviour
{
  public bool isStuck = true;
  public Rigidbody arrowRB;
  public Collider arrowCollider;

  private void Start()
  {
  }

  public void stuckArrow(bool stuck)
  {
    this.isStuck = stuck;
    this.arrowRB.isKinematic = stuck;
    this.arrowCollider.enabled = !stuck;
  }

  private void Update()
  {
    if (!((Object) this.transform.parent == (Object) null) || !this.isStuck)
      return;
    this.stuckArrow(false);
  }

  private void OnDrawGizmosSelected()
  {
  }
}
