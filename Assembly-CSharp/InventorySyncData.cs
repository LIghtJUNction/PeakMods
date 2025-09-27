// Decompiled with JetBrains decompiler
// Type: InventorySyncData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using Zorro.Core.Serizalization;

#nullable disable
public struct InventorySyncData : IBinarySerializable
{
  public byte slotCount;
  public InventorySyncData.SlotData[] slots;
  public InventorySyncData.SlotData tempSlot;
  public bool hasBackpack;
  public ItemInstanceData backpackInstanceData;

  public InventorySyncData(ItemSlot[] itemSlots, BackpackSlot backpackSlot, ItemSlot tempSlot)
  {
    this.slotCount = (byte) itemSlots.Length;
    this.slots = new InventorySyncData.SlotData[itemSlots.Length];
    for (int index = 0; index < (int) this.slotCount; ++index)
    {
      ushort num = (UnityEngine.Object) itemSlots[index].prefab == (UnityEngine.Object) null ? ushort.MaxValue : itemSlots[index].prefab.itemID;
      this.slots[index] = new InventorySyncData.SlotData()
      {
        ItemID = num,
        Data = itemSlots[index].data
      };
    }
    this.tempSlot = new InventorySyncData.SlotData()
    {
      ItemID = (UnityEngine.Object) tempSlot.prefab == (UnityEngine.Object) null ? ushort.MaxValue : tempSlot.prefab.itemID,
      Data = tempSlot.data
    };
    this.hasBackpack = !backpackSlot.IsEmpty();
    this.backpackInstanceData = backpackSlot.data;
  }

  public void Serialize(BinarySerializer serializer)
  {
    serializer.WriteByte(this.slotCount);
    for (int index = 0; index < (int) this.slotCount; ++index)
      this.slots[index].Serialize(serializer);
    this.tempSlot.Serialize(serializer);
    serializer.WriteBool(this.hasBackpack);
    if (!this.hasBackpack)
      return;
    if (this.backpackInstanceData == null)
    {
      this.backpackInstanceData = new ItemInstanceData(Guid.NewGuid());
      ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
    }
    serializer.WriteGuid(this.backpackInstanceData.guid);
    this.backpackInstanceData.Serialize(serializer);
  }

  public void Deserialize(BinaryDeserializer deserializer)
  {
    this.slotCount = deserializer.ReadByte();
    this.slots = new InventorySyncData.SlotData[(int) this.slotCount];
    this.tempSlot = new InventorySyncData.SlotData();
    for (int index = 0; index < (int) this.slotCount; ++index)
    {
      InventorySyncData.SlotData slotData = new InventorySyncData.SlotData();
      slotData.Deserialize(deserializer);
      this.slots[index] = slotData;
    }
    this.tempSlot.Deserialize(deserializer);
    this.hasBackpack = deserializer.ReadBool();
    if (!this.hasBackpack)
      return;
    Guid guid = deserializer.ReadGuid();
    if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.backpackInstanceData))
    {
      this.backpackInstanceData = new ItemInstanceData(guid);
      ItemInstanceDataHandler.AddInstanceData(this.backpackInstanceData);
    }
    this.backpackInstanceData.Deserialize(deserializer);
  }

  public struct SlotData : IBinarySerializable
  {
    public ushort ItemID;
    public ItemInstanceData Data;

    public void Serialize(BinarySerializer serializer)
    {
      serializer.WriteUshort(this.ItemID);
      if (this.ItemID == ushort.MaxValue)
        return;
      serializer.WriteGuid(this.Data.guid);
      this.Data.Serialize(serializer);
    }

    public void Deserialize(BinaryDeserializer deserializer)
    {
      this.ItemID = deserializer.ReadUShort();
      if (this.ItemID == ushort.MaxValue)
        return;
      Guid guid = deserializer.ReadGuid();
      if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out this.Data))
      {
        this.Data = new ItemInstanceData(guid);
        ItemInstanceDataHandler.AddInstanceData(this.Data);
      }
      this.Data.Deserialize(deserializer);
    }
  }
}
