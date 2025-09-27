// Decompiled with JetBrains decompiler
// Type: BackpackVisuals
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public abstract class BackpackVisuals : MonoBehaviour
{
  public Transform[] backpackSlots;
  private Dictionary<byte, (GameObject, ushort)> visualItems = new Dictionary<byte, (GameObject, ushort)>();
  private Dictionary<byte, Item> spawnedVisualItems = new Dictionary<byte, Item>();
  protected bool m_shuttingDown;

  public abstract BackpackData GetBackpackData();

  private void OnDestroy()
  {
    foreach ((GameObject, ushort) tuple in this.visualItems.Values)
      PhotonNetwork.Destroy(tuple.Item1);
  }

  public void RefreshVisuals()
  {
    if (!PhotonNetwork.IsMasterClient)
      return;
    BackpackData backpackData = this.GetBackpackData();
    if (backpackData == null)
      return;
    for (byte index = 0; index < (byte) 4; ++index)
    {
      ItemSlot itemSlot = backpackData.itemSlots[(int) index];
      Optionable<ushort> optionable1 = itemSlot.IsEmpty() ? Optionable<ushort>.None : Optionable<ushort>.Some(itemSlot.prefab.itemID);
      (GameObject, ushort) tuple1;
      Optionable<ushort> optionable2 = this.visualItems.TryGetValue(index, out tuple1) ? Optionable<ushort>.Some(tuple1.Item2) : Optionable<ushort>.None;
      if (optionable1 != optionable2)
      {
        if (optionable1.IsSome && optionable2.IsSome)
          Debug.LogError((object) "Item Visuals Missmatch!");
        else if (optionable1.IsSome && optionable2.IsNone)
        {
          Debug.Log((object) $"Spawning Backpack Visual for {optionable1.Value}");
          GameObject visual = PhotonNetwork.Instantiate("0_Items/" + itemSlot.GetPrefabName(), new Vector3(0.0f, -500f, 0.0f), Quaternion.identity);
          this.PutItemInBackpack(visual, index);
          visual.GetComponent<PhotonView>().RPC("SetItemInstanceDataRPC", RpcTarget.All, (object) itemSlot.data);
          this.visualItems.Add(index, (visual, optionable1.Value));
        }
        else if (optionable1.IsNone || optionable2.IsSome)
        {
          Debug.Log((object) $"Removing backpack visual for {optionable2.Value}");
          (GameObject, ushort) tuple2;
          if (!this.visualItems.TryGetValue(index, out tuple2))
            Debug.LogError((object) $"Failed to get spawned object from slotID {index}");
          PhotonView component = tuple2.Item1.GetComponent<PhotonView>();
          Debug.Log((object) $"Destroying photon view: {component}");
          PhotonNetwork.Destroy(component);
          this.visualItems.Remove(index);
        }
        else
          Debug.LogError((object) "Should be unreachable");
      }
      else if (optionable1.IsNone)
        Debug.Log((object) $"Not Spawning backpack visual for slot id: {index} because it's empty...");
    }
  }

  protected abstract void PutItemInBackpack(GameObject visual, byte slotID);

  private void OnApplicationQuit() => this.m_shuttingDown = true;

  public void RemoveVisuals()
  {
    if (this.m_shuttingDown)
      return;
    foreach ((GameObject targetGo, ushort _) in this.visualItems.Values)
    {
      if (PhotonNetwork.IsMasterClient)
        PhotonNetwork.Destroy(targetGo);
      else
        targetGo.gameObject.SetActive(false);
    }
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.visualItems.Clear();
  }

  public bool TryGetSpawnedItem(byte slotID, out Item item)
  {
    return this.spawnedVisualItems.TryGetValue(slotID, out item) && (Object) item != (Object) null;
  }

  public void SetSpawnedBackpackItem(byte slotID, Item item)
  {
    this.spawnedVisualItems[slotID] = item;
  }
}
