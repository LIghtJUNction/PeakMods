// Decompiled with JetBrains decompiler
// Type: RopeAnchorWithRope
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System.Collections;
using UnityEngine;

#nullable disable
public class RopeAnchorWithRope : MonoBehaviourPunCallbacks
{
  public float ropeSegmentLength = 20f;
  public float spoolOutTime = 5f;
  public GameObject ropePrefab;
  public GameObject ropeInstance;
  public RopeAnchor anchor;
  public Rope rope;

  public override void OnJoinedRoom()
  {
    base.OnJoinedRoom();
    this.SpawnRope();
  }

  public Rope SpawnRope()
  {
    if (!this.photonView.IsMine)
      return (Rope) null;
    this.ropeInstance = PhotonNetwork.Instantiate(this.ropePrefab.name, this.anchor.anchorPoint.position, this.anchor.anchorPoint.rotation);
    this.rope = this.ropeInstance.GetComponent<Rope>();
    this.rope.photonView.RPC("AttachToAnchor_Rpc", RpcTarget.AllBuffered, (object) this.anchor.photonView);
    this.rope.Segments = this.ropeSegmentLength;
    this.StartCoroutine(SpoolOut());
    return this.rope;

    IEnumerator SpoolOut()
    {
      float elapsed = 0.0f;
      while ((double) elapsed < (double) this.spoolOutTime)
      {
        elapsed += Time.deltaTime;
        this.rope.Segments = Mathf.Lerp(0.0f, this.ropeSegmentLength, (elapsed / this.spoolOutTime).Clamp01());
        yield return (object) null;
      }
    }
  }

  public virtual void Awake() => this.anchor = this.GetComponent<RopeAnchor>();
}
