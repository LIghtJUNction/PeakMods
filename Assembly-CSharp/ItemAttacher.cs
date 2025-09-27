// Decompiled with JetBrains decompiler
// Type: ItemAttacher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class ItemAttacher : MonoBehaviour
{
  private bool attached;
  public Item itemPrefabReference;
  public Item attachedItem;
  public Vector3 offset;

  private void Update()
  {
    if (!PhotonNetwork.InRoom || !(bool) (Object) Character.localCharacter || this.attached)
      return;
    this.TryAttach();
  }

  private void TryAttach()
  {
    foreach (Item obj in Item.ALL_ITEMS)
    {
      if ((int) obj.itemID == (int) this.itemPrefabReference.itemID && obj.itemState == ItemState.Ground && obj.rig.isKinematic && (double) Vector3.Distance(obj.transform.position, this.transform.position) < 1.0)
      {
        obj.transform.SetParent(this.transform, true);
        obj.GetComponent<ItemPhysicsSyncer>().shouldSync = false;
        obj.transform.localPosition = this.offset;
        this.attached = true;
        this.attachedItem = obj;
      }
    }
  }
}
