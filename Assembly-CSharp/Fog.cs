// Decompiled with JetBrains decompiler
// Type: Fog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
public class Fog : MonoBehaviour
{
  public float fogHeight;
  public float fogSpeed = 0.4f;
  public float amount;
  private static readonly int FogHeight = Shader.PropertyToID(nameof (FogHeight));
  private Transform[] stops;
  private int currentStop;
  private float sinceStop;
  public float maxWaitTime = 180f;
  public float startMoveHeightThreshold = 60f;
  private bool waiting;
  private PhotonView view;
  public ParticleSystem fogParticles;
  private float syncCounter;

  private bool IsInFog
  {
    get => (double) Character.localCharacter.Center.y < (double) this.transform.position.y;
  }

  private void Start() => this.view = this.GetComponent<PhotonView>();

  private void Update()
  {
    if ((Object) Character.localCharacter == (Object) null)
      return;
    if (this.stops == null)
    {
      Debug.LogError((object) "Disabling fog movement: No stops were found");
      this.enabled = false;
    }
    else
    {
      this.Movement();
      this.MakePlayerCold();
      this.ApplyVisuals();
      if (this.view.IsMine)
        this.Sync();
      if ((Object) this.fogParticles == (Object) null)
        return;
      this.fogParticles.transform.position = Character.localCharacter.Center;
      if (this.IsInFog)
      {
        this.fogParticles.Play();
        Character.localCharacter.data.isInFog = true;
      }
      else
      {
        this.fogParticles.Stop();
        Character.localCharacter.data.isInFog = false;
      }
    }
  }

  private void Sync()
  {
    this.syncCounter += Time.deltaTime;
    if ((double) this.syncCounter <= 5.0)
      return;
    this.syncCounter = 0.0f;
    this.view.RPC("RPCA_SyncFog", RpcTarget.Others, (object) this.fogHeight);
  }

  private void ApplyVisuals()
  {
    this.transform.position = new Vector3(Character.localCharacter.Center.x, this.fogHeight, Mathf.Clamp(Character.localCharacter.Center.z, -10000f, 870f));
    Shader.SetGlobalFloat(Fog.FogHeight, this.transform.position.y);
  }

  private void MakePlayerCold()
  {
    if ((Object) Character.localCharacter == (Object) null || !this.IsInFog)
      return;
    Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, this.amount * Time.deltaTime);
  }

  private void Movement()
  {
    if (this.waiting)
      this.Wait();
    else
      this.Move();
  }

  private void Wait()
  {
    if (!this.view.IsMine)
      return;
    this.sinceStop += Time.deltaTime;
    if (!this.TimeToMove() && !this.PlayersHaveMovedOn())
      return;
    this.view.RPC("RPCA_Resume", RpcTarget.All);
  }

  private bool TimeToMove()
  {
    return (double) this.sinceStop > (double) this.maxWaitTime && this.currentStop > 0;
  }

  private bool PlayersHaveMovedOn()
  {
    if (Character.AllCharacters.Count == 0)
      return false;
    float num = this.StopHeight() + this.startMoveHeightThreshold;
    for (int index = 0; index < Character.AllCharacters.Count; ++index)
    {
      if ((double) Character.AllCharacters[index].Center.y < (double) num)
        return false;
    }
    Debug.Log((object) "Players have moved on");
    return true;
  }

  [PunRPC]
  private void RPCA_Resume()
  {
    ++this.currentStop;
    this.waiting = false;
    GUIManager.instance.TheFogRises();
  }

  private void Move()
  {
    if (this.currentStop >= this.stops.Length)
      return;
    this.fogHeight += Time.deltaTime * this.fogSpeed;
    if ((double) this.fogHeight <= (double) this.StopHeight())
      return;
    this.Stop();
  }

  private void Stop()
  {
    this.sinceStop = 0.0f;
    this.waiting = true;
  }

  private float StopHeight() => this.stops[this.currentStop].transform.position.y;

  [PunRPC]
  public void RPCA_SyncFog(float setHeight) => this.fogHeight = setHeight;
}
