// Decompiled with JetBrains decompiler
// Type: ItemDatabase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

#nullable disable
[ConsoleClassCustomizer("Item")]
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scouts/ItemDatabase")]
public class ItemDatabase : ObjectDatabaseAsset<ItemDatabase, Item>
{
  public Dictionary<ushort, Item> itemLookup = new Dictionary<ushort, Item>();

  public override void OnLoaded() => base.OnLoaded();

  public void LoadItems()
  {
  }

  [ContextMenu("Reload entire database")]
  public void ReloadAllItems()
  {
  }

  private ushort GetAvailableID()
  {
    for (ushort key = 0; key < ushort.MaxValue; ++key)
    {
      if (!this.itemLookup.ContainsKey(key))
        return key;
    }
    return 0;
  }

  private bool ItemExistsInDatabase(Item item, out ushort itemID)
  {
    foreach (ushort key in this.itemLookup.Keys)
    {
      if (this.itemLookup[key].Equals((object) item))
      {
        itemID = key;
        return true;
      }
    }
    itemID = (ushort) 0;
    return false;
  }

  [ConsoleCommand]
  public static void Add(Item item)
  {
    if ((Object) MainCamera.instance == (Object) null || !PhotonNetwork.IsConnected)
      return;
    Transform transform = MainCamera.instance.transform;
    RaycastHit hitInfo;
    if (!Physics.Raycast(transform.position, transform.forward, out hitInfo))
      hitInfo = new RaycastHit();
    ItemDatabase.Add(item, hitInfo.point + hitInfo.normal);
  }

  public static void Add(Item item, Vector3 point)
  {
    if (!PhotonNetwork.IsConnected)
      return;
    Debug.Log((object) $"Spawn item: {item} at {point}");
    PhotonNetwork.Instantiate("0_Items/" + item.name, point, Quaternion.identity).GetComponent<Item>().RequestPickup(Character.localCharacter.GetComponent<PhotonView>());
  }

  public static bool TryGetItem(ushort itemID, out Item item)
  {
    return SingletonAsset<ItemDatabase>.Instance.itemLookup.TryGetValue(itemID, out item);
  }

  public void AddAllNamesToCSV()
  {
    for (int index = 0; index < this.Objects.Count; ++index)
      this.Objects[index].AddNameToCSV();
  }

  public void AddAllPromptsToCSV()
  {
    List<string> totalStrings = new List<string>();
    for (int index = 0; index < this.Objects.Count; ++index)
    {
      List<string> csv = this.Objects[index].AddPromptToCSV(totalStrings);
      totalStrings.AddRange((IEnumerable<string>) csv);
    }
  }
}
