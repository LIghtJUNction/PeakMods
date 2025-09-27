// Decompiled with JetBrains decompiler
// Type: Backpack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
public class Backpack : Item
{
  public Transform[] backpackSlots;
  public float openRadialMenuTime = 0.25f;
  public GameObject groundMesh;
  public GameObject heldMesh;

  protected override void AddPhysics()
  {
    base.AddPhysics();
    this.rig.sleepThreshold = 0.0f;
  }

  public override void Interact(Character interactor)
  {
    GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromBackpackItem((Item) this));
  }

  protected override void Update()
  {
    this.groundMesh.gameObject.SetActive(this.itemState == ItemState.Ground);
    this.heldMesh.gameObject.SetActive(this.itemState != 0);
    base.Update();
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  public void Wear(Character interactor) => base.Interact(interactor);

  private void DisableVisuals()
  {
    this.mainRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
  }

  private void EnableVisuals() => this.mainRenderer.shadowCastingMode = ShadowCastingMode.On;

  public void Stash(Character interactor, byte backpackSlotID)
  {
    if (!(bool) (UnityEngine.Object) interactor.data.currentItem || !this.HasSpace())
      return;
    CharacterItems items = interactor.refs.items;
    if (items.currentSelectedSlot.IsNone)
    {
      Debug.LogError((object) "Need item slot selected to stash item in backpack!");
    }
    else
    {
      ItemSlot itemSlot = interactor.player.GetItemSlot(items.currentSelectedSlot.Value);
      if (itemSlot.IsEmpty())
      {
        Debug.LogError((object) $"Item slot {itemSlot.itemSlotID} is empty!");
      }
      else
      {
        this.view.RPC("RPCAddItemToBackpack", RpcTarget.All, (object) interactor.player.GetComponent<PhotonView>(), (object) items.currentSelectedSlot.Value, (object) backpackSlotID);
        interactor.player.EmptySlot(items.currentSelectedSlot);
        if (items.currentSelectedSlot.IsSome && items.currentSelectedSlot.Value == (byte) 250)
          interactor.photonView.RPC("DestroyHeldItemRpc", RpcTarget.All);
        else
          items.EquipSlot(Optionable<byte>.None);
      }
    }
  }

  [PunRPC]
  public void RPCAddItemToBackpack(PhotonView playerView, byte slotID, byte backpackSlotID)
  {
    BackpackData data1 = this.GetData<BackpackData>(DataEntryKey.BackpackData);
    ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(slotID);
    Item prefab = itemSlot.prefab;
    ItemInstanceData data2 = itemSlot.data;
    int backpackSlotID1 = (int) backpackSlotID;
    data1.AddItem(prefab, data2, (byte) backpackSlotID1);
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.GetComponent<BackpackVisuals>().RefreshVisuals();
  }

  private void OnDestroy() => this.GetComponent<BackpackVisuals>().RemoveVisuals();

  private bool HasSpace() => this.GetData<BackpackData>(DataEntryKey.BackpackData).HasFreeSlot();

  public int FilledSlotCount()
  {
    return this.GetData<BackpackData>(DataEntryKey.BackpackData).FilledSlotCount();
  }

  public override string GetInteractionText() => LocalizedText.GetText("open");

  public override void OnInstanceDataRecieved()
  {
    base.OnInstanceDataRecieved();
    this.GetComponent<BackpackVisuals>().RefreshVisuals();
  }

  [ConsoleCommand]
  public static void PrintBackpacks()
  {
    foreach (Backpack backpack in UnityEngine.Object.FindObjectsByType<Backpack>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID))
    {
      List<ItemSlot> list = ((IEnumerable<ItemSlot>) backpack.GetData<BackpackData>(DataEntryKey.BackpackData).itemSlots).Where<ItemSlot>((Func<ItemSlot, bool>) (slot => !slot.IsEmpty())).ToList<ItemSlot>();
      Debug.Log((object) $"Backpack: {backpack.GetInstanceID()}, Full Slots: {list.Count}");
      foreach (ItemSlot itemSlot in list)
        Debug.Log((object) $"Slot: {itemSlot.GetPrefabName()}, data entries: {itemSlot.data.data.Count}");
    }
  }

  public bool IsConstantlyInteractable(Character interactor) => false;

  public float GetInteractTime(Character interactor) => this.openRadialMenuTime;

  public void Interact_CastFinished(Character interactor)
  {
  }

  public void CancelCast(Character interactor)
  {
  }

  public bool holdOnFinish => false;
}
