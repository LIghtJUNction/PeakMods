// Decompiled with JetBrains decompiler
// Type: TiedBalloon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using UnityEngine;

#nullable disable
public class TiedBalloon : MonoBehaviourPunCallbacks
{
  public LineRenderer lr;
  public Transform anchor;
  public Transform start;
  public Transform end;
  public Rigidbody rb;
  public MeshRenderer balloonRenderer;
  public float floatForce = 10f;
  public int colorIndex;
  private float initialHeight;
  public float popHeight = 100f;
  public float popTime = 10f;
  private CharacterBalloons characterBalloons;
  private float initialTime;
  private Vector3[] positions = new Vector3[2];

  public void Init(CharacterBalloons characterBalloons, float height, int colorID)
  {
    this.photonView.RPC("RPC_Init", RpcTarget.All, (object) characterBalloons.photonView.ViewID, (object) height, (object) colorID);
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    if (!this.photonView.IsMine)
      return;
    this.photonView.RPC("RPC_Init", newPlayer, (object) this.characterBalloons.photonView.ViewID, (object) this.characterBalloons.character.Center.y, (object) this.colorIndex);
  }

  [PunRPC]
  public void RPC_Init(int characterID, float height, int colorID)
  {
    this.StartCoroutine(this.InitRoutine(characterID, height, colorID));
  }

  private IEnumerator InitRoutine(int characterID, float height, int colorID)
  {
    TiedBalloon tiedBalloon = this;
    while (!(bool) (Object) Character.localCharacter)
      yield return (object) null;
    PhotonView photonView = PhotonView.Find(characterID);
    if ((Object) photonView == (Object) null)
    {
      Debug.LogError((object) "Tried to assign balloon to nonexistent photon ID.");
    }
    else
    {
      CharacterBalloons component = photonView.GetComponent<CharacterBalloons>();
      Debug.Log((object) $"Init Balloon for view {characterID} with color {colorID}");
      tiedBalloon.balloonRenderer.material = Character.localCharacter.refs.balloons.balloonColors[colorID];
      tiedBalloon.colorIndex = colorID;
      tiedBalloon.initialHeight = height;
      tiedBalloon.initialTime = Time.time;
      tiedBalloon.characterBalloons = component;
      component.tiedBalloons.Add(tiedBalloon);
    }
  }

  private void LateUpdate() => this.UpdateLineRenderer();

  private void FixedUpdate()
  {
    this.rb.AddForce(Vector3.up * this.floatForce, ForceMode.Acceleration);
    this.UpdateLineRenderer();
    if (!this.photonView.IsMine || (double) this.rb.transform.position.y <= (double) this.initialHeight + (double) this.popHeight && (double) Time.time <= (double) this.initialTime + (double) this.popTime)
      return;
    this.Pop();
  }

  public void Pop()
  {
    if (!this.photonView.IsMine)
      return;
    PhotonNetwork.Destroy(this.gameObject);
  }

  private void OnDestroy() => this.characterBalloons.RemoveBalloon(this);

  private void UpdateLineRenderer()
  {
    this.positions[0] = this.start.position;
    this.positions[1] = this.end.position;
    this.lr.SetPositions(this.positions);
  }
}
