// Decompiled with JetBrains decompiler
// Type: SyncedAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class SyncedAnimation : MonoBehaviour
{
  private PhotonView view;
  private Animator anim;
  private float syncCounter;

  private void Start()
  {
    this.view = this.GetComponent<PhotonView>();
    this.anim = this.GetComponent<Animator>();
  }

  private void Update()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.syncCounter += Time.deltaTime;
    if ((double) this.syncCounter <= 5.0)
      return;
    this.view.RPC("RPCA_SyncAnim", RpcTarget.All, (object) (float) ((double) this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0));
    this.syncCounter = 0.0f;
  }

  [PunRPC]
  public void RPCA_SyncAnim(float syncTime)
  {
    this.anim.Play(this.anim.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, syncTime);
  }
}
