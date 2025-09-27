// Decompiled with JetBrains decompiler
// Type: TickTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class TickTrigger : MonoBehaviour
{
  public float tickChance = 0.01f;

  private void Start()
  {
    if ((double) Random.value <= (double) this.tickChance)
      return;
    Object.Destroy((Object) this.gameObject);
  }

  private void OnTriggerEnter(Collider other)
  {
    Character componentInParent = other.GetComponentInParent<Character>();
    if (!(bool) (Object) componentInParent || !componentInParent.IsLocal)
      return;
    foreach (KeyValuePair<Bugfix, Character> allAttachedBug in Bugfix.AllAttachedBugs)
    {
      if ((Object) allAttachedBug.Value == (Object) componentInParent)
        return;
    }
    PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity).GetComponent<PhotonView>().RPC("AttachBug", RpcTarget.All, (object) componentInParent.photonView.ViewID);
    Object.Destroy((Object) this.gameObject);
  }
}
