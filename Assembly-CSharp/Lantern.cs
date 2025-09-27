// Decompiled with JetBrains decompiler
// Type: Lantern
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
public class Lantern : ItemComponent
{
  [SerializeField]
  private bool lit;
  public string actionPromptWhenUnlit;
  public string actionPromptWhenLit;
  public float useTimeWhenUnlit;
  public float useTimeWhenLit;
  public Light lanternLight;
  public AnimationCurve lightCurve;
  public float lightSpeed;
  public float lightIntensity = 10f;
  public float startingFuel;
  [SerializeField]
  private float fuel;
  public ParticleSystem fireParticle;
  private new Item item;
  private float currentLightIntensity;
  public bool activeByDefault;

  public override void Awake()
  {
    base.Awake();
    this.item = this.GetComponent<Item>();
  }

  public override void OnEnable() => this.item.onStashAction += new Action(this.SnuffLantern);

  public override void OnDisable() => this.item.onStashAction -= new Action(this.SnuffLantern);

  private void Start()
  {
    if (this.HasData(DataEntryKey.FlareActive) && this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value)
    {
      this.fireParticle.main.prewarm = true;
      this.fireParticle.Play();
    }
    if (!this.activeByDefault || this.item.itemState != ItemState.Held)
      return;
    this.lanternLight.gameObject.SetActive(false);
    this.fireParticle.Stop();
  }

  public override void OnInstanceDataSet()
  {
    if (this.HasData(DataEntryKey.FlareActive))
      this.lit = this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value;
    this.fuel = this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value;
    this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
  }

  private void Update()
  {
    if (!PhotonNetwork.InRoom || this.activeByDefault && this.item.rig.isKinematic)
      return;
    if (this.lanternLight.gameObject.activeSelf != this.lit)
    {
      this.lanternLight.gameObject.SetActive(this.lit);
      if (this.lit)
      {
        this.fireParticle.Play();
        this.currentLightIntensity = 0.0f;
        DOTween.To((DOGetter<float>) (() => this.currentLightIntensity), (DOSetter<float>) (x => this.currentLightIntensity = x), this.lightIntensity, 1f);
      }
      else
      {
        this.fireParticle.Clear();
        this.fireParticle.Stop();
      }
    }
    this.lanternLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.currentLightIntensity;
    this.item.UIData.mainInteractPrompt = this.lit ? this.actionPromptWhenLit : this.actionPromptWhenUnlit;
    this.item.usingTimePrimary = this.lit ? this.useTimeWhenLit : this.useTimeWhenUnlit;
    this.GetData<OptionableIntItemData>(DataEntryKey.ItemUses).Value = (double) this.fuel > 0.0 ? -1 : 0;
    this.UpdateFuel();
  }

  private void UpdateFuel()
  {
    if (!this.lit || !this.photonView.IsMine)
      return;
    this.fuel -= Time.deltaTime;
    if ((double) this.fuel <= 0.0)
    {
      this.fuel = 0.0f;
      this.SnuffLantern();
    }
    this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.SetupDefaultFuel)).Value = this.fuel;
    this.item.SetUseRemainingPercentage(this.fuel / this.startingFuel);
  }

  private FloatItemData SetupDefaultFuel()
  {
    return new FloatItemData() { Value = this.startingFuel };
  }

  public void ToggleLantern()
  {
    this.photonView.RPC("LightLanternRPC", RpcTarget.All, (object) !this.lit);
  }

  public void SnuffLantern()
  {
    this.photonView.RPC("LightLanternRPC", RpcTarget.All, (object) false);
  }

  [PunRPC]
  public void LightLanternRPC(bool litValue)
  {
    this.fireParticle.main.prewarm = false;
    this.lit = litValue;
    this.GetData<BoolItemData>(DataEntryKey.FlareActive).Value = this.lit;
  }
}
