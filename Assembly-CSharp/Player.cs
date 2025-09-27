// Decompiled with JetBrains decompiler
// Type: Player
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

#nullable disable
public class Player : MonoBehaviourPunCallbacks
{
  public const int BACKPACKSLOTINDEX = 3;
  public ItemSlot[] itemSlots = new ItemSlot[3];
  public ItemSlot tempFullSlot;
  public BackpackSlot backpackSlot;
  public Action<int> hotbarSelectionChanged;
  public Action<ItemSlot[]> itemsChangedAction;
  public static Player localPlayer;
  public bool hasClosedEndScreen;
  public bool doneWithCutscene;
  private PhotonView view;

  public Character character => PlayerHandler.GetPlayerCharacter(this.view.Owner);

  private void Awake()
  {
    this.view = this.GetComponent<PhotonView>();
    for (byte slotID = 0; (int) slotID < this.itemSlots.Length; ++slotID)
      this.itemSlots[(int) slotID] = new ItemSlot(slotID);
    this.tempFullSlot = (ItemSlot) new TemporaryItemSlot((byte) 250);
    this.backpackSlot = new BackpackSlot((byte) 3);
    if ((UnityEngine.Object) this.view != (UnityEngine.Object) null)
    {
      PlayerHandler.RegisterPlayer(this);
      if (this.view.IsMine)
        Player.localPlayer = this;
    }
    this.gameObject.name = "Player: " + this.view.Owner.NickName;
  }

