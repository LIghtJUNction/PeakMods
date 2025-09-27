// Decompiled with JetBrains decompiler
// Type: RopeAnchorProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System.Collections;
using UnityEngine;

#nullable disable
public class RopeAnchorProjectile : MonoBehaviourPunCallbacks
{
  public PhotonView photonView;
  public bool shot;
  private Vector3 startPosition;
  private Quaternion startRotation;
  private Vector3 lastShotTo;
  private float lastShotTravelTime;
  private float lastShotRopeLength;
  private Vector3 lastShotFlyingRotation;

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (!PhotonNetwork.IsMasterClient || !this.shot)
      return;
    this.photonView.RPC("GetShot", newPlayer, (object) this.lastShotTo, (object) this.lastShotTravelTime, (object) this.lastShotRopeLength, (object) this.lastShotFlyingRotation);
  }

  private void Awake() => this.photonView = this.GetComponent<PhotonView>();

  [PunRPC]
  public void GetShot(Vector3 to, float travelTime, float ropeLength, Vector3 flyingRotation)
  {
    this.lastShotTo = to;
    this.lastShotTravelTime = travelTime;
    this.lastShotRopeLength = ropeLength;
    this.lastShotFlyingRotation = flyingRotation;
    this.shot = true;
    this.startRotation = this.transform.rotation;
    this.startPosition = this.transform.position;
    this.StartCoroutine(SpawnRopeRoutine());

    IEnumerator SpawnRopeRoutine()
    {
      float travelledTime = 0.0f;
      this.transform.rotation = ExtQuaternion.FromUpAndRightPrioUp(Vector3.down, flyingRotation);
      while ((double) travelledTime < (double) travelTime)
      {
        travelledTime += Time.deltaTime;
        this.transform.position = Vector3.Lerp(this.startPosition, to, (travelledTime / travelTime).Clamp01());
        yield return (object) null;
      }
      this.transform.position = to;
      this.transform.rotation = this.startRotation;
      AnimationJuice component1 = this.GetComponent<AnimationJuice>();
      component1.PlayParticle(0);
      component1.Screenshake(15f);
      RopeAnchorWithRope component2 = this.GetComponent<RopeAnchorWithRope>();
      component2.ropeSegmentLength = ropeLength;
      component2.SpawnRope();
    }
  }
}
