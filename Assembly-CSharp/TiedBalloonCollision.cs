// Decompiled with JetBrains decompiler
// Type: TiedBalloonCollision
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TiedBalloonCollision : MonoBehaviour
{
  public TiedBalloon tiedBalloon;

  private void OnCollisionEnter(Collision collision)
  {
    if (!this.tiedBalloon.photonView.IsMine || !(bool) (Object) collision.collider.GetComponent<StickyCactus>())
      return;
    this.tiedBalloon.Pop();
  }
}
