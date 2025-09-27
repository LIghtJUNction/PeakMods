// Decompiled with JetBrains decompiler
// Type: ItemCooking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

#nullable disable
public class ItemCooking : ItemComponent
{
  public int preCooked;
  [SerializeField]
  protected bool disableCooking;
  [FormerlySerializedAs("burnInstantly")]
  public bool wreckWhenCooked;
  private Renderer[] renderers;
  private Color[] defaultTints;
  private bool setup;
  public static Color DefaultCookColorMultiplier = new Color(0.66f, 0.47f, 0.25f);
  public static Color BurntCookColorMultiplier = new Color(0.05f, 0.05f, 0.1f);
  public const int COOKING_MAX = 12;

  public int timesCookedLocal { get; protected set; }

  public bool canBeCooked => !this.disableCooking;

  public override void OnInstanceDataSet() => this.UpdateCookedBehavior();

  public virtual void UpdateCookedBehavior()
  {
    IntItemData data = this.item.GetData<IntItemData>(DataEntryKey.CookedAmount);
    if (data.Value == 0)
      data.Value += this.preCooked;
    if (!this.setup)
    {
      this.setup = true;
      this.renderers = (Renderer[]) this.GetComponentsInChildren<MeshRenderer>();
      Renderer[] componentsInChildren = (Renderer[]) this.GetComponentsInChildren<SkinnedMeshRenderer>(true);
      if (componentsInChildren.Length != 0)
        this.renderers = ((IEnumerable<Renderer>) this.renderers).Concat<Renderer>((IEnumerable<Renderer>) componentsInChildren).ToArray<Renderer>();
      this.defaultTints = new Color[this.renderers.Length];
      for (int index = 0; index < this.renderers.Length; ++index)
        this.defaultTints[index] = this.renderers[index].material.GetColor("_Tint");
    }
    int num = data.Value - this.timesCookedLocal;
    this.CookVisually(data.Value);
    if (num > 0)
    {
      for (int totalCooked = 1 + this.timesCookedLocal; totalCooked <= data.Value; ++totalCooked)
        this.ChangeStatsCooked(totalCooked);
    }
    this.timesCookedLocal = data.Value;
  }

  protected virtual void CookVisually(int cookedAmount)
  {
    if (cookedAmount <= 0)
      return;
    for (int index1 = 0; index1 < this.renderers.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.renderers[index1].materials.Length; ++index2)
        this.renderers[index1].materials[index2].SetColor("_Tint", this.defaultTints[index1] * ItemCooking.GetCookColor(cookedAmount));
    }
  }

  public static Color GetCookColor(int cookAmount)
  {
    Color cookColor = Color.white;
    switch (cookAmount)
    {
      case 1:
        cookColor = ItemCooking.DefaultCookColorMultiplier;
        break;
      case 2:
        cookColor = ItemCooking.DefaultCookColorMultiplier * 0.5f;
        break;
      default:
        if (cookAmount > 2)
        {
          cookColor = ItemCooking.BurntCookColorMultiplier;
          break;
        }
        break;
    }
    cookColor.a = 1f;
    return cookColor;
  }

  [PunRPC]
  private void FinishCookingRPC()
  {
    this.CancelCookingVisuals();
    IntItemData data = this.GetData<IntItemData>(DataEntryKey.CookedAmount);
    if (this.wreckWhenCooked)
      data.Value = 5;
    else if (data.Value < 12)
      ++data.Value;
    this.item.WasActive();
    this.UpdateCookedBehavior();
  }

  public void StartCookingVisuals()
  {
    this.photonView.RPC("EnableCookingSmokeRPC", RpcTarget.All, (object) true);
  }

  [PunRPC]
  private void EnableCookingSmokeRPC(bool active) => this.item.particles.EnableSmoke(active);

  private void ChangeStatsCooked(int totalCooked)
  {
    if (this.wreckWhenCooked && totalCooked > 0)
    {
      ItemComponent[] components1 = this.GetComponents<ItemComponent>();
      for (int index = components1.Length - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) components1[index] != (UnityEngine.Object) this)
          UnityEngine.Object.Destroy((UnityEngine.Object) components1[index]);
      }
      ItemAction[] components2 = this.GetComponents<ItemAction>();
      for (int index = components2.Length - 1; index >= 0; --index)
        UnityEngine.Object.Destroy((UnityEngine.Object) components2[index]);
      this.item.overrideUsability = Optionable<bool>.Some(false);
    }
    else
    {
      Action_RestoreHunger component = this.GetComponent<Action_RestoreHunger>();
      if ((bool) (UnityEngine.Object) component)
      {
        if (totalCooked < 2)
          component.restorationAmount *= 2f;
        else if (totalCooked > 2)
          component.restorationAmount = Mathf.Max(component.restorationAmount - 0.05f, 0.0f);
      }
      Action_GiveExtraStamina giveExtraStamina = this.GetComponent<Action_GiveExtraStamina>();
      if (!(bool) (UnityEngine.Object) giveExtraStamina)
      {
        giveExtraStamina = this.gameObject.AddComponent<Action_GiveExtraStamina>();
        giveExtraStamina.OnConsumed = true;
      }
      if (totalCooked < 2)
        giveExtraStamina.amount = Mathf.Max(0.1f, giveExtraStamina.amount * 1.5f);
      else if (totalCooked > 2)
        giveExtraStamina.amount = 0.0f;
      Action_ModifyStatus actionModifyStatus = ((IEnumerable<Action_ModifyStatus>) this.GetComponents<Action_ModifyStatus>()).FirstOrDefault<Action_ModifyStatus>((Func<Action_ModifyStatus, bool>) (a => a.statusType == CharacterAfflictions.STATUSTYPE.Poison));
      this.GetComponent<Action_InflictPoison>();
      if (totalCooked <= 3)
        return;
      if (!(bool) (UnityEngine.Object) actionModifyStatus)
      {
        actionModifyStatus = this.gameObject.AddComponent<Action_ModifyStatus>();
        actionModifyStatus.OnConsumed = true;
        actionModifyStatus.statusType = CharacterAfflictions.STATUSTYPE.Poison;
      }
      actionModifyStatus.changeAmount += 0.1f;
    }
  }

  public void CancelCookingVisuals()
  {
    this.photonView.RPC("EnableCookingSmokeRPC", RpcTarget.All, (object) false);
  }

  public void FinishCooking()
  {
    if (!this.photonView.AmController)
      return;
    this.photonView.RPC("FinishCookingRPC", RpcTarget.All);
    if ((bool) (UnityEngine.Object) this.item.holderCharacter)
    {
      Action<ItemSlot[]> itemsChangedAction = this.item.holderCharacter.player.itemsChangedAction;
      if (itemsChangedAction != null)
        itemsChangedAction(this.item.holderCharacter.player.itemSlots);
      if ((bool) (UnityEngine.Object) this.item.holderCharacter.GetComponent<CharacterItems>() && (bool) (UnityEngine.Object) this.item.holderCharacter.GetComponent<CharacterItems>().cookSfx)
        this.item.holderCharacter.GetComponent<CharacterItems>().cookSfx.Play(this.transform.position);
    }
    Debug.Log((object) "Cooking Finished");
  }
}
