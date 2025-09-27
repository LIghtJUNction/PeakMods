// Decompiled with JetBrains decompiler
// Type: BreakableBridge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using pworld.Scripts.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class BreakableBridge : OnNetworkStart
{
  private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");
  private static readonly int BreakAmount = Shader.PropertyToID("_BreakAmount");
  private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");
  public SFX_Instance[] breakSfx;
  [Range(0.0f, 1f)]
  public float breakPoint = 0.4f;
  [Range(0.0f, 1f)]
  public float breakChance = 0.5f;
  public Vector3 axisMul = new Vector3(1f, 1f, 1f);
  public float shakeScale = 30f;
  public float fallTime = 5f;
  public float amount = 1f;
  public float startShakeDistance = 10f;
  public float startShakeAmount = 400f;
  public float climbingScreenShake = 240f;
  public float screenShakeTickTime = 0.2f;
  public bool debug;
  public bool isShaking;
  public float localTouchStamp;
  public int holdsPeople;
  public int peopleOnBridge;
  public Transform fullMesh;
  public ParticleSystem breakParticles;
  private readonly Dictionary<Character, float> peopleOnBridgeDict = new Dictionary<Character, float>();
  private PhotonView photonView;
  private Renderer rend;
  private AudioSource source;
  private JungleVine jungleVine;
  private float timeUntilBreak;
  private bool isBreaking;
  private bool isFallen;

  public bool LocalCharacterOnBridge
  {
    get => (double) Time.time - (double) this.localTouchStamp < 0.20000000298023224;
  }

  private float DistanceToLocalPlayer
  {
    get => Vector3.Distance(Character.localCharacter.Center, this.transform.position);
  }

  private void Awake()
  {
    this.jungleVine = this.GetComponent<JungleVine>();
    this.photonView = this.GetComponent<PhotonView>();
    this.source = this.GetComponent<AudioSource>();
    foreach (CollisionModifier componentsInChild in this.GetComponentsInChildren<CollisionModifier>())
    {
      componentsInChild.applyEffects = false;
      componentsInChild.onCollide += new Action<Character, CollisionModifier, Collision, Bodypart>(this.OnBridgeCollision);
    }
    this.rend = this.GetComponentInChildren<Renderer>();
    this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0.0f);
    this.rend.material.SetFloat(BreakableBridge.AlphaClip, 0.01f);
    if (this.holdsPeople != 0)
      return;
    this.holdsPeople = 5;
  }

  public override void NetworkStart()
  {
    this.holdsPeople = UnityEngine.Random.Range(1, 5);
    this.photonView.RPC("SyncHoldsPeopleRPC", RpcTarget.All, (object) this.holdsPeople);
  }

  private void Update()
  {
    if (this.isShaking)
    {
      this.source.pitch += 0.1f * Time.deltaTime;
      this.source.volume += 0.1f * Time.deltaTime;
      this.source.enabled = true;
    }
    if (!this.photonView.IsMine || !this.isBreaking || this.isShaking || this.isFallen)
      return;
    this.timeUntilBreak -= Time.deltaTime;
    if ((double) this.timeUntilBreak >= 0.0)
      return;
    this.photonView.RPC("ShakeBridge_Rpc", RpcTarget.All);
  }

  private void FixedUpdate()
  {
    this.peopleOnBridge = 0;
    if (this.debug)
      Debug.Log((object) $"FixedUpdate: {Time.frameCount}, peopleOnBridge: {this.peopleOnBridge}");
    this.peopleOnBridge = 0;
    foreach (Character key in this.peopleOnBridgeDict.Keys.ToList<Character>())
    {
      this.peopleOnBridgeDict[key] += Time.deltaTime;
      if ((double) this.peopleOnBridgeDict[key] < 0.25)
        ++this.peopleOnBridge;
    }
  }

  private void OnDestroy()
  {
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (PhotonNetwork.IsMasterClient || newPlayer == PhotonNetwork.LocalPlayer)
      return;
    this.photonView.RPC("SyncHoldsPeopleRPC", newPlayer, (object) this.holdsPeople);
  }

  [PunRPC]
  public void SyncHoldsPeopleRPC(int holdsPeople) => this.holdsPeople = holdsPeople;

  public void AddCollisionModifiers()
  {
    Debug.Log((object) nameof (AddCollisionModifiers));
    Collider[] componentsInChildren = this.GetComponentsInChildren<Collider>();
    Debug.Log((object) $"colliers: {componentsInChildren.Length}");
    foreach (Component component in componentsInChildren)
      component.gameObject.AddComponent<CollisionModifier>();
  }

  private void OnBridgeCollision(
    Character character,
    CollisionModifier collider,
    Collision collision,
    Bodypart bodypart)
  {
    if (this.isBreaking)
      return;
    if ((UnityEngine.Object) character == (UnityEngine.Object) Character.localCharacter)
      this.localTouchStamp = Time.time;
    if (!this.photonView.IsMine)
      return;
    if (!this.peopleOnBridgeDict.TryAdd(character, 0.0f))
      this.peopleOnBridgeDict[character] = 0.0f;
    if (this.peopleOnBridge < this.holdsPeople || this.isShaking || this.holdsPeople >= this.peopleOnBridge)
      return;
    this.isBreaking = true;
    this.timeUntilBreak = UnityEngine.Random.Range(2.5f, 7.5f);
  }

  [PunRPC]
  private void ShakeBridge_Rpc()
  {
    Debug.Log((object) "start shake rock");
    this.isShaking = true;
    this.source.enabled = true;
    this.source.Play();
    if (!this.isShaking)
      this.source.volume = 0.125f;
    if ((double) this.DistanceToLocalPlayer < (double) this.startShakeDistance)
    {
      Debug.Log((object) $"start shake {this.startShakeAmount}");
      GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount);
    }
    this.StartCoroutine(RockShake());

    IEnumerator RockShake()
    {
      BreakableBridge breakableBridge = this;
      Debug.Log((object) "Start shaking");
      float duration = 0.0f;
      float timeUntilShake = 0.0f;
      breakableBridge.rend.material.SetFloat(BreakableBridge.JitterAmount, 1f);
      while ((double) duration < (double) breakableBridge.fallTime)
      {
        timeUntilShake -= Time.deltaTime;
        if (breakableBridge.LocalCharacterOnBridge && (double) timeUntilShake <= 0.0)
        {
          GamefeelHandler.instance.AddPerlinShake(breakableBridge.climbingScreenShake);
          Debug.Log((object) "Clime shake");
          timeUntilShake = breakableBridge.screenShakeTickTime;
        }
        Vector3 zero = (Vector3) Vector2.zero;
        zero.x += Mathf.PerlinNoise1D((float) (100.0 + (double) duration * (double) breakableBridge.shakeScale)) * breakableBridge.axisMul.x;
        zero.y += Mathf.PerlinNoise1D((float) (10300.0 + (double) duration * (double) breakableBridge.shakeScale)) * breakableBridge.axisMul.y;
        zero.z += Mathf.PerlinNoise1D((float) (1340.0 + (double) duration * (double) breakableBridge.shakeScale)) * breakableBridge.axisMul.z;
        zero *= breakableBridge.amount;
        duration += Time.deltaTime;
        yield return (object) null;
      }
      breakableBridge.rend.material.SetFloat(BreakableBridge.JitterAmount, 0.0f);
      Debug.Log((object) "Done shaking");
      if (breakableBridge.isShaking)
      {
        for (int index = 0; index < breakableBridge.breakSfx.Length; ++index)
          breakableBridge.breakSfx[index].Play(breakableBridge.transform.position);
      }
      breakableBridge.isShaking = false;
      breakableBridge.fullMesh.localPosition = 0.ToVec();
      breakableBridge.source.volume = 0.0f;
      breakableBridge.source.Stop();
      if (breakableBridge.photonView.IsMine)
        breakableBridge.photonView.RPC("Fall_Rpc", RpcTarget.All);
    }
  }

  [PunRPC]
  private void Fall_Rpc()
  {
    this.StartCoroutine(DestroyRoutine());

    IEnumerator DestroyRoutine()
    {
      BreakableBridge breakableBridge = this;
      breakableBridge.isFallen = true;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) breakableBridge.jungleVine.colliderRoot.gameObject);
      if ((UnityEngine.Object) breakableBridge.breakParticles != (UnityEngine.Object) null)
        breakableBridge.breakParticles.Play();
      float normalizedTime = 0.0f;
      while ((double) normalizedTime < 1.0)
      {
        normalizedTime += Time.deltaTime * 0.7f;
        breakableBridge.rend.material.SetFloat(BreakableBridge.BreakAmount, normalizedTime);
        yield return (object) null;
      }
      Debug.Log((object) $"Destroy: {breakableBridge.gameObject}", (UnityEngine.Object) breakableBridge.gameObject);
      yield return (object) null;
    }
  }
}
