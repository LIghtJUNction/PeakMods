// Decompiled with JetBrains decompiler
// Type: BingBongTimeControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class BingBongTimeControl : MonoBehaviour
{
  private PhotonView view;
  public float currentTimeScale = 1f;
  private float syncCounter;
  private BingBongPowers bingBongPowers;
  private string descr = "Reset time: [R]\n\nFreeze: [F]\n\nFaster: [LMB]\n\nSlower: [RMB]";

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void Update()
  {
    this.syncCounter += Time.unscaledDeltaTime;
    if (Input.GetKeyDown(KeyCode.R))
      this.currentTimeScale = 1f;
    if (Input.GetKeyDown(KeyCode.F))
      this.currentTimeScale = 0.0f;
    if (Input.GetKeyDown(KeyCode.Mouse0))
      this.currentTimeScale += Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
    if (Input.GetKeyDown(KeyCode.Mouse1))
      this.currentTimeScale -= Mathf.Clamp(0.1f, this.currentTimeScale * 0.3f, 0.5f);
    this.currentTimeScale = Mathf.Clamp(this.currentTimeScale, 0.02f, 10f);
    if ((double) Time.timeScale == (double) this.currentTimeScale)
      return;
    this.bingBongPowers.SetTip($"Time Scale: {this.currentTimeScale:P0}", 1);
    if ((double) this.syncCounter <= 0.10000000149011612)
      return;
    this.view.RPC("RPCA_SyncTime", RpcTarget.All, (object) this.currentTimeScale);
  }

  [PunRPC]
  public void RPCA_SyncTime(float newTime) => Time.timeScale = newTime;

  private void OnDestroy() => Time.timeScale = 1f;

  private void OnEnable()
  {
    this.bingBongPowers = this.GetComponent<BingBongPowers>();
    this.bingBongPowers.SetTexts("TIME", this.descr);
  }
}
