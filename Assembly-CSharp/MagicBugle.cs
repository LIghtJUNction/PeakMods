// Decompiled with JetBrains decompiler
// Type: MagicBugle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

#nullable disable
public class MagicBugle : ItemComponent
{
  public float initialTootCost;
  public float totalTootTime;
  private bool tooting;
  [SerializeField]
  [ReadOnly]
  private float fuel;
  public Action_ApplyMassAffliction massAffliction;
  private float tootTick;

  public float currentFuel => this.fuel;

  public override void Awake()
  {
    base.Awake();
    this.item.OnPrimaryStarted += new Action(this.StartToot);
    this.item.OnPrimaryCancelled += new Action(this.CancelToot);
  }

  public void OnDestroy()
  {
    this.item.OnPrimaryHeld -= new Action(this.StartToot);
    this.item.OnPrimaryCancelled -= new Action(this.CancelToot);
  }

  public override void OnInstanceDataSet()
  {
    if (this.HasData(DataEntryKey.Fuel))
    {
      this.fuel = this.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
      this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
    }
    else
    {
      if (!this.photonView.IsMine)
        return;
      this.fuel = this.totalTootTime;
      this.item.SetUseRemainingPercentage(1f);
    }
  }

  private void Update() => this.UpdateToot();

  private void UpdateToot()
  {
    if (!this.tooting || !this.photonView.IsMine)
      return;
    this.fuel -= Time.deltaTime;
    if ((double) this.fuel <= 0.0)
    {
      this.fuel = 0.0f;
      this.CancelToot();
    }
    else
    {
      this.tootTick -= Time.deltaTime;
      if ((double) this.tootTick <= 0.0)
      {
        this.massAffliction.RunAction();
        this.tootTick = 0.1f;
      }
    }
    this.GetData<FloatItemData>(DataEntryKey.Fuel).Value = this.fuel;
    this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
  }

  private void StartToot()
  {
    Debug.Log((object) "Started toot");
    if ((double) this.fuel < (double) this.initialTootCost)
      return;
    this.fuel -= this.initialTootCost;
    this.tooting = true;
    this.item.SetUseRemainingPercentage(this.fuel / this.totalTootTime);
  }

  private void CancelToot()
  {
    Debug.Log((object) "Cancelled toot");
    this.tooting = false;
  }
}
