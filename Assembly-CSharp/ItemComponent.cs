// Decompiled with JetBrains decompiler
// Type: ItemComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

#nullable disable
public abstract class ItemComponent : MonoBehaviourPunCallbacks
{
  [NonSerialized]
  public Item item;
  protected PhotonView photonView;

  public virtual void Awake()
  {
    this.item = this.GetComponent<Item>();
    this.photonView = this.GetComponent<PhotonView>();
    this.StartCoroutine(this.InitializeNextFrame());
  }

  public IEnumerator InitializeNextFrame()
  {
    yield return (object) null;
  }

  public T GetData<T>(DataEntryKey key) where T : DataEntryValue, new()
  {
    return this.item.GetData<T>(key);
  }

  public T GetData<T>(DataEntryKey key, Func<T> getNew) where T : DataEntryValue, new()
  {
    return this.item.GetData<T>(key, getNew);
  }

  public bool HasData(DataEntryKey key) => this.item.data != null && this.item.data.HasData(key);

  public abstract void OnInstanceDataSet();

  public void ForceSync()
  {
    if (!this.photonView.IsMine)
      Debug.LogError((object) "Not allowed to force sync an object you don't own..");
    else
      this.photonView.RPC("SetItemInstanceDataRPC", RpcTarget.Others, (object) this.item.data);
  }
}
