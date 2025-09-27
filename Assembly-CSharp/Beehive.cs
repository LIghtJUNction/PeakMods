// Decompiled with JetBrains decompiler
// Type: Beehive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Beehive : ItemComponent
{
  public bool spawnBees = true;
  public BeeSwarm beeSwarmPrefab;
  public BeeSwarm currentBees;
  public int instanceID;
  private static int currentMaxInstanceID = 1;
  public static List<Beehive> ALL_BEEHIVES = new List<Beehive>();
  private bool initialized;

  public override void OnJoinedRoom() => this.Init();

  public void Start()
  {
    if (!PhotonNetwork.InRoom)
      return;
    this.Init();
  }

  private void Init()
  {
    if (this.initialized)
      return;
    this.initialized = true;
    if (!PhotonNetwork.IsMasterClient)
      return;
    if (!this.HasData(DataEntryKey.InstanceID))
    {
      this.instanceID = Beehive.currentMaxInstanceID;
      ++Beehive.currentMaxInstanceID;
      this.photonView.RPC("SetInstanceIDRPC", RpcTarget.All, (object) this.instanceID, (object) Beehive.currentMaxInstanceID);
    }
    if (this.HasData(DataEntryKey.SpawnedBees) || !this.spawnBees)
      return;
    this.currentBees = PhotonNetwork.Instantiate(this.beeSwarmPrefab.gameObject.name, this.transform.position, Quaternion.identity).GetComponent<BeeSwarm>();
    this.currentBees.SetBeehive(this);
    this.GetData<BoolItemData>(DataEntryKey.SpawnedBees).Value = true;
  }

  public override void OnInstanceDataSet()
  {
    this.instanceID = this.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
  }

  [PunRPC]
  public void SetInstanceIDRPC(int instanceID, int maxInstanceID)
  {
    this.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
    Beehive.currentMaxInstanceID = maxInstanceID;
  }

  public override void OnEnable()
  {
    base.OnEnable();
    Beehive.ALL_BEEHIVES.Add(this);
  }

  public override void OnDisable()
  {
    base.OnDisable();
    Beehive.ALL_BEEHIVES.Remove(this);
  }

  public static Beehive GetBeehive(int instanceID)
  {
    for (int index = 0; index < Beehive.ALL_BEEHIVES.Count; ++index)
    {
      if ((Object) Beehive.ALL_BEEHIVES[index] != (Object) null && Beehive.ALL_BEEHIVES[index].instanceID == instanceID)
        return Beehive.ALL_BEEHIVES[index];
    }
    return (Beehive) null;
  }

  private void OnDestroy()
  {
    if (!PhotonNetwork.IsMasterClient || !((Object) this.currentBees != (Object) null))
      return;
    this.currentBees.HiveDestroyed(this.item.Center());
  }
}
