// Decompiled with JetBrains decompiler
// Type: MainCameraMovement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
[DefaultExecutionOrder(500)]
public class MainCameraMovement : Singleton<MainCameraMovement>
{
  private float currentFov;
  private float currentForwardOffset = 0.5f;
  private MainCamera cam;
  private FovSetting fovSetting;
  private ExtraFovSetting extraFovSetting;
  public float characterPovLerpRate = 5f;
  public float characterPovMaxDistance = 0.1f;
  private bool isSpectating;
  internal bool isGodCam;
  public GodCam godcam;
  private float spectateZoom = 2f;
  public float spectateZoomMin = 1f;
  public float spectateZoomMax = 5f;
  public float spectateZoomButtonSpeed = 30f;
  private float sinceSwitch;
  private float ragdollCam;
  private Quaternion physicsRot;
  private Vector3 targetPlayerPovPosition;

  public static Character specCharacter { get; protected set; }

  private void Start()
  {
    this.cam = this.GetComponent<MainCamera>();
    this.currentFov = this.cam.cam.fieldOfView;
    this.fovSetting = GameHandler.Instance.SettingsHandler.GetSetting<FovSetting>();
    this.extraFovSetting = GameHandler.Instance.SettingsHandler.GetSetting<ExtraFovSetting>();
  }

  public static bool IsSpectating => Singleton<MainCameraMovement>.Instance.isSpectating;

  private void LateUpdate()
  {
    if (this.isGodCam)
    {
      this.godcam.Update(this.transform, this.cam);
    }
    else
    {
      this.UpdateVariables();
      if ((bool) (UnityEngine.Object) this.cam.camOverride)
        this.OverrideCam();
      else if ((bool) (UnityEngine.Object) Character.localCharacter && Character.localCharacter.data.fullyPassedOut)
      {
        this.Spectate();
        if (this.isSpectating)
          return;
        this.StartSpectate();
      }
      else
      {
        if (this.isSpectating)
          this.StopSpectating();
        MainCameraMovement.specCharacter = (Character) null;
        this.CharacterCam();
      }
    }
  }

  private void StartSpectate() => this.isSpectating = true;

  private void StopSpectating()
  {
    this.isSpectating = false;
    MainCameraMovement.specCharacter = (Character) null;
    if (!((UnityEngine.Object) Character.localCharacter.Ghost != (UnityEngine.Object) null))
      return;
    PhotonNetwork.Destroy(Character.localCharacter.Ghost.gameObject);
  }

  private void UpdateVariables() => this.sinceSwitch += Time.deltaTime;

  private void Spectate()
  {
    Character specCharacter = MainCameraMovement.specCharacter;
    if (!this.HandleSpecSelection())
    {
      this.NoOneToSpectate();
    }
    else
    {
      PlayerGhost playerGhost = Character.localCharacter.Ghost;
      if ((UnityEngine.Object) playerGhost == (UnityEngine.Object) null && Character.localCharacter.data.dead)
      {
        playerGhost = PhotonNetwork.Instantiate("PlayerGhost", Vector3.zero, Quaternion.identity).GetComponent<PlayerGhost>();
        playerGhost.m_view.RPC("RPCA_InitGhost", RpcTarget.AllBuffered, (object) Character.localCharacter.refs.view, (object) MainCameraMovement.specCharacter.refs.view);
      }
      if ((bool) (UnityEngine.Object) playerGhost && (UnityEngine.Object) playerGhost.m_target != (UnityEngine.Object) MainCameraMovement.specCharacter)
        playerGhost.m_view.RPC("RPCA_SetTarget", RpcTarget.AllBuffered, (object) MainCameraMovement.specCharacter.refs.view);
      this.transform.position = MainCameraMovement.specCharacter.Center;
      Vector3 lookDirection = MainCameraMovement.specCharacter.data.lookDirection;
      if ((UnityEngine.Object) Character.localCharacter != (UnityEngine.Object) null)
        lookDirection = Character.localCharacter.data.lookDirection;
      this.transform.rotation = Quaternion.LookRotation(lookDirection);
      this.spectateZoom += Character.localCharacter.input.scrollInput * -0.5f;
      if (Character.localCharacter.input.scrollForwardIsPressed)
        this.spectateZoom -= this.spectateZoomButtonSpeed * Time.deltaTime;
      else if (Character.localCharacter.input.scrollBackwardIsPressed)
        this.spectateZoom += this.spectateZoomButtonSpeed * Time.deltaTime;
      this.spectateZoom = Mathf.Clamp(this.spectateZoom, this.spectateZoomMin, this.spectateZoomMax);
      Character.localCharacter.data.spectateZoom = Mathf.Lerp(Character.localCharacter.data.spectateZoom, this.spectateZoom, Time.deltaTime * 5f);
      this.transform.position += this.transform.TransformDirection(new Vector3(0.0f, 0.5f, -1f * Character.localCharacter.data.spectateZoom));
    }
  }

  private void NoOneToSpectate()
  {
  }

