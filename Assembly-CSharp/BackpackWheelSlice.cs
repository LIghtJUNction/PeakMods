// Decompiled with JetBrains decompiler
// Type: BackpackWheelSlice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
public class BackpackWheelSlice : 
  UIWheelSlice,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  private BackpackWheel backpackWheel;
  private BackpackReference backpack;
  private BackpackData backpackData;
  private ItemSlot itemSlot;
  public RawImage image;
  public bool stashSlice;
  private int cookedAmount;
  private bool hasItem;

  public byte backpackSlot { get; private set; }

  private void UpdateInteractable() => this.button.interactable = this.canInteract;

  private bool canInteract
  {
    get
    {
      if (this.isBackpackWear || this.stashSlice || this.hasItem)
        return true;
      return (UnityEngine.Object) Character.localCharacter.data.currentItem != (UnityEngine.Object) null && Character.localCharacter.data.currentItem.UIData.canBackpack;
    }
  }

  public void InitItemSlot((BackpackReference, byte slotID) slot, BackpackWheel wheel)
  {
    this.SharedInit(slot.Item1, wheel);
    this.backpackSlot = slot.slotID;
    this.backpackData = this.backpack.GetData();
    this.itemSlot = this.backpackData.itemSlots[(int) this.backpackSlot];
    this.SetItemIcon(this.itemSlot.prefab, this.itemSlot.data);
    this.UpdateInteractable();
  }

  public void InitPickupBackpack(BackpackReference backpack, BackpackWheel wheel)
  {
    this.backpackSlot = byte.MaxValue;
    this.SharedInit(backpack, wheel);
    this.UpdateInteractable();
  }

  public void InitStashSlot(BackpackReference bpRef, BackpackWheel wheel)
  {
    this.backpack = bpRef;
    this.backpackWheel = wheel;
    this.SetItemIcon(Character.localCharacter.data.currentItem, (UnityEngine.Object) Character.localCharacter.data.currentItem != (UnityEngine.Object) null ? Character.localCharacter.data.currentItem.data : (ItemInstanceData) null);
    this.UpdateInteractable();
  }

  private void SharedInit(BackpackReference bpRef, BackpackWheel wheel)
  {
    this.backpack = bpRef;
    this.backpackWheel = wheel;
    if (bpRef.type == BackpackReference.BackpackType.Item)
    {
      Backpack component = UnityEngine.Resources.Load<GameObject>("0_Items/Backpack").GetComponent<Backpack>();
      if (this.backpackSlot == byte.MaxValue)
        this.gameObject.SetActive(true);
      this.SetItemIcon((Item) component, (ItemInstanceData) null);
    }
    else
    {
      this.SetItemIcon((Item) null, (ItemInstanceData) null);
      if (this.backpackSlot != byte.MaxValue)
        return;
      this.gameObject.SetActive(false);
    }
  }

  private void SetItemIcon(Item iconHolder, ItemInstanceData itemInstanceData)
  {
    if ((UnityEngine.Object) iconHolder == (UnityEngine.Object) null)
    {
      this.image.enabled = false;
      this.hasItem = false;
    }
    else
    {
      this.image.enabled = true;
      this.image.texture = (Texture) iconHolder.UIData.GetIcon();
      this.hasItem = true;
    }
    this.UpdateCookedAmount(iconHolder, itemInstanceData);
  }

  private void UpdateCookedAmount(Item item, ItemInstanceData itemInstanceData)
  {
    if ((UnityEngine.Object) item == (UnityEngine.Object) null || itemInstanceData == null)
    {
      this.cookedAmount = 0;
      this.image.color = Color.white;
    }
    else
    {
      IntItemData intItemData;
      if (!itemInstanceData.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || this.cookedAmount == intItemData.Value)
        return;
      this.image.color = Color.white;
      this.image.color = ItemCooking.GetCookColor(intItemData.Value);
      this.cookedAmount = intItemData.Value;
    }
  }

  public bool isBackpackWear => this.backpackSlot == byte.MaxValue;

  public void Hover()
  {
    if (!this.canInteract)
      return;
    this.backpackWheel.Hover(new BackpackWheelSlice.SliceData()
    {
      isBackpackWear = this.isBackpackWear,
      isStashSlice = this.stashSlice,
      backpackReference = this.backpack,
      slotID = this.backpackSlot
    });
  }

  public void Dehover()
  {
    this.backpackWheel.Dehover(new BackpackWheelSlice.SliceData()
    {
      isBackpackWear = this.backpackSlot == byte.MaxValue,
      isStashSlice = this.stashSlice,
      backpackReference = this.backpack,
      slotID = this.backpackSlot
    });
  }

  public void OnPointerEnter(PointerEventData eventData) => this.Hover();

  public void OnPointerExit(PointerEventData eventData) => this.Dehover();

  public struct SliceData : IEquatable<BackpackWheelSlice.SliceData>
  {
    public bool isBackpackWear;
    public bool isStashSlice;
    public BackpackReference backpackReference;
    public byte slotID;

    public bool Equals(BackpackWheelSlice.SliceData other)
    {
      return this.isBackpackWear == other.isBackpackWear && (int) this.slotID == (int) other.slotID;
    }

    public override bool Equals(object obj)
    {
      return obj is BackpackWheelSlice.SliceData other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine<bool, BackpackReference, byte>(this.isBackpackWear, this.backpackReference, this.slotID);
    }
  }
}
