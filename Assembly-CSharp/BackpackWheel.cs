// Decompiled with JetBrains decompiler
// Type: BackpackWheel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zorro.Core;

#nullable disable
public class BackpackWheel : UIWheel
{
  public BackpackWheelSlice[] slices;
  public TextMeshProUGUI chosenItemText;
  public Optionable<BackpackWheelSlice.SliceData> chosenSlice;
  public BackpackReference backpack;
  public RawImage currentlyHeldItem;
  private int currentlyHeldItemCookedAmount;

  public void InitWheel(BackpackReference bp)
  {
    this.backpack = bp;
    this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
    this.chosenItemText.text = "";
    ItemSlot[] itemSlots = this.backpack.GetData().itemSlots;
    for (byte index = 0; (int) index < itemSlots.Length; ++index)
      this.slices[(int) index + 1].InitItemSlot((bp, index), this);
    this.gameObject.SetActive(true);
    this.slices[0].InitPickupBackpack(bp, this);
    if ((bool) (Object) Character.localCharacter.data.currentItem)
    {
      this.currentlyHeldItem.texture = (Texture) Character.localCharacter.data.currentItem.UIData.GetIcon();
      this.UpdateCookedAmount(Character.localCharacter.data.currentItem);
      this.currentlyHeldItem.enabled = true;
    }
    else
    {
      this.UpdateCookedAmount((Item) null);
      this.currentlyHeldItem.enabled = false;
    }
  }

  private void UpdateCookedAmount(Item item)
  {
    if ((Object) item == (Object) null || item.data == null)
    {
      this.currentlyHeldItemCookedAmount = 0;
      this.currentlyHeldItem.color = Color.white;
    }
    else
    {
      IntItemData intItemData;
      if (!item.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData) || this.currentlyHeldItemCookedAmount == intItemData.Value)
        return;
      this.currentlyHeldItem.color = Color.white;
      this.currentlyHeldItem.color = ItemCooking.GetCookColor(intItemData.Value);
      this.currentlyHeldItemCookedAmount = intItemData.Value;
    }
  }

  protected override void Update()
  {
    if (!Character.localCharacter.input.interactIsPressed)
    {
      this.Choose();
      GUIManager.instance.CloseBackpackWheel();
    }
    else if ((Object) this.backpack.locationTransform != (Object) null && (double) Vector3.Distance(this.backpack.locationTransform.position, Character.localCharacter.Center) > 6.0)
    {
      GUIManager.instance.CloseBackpackWheel();
    }
    else
    {
      if (this.chosenSlice.IsSome && !this.chosenSlice.Value.isBackpackWear && !this.slices[(int) this.chosenSlice.Value.slotID + 1].image.enabled)
        this.currentlyHeldItem.transform.position = Vector3.Lerp(this.currentlyHeldItem.transform.position, this.slices[(int) this.chosenSlice.Value.slotID + 1].transform.GetChild(0).GetChild(0).position, Time.deltaTime * 20f);
      else
        this.currentlyHeldItem.transform.localPosition = Vector3.Lerp(this.currentlyHeldItem.transform.localPosition, Vector3.zero, Time.deltaTime * 20f);
      base.Update();
    }
  }

  public void Choose()
  {
    if (!this.chosenSlice.IsSome)
      return;
    Debug.Log((object) $"Chose slice {this.chosenSlice.Value.slotID}");
    if (this.chosenSlice.Value.isBackpackWear)
    {
      Backpack backpack;
      if (!this.chosenSlice.Value.backpackReference.TryGetBackpackItem(out backpack))
        return;
      backpack.Wear(Character.localCharacter);
    }
    else if (this.chosenSlice.Value.isStashSlice)
    {
      this.TryStash(this.chosenSlice.Value.slotID);
    }
    else
    {
      Item obj;
      if (this.chosenSlice.Value.backpackReference.GetVisuals().TryGetSpawnedItem(this.chosenSlice.Value.slotID, out obj))
      {
        obj.Interact(Character.localCharacter);
      }
      else
      {
        if (!(bool) (Object) Character.localCharacter.data.currentItem)
          return;
        this.TryStash(this.chosenSlice.Value.slotID);
      }
    }
  }

  private void TryStash(byte backpackSlotID)
  {
    Backpack backpack;
    if (this.backpack.TryGetBackpackItem(out backpack))
      backpack.Stash(Character.localCharacter, backpackSlotID);
    else
      this.backpack.view.GetComponent<CharacterBackpackHandler>().StashInBackpack(Character.localCharacter, backpackSlotID);
  }

  public void Hover(BackpackWheelSlice.SliceData sliceData)
  {
    if (sliceData.isBackpackWear)
    {
      if (sliceData.backpackReference.type == BackpackReference.BackpackType.Equipped)
        return;
      this.chosenItemText.text = LocalizedText.GetText("WEARBACKPACK");
      this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
    }
    else if (sliceData.isStashSlice)
    {
      Item currentItem = Character.localCharacter.data.currentItem;
      if ((Object) currentItem != (Object) null)
      {
        this.chosenItemText.text = LocalizedText.GetText("STASHITEM").Replace("#", currentItem.GetItemName());
        this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
      }
      else
      {
        this.chosenItemText.text = "";
        this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
      }
    }
    else
    {
      ItemSlot itemSlot = this.backpack.GetData().itemSlots[(int) sliceData.slotID];
      bool flag = false;
      if (itemSlot.IsEmpty() && (bool) (Object) Character.localCharacter.data.currentItem)
      {
        if ((bool) (Object) Character.localCharacter.data.currentItem)
        {
          this.chosenItemText.text = LocalizedText.GetText("STASHITEM").Replace("#", Character.localCharacter.data.currentItem.GetItemName());
          flag = true;
        }
      }
      else
      {
        Item prefab = itemSlot.prefab;
        if ((Object) prefab != (Object) null)
        {
          this.chosenItemText.text = LocalizedText.GetText("TAKEITEM").Replace("#", prefab.GetItemName(itemSlot.data));
          flag = true;
        }
      }
      if (!flag)
        return;
      this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.Some(sliceData);
    }
  }

  public void Dehover(BackpackWheelSlice.SliceData sliceData)
  {
    if (!this.chosenSlice.IsSome || !this.chosenSlice.Value.Equals(sliceData))
      return;
    this.chosenItemText.text = "";
    this.chosenSlice = Optionable<BackpackWheelSlice.SliceData>.None;
  }

  protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
  {
    float num1 = 0.0f;
    BackpackWheelSlice backpackWheelSlice = (BackpackWheelSlice) null;
    if ((double) gamepadVector.sqrMagnitude >= 0.5)
    {
      for (int index = 0; index < this.slices.Length; ++index)
      {
        float num2 = Vector3.Angle((Vector3) gamepadVector, this.slices[index].GetUpVector());
        if ((Object) backpackWheelSlice == (Object) null || (double) num2 < (double) num1)
        {
          backpackWheelSlice = this.slices[index];
          num1 = num2;
        }
      }
    }
    if ((Object) backpackWheelSlice != (Object) null)
    {
      EventSystem.current.SetSelectedGameObject(backpackWheelSlice.button.gameObject);
      backpackWheelSlice.Hover();
    }
    else
    {
      EventSystem.current.SetSelectedGameObject((GameObject) null);
      this.Dehover(this.chosenSlice.Value);
    }
  }
}