  public bool AddItem(ushort itemID, ItemInstanceData instanceData, out ItemSlot slot)
  {
    if (instanceData == null)
    {
      instanceData = new ItemInstanceData(Guid.NewGuid());
      ItemInstanceDataHandler.AddInstanceData(instanceData);
    }
    if (!PhotonNetwork.IsMasterClient)
    {
      Debug.LogError((object) "Only Master Client can add items!");
      slot = (ItemSlot) null;
      return false;
    }
    Item ItemPrefab;
    if (!ItemDatabase.TryGetItem(itemID, out ItemPrefab))
    {
      Debug.LogError((object) $"Failed to get item from item ID: {itemID}");
      slot = (ItemSlot) null;
      return false;
    }
    slot = AddToSlot();
    if (slot == null)
    {
      Debug.LogError((object) $"Failed adding {ItemPrefab.name} to {this.name}'s inventory, no slots available!");
      return false;
    }
    Debug.Log((object) $"Granting {this.name}: {ItemPrefab.name} and added to slot: {slot.itemSlotID}");
    this.view.RPC("SyncInventoryRPC", RpcTarget.Others, (object) IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot)), (object) false);
    return true;

    ItemSlot AddToSlot()
    {
      if (ItemPrefab is Backpack)
      {
        if (!this.backpackSlot.IsEmpty())
          return (ItemSlot) null;
        this.backpackSlot.hasBackpack = true;
        this.backpackSlot.data = instanceData;
        return (ItemSlot) this.backpackSlot;
      }
      for (int index = 0; index < this.itemSlots.Length; ++index)
      {
        if (this.itemSlots[index].IsEmpty())
        {
          this.itemSlots[index].SetItem(ItemPrefab, instanceData);
          return this.itemSlots[index];
        }
      }
      if (!this.tempFullSlot.IsEmpty() || this.character.data.isClimbingAnything)
        return (ItemSlot) null;
      this.tempFullSlot.SetItem(ItemPrefab, instanceData);
      return this.tempFullSlot;
    }
  }

  [PunRPC]
  public void SyncInventoryRPC(byte[] data, bool forceSync)
  {
    if (!forceSync && PhotonNetwork.IsMasterClient)
    {
      Debug.LogError((object) "SyncInventoryRPC should not sync to Master client. They are the boss");
    }
    else
    {
      InventorySyncData fromManagedArray = IBinarySerializable.GetFromManagedArray<InventorySyncData>(data);
      for (byte index = 0; (int) index < this.itemSlots.Length; ++index)
      {
        Item obj;
        this.itemSlots[(int) index].prefab = ItemDatabase.TryGetItem(fromManagedArray.slots[(int) index].ItemID, out obj) ? obj : (Item) null;
        this.itemSlots[(int) index].data = fromManagedArray.slots[(int) index].Data;
        Debug.Log((object) $"Sync Inventory on {this.name} is setting slot: {index} to {this.itemSlots[(int) index].ToString()}");
      }
      Debug.Log((object) $"Sync Inventory on {this.name} is setting backpack: {fromManagedArray.hasBackpack}");
      this.backpackSlot.hasBackpack = fromManagedArray.hasBackpack;
      this.backpackSlot.data = fromManagedArray.backpackInstanceData;
      Item obj1;
      this.tempFullSlot.prefab = ItemDatabase.TryGetItem(fromManagedArray.tempSlot.ItemID, out obj1) ? obj1 : (Item) null;
      this.tempFullSlot.data = fromManagedArray.tempSlot.Data;
      if (!this.view.IsMine)
        return;
      this.character.refs.items.RefreshAllCharacterCarryWeightRPC();
    }
  }

  [PunRPC]
  public void RPCRemoveItemFromSlot(byte slotID)
  {
    if (!PhotonNetwork.IsMasterClient)
    {
      Debug.LogError((object) "Only Master Client can remove items!");
    }
    else
    {
      this.GetItemSlot(slotID).EmptyOut();
      this.view.RPC("SyncInventoryRPC", RpcTarget.Others, (object) IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot)), (object) false);
    }
  }

  public void EmptySlot(Optionable<byte> slot)
  {
    if (slot.IsNone)
    {
      Debug.LogError((object) "Can't empty none slot");
    }
    else
    {
      byte slotID = slot.Value;
      this.GetItemSlot(slotID).EmptyOut();
      if (PhotonNetwork.IsMasterClient)
        this.view.RPC("SyncInventoryRPC", RpcTarget.Others, (object) IBinarySerializable.ToManagedArray<InventorySyncData>(new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot)), (object) false);
      else
        this.view.RPC("RPCRemoveItemFromSlot", RpcTarget.MasterClient, (object) slotID);
    }
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    InventorySyncData syncData = new InventorySyncData(this.itemSlots, this.backpackSlot, this.tempFullSlot);
    this.view.RPC("SyncInventoryRPC", newPlayer, (object) IBinarySerializable.ToManagedArray<InventorySyncData>(syncData), (object) false);
  }

  [PunRPC]
  public void RPC_SetInventory(byte[] newInventory)
  {
  }

  public ItemSlot GetItemSlot(byte slotID)
  {
    if (slotID == (byte) 3)
      return (ItemSlot) this.backpackSlot;
    return slotID == (byte) 250 ? this.tempFullSlot : this.itemSlots[(int) slotID];
  }

  public bool HasEmptySlot(ushort itemID)
  {
    Item obj;
    if (!ItemDatabase.TryGetItem(itemID, out obj))
    {
      Debug.LogError((object) $"Failed to get item from item ID: {itemID}");
      return false;
    }
    if (obj is Backpack)
      return this.backpackSlot.IsEmpty();
    foreach (ItemSlot itemSlot in this.itemSlots)
    {
      if (itemSlot.IsEmpty())
        return true;
    }
    return this.tempFullSlot.IsEmpty();
  }

  [ContextMenu("Debug Print Player ID")]
  private void DebugPrintPlayerID() => Debug.Log((object) this.photonView.Owner.ActorNumber);

  public bool HasInAnySlot(ushort itemID)
  {
    foreach (ItemSlot itemSlot in this.itemSlots)
    {
      if (!itemSlot.IsEmpty() && (int) itemSlot.prefab.itemID == (int) itemID)
        return true;
    }
    return false;
  }

  [ConsoleCommand]
  public static void PrintInventory(Player player)
  {
    byte num = 0;
    foreach (ItemSlot itemSlot in player.itemSlots)
    {
      Debug.Log((object) $"Slot{num}: {itemSlot.ToString()}");
      if (!itemSlot.IsEmpty())
      {
        Debug.Log((object) $"Data [{itemSlot.data.guid}, keys: {itemSlot.data.data.Count}]");
        foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in itemSlot.data.data)
        {
          Debug.Log((object) $"{keyValuePair.Key} : {keyValuePair.Value.GetType().Name}");
          Debug.Log((object) keyValuePair.Value.ToString());
        }
      }
      ++num;
    }
  }
}
