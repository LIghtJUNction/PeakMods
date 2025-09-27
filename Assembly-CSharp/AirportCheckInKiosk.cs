// Decompiled with JetBrains decompiler
// Type: AirportCheckInKiosk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class AirportCheckInKiosk : MonoBehaviourPun, IInteractibleConstant, IInteractible
{
  public float interactTime;
  private MaterialPropertyBlock mpb;
  private MeshRenderer[] _mr;

  public bool IsInteractible(Character interactor) => true;

  public void Awake() => this.mpb = new MaterialPropertyBlock();

  private void Start()
  {
    if (GameHandler.GetService<NextLevelService>().Data.IsSome)
      Debug.Log((object) $"seconds left until next map... {GameHandler.GetService<NextLevelService>().Data.Value.SecondsLeft}");
    GameHandler.GetService<RichPresenceService>().SetState(RichPresenceState.Status_Airport);
  }

  public void Interact(Character interactor)
  {
  }

  private MeshRenderer[] meshRenderers
  {
    get
    {
      if (this._mr == null)
      {
        this._mr = this.GetComponentsInChildren<MeshRenderer>();
        MonoBehaviour.print((object) this._mr.Length);
      }
      return this._mr;
    }
    set => this._mr = value;
  }

  public void HoverEnter()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
    {
      if ((UnityEngine.Object) this.meshRenderers[index] != (UnityEngine.Object) null)
        this.meshRenderers[index].SetPropertyBlock(this.mpb);
    }
  }

  public void HoverExit()
  {
    if (this.mpb == null)
      return;
    this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0.0f);
    for (int index = 0; index < this.meshRenderers.Length; ++index)
      this.meshRenderers[index].SetPropertyBlock(this.mpb);
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("BOARDFLIGHT");

  public string GetName() => LocalizedText.GetText("GATEKIOSK");

  public bool IsConstantlyInteractable(Character interactor) => this.IsInteractible(interactor);

  public float GetInteractTime(Character interactor) => this.interactTime;

  public void Interact_CastFinished(Character interactor)
  {
    GUIManager.instance.boardingPass.Open();
    GUIManager.instance.boardingPass.kiosk = this;
  }

  public void StartGame(int ascent)
  {
    this.photonView.RPC("LoadIslandMaster", RpcTarget.MasterClient, (object) ascent);
  }

  public void CancelCast(Character interactor)
  {
  }

  public void ReleaseInteract(Character interactor)
  {
  }

  [PunRPC]
  public void LoadIslandMaster(int ascent)
  {
    MenuWindow.CloseAllWindows();
    if (!PhotonNetwork.IsMasterClient)
      return;
    Debug.Log((object) "Loading scene as master.");
    NextLevelService service = GameHandler.GetService<NextLevelService>();
    string str = "WilIsland";
    if (service.Data.IsSome)
      str = SingletonAsset<MapBaker>.Instance.GetLevel(service.Data.Value.CurrentLevelIndex);
    else if (PhotonNetwork.OfflineMode)
      str = SingletonAsset<MapBaker>.Instance.GetLevel(0);
    if (string.IsNullOrEmpty(str))
      str = "WilIsland";
    this.photonView.RPC("BeginIslandLoadRPC", RpcTarget.All, (object) str, (object) ascent);
  }

  [PunRPC]
  public void BeginIslandLoadRPC(string sceneName, int ascent)
  {
    GameHandler.AddStatus<SceneSwitchingStatus>((GameStatus) new SceneSwitchingStatus());
    Debug.Log((object) ("Begin scene load RPC: " + sceneName));
    Ascents.currentAscent = ascent;
    RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, (Action) null, RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(sceneName, true, true, 0.0f));
  }

  public bool holdOnFinish { get; }
}
