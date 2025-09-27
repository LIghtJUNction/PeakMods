// Decompiled with JetBrains decompiler
// Type: ShittyPiton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
public class ShittyPiton : MonoBehaviourPunCallbacks
{
  private ClimbHandle handle;
  private PhotonView view;
  private float totalSecondsOfHang;
  public GameObject crack;
  public GameObject vfx;
  private float crackScale;
  private int cracksToBreak = 4;
  private float sinceCrack = 10f;
  private bool disabled;
  public SFX_Instance[] cracKSFX;
  public SFX_Instance[] breakSFX;
  private bool isHung;
  private bool isBreaking;

  private void Start()
  {
    this.view = this.GetComponent<PhotonView>();
    this.handle = this.GetComponent<ClimbHandle>();
    this.handle.onHangStart += new Action<Character>(this.OnHang);
    this.handle.onHangStop += new Action(this.OnHangStop);
    this.totalSecondsOfHang = UnityEngine.Random.Range(1f, 5f);
  }

  private void OnHangStop() => this.isHung = false;

  private void OnHang(Character character) => this.isHung = true;

  private void Update()
  {
    if (this.isBreaking)
    {
      if (this.isHung)
        this.sinceCrack += Time.deltaTime;
      if ((double) this.sinceCrack > 1.5)
      {
        this.Crack();
        this.sinceCrack = 0.0f;
      }
      this.crack.transform.localScale = Vector3.Lerp(this.crack.transform.localScale, Vector3.one * this.crackScale, Time.deltaTime * 15f);
    }
    else
    {
      if (!this.view.IsMine || !this.isHung)
        return;
      this.totalSecondsOfHang -= Time.deltaTime;
      if ((double) this.totalSecondsOfHang >= 0.0)
        return;
      this.view.RPC("RPCA_StartBreaking", RpcTarget.All);
    }
  }

  private void Crack()
  {
    this.crackScale += 0.75f;
    --this.cracksToBreak;
    GamefeelHandler.instance.AddPerlinShakeProximity(this.transform.position, 2f + this.crackScale);
    for (int index = 0; index < this.cracKSFX.Length; ++index)
      this.cracKSFX[index].Play(this.transform.position);
    if (this.cracksToBreak > 0 || !this.view.IsMine)
      return;
    this.view.RPC("RPCA_Break", RpcTarget.All);
  }

  [PunRPC]
  private void RPCA_Break()
  {
    this.vfx.transform.SetParent((Transform) null);
    this.vfx.SetActive(true);
    for (int index = 0; index < this.breakSFX.Length; ++index)
      this.breakSFX[index].Play(this.transform.position);
    this.disabled = true;
    this.crack.transform.SetParent((Transform) null);
    this.handle.Break();
  }

  [PunRPC]
  public void RPCA_StartBreaking()
  {
    this.isBreaking = true;
    this.crack.SetActive(true);
    this.crack.transform.localScale *= 0.0f;
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!this.disabled || newPlayer.IsLocal || !PhotonNetwork.IsMasterClient)
      return;
    this.view.RPC("RPCA_Break", newPlayer);
  }
}
