// Decompiled with JetBrains decompiler
// Type: InventoryItemUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class InventoryItemUI : MonoBehaviour
{
  public RectTransform rectTransform;
  public RawImage icon;
  public Image fill;
  public Image selectedSlotIcon;
  public Texture defaultIcon;
  public TextMeshProUGUI nameText;
  public bool isBackpack;
  public GameObject backpackFilledSlotsObject;
  public TextMeshProUGUI backpackFilledSlotsAmountText;
  private DG.Tweening.Sequence mySequence;
  private Item _itemPrefab;
  private bool _hasBackpack;
  public GameObject fuelBar;
  public Image fuelBarFill;
  public Texture backpackIcon;
  public Texture carryingIcon;
  public ItemInstanceData _itemData;
  private int cookedAmount;
  public bool isTemporarySlot;
  private Vector2 startingSizeDelta;

  public void Start() => this.startingSizeDelta = this.rectTransform.sizeDelta;

  private void UpdateCookedAmount()
  {
    if (this._itemData == null)
    {
      this.cookedAmount = 0;
      this.icon.color = Color.white;
    }
    else
    {
      IntItemData intItemData;
      if (!this._itemData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || this.cookedAmount == intItemData.Value)
        return;
      this.icon.color = Color.white;
      this.icon.color = ItemCooking.GetCookColor(intItemData.Value);
      this.cookedAmount = intItemData.Value;
    }
  }

  public void SetItem(ItemSlot slot)
  {
    if (this.isBackpack)
    {
      if ((bool) (Object) Character.observedCharacter.data.carriedPlayer)
      {
        this.icon.color = Character.observedCharacter.data.carriedPlayer.refs.customization.PlayerColor;
        this.icon.texture = this.carryingIcon;
        this.backpackFilledSlotsObject.SetActive(false);
      }
      else
      {
        this.icon.texture = this.backpackIcon;
        if (slot.IsEmpty())
        {
          this._hasBackpack = false;
          this.icon.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
          this.backpackFilledSlotsObject.SetActive(false);
          this.fill.enabled = false;
        }
        else
        {
          this._hasBackpack = true;
          this.icon.color = Color.white;
          BackpackData backpackData;
          if (!((Object) this.backpackFilledSlotsObject != (Object) null) || !slot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
            return;
          int num = backpackData.FilledSlotCount();
          this.backpackFilledSlotsObject.SetActive(num > 0);
          this.backpackFilledSlotsAmountText.text = num.ToString();
        }
      }
    }
    else if ((Object) this._itemPrefab == (Object) slot.prefab)
    {
      this.TrySetFuel(slot.data);
      this.UpdateNameText();
      this.UpdateCookedAmount();
    }
    else
    {
      this._itemPrefab = slot.prefab;
      this._itemData = slot.data;
      this.UpdateNameText();
      this.UpdateCookedAmount();
      this.SetSelected();
      if (!slot.IsEmpty())
      {
        if ((Object) this._itemPrefab == (Object) null)
        {
          this.icon.transform.localScale = Vector3.zero;
          this.icon.transform.DOScale(1f, 0.5f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutElastic);
        }
        this.icon.texture = (Texture) this._itemPrefab.UIData.GetIcon();
        this.icon.enabled = true;
        this.TrySetFuel(slot.data);
      }
      else
        this.Clear();
    }
  }

  public void Clear()
  {
    this.fill.enabled = false;
    this.icon.enabled = false;
    this._itemPrefab = (Item) null;
    this._itemData = (ItemInstanceData) null;
    this.nameText.enabled = false;
    this.nameText.text = "";
    this.TrySetFuel((ItemInstanceData) null);
  }

  public void TrySetFuel(ItemInstanceData data)
  {
    if (!(bool) (Object) this.fuelBar)
      return;
    if ((Object) Character.observedCharacter != (Object) Character.localCharacter)
      this.fuelBar.SetActive(false);
    else if (data == null || (Object) this._itemPrefab == (Object) null || !data.HasData(DataEntryKey.UseRemainingPercentage))
    {
      this.fuelBar.SetActive(false);
      this.fuelBarFill.fillAmount = 1f;
    }
    else
    {
      this.fuelBar.SetActive(true);
      FloatItemData floatItemData;
      if (!data.TryGetDataEntry<FloatItemData>(DataEntryKey.UseRemainingPercentage, out floatItemData))
        return;
      this.fuelBarFill.fillAmount = floatItemData.Value;
    }
  }

  public void UpdateNameText()
  {
    string str = (Object) this._itemPrefab != (Object) null || this.isBackpack && this._hasBackpack ? (!((Object) this._itemPrefab != (Object) null) ? "Backpack" : this._itemPrefab.GetItemName(this._itemData)) : "";
    if (this.nameText.text != str)
      this.SetSelected();
    this.nameText.text = str;
  }

  public void SetSelected()
  {
    Optionable<byte> currentSelectedSlot = Character.observedCharacter.refs.items.currentSelectedSlot;
    bool flag = currentSelectedSlot.IsSome && (int) currentSelectedSlot.Value == this.transform.GetSiblingIndex();
    if (this.isTemporarySlot)
      flag = true;
    if (this.isBackpack)
      flag = currentSelectedSlot.Value == (byte) 3;
    if ((Object) this._itemPrefab != (Object) null || this.isBackpack && (this._hasBackpack || (bool) (Object) Character.observedCharacter.data.carriedPlayer) || this.isTemporarySlot)
    {
      if (flag)
      {
        this.mySequence.Kill();
        this.rectTransform.DOKill();
        this.rectTransform.DOSizeDelta(this.startingSizeDelta * 1.2f, 0.5f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutElastic);
        this.fill.enabled = true;
        this.fill.transform.localScale = Vector3.zero;
        this.fill.transform.DOKill();
        this.fill.transform.DOScale(1f, 0.25f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutCubic);
        this.nameText.enabled = true;
      }
      else
      {
        this.mySequence.Kill();
        this.rectTransform.DOKill();
        this.rectTransform.DOSizeDelta(this.startingSizeDelta, 0.2f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutCubic);
        this.fill.enabled = false;
        this.nameText.enabled = false;
      }
    }
    else if (flag)
    {
      this.mySequence.Kill();
      this.mySequence = DOTween.Sequence();
      this.mySequence.Append((Tween) this.rectTransform.DOSizeDelta(this.startingSizeDelta * 1.2f, 0.075f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutCubic));
      this.mySequence.Append((Tween) this.rectTransform.DOSizeDelta(this.startingSizeDelta, 0.125f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.InSine));
    }
    else
    {
      this.mySequence.Kill();
      this.rectTransform.DOKill();
      this.rectTransform.sizeDelta = this.startingSizeDelta;
    }
  }

  private void OnDisable()
  {
    this.mySequence.Kill();
    this.rectTransform.DOKill();
    this.rectTransform.sizeDelta = this.startingSizeDelta;
    this.fill.enabled = false;
    this.nameText.enabled = false;
    this.nameText.text = "";
  }
}
