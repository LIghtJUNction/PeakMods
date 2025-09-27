// Decompiled with JetBrains decompiler
// Type: ItemPhysicsSyncer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class ItemPhysicsSyncer : PhotonBinaryStreamSerializer<ItemPhysicsSyncData>
{
  private Item m_item;
  private PhotonView m_photonView;
  private Optionable<Vector3> m_lastPos;
  private Optionable<ItemState> m_lastState;
  private Coroutine m_fadeRoutine;
  public bool debug;
  [SerializeField]
  internal bool shouldSync = true;
  private int forceSyncFrames;
  [SerializeField]
  private Vector3 lastRecievedLinearVelocity;
  [SerializeField]
  private Vector3 lastRecievedAngularVelocity;
  [SerializeField]
  private Vector3 lastRecievedPosition;

  protected override void Awake()
  {
    base.Awake();
    this.m_photonView = this.GetComponent<PhotonView>();
    this.m_item = this.GetComponent<Item>();
  }

  public void ForceSyncForFrames(int frames = 10) => this.forceSyncFrames = frames;

  private void FixedUpdate()
  {
    Rigidbody rig = this.m_item.rig;
    if ((Object) rig == (Object) null || !PhotonNetwork.InRoom || this.m_photonView.IsMine)
      return;
    if (this.RemoteValue.IsNone)
    {
      if (!this.debug)
        return;
      Debug.Log((object) "NO REMOTE VALUE");
    }
    else if (!this.shouldSync)
    {
      if (!this.debug)
        return;
      Debug.Log((object) "NOT SYNCING");
    }
    else
    {
      if (this.m_item.itemState != ItemState.Ground)
        return;
      if (this.m_lastPos.IsNone)
      {
        if (!this.debug)
          return;
        Debug.Log((object) "NO LAST POSITION");
      }
      else
      {
        double num = 1.0 / (double) PhotonNetwork.SerializationRate;
        this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
        float t = this.sinceLastPackage / (float) num;
        ItemPhysicsSyncData itemPhysicsSyncData = this.RemoteValue.Value;
        Vector3 vector3_1 = Vector3.Lerp(this.m_lastPos.Value, (Vector3) itemPhysicsSyncData.position, t);
        Vector3 vector3_2 = vector3_1 - rig.position;
        this.lastRecievedPosition = vector3_1;
        rig.MovePosition(rig.position + vector3_2 * 0.5f);
        rig.MoveRotation(Quaternion.RotateTowards(rig.rotation, itemPhysicsSyncData.rotation, Time.fixedDeltaTime * this.maxAngleChangePerSecond));
        if (!this.debug)
          return;
        Debug.Log((object) ("MOVING TO POSITION " + vector3_1.ToString()));
      }
    }
  }

  protected virtual float maxAngleChangePerSecond => 90f;

  public void ResetRecievedData()
  {
    this.lastRecievedPosition = this.transform.position;
    this.RemoteValue = Optionable<ItemPhysicsSyncData>.None;
  }

  public override ItemPhysicsSyncData GetDataToWrite()
  {
    ItemPhysicsSyncData dataToWrite = new ItemPhysicsSyncData();
    Rigidbody rig = this.m_item.rig;
    if ((Object) rig != (Object) null)
    {
      dataToWrite.linearVelocity = (float3) rig.linearVelocity;
      dataToWrite.angularVelocity = (float3) rig.angularVelocity;
      dataToWrite.position = (float3) rig.position;
      dataToWrite.rotation = rig.rotation;
      if (this.debug)
        Debug.Log((object) ("SENDING POSITION " + dataToWrite.position.ToString()));
    }
    return dataToWrite;
  }

  public override bool ShouldSendData()
  {
    if (this.forceSyncFrames > 0 && this.m_item.itemState == ItemState.Ground)
    {
      --this.forceSyncFrames;
      return true;
    }
    return this.shouldSync && !this.m_item.rig.isKinematic && !this.m_item.rig.IsSleeping() && this.m_item.itemState == ItemState.Ground;
  }

  public override void OnDataReceived(ItemPhysicsSyncData data)
  {
    if (this.debug)
      Debug.Log((object) "RECIEVED DATA 1");
    base.OnDataReceived(data);
    Rigidbody rig = this.m_item.rig;
    if ((Object) rig == (Object) null || this.m_item.itemState != ItemState.Ground)
      return;
    if (!this.shouldSync)
    {
      if (!this.debug)
        return;
      Debug.Log((object) "SHOULDSYNC OFF");
    }
    else
    {
      if (rig.isKinematic)
        return;
      if (this.debug)
        Debug.Log((object) "RECIEVED DATA 2");
      this.m_lastPos = Optionable<Vector3>.Some(rig.position);
      rig.linearVelocity = (Vector3) data.linearVelocity;
      rig.angularVelocity = (Vector3) data.angularVelocity;
      this.lastRecievedLinearVelocity = (Vector3) data.linearVelocity;
      this.lastRecievedAngularVelocity = (Vector3) data.angularVelocity;
    }
  }
}
