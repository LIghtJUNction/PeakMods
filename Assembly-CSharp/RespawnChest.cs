// Decompiled with JetBrains decompiler
// Type: RespawnChest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RespawnChest : Luggage, IInteractible
{
  public GameObject skeleton;

  public override string GetInteractionText()
  {
    return Character.PlayerIsDeadOrDown() ? LocalizedText.GetText("REVIVESCOUTS") : LocalizedText.GetText("TOUCH");
  }

  private void DebugSpawn() => this.SpawnItems(this.GetSpawnSpots());

  public override void Interact(Character interactor)
  {
  }

  public override void Interact_CastFinished(Character interactor)
  {
    base.Interact_CastFinished(interactor);
    GlobalEvents.TriggerRespawnChestOpened(this, interactor);
  }

  public override List<PhotonView> SpawnItems(List<Transform> spawnSpots)
  {
    List<PhotonView> photonViewList = new List<PhotonView>();
    if (!PhotonNetwork.IsMasterClient)
      return photonViewList;
    if (Ascents.canReviveDead && Character.PlayerIsDeadOrDown())
    {
      this.photonView.RPC("RemoveSkeletonRPC", RpcTarget.AllBuffered);
      this.RespawnAllPlayersHere();
    }
    else
      base.SpawnItems(spawnSpots);
    return photonViewList;
  }

  [PunRPC]
  private void RemoveSkeletonRPC() => this.skeleton.SetActive(false);

  private void RespawnAllPlayersHere()
  {
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (allCharacter.data.dead || allCharacter.data.fullyPassedOut)
        allCharacter.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, (object) (this.transform.position + this.transform.up * 8f), (object) true);
    }
  }

  public new bool IsInteractible(Character interactor) => this.state == Luggage.LuggageState.Closed;
}
