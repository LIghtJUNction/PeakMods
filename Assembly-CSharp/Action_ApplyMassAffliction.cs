// Decompiled with JetBrains decompiler
// Type: Action_ApplyMassAffliction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

#nullable disable
public class Action_ApplyMassAffliction : Action_ApplyAffliction
{
  public float radius;
  public bool ignoreCaster;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(this.transform.position, this.radius);
  }

  public override void RunAction()
  {
    if (this.affliction == null)
    {
      Debug.LogError((object) "Your affliction is null bro");
    }
    else
    {
      Debug.Log((object) "Running RPC");
      this.item.photonView.RPC("TryAddAfflictionToLocalCharacter", RpcTarget.All);
    }
  }

  [PunRPC]
  public void TryAddAfflictionToLocalCharacter()
  {
    if (this.ignoreCaster && (Object) this.item.holderCharacter == (Object) Character.localCharacter || (double) Vector3.Distance(Character.localCharacter.Center, this.transform.position) > (double) this.radius)
      return;
    Character.localCharacter.refs.afflictions.AddAffliction(this.affliction);
    if (this.extraAfflictions == null)
      return;
    foreach (Affliction extraAffliction in this.extraAfflictions)
      Character.localCharacter.refs.afflictions.AddAffliction(extraAffliction);
  }
}
