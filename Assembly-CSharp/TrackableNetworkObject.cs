// Decompiled with JetBrains decompiler
// Type: TrackableNetworkObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (PhotonView))]
public class TrackableNetworkObject : ItemComponent
{
  public static List<TrackableNetworkObject> ALL_TRACKABLES = new List<TrackableNetworkObject>();
  public int instanceID;
  private static int currentMaxInstanceID = 1;
  public TrackNetworkedObject currentTracker;
  public static Action<int> OnTrackableObjectCreated;

  public new PhotonView photonView { get; private set; }

  public bool hasTracker => (UnityEngine.Object) this.currentTracker != (UnityEngine.Object) null;

  public override void Awake()
  {
    base.Awake();
    this.photonView = this.GetComponent<PhotonView>();
  }

  private new void OnEnable() => TrackableNetworkObject.ALL_TRACKABLES.Add(this);

  public new void OnDisable() => TrackableNetworkObject.ALL_TRACKABLES.Remove(this);

  private void Start()
  {
    this.Init();
    if (TrackableNetworkObject.OnTrackableObjectCreated == null)
      return;
    Debug.Log((object) $"OnTrackableObjectCreated on photon view {this.photonView.ViewID} with instance ID {this.instanceID}");
    TrackableNetworkObject.OnTrackableObjectCreated(this.instanceID);
  }

  public override void OnJoinedRoom()
  {
    this.Init();
    if (TrackableNetworkObject.OnTrackableObjectCreated == null)
      return;
    TrackableNetworkObject.OnTrackableObjectCreated(this.instanceID);
  }

  public static TrackableNetworkObject GetTrackableObject(int instanceID)
  {
    for (int index = 0; index < TrackableNetworkObject.ALL_TRACKABLES.Count; ++index)
    {
      if ((UnityEngine.Object) TrackableNetworkObject.ALL_TRACKABLES[index] != (UnityEngine.Object) null && TrackableNetworkObject.ALL_TRACKABLES[index].instanceID == instanceID)
        return TrackableNetworkObject.ALL_TRACKABLES[index];
    }
    return (TrackableNetworkObject) null;
  }

  private void Init()
  {
    if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient || this.GetData<IntItemData>(DataEntryKey.InstanceID).Value != 0)
      return;
    this.instanceID = TrackableNetworkObject.currentMaxInstanceID;
    ++TrackableNetworkObject.currentMaxInstanceID;
    this.photonView.RPC("SetInstanceIDRPC", RpcTarget.All, (object) this.instanceID, (object) TrackableNetworkObject.currentMaxInstanceID);
    Debug.Log((object) $"Setting instance id to {this.instanceID}");
  }

  public override void OnInstanceDataSet()
  {
    this.instanceID = this.GetData<IntItemData>(DataEntryKey.InstanceID).Value;
  }

  [PunRPC]
  public void SetInstanceIDRPC(int instanceID, int maxInstanceID)
  {
    this.GetData<IntItemData>(DataEntryKey.InstanceID).Value = instanceID;
    TrackableNetworkObject.currentMaxInstanceID = maxInstanceID;
  }
}
