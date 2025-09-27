// Decompiled with JetBrains decompiler
// Type: PSM_AddPlayerCountBasedDespawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class PSM_AddPlayerCountBasedDespawner : PropSpawnerMod
{
  public bool onePerPlayer;
  public int destroyAllIfLessThan;

  public override void ModifyObject(GameObject spawned, PropSpawner.SpawnData spawnData)
  {
    if (!(bool) (Object) spawned.GetComponent<PhotonView>())
      return;
    DestroyBasedOnPlayerCount basedOnPlayerCount = spawned.AddComponent<DestroyBasedOnPlayerCount>();
    if (this.onePerPlayer)
      basedOnPlayerCount.destroyIfPlayerCountIsLessThan = spawnData.spawnCount + 1;
    else
      basedOnPlayerCount.destroyIfPlayerCountIsLessThan = this.destroyAllIfLessThan;
  }
}
