// Decompiled with JetBrains decompiler
// Type: RopeSpool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using UnityEngine;

#nullable disable
public class RopeSpool : ItemComponent
{
  public float segments;
  public float minSegments = 3.5f;
  public float ropeStartFuel = 60f;
  private float ropeFuel = 60f;
  public GameObject ropePrefab;
  public Transform ropeBase;
  public Transform ropeStart;
  public Transform ropeSpoolTf;
  public GameObject ropeInstance;
  public Rigidbody rig;
  public Rope rope;
  private float scroll;
  private float segsVel;
  private RopeTier ropeTier;
  public bool isAntiRope;

  public bool IsOutOfRope => (double) this.ropeFuel <= 2.0;

  public float RopeFuel
  {
    get
    {
      return this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value;
    }
    set
    {
      this.GetData<FloatItemData>(DataEntryKey.Fuel, new Func<FloatItemData>(this.DefaultFuel)).Value = value;
      this.ropeFuel = value;
      if ((double) this.ropeFuel <= 2.0)
        this.photonView.RPC("Consume", RpcTarget.All, (object) ((UnityEngine.Object) this.item.holderCharacter == (UnityEngine.Object) null ? -1 : this.item.holderCharacter.photonView.ViewID));
      this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
    }
  }

  private FloatItemData DefaultFuel()
  {
    return new FloatItemData()
    {
      Value = this.ropeStartFuel
    };
  }

  public float Segments
  {
    get => this.segments;
    set => this.segments = value;
  }

  public override void Awake()
  {
    base.Awake();
    this.ropeTier = this.GetComponent<RopeTier>();
    this.rig = this.GetComponent<Rigidbody>();
  }

  private void OnDestroy()
  {
    if (this.item.itemState == ItemState.Held && this.photonView.IsMine)
      this.ClearRope();
    if (!this.photonView.IsMine)
      return;
    this.ropeFuel = this.RopeFuel;
    this.item.SetUseRemainingPercentage(this.ropeFuel / this.ropeStartFuel);
  }

  private void Update()
  {
    if (this.item.itemState != ItemState.Held || this.IsOutOfRope || !this.photonView.IsMine)
      return;
    if ((UnityEngine.Object) this.ropeInstance == (UnityEngine.Object) null && !this.IsOutOfRope)
    {
      this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.ropeBase.position, this.ropeBase.rotation);
      this.rope = this.ropeInstance.GetComponent<Rope>();
      this.rope.photonView.RPC("AttachToSpool_Rpc", RpcTarget.AllBuffered, (object) this.photonView);
      this.Segments = 0.0f;
      this.segsVel = 0.0f;
      this.scroll = 0.0f;
      this.rope.Segments = this.Segments;
    }
    this.item.SetUseRemainingPercentage(((this.ropeFuel - this.rope.Segments) / this.ropeStartFuel).Clamp01());
    this.scroll = !this.item.holderCharacter.input.scrollForwardIsPressed ? (!this.item.holderCharacter.input.scrollBackwardIsPressed ? this.item.holderCharacter.input.scrollInput : -0.4f) : 0.4f;
    if (!this.ropeTier.LookingToPlaceAnchor)
      return;
    this.scroll = 0.0f;
    this.segsVel = 0.0f;
  }

  private void FixedUpdate()
  {
    this.segsVel = Mathf.Lerp(this.segsVel, this.scroll, Time.fixedDeltaTime * 4f);
    this.segsVel = Mathf.Clamp(this.segsVel, -1f, 5f);
    if (!this.photonView.IsMine || !((UnityEngine.Object) this.rope != (UnityEngine.Object) null))
      return;
    this.Segments += (float) ((double) this.segsVel * (double) Time.fixedDeltaTime * 25.0);
    this.Segments = Mathf.Clamp(this.Segments, this.minSegments, Mathf.Min(this.ropeFuel, (float) Rope.MaxSegments));
    this.ropeSpoolTf.transform.localEulerAngles += new Vector3(0.0f, 0.0f, (this.Segments - this.rope.Segments) * -50f);
    this.rope.Segments = this.Segments;
  }

  public void ClearRope()
  {
    Debug.Log((object) $"ClearRope{this.ropeInstance}");
    if ((UnityEngine.Object) this.ropeInstance != (UnityEngine.Object) null)
    {
      Debug.Log((object) "Destroy rope");
      PhotonNetwork.Destroy(this.rope.view);
    }
    this.rope = (Rope) null;
  }

  public override void OnInstanceDataSet()
  {
    if (!this.HasData(DataEntryKey.Fuel))
      return;
    Debug.Log((object) "HasData");
    this.ropeFuel = this.GetData<FloatItemData>(DataEntryKey.Fuel).Value;
    Debug.Log((object) $"ropeFuel {this.ropeFuel}");
  }
}
