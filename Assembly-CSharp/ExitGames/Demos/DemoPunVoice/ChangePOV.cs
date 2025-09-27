// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.ChangePOV
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

public class ChangePOV : MonoBehaviour, IMatchmakingCallbacks
{
  private FirstPersonController firstPersonController;
  private ThirdPersonController thirdPersonController;
  private OrthographicController orthographicController;
  private Vector3 initialCameraPosition;
  private Quaternion initialCameraRotation;
  private Camera defaultCamera;
  [SerializeField]
  private GameObject ButtonsHolder;
  [SerializeField]
  private Button FirstPersonCamActivator;
  [SerializeField]
  private Button ThirdPersonCamActivator;
  [SerializeField]
  private Button OrthographicCamActivator;

  public static event ChangePOV.OnCameraChanged CameraChanged;

  private void OnEnable()
  {
    CharacterInstantiation.CharacterInstantiated += new CharacterInstantiation.OnCharacterInstantiated(this.OnCharacterInstantiated);
    PhotonNetwork.AddCallbackTarget((object) this);
  }

  private void OnDisable()
  {
    CharacterInstantiation.CharacterInstantiated -= new CharacterInstantiation.OnCharacterInstantiated(this.OnCharacterInstantiated);
    PhotonNetwork.RemoveCallbackTarget((object) this);
  }

  private void Start()
  {
    this.defaultCamera = Camera.main;
    this.initialCameraPosition = new Vector3(this.defaultCamera.transform.position.x, this.defaultCamera.transform.position.y, this.defaultCamera.transform.position.z);
    this.initialCameraRotation = new Quaternion(this.defaultCamera.transform.rotation.x, this.defaultCamera.transform.rotation.y, this.defaultCamera.transform.rotation.z, this.defaultCamera.transform.rotation.w);
    this.FirstPersonCamActivator.onClick.AddListener(new UnityAction(this.FirstPersonMode));
    this.ThirdPersonCamActivator.onClick.AddListener(new UnityAction(this.ThirdPersonMode));
    this.OrthographicCamActivator.onClick.AddListener(new UnityAction(this.OrthographicMode));
  }

  private void OnCharacterInstantiated(GameObject character)
  {
    this.firstPersonController = character.GetComponent<FirstPersonController>();
    this.firstPersonController.enabled = false;
    this.thirdPersonController = character.GetComponent<ThirdPersonController>();
    this.thirdPersonController.enabled = false;
    this.orthographicController = character.GetComponent<OrthographicController>();
    this.ButtonsHolder.SetActive(true);
  }

  private void FirstPersonMode() => this.ToggleMode((BaseController) this.firstPersonController);

  private void ThirdPersonMode() => this.ToggleMode((BaseController) this.thirdPersonController);

  private void OrthographicMode() => this.ToggleMode((BaseController) this.orthographicController);

  private void ToggleMode(BaseController controller)
  {
    if ((Object) controller == (Object) null || (Object) controller.ControllerCamera == (Object) null)
      return;
    controller.ControllerCamera.gameObject.SetActive(true);
    controller.enabled = true;
    this.FirstPersonCamActivator.interactable = !((Object) controller == (Object) this.firstPersonController);
    this.ThirdPersonCamActivator.interactable = !((Object) controller == (Object) this.thirdPersonController);
    this.OrthographicCamActivator.interactable = !((Object) controller == (Object) this.orthographicController);
    this.BroadcastChange(controller.ControllerCamera);
  }

  private void BroadcastChange(Camera camera)
  {
    if ((Object) camera == (Object) null || ChangePOV.CameraChanged == null)
      return;
    ChangePOV.CameraChanged(camera);
  }

  public void OnFriendListUpdate(List<FriendInfo> friendList)
  {
  }

  public void OnCreatedRoom()
  {
  }

  public void OnCreateRoomFailed(short returnCode, string message)
  {
  }

  public void OnJoinedRoom()
  {
  }

  public void OnJoinRoomFailed(short returnCode, string message)
  {
  }

  public void OnJoinRandomFailed(short returnCode, string message)
  {
  }

  public void OnLeftRoom()
  {
    if ((bool) (Object) this.defaultCamera)
      this.defaultCamera.gameObject.SetActive(true);
    this.FirstPersonCamActivator.interactable = true;
    this.ThirdPersonCamActivator.interactable = true;
    this.OrthographicCamActivator.interactable = false;
    this.defaultCamera.transform.position = this.initialCameraPosition;
    this.defaultCamera.transform.rotation = this.initialCameraRotation;
    this.ButtonsHolder.SetActive(false);
  }

  public delegate void OnCameraChanged(Camera newCamera);
}
