// Decompiled with JetBrains decompiler
// Type: Action_ReduceUses
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class Action_ReduceUses : ItemAction
{
  public bool consumeOnFullyUsed;

  public override void RunAction() => this.item.photonView.RPC("ReduceUsesRPC", RpcTarget.All);

  [PunRPC]
  public void ReduceUsesRPC()
  {
    OptionableIntItemData data = this.item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);
    if (!data.HasData || data.Value <= 0)
      return;
    --data.Value;
    if (this.item.totalUses > 0)
      this.item.SetUseRemainingPercentage((float) data.Value / (float) this.item.totalUses);
    if (data.Value != 0 || !this.consumeOnFullyUsed || !(bool) (Object) this.character || !this.character.IsLocal || !((Object) this.character.data.currentItem == (Object) this.item))
      return;
    this.item.StartCoroutine(this.item.ConsumeDelayed());
  }
}
