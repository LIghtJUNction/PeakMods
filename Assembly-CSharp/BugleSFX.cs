// Decompiled with JetBrains decompiler
// Type: BugleSFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class BugleSFX : MonoBehaviourPun
{
  private Item item;
  public bool hold;
  private bool t;
  private int currentClip;
  public AudioClip[] bugle;
  public AudioSource buglePlayer;
  public AudioSource bugleEnd;
  public MagicBugle magicBugle;
  public ParticleSystem particle1;
  public ParticleSystem particle2;
  public float currentPitch;
  public float pitchMin = 0.7f;
  public float pitchMax = 1.3f;
  public float volume = 0.35f;
  public bool isProp;

  private void Start() => this.item = this.GetComponent<Item>();

  private void UpdateTooting()
  {
    if (!this.photonView.IsMine)
      return;
    bool flag = this.item.isUsingPrimary;
    if ((bool) (Object) this.magicBugle && (double) this.magicBugle.currentFuel <= 0.019999999552965164)
      flag = false;
    if (flag == this.hold)
      return;
    if (flag)
      this.photonView.RPC("RPC_StartToot", RpcTarget.All, (object) Random.Range(0, this.bugle.Length), (object) (float) (((double) Vector3.Dot(this.item.holderCharacter.data.lookDirection, Vector3.up) + 1.0) / 2.0));
    else
      this.photonView.RPC("RPC_EndToot", RpcTarget.All);
    this.hold = flag;
  }

  [PunRPC]
  private void RPC_StartToot(int clip, float pitch)
  {
    this.currentPitch = pitch;
    this.currentClip = clip;
    this.hold = true;
    if (!(bool) (Object) this.particle1 || !(bool) (Object) this.particle2)
      return;
    if (!this.particle1.isPlaying)
      this.particle1.Play();
    if (!this.particle2.isPlaying)
      this.particle2.Play();
    ParticleSystem.EmissionModule emission1 = this.particle1.emission;
    ParticleSystem.EmissionModule emission2 = this.particle2.emission;
    emission1.enabled = true;
    emission2.enabled = true;
  }

  [PunRPC]
  private void RPC_EndToot()
  {
    this.hold = false;
    if (!(bool) (Object) this.particle1 || !(bool) (Object) this.particle2)
      return;
    ParticleSystem.EmissionModule emission1 = this.particle1.emission;
    ParticleSystem.EmissionModule emission2 = this.particle2.emission;
    emission1.enabled = false;
    emission2.enabled = false;
  }

  private void Update()
  {
    this.UpdateTooting();
    if (this.hold && !this.t && !this.isProp)
    {
      this.buglePlayer.clip = this.bugle[this.currentClip];
      this.buglePlayer.pitch = Mathf.Lerp(this.pitchMin, this.pitchMax, this.currentPitch);
      this.buglePlayer.Play();
      this.buglePlayer.volume = 0.0f;
      this.t = true;
    }
    this.item.defaultPos = new Vector3(this.item.defaultPos.x, this.hold ? 0.5f : 0.0f, this.item.defaultPos.z);
    if (this.hold)
      this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, this.volume, 10f * Time.deltaTime);
    if (!this.hold)
      this.buglePlayer.volume = Mathf.Lerp(this.buglePlayer.volume, 0.0f, 10f * Time.deltaTime);
    if (this.hold || !this.t)
      return;
    this.t = false;
  }
}
