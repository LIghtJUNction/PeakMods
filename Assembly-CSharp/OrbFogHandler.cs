// Decompiled with JetBrains decompiler
// Type: OrbFogHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class OrbFogHandler : Singleton<OrbFogHandler>, IInRoomCallbacks
{
  public float speed = 0.3f;
  public float maxWaitTime = 500f;
  public float currentWaitTime;
  public bool hasArrived;
  public bool isMoving;
  public float currentSize;
  public float currentStartHeight;
  public float currentStartForward;
  public float dispelFogAmount;
  private FogSphere sphere;
  private FogSphereOrigin[] origins;
  private int currentID;
  private float syncCounter;
  private PhotonView photonView;
  public AnimationCurve fogRevealCurve;
  public AnimationCurve fogFadeCurve;
  public float currentCloseFog = 1f;

  protected override void Awake()
  {
    base.Awake();
    this.photonView = this.GetComponent<PhotonView>();
  }

  private void Start()
  {
    this.sphere = this.GetComponentInChildren<FogSphere>();
    this.origins = this.transform.root.GetComponentsInChildren<FogSphereOrigin>();
    this.InitNewSphere(this.origins[this.currentID]);
  }

  private void OnEnable() => PhotonNetwork.AddCallbackTarget((object) this);

  private void OnDisable()
  {
    PhotonNetwork.RemoveCallbackTarget((object) this);
    Shader.SetGlobalFloat("FakeMountainEnabled", 1f);
  }

  private void Update()
  {
    int num = (Object) this.sphere != (Object) null ? 1 : 0;
    if (!this.hasArrived)
    {
      bool flag = this.currentID >= this.origins.Length || this.origins[this.currentID].disableFog;
      if (Ascents.fogEnabled && !flag)
      {
        if (this.isMoving)
          this.Move();
        else
          this.WaitToMove();
      }
    }
    if (PhotonNetwork.IsMasterClient)
      this.Sync();
    this.ApplyMeshEffects();
    this.currentCloseFog = Mathf.Lerp(this.currentCloseFog, Mathf.Lerp(1f, 5f, this.dispelFogAmount), Time.deltaTime * 1f);
    Shader.SetGlobalFloat("CloseDistanceMod", this.currentCloseFog);
  }

  private void Sync()
  {
    this.syncCounter += Time.deltaTime;
    if ((double) this.syncCounter <= 5.0)
      return;
    this.syncCounter = 0.0f;
    this.photonView.RPC("RPCA_SyncFog", RpcTarget.Others, (object) this.currentSize, (object) this.isMoving);
  }

  [PunRPC]
  public void RPCA_SyncFog(float s, bool moving)
  {
    this.currentSize = s;
    this.isMoving = moving;
  }

  public IEnumerator WaitForFogCatchUp()
  {
    this.isMoving = true;
    while ((double) this.currentSize > 30.0 && this.isMoving && !this.hasArrived)
    {
      this.currentSize = Mathf.Lerp(this.currentSize, 29.5f, Time.deltaTime);
      this.currentSize = Mathf.MoveTowards(this.currentSize, 29.5f, Time.deltaTime);
      yield return (object) null;
    }
  }

  public IEnumerator WaitForReveal()
  {
    float c = 0.0f;
    float t = 5f;
    this.sphere.ENABLE = 1f;
    while ((double) c < (double) t)
    {
      c += Time.deltaTime;
      this.sphere.REVEAL_AMOUNT = this.fogRevealCurve.Evaluate(c / t);
      this.sphere.ENABLE = this.fogFadeCurve.Evaluate(c / t);
      yield return (object) null;
    }
    this.sphere.REVEAL_AMOUNT = 1f;
    this.sphere.ENABLE = 0.0f;
    this.currentSize = 800f;
  }

  public IEnumerator DisableFog()
  {
    float c = 0.0f;
    float t = 1f;
    while ((double) c < (double) t)
    {
      c += Time.deltaTime;
      this.sphere.ENABLE = (float) (1.0 - (double) c / (double) t);
      yield return (object) null;
    }
    this.sphere.ENABLE = 0.0f;
    this.sphere.REVEAL_AMOUNT = 0.0f;
    this.currentSize = 800f;
  }

  private void Move()
  {
    this.sphere.REVEAL_AMOUNT = 0.0f;
    this.sphere.ENABLE = Mathf.MoveTowards(this.sphere.ENABLE, 1f, Time.deltaTime * 0.1f);
    this.currentSize -= this.speed * Time.deltaTime;
    if ((double) this.currentSize > 30.0)
      return;
    this.Stop();
  }

  private void Stop()
  {
    this.hasArrived = true;
    this.isMoving = false;
  }

  private void WaitToMove()
  {
    this.currentWaitTime += Time.deltaTime;
    if (!this.PlayersHaveMovedOn() && !this.TimeToMove() || !PhotonNetwork.IsMasterClient)
      return;
    this.photonView.RPC("StartMovingRPC", RpcTarget.All);
  }

  private bool TimeToMove()
  {
    return Ascents.currentAscent >= 0 && (double) this.currentWaitTime > (double) this.maxWaitTime && this.currentID > 0;
  }

  private bool PlayersHaveMovedOn()
  {
    if (Character.AllCharacters.Count == 0 || Ascents.currentAscent < 0)
      return false;
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      if ((double) Character.AllCharacters[index].Center.y < (double) this.currentStartHeight || (double) Character.AllCharacters[index].Center.z < (double) this.currentStartForward)
        return false;
    }
    Debug.Log((object) "Players have moved on");
    return true;
  }

  private void ApplyMeshEffects() => this.sphere.currentSize = this.currentSize;

  public void InitNewSphere(FogSphereOrigin newOrigin)
  {
    this.sphere.fogPoint = newOrigin.transform.position;
    this.currentSize = newOrigin.size;
    this.currentStartHeight = newOrigin.moveOnHeight;
    this.currentStartForward = newOrigin.moveOnForward;
  }

  [PunRPC]
  public void StartMovingRPC()
  {
    this.currentWaitTime = 0.0f;
    this.hasArrived = false;
    this.isMoving = true;
    GUIManager.instance.TheFogRises();
  }

  public void SetFogOrigin(int id)
  {
    this.currentID = id;
    if (this.currentID < this.origins.Length)
    {
      this.hasArrived = false;
      this.sphere.gameObject.SetActive(true);
      this.InitNewSphere(this.origins[this.currentID]);
    }
    else
    {
      this.hasArrived = true;
      Debug.Log((object) "Last section, disabling fog sphere");
      this.sphere.gameObject.SetActive(false);
    }
  }

  public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    this.photonView.RPC("RPCA_SyncFog", newPlayer, (object) this.currentSize, (object) this.isMoving);
    if (!PhotonNetwork.IsMasterClient)
      return;
    this.StartCoroutine(KillLateJoinedPlayer());

    IEnumerator KillLateJoinedPlayer()
    {
      bool flag = false;
      for (float timeout = 30f; !flag && (double) timeout >= 0.0; flag = (Object) PlayerHandler.GetPlayerCharacter(newPlayer) != (Object) null)
      {
        timeout -= Time.deltaTime;
        yield return (object) null;
      }
      if (flag && this.isMoving)
      {
        Debug.Log((object) $"{newPlayer.ActorNumber} has spawned but fog is active. Killing them... ");
        PlayerHandler.GetPlayerCharacter(newPlayer);
      }
    }
  }

  public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
  {
  }

  public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
  {
  }

  public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
  {
  }

  public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
  {
  }
}
