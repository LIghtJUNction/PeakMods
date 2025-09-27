// Decompiled with JetBrains decompiler
// Type: ItemBackpackVisuals
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class ItemBackpackVisuals : BackpackVisuals
{
  private Item item;

  private void Awake() => this.item = this.GetComponent<Item>();

  public override BackpackData GetBackpackData()
  {
    return this.GetComponent<Item>().GetData<BackpackData>(DataEntryKey.BackpackData);
  }

  protected override void PutItemInBackpack(GameObject visual, byte slotID)
  {
    visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", RpcTarget.All, (object) slotID, (object) BackpackReference.GetFromBackpackItem(this.item));
  }
}
