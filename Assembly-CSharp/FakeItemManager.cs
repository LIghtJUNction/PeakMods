// Decompiled with JetBrains decompiler
// Type: FakeItemManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Zorro.Core.Serizalization;

#nullable disable
public class FakeItemManager : MonoBehaviourPunCallbacks
{
  private static FakeItemManager _instance;
  internal FakeItemManager.FakeItemData fakeItemData;
  [SerializeField]
  [ReadOnly]
  private List<FakeItem> allFakeItems = new List<FakeItem>();

  public static FakeItemManager Instance
  {
    get
    {
      if ((Object) FakeItemManager._instance == (Object) null)
      {
        FakeItemManager._instance = Object.FindFirstObjectByType<FakeItemManager>();
        if ((Object) FakeItemManager._instance == (Object) null)
          FakeItemManager._instance = GameUtils.instance.gameObject.AddComponent<FakeItemManager>();
      }
      return FakeItemManager._instance;
    }
    private set => FakeItemManager._instance = value;
  }

  private void Awake()
  {
    FakeItemManager.Instance = this;
    if (this.fakeItemData.hiddenItems != null)
      return;
    this.fakeItemData.hiddenItems = new List<int>();
  }

  public void CullNullItems()
  {
    for (int index = this.allFakeItems.Count - 1; index >= 0; --index)
    {
      if ((Object) this.allFakeItems[index] == (Object) null)
        this.allFakeItems.RemoveAt(index);
    }
  }

  public int GetAvailableIndex()
  {
    for (int availableIndex = 0; availableIndex < 99999; ++availableIndex)
    {
      bool flag = false;
      for (int index = 0; index < this.allFakeItems.Count; ++index)
      {
        if ((Object) this.allFakeItems[index] != (Object) null && this.allFakeItems[index].index == availableIndex)
          flag = true;
      }
      if (!flag)
        return availableIndex;
    }
    return -1;
  }

  public bool TryGetFakeItem(int index, out FakeItem item)
  {
    for (int index1 = 0; index1 < this.allFakeItems.Count; ++index1)
    {
      if ((Object) this.allFakeItems[index1] != (Object) null && this.allFakeItems[index1].index == index)
      {
        item = this.allFakeItems[index1];
        return true;
      }
    }
    item = (FakeItem) null;
    return false;
  }

  public void RefreshList()
  {
    this.allFakeItems = ((IEnumerable<FakeItem>) Object.FindObjectsByType<FakeItem>(FindObjectsInactive.Include, FindObjectsSortMode.None)).ToList<FakeItem>();
    for (int index = 0; index < this.allFakeItems.Count; ++index)
      this.allFakeItems[index].index = index;
  }

  public void AddToList(FakeItem item)
  {
    if (Application.isPlaying)
      return;
    this.allFakeItems.Add(item);
  }

  public void RemoveFromList(FakeItem item)
  {
    if (Application.isPlaying)
      return;
    this.allFakeItems.Remove(item);
  }

  public bool Contains(FakeItem item) => this.allFakeItems.Contains(item);

  public int ItemCount => this.allFakeItems.Count;

  [PunRPC]
  public void RPC_RequestFakeItemPickup(PhotonView characterView, int fakeItemIndex)
  {
    Character component = characterView.GetComponent<Character>();
    FakeItem fakeItem;
    if (!this.TryGetFakeItem(fakeItemIndex, out fakeItem))
      return;
    ItemSlot slot;
    if ((!component.player.AddItem(fakeItem.realItemPrefab.itemID, (ItemInstanceData) null, out slot) ? 0 : (!fakeItem.pickedUp ? 1 : 0)) != 0)
    {
      component.refs.view.RPC("OnPickupAccepted", component.player.photonView.Owner, (object) slot.itemSlotID);
      this.photonView.RPC("RPC_FakeItemPickupSuccess", RpcTarget.All, (object) fakeItemIndex);
    }
    else
      this.photonView.RPC("RPC_DenyFakeItemPickup", component.player.photonView.Owner, (object) fakeItem.index);
  }

  [PunRPC]
  internal void RPC_FakeItemPickupSuccess(int fakeItemIndex)
  {
    FakeItem fakeItem;
    if (!this.TryGetFakeItem(fakeItemIndex, out fakeItem))
      return;
    fakeItem.PickUpVisibly();
  }

  [PunRPC]
  public void RPC_DenyFakeItemPickup(int fakeItemIndex)
  {
    FakeItem fakeItem;
    if (!this.TryGetFakeItem(fakeItemIndex, out fakeItem))
      return;
    fakeItem.UnPickUpVisibly();
  }

  [PunRPC]
  public void RPC_RequestStickFakeItemToPlayer(
    int characterViewID,
    int fakeItemIndex,
    int bodyPartType,
    Vector3 offset)
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    PhotonView photonView = PhotonNetwork.GetPhotonView(characterViewID);
    photonView.GetComponent<Character>().GetBodypart((BodypartType) bodyPartType);
    FakeItem fakeItem;
    if (!this.TryGetFakeItem(fakeItemIndex, out fakeItem))
      return;
    if (fakeItem.gameObject.activeInHierarchy)
    {
      StickyItemComponent component;
      if (PhotonNetwork.InstantiateItemRoom(fakeItem.realItemPrefab.name, fakeItem.transform.position, fakeItem.transform.rotation).TryGetComponent<StickyItemComponent>(out component))
        component.photonView.RPC("RPC_StickToCharacterRemote", photonView.Owner, (object) characterViewID, (object) bodyPartType, (object) offset);
      this.photonView.RPC("RPC_FakeItemPickupSuccess", RpcTarget.All, (object) fakeItemIndex);
    }
    else
      this.photonView.RPC("RPC_DenyFakeItemPickup", RpcTarget.All, (object) fakeItemIndex);
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    byte[] managedArray = IBinarySerializable.ToManagedArray<FakeItemManager.FakeItemData>(this.fakeItemData);
    this.photonView.RPC("RPC_SyncFakeItems", newPlayer, (object) managedArray);
  }

  [PunRPC]
  public void RPC_SyncFakeItems(byte[] data)
  {
    this.fakeItemData = IBinarySerializable.GetFromManagedArray<FakeItemManager.FakeItemData>(data);
    for (int index = 0; index < this.fakeItemData.hiddenItems.Count; ++index)
    {
      FakeItem fakeItem;
      if (this.TryGetFakeItem(this.fakeItemData.hiddenItems[index], out fakeItem))
      {
        fakeItem.gameObject.SetActive(false);
        fakeItem.pickedUp = true;
      }
    }
  }

  public struct FakeItemData : IBinarySerializable
  {
    public List<int> hiddenItems;

    public void Serialize(BinarySerializer serializer)
    {
      serializer.WriteInt(this.hiddenItems.Count);
      for (int index = 0; index < this.hiddenItems.Count; ++index)
        serializer.WriteInt(this.hiddenItems[index]);
    }

    public void Deserialize(BinaryDeserializer deserializer)
    {
      int num = deserializer.ReadInt();
      this.hiddenItems = new List<int>();
      for (int index = 0; index < num; ++index)
        this.hiddenItems.Add(deserializer.ReadInt());
    }
  }
}
