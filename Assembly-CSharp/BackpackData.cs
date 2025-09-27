// Decompiled with JetBrains decompiler
// Type: BackpackData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using Zorro.Core.Serizalization;

#nullable disable
public class BackpackData : DataEntryValue
{
  public const int slots = 4;
  public ItemSlot[] itemSlots = new ItemSlot[4];

  public override void Init()
  {
    base.Init();
    for (byte slotID = 0; (int) slotID < this.itemSlots.Length; ++slotID)
      this.itemSlots[(int) slotID] = new ItemSlot(slotID);
  }

  public override void SerializeValue(BinarySerializer serializer)
  {
    InventorySyncData inventorySyncData;
    ref InventorySyncData local = ref inventorySyncData;
    ItemSlot[] itemSlots = this.itemSlots;
    BackpackSlot backpackSlot = new BackpackSlot((byte) 4);
    backpackSlot.data = new ItemInstanceData(Guid.Empty);
    TemporaryItemSlot tempSlot = new TemporaryItemSlot((byte) 250);
    local = new InventorySyncData(itemSlots, backpackSlot, (ItemSlot) tempSlot);
    inventorySyncData.Serialize(serializer);
  }

  public override void DeserializeValue(BinaryDeserializer deserializer)
  {
    InventorySyncData inventorySyncData = new InventorySyncData();
    inventorySyncData.Deserialize(deserializer);
    for (byte slotID = 0; slotID < (byte) 4; ++slotID)
    {
      if (this.itemSlots[(int) slotID] == null)
        this.itemSlots[(int) slotID] = new ItemSlot(slotID);
      Item obj;
      this.itemSlots[(int) slotID].prefab = ItemDatabase.TryGetItem(inventorySyncData.slots[(int) slotID].ItemID, out obj) ? obj : (Item) null;
      this.itemSlots[(int) slotID].data = inventorySyncData.slots[(int) slotID].Data;
      Debug.Log((object) $"Sync Back Inventory is setting slot: {slotID} to {this.itemSlots[(int) slotID].ToString()}");
    }
  }

  public void AddItem(Item prefab, ItemInstanceData data, byte backpackSlotID)
  {
    if (data == null)
    {
      Debug.Log((object) "DATA IS NULL??");
      data = new ItemInstanceData(Guid.NewGuid());
      ItemInstanceDataHandler.AddInstanceData(data);
    }
    if ((int) backpackSlotID >= this.itemSlots.Length || !this.itemSlots[(int) backpackSlotID].IsEmpty())
      return;
    Debug.Log((object) $"Added item: {prefab.gameObject.name} to slot {backpackSlotID}");
    this.itemSlots[(int) backpackSlotID].prefab = prefab;
    this.itemSlots[(int) backpackSlotID].data = data;
  }

  public bool HasFreeSlot()
  {
    for (int index = 0; index < this.itemSlots.Length; ++index)
    {
      if (this.itemSlots[index].IsEmpty())
        return true;
    }
    return false;
  }

  public int FilledSlotCount()
  {
    int length = this.itemSlots.Length;
    for (int index = 0; index < this.itemSlots.Length; ++index)
    {
      if (this.itemSlots[index].IsEmpty())
        --length;
    }
    return length;
  }

  public override string ToString()
  {
    string str = "";
    foreach (ItemSlot itemSlot in this.itemSlots)
      str = $"{str}{itemSlot.ToString()}, {Environment.NewLine}";
    return str;
  }
}