  private bool HandleSpecSelection()
  {
    if ((bool) (UnityEngine.Object) MainCameraMovement.specCharacter && MainCameraMovement.specCharacter.data.dead)
      MainCameraMovement.specCharacter = (Character) null;
    if ((UnityEngine.Object) MainCameraMovement.specCharacter == (UnityEngine.Object) null)
      this.GetSpecPlayer();
    if ((UnityEngine.Object) MainCameraMovement.specCharacter == (UnityEngine.Object) null)
      return false;
    if (Character.localCharacter.input.spectateLeftWasPressed && (double) this.sinceSwitch > 0.20000000298023224)
    {
      Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerLeft), 5f, 5f);
      this.sinceSwitch = 0.0f;
    }
    if (Character.localCharacter.input.spectateRightWasPressed && (double) this.sinceSwitch > 0.20000000298023224)
    {
      Transitions.instance.PlayTransition(TransitionType.SpectateSwitch, new Action(this.SwapSpecPlayerRight), 5f, 5f);
      this.sinceSwitch = 0.0f;
    }
    return !((UnityEngine.Object) MainCameraMovement.specCharacter == (UnityEngine.Object) null);
  }

  public void SwapSpecPlayerLeft() => this.SwapSpecPlayer(-1);

  public void SwapSpecPlayerRight() => this.SwapSpecPlayer(1);

  private void SwapSpecPlayer(int add)
  {
    List<Character> playerList = new List<Character>();
    foreach (Character allPlayerCharacter in PlayerHandler.GetAllPlayerCharacters())
    {
      if (!allPlayerCharacter.data.dead && !allPlayerCharacter.isBot)
        playerList.Add(allPlayerCharacter);
    }
    if (playerList.Count == 0)
      MainCameraMovement.specCharacter = (Character) null;
    else if ((UnityEngine.Object) MainCameraMovement.specCharacter == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "WE FOUND IT");
    }
    else
    {
      int index = MainCameraMovement.specCharacter.GetPlayerListID(playerList) + add;
      if (index < 0)
        index = playerList.Count - 1;
      if (index >= playerList.Count)
        index = 0;
      MainCameraMovement.specCharacter = playerList[index];
    }
  }

  private void GetSpecPlayer()
  {
    List<Character> playerCharacters = PlayerHandler.GetAllPlayerCharacters();
    if (playerCharacters.Count == 0)
      return;
    if (!Character.localCharacter.data.dead)
    {
      MainCameraMovement.specCharacter = Character.localCharacter;
    }
    else
    {
      for (int index = 0; index < playerCharacters.Count; ++index)
      {
        if (!playerCharacters[index].data.dead && !playerCharacters[index].isBot)
        {
          MainCameraMovement.specCharacter = playerCharacters[index];
          break;
        }
      }
    }
  }

  private void CharacterCam()
  {
    if ((UnityEngine.Object) Character.localCharacter == (UnityEngine.Object) null)
      return;
    this.cam.cam.fieldOfView = this.GetFov();
    if ((UnityEngine.Object) Character.localCharacter == (UnityEngine.Object) null || (UnityEngine.Object) Character.localCharacter == (UnityEngine.Object) null)
      return;
    if (Character.localCharacter.data.lookDirection != Vector3.zero)
    {
      this.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
      float b = 1f - Character.localCharacter.data.currentRagdollControll;
      this.ragdollCam = (double) b <= (double) this.ragdollCam ? Mathf.Lerp(this.ragdollCam, b, Time.deltaTime * 0.5f) : Mathf.Lerp(this.ragdollCam, b, Time.deltaTime * 5f);
      this.physicsRot = Quaternion.Lerp(this.physicsRot, Character.localCharacter.GetBodypartRig(BodypartType.Head).transform.rotation, Time.deltaTime * 10f);
      this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.physicsRot, this.ragdollCam);
      this.transform.Rotate(GamefeelHandler.instance.GetRotation(), Space.World);
    }
    this.targetPlayerPovPosition = Vector3.Lerp(Character.localCharacter.GetCameraPos(this.GetHeadOffset()), Character.localCharacter.GetBodypart(BodypartType.Torso).transform.position, this.ragdollCam);
    if ((double) Vector3.Distance(this.transform.position, this.targetPlayerPovPosition) > (double) this.characterPovMaxDistance)
      this.transform.position = this.targetPlayerPovPosition + (this.transform.position - this.targetPlayerPovPosition).normalized * this.characterPovMaxDistance;
    this.transform.position = Vector3.Lerp(this.transform.position, this.targetPlayerPovPosition, Time.deltaTime * this.characterPovLerpRate);
  }

  private void OverrideCam()
  {
    this.cam.cam.fieldOfView = this.cam.camOverride.fov;
    this.cam.transform.position = this.cam.camOverride.transform.position;
    this.cam.transform.rotation = this.cam.camOverride.transform.rotation;
  }

  private float GetHeadOffset()
  {
    this.currentForwardOffset = !Character.localCharacter.data.isClimbing ? Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f) : Mathf.Lerp(this.currentForwardOffset, -0.5f, Time.deltaTime * 5f);
    return this.currentForwardOffset;
  }

  private float GetFov()
  {
    float fov = this.fovSetting.Value;
    if ((double) fov < 60.0)
      fov = 70f;
    if ((UnityEngine.Object) Character.localCharacter == (UnityEngine.Object) null)
      return fov;
    this.currentFov = Mathf.Lerp(this.currentFov, fov + (Character.localCharacter.data.isClimbing ? this.extraFovSetting.Value : 0.0f), Time.deltaTime * 5f);
    return this.currentFov;
  }
}
