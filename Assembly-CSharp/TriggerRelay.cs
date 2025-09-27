// Decompiled with JetBrains decompiler
// Type: TriggerRelay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class TriggerRelay : MonoBehaviour
{
  internal PhotonView view;

  private void Awake() => this.view = this.GetComponent<PhotonView>();

  [PunRPC]
  public void RPCA_Trigger(int childID)
  {
    this.transform.GetChild(childID).GetComponent<TriggerEvent>().Trigger();
  }

  [PunRPC]
  public void RPCA_TriggerWithTarget(int childID, int targetID)
  {
    this.transform.GetChild(childID).GetComponent<SlipperyJellyfish>().Trigger(targetID);
  }
}
