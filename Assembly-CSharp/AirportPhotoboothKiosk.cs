// Decompiled with JetBrains decompiler
// Type: AirportPhotoboothKiosk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class AirportPhotoboothKiosk : MonoBehaviour, IInteractible
{
  public PhotonView view;
  public Camera displayCamera;
  public Camera actualCamera;
  public Animator anim;
  private MaterialPropertyBlock mpb;
  public GameObject photoCanvas;
  public GameObject screen;
  public RenderTexture[] photoTextures;
  public Image flashImage;
  public Image photosensitiveFlashImage;
  public Transform insidePlaneTf;
  private bool inPhotobooth;
  private MeshRenderer[] _mr;
  private bool takingPhoto;

  public bool IsInteractible(Character interactor) => true;

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

  public void Awake() => this.mpb = new MaterialPropertyBlock();

  private void Start()
  {
    this.flashImage.enabled = !GUIManager.instance.photosensitivity;
    this.photosensitiveFlashImage.enabled = GUIManager.instance.photosensitivity;
  }

  private void Update()
  {
    this.inPhotobooth = (Object) Character.localCharacter != (Object) null && (double) Character.localCharacter.Center.x < (double) this.insidePlaneTf.position.x;
    this.displayCamera.enabled = this.inPhotobooth;
    this.screen.SetActive(this.inPhotobooth);
  }

  public void Interact(Character interactor)
  {
    if (this.takingPhoto)
      return;
    this.view.RPC("InteractRPC", RpcTarget.All);
  }

  [PunRPC]
  private void InteractRPC()
  {
    this.takingPhoto = true;
    this.StartCoroutine(this.PhotoboothRoutine());
  }

  private IEnumerator PhotoboothRoutine()
  {
    this.anim.SetTrigger("Start");
    yield return (object) new WaitForSeconds(3f);
    this.actualCamera.targetTexture = this.photoTextures[0];
    this.actualCamera.Render();
    yield return (object) new WaitForSeconds(1f);
    this.anim.SetTrigger("Start");
    yield return (object) new WaitForSeconds(3f);
    this.actualCamera.targetTexture = this.photoTextures[1];
    this.actualCamera.Render();
    yield return (object) new WaitForSeconds(1f);
    this.anim.SetTrigger("Start");
    yield return (object) new WaitForSeconds(3f);
    this.actualCamera.targetTexture = this.photoTextures[2];
    this.actualCamera.Render();
    yield return (object) new WaitForSeconds(1f);
    this.anim.SetTrigger("Start");
    yield return (object) new WaitForSeconds(3f);
    this.actualCamera.targetTexture = this.photoTextures[3];
    this.actualCamera.Render();
    yield return (object) new WaitForSeconds(1f);
    if (this.inPhotobooth)
    {
      this.photoCanvas.SetActive(true);
      yield return (object) new WaitForSeconds(3f);
      SteamScreenshots.TriggerScreenshot();
      this.takingPhoto = false;
      yield return (object) new WaitForSeconds(2f);
      this.photoCanvas.SetActive(false);
    }
    else
    {
      yield return (object) new WaitForSeconds(5f);
      this.takingPhoto = false;
    }
  }

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
      this.meshRenderers[index].SetPropertyBlock(this.mpb);
  }

  public Vector3 Center() => this.transform.position;

  public Transform GetTransform() => this.transform;

  public string GetInteractionText() => LocalizedText.GetText("START2");

  public string GetName() => LocalizedText.GetText("PHOTOBOOTH");
}
