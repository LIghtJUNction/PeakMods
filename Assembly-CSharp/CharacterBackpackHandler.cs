// Decompiled with JetBrains decompiler
// Type: CharacterBackpackHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class CharacterBackpackHandler : MonoBehaviour
{
  private Character character;
  private CharacterItems characterItems;
  private PhotonView photonView;
  public BackpackOnBackVisuals backpackVisuals;
  public GameObject backpack;
  private bool lastShow;
  public SFX_Instance[] wearSFX;
  private bool t;

  private void Awake()
  {
    this.character = this.GetComponent<Character>();
    this.characterItems = this.GetComponent<CharacterItems>();
    this.photonView = this.GetComponent<PhotonView>();
  }

  private void LateUpdate()
  {
    int num = !this.character.player.backpackSlot.IsEmpty() ? 1 : 0;
    bool flag1 = this.characterItems.currentSelectedSlot.IsSome && this.characterItems.currentSelectedSlot.Value == (byte) 3;
    bool flag2 = num != 0 && !flag1;
    bool flag3 = flag2;
    if (this.character.photonView.IsMine && !MainCameraMovement.IsSpectating)
      flag3 = false;
    this.backpack.SetActive(flag3);
    if (flag2)
    {
      if (!this.t)
      {
        for (int index = 0; index < this.wearSFX.Length; ++index)
          this.wearSFX[index].Play(this.character.refs.hip.transform.position);
      }
      this.t = true;
    }
    else
      this.t = false;
    if (PhotonNetwork.IsMasterClient)
    {
      if (!this.lastShow & flag2)
        this.StartCoroutine(this.RefreshVisualsDelayed());
      else if (this.lastShow && !flag2)
        this.backpackVisuals.RemoveVisuals();
    }
    this.lastShow = flag2;
  }

  private IEnumerator RefreshVisualsDelayed()
  {
    yield return (object) null;
    this.backpackVisuals.RefreshVisuals();
  }

  public void StashInBackpack(Character interactor, byte backpackSlotID)
  {
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
        this.photonView.RPC("RPCAddItemToCharacterBackpack", RpcTarget.All, (object) interactor.player.GetComponent<PhotonView>(), (object) items.currentSelectedSlot.Value, (object) backpackSlotID);
        interactor.player.EmptySlot(items.currentSelectedSlot);
        items.EquipSlot(Optionable<byte>.None);
      }
    }
  }

  [PunRPC]
  public void RPCAddItemToCharacterBackpack(
    PhotonView playerView,
    byte inventorySlotID,
    byte backpackSlotID)
  {
    BackpackData backpackData;
    if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
      backpackData = this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
    ItemSlot itemSlot = playerView.GetComponent<Player>().GetItemSlot(inventorySlotID);
    backpackData.AddItem(itemSlot.prefab, itemSlot.data, backpackSlotID);
    if (PhotonNetwork.IsMasterClient)
      this.backpackVisuals.RefreshVisuals();
    if (!this.character.IsLocal)
      return;
    this.character.refs.afflictions.UpdateWeight();
  }
}
