// Decompiled with JetBrains decompiler
// Type: Luggage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Luggage : Spawner, IInteractibleConstant, IInteractible
{
  public string displayName;
  private Animator anim;
  [SerializeField]
  protected Luggage.LuggageState state;
  private PhotonView photonView;
  public float timeToOpen;
  private MaterialPropertyBlock mpb;
  public static List<Luggage> ALL_LUGGAGE = new List<Luggage>();
  private MeshRenderer[] _mr;

  private MeshRenderer[] meshRenderers
  {
    get
    {
      if (this._mr == null)
        this._mr = this.GetComponentsInChildren<MeshRenderer>();
      return this._mr;
    }
    set => this._mr = value;
  }

  private void Awake()
  {
    this.photonView = this.GetComponent<PhotonView>();
    this.anim = this.GetComponent<Animator>();
    this.mpb = new MaterialPropertyBlock();
    Luggage.ALL_LUGGAGE.Add(this);
  }

  public virtual void Interact(Character interactor) => this.anim.Play("Luggage_Unclasp");

  [PunRPC]
  protected void OpenLuggageRPC(bool spawnItems)
  {
    if (this.state != Luggage.LuggageState.Closed)
      return;
    this.anim.Play("Luggage_Open");
    Luggage.ALL_LUGGAGE.Remove(this);
    this.state = Luggage.LuggageState.Open;
    if (!spawnItems)
      return;
    this.StartCoroutine(SpawnItemRoutine());

    IEnumerator SpawnItemRoutine()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.\u003C\u003E1__state;
      Luggage luggage = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E1__state = -1;
        luggage.SpawnItems(luggage.GetSpawnSpots());
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = (object) new WaitForSeconds(0.1f);
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = 1;
      return true;
    }
  }

  private void OnDestroy()
  {
    if (!Luggage.ALL_LUGGAGE.Contains(this))
      return;
    Luggage.ALL_LUGGAGE.Remove(this);
  }

  public Vector3 Center()
  {
    return HelperFunctions.GetTotalBounds((IEnumerable<Renderer>) this.meshRenderers).center;
  }

  public Transform GetTransform() => this.transform;

  public virtual string GetInteractionText() => LocalizedText.GetText("OPEN");

  public string GetName() => LocalizedText.GetText(this.displayName);

  public bool IsInteractible(Character interactor) => this.state == Luggage.LuggageState.Closed;

  public void HoverEnter()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
    {
      if ((Object) this.meshRenderers[index] != (Object) null)
        this.meshRenderers[index].SetPropertyBlock(this.mpb);
    }
  }

  public void HoverExit()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
    {
      if ((Object) this.meshRenderers[index] != (Object) null)
        this.meshRenderers[index].SetPropertyBlock(this.mpb);
    }
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  public bool IsConstantlyInteractable(Character interactor)
  {
    return this.state == Luggage.LuggageState.Closed;
  }

  public float GetInteractTime(Character interactor) => this.timeToOpen;

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
  {
    base.OnPlayerEnteredRoom(newPlayer);
    if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber || !PhotonNetwork.IsMasterClient || this.state != Luggage.LuggageState.Open)
      return;
    this.photonView.RPC("OpenLuggageRPC", RpcTarget.All, (object) false);
  }

  public virtual void Interact_CastFinished(Character interactor)
  {
    if (this.state != Luggage.LuggageState.Closed)
      return;
    this.photonView.RPC("OpenLuggageRPC", RpcTarget.All, (object) true);
    GlobalEvents.TriggerLuggageOpened(this, interactor);
  }

  public void CancelCast(Character interactor) => this.anim.SetTrigger("Reclasp");

  public bool holdOnFinish => false;

  public enum LuggageState
  {
    Closed,
    Open,
  }
}
