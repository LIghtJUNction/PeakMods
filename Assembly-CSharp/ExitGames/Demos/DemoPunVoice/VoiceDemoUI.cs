// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.VoiceDemoUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

public class VoiceDemoUI : MonoBehaviour
{
  [SerializeField]
  private Text punState;
  [SerializeField]
  private Text voiceState;
  private PunVoiceClient punVoiceClient;
  private Canvas canvas;
  [SerializeField]
  private Button punSwitch;
  private Text punSwitchText;
  [SerializeField]
  private Button voiceSwitch;
  private Text voiceSwitchText;
  [SerializeField]
  private Button calibrateButton;
  private Text calibrateText;
  [SerializeField]
  private Text voiceDebugText;
  private PhotonVoiceView recorder;
  [SerializeField]
  private GameObject inGameSettings;
  [SerializeField]
  private GameObject globalSettings;
  [SerializeField]
  private Text devicesInfoText;
  private GameObject debugGO;
  private bool debugMode;
  private float volumeBeforeMute;
  private DebugLevel previousDebugLevel;
  [SerializeField]
  private int calibrationMilliSeconds = 2000;

  public bool DebugMode
  {
    get => this.debugMode;
    set
    {
      this.debugMode = value;
      this.debugGO.SetActive(this.debugMode);
      this.voiceDebugText.text = string.Empty;
      if (this.debugMode)
      {
        this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
        this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = DebugLevel.ALL;
      }
      else
        this.punVoiceClient.Client.LoadBalancingPeer.DebugOut = this.previousDebugLevel;
      if (VoiceDemoUI.DebugToggled == null)
        return;
      VoiceDemoUI.DebugToggled(this.debugMode);
    }
  }

  public static event VoiceDemoUI.OnDebugToggle DebugToggled;

  private void Awake()
  {
    this.punVoiceClient = PunVoiceClient.Instance;
    Debug.LogWarning((object) "VoiceDemoUI selected a punVoiceClient.Instance", (UnityEngine.Object) this.punVoiceClient);
  }

  private void OnDestroy()
  {
    ChangePOV.CameraChanged -= new ChangePOV.OnCameraChanged(this.OnCameraChanged);
    BetterToggle.ToggleValueChanged -= new BetterToggle.OnToggle(this.BetterToggle_ToggleValueChanged);
    CharacterInstantiation.CharacterInstantiated -= new CharacterInstantiation.OnCharacterInstantiated(this.CharacterInstantiation_CharacterInstantiated);
    this.punVoiceClient.Client.StateChanged -= new Action<ClientState, ClientState>(this.VoiceClientStateChanged);
    PhotonNetwork.NetworkingClient.StateChanged -= new Action<ClientState, ClientState>(this.PunClientStateChanged);
  }

  private void CharacterInstantiation_CharacterInstantiated(GameObject character)
  {
    PhotonVoiceView component = character.GetComponent<PhotonVoiceView>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    this.recorder = component;
  }

  private void InitToggles(Toggle[] toggles)
  {
    if (toggles == null)
      return;
    for (int index = 0; index < toggles.Length; ++index)
    {
      Toggle toggle = toggles[index];
      switch (toggle.name)
      {
        case "Mute":
          toggle.isOn = (double) AudioListener.volume <= 1.0 / 1000.0;
          break;
        case "VoiceDetection":
          if ((UnityEngine.Object) this.recorder != (UnityEngine.Object) null && (UnityEngine.Object) this.recorder.RecorderInUse != (UnityEngine.Object) null)
          {
            toggle.isOn = this.recorder.RecorderInUse.VoiceDetection;
            break;
          }
          break;
        case "DebugVoice":
          toggle.isOn = this.DebugMode;
          break;
        case "Transmit":
          if ((UnityEngine.Object) this.recorder != (UnityEngine.Object) null && (UnityEngine.Object) this.recorder.RecorderInUse != (UnityEngine.Object) null)
          {
            toggle.isOn = this.recorder.RecorderInUse.TransmitEnabled;
            break;
          }
          break;
        case "DebugEcho":
          if ((UnityEngine.Object) this.recorder != (UnityEngine.Object) null && (UnityEngine.Object) this.recorder.RecorderInUse != (UnityEngine.Object) null)
          {
            toggle.isOn = this.recorder.RecorderInUse.DebugEchoMode;
            break;
          }
          break;
        case "AutoConnectAndJoin":
          toggle.isOn = this.punVoiceClient.AutoConnectAndJoin;
          break;
      }
    }
  }

  private void BetterToggle_ToggleValueChanged(Toggle toggle)
  {
    switch (toggle.name)
    {
      case "Mute":
        if (toggle.isOn)
        {
          this.volumeBeforeMute = AudioListener.volume;
          AudioListener.volume = 0.0f;
          break;
        }
        AudioListener.volume = this.volumeBeforeMute;
        this.volumeBeforeMute = 0.0f;
        break;
      case "Transmit":
        if (!(bool) (UnityEngine.Object) this.recorder.RecorderInUse)
          break;
        this.recorder.RecorderInUse.TransmitEnabled = toggle.isOn;
        break;
      case "VoiceDetection":
        if (!(bool) (UnityEngine.Object) this.recorder.RecorderInUse)
          break;
        this.recorder.RecorderInUse.VoiceDetection = toggle.isOn;
        break;
      case "DebugEcho":
        if (!(bool) (UnityEngine.Object) this.recorder.RecorderInUse)
          break;
        this.recorder.RecorderInUse.DebugEchoMode = toggle.isOn;
        break;
      case "DebugVoice":
        this.DebugMode = toggle.isOn;
        break;
      case "AutoConnectAndJoin":
        this.punVoiceClient.AutoConnectAndJoin = toggle.isOn;
        break;
    }
  }

  private void OnCameraChanged(Camera newCamera) => this.canvas.worldCamera = newCamera;

  private void Start()
  {
    ChangePOV.CameraChanged += new ChangePOV.OnCameraChanged(this.OnCameraChanged);
    BetterToggle.ToggleValueChanged += new BetterToggle.OnToggle(this.BetterToggle_ToggleValueChanged);
    CharacterInstantiation.CharacterInstantiated += new CharacterInstantiation.OnCharacterInstantiated(this.CharacterInstantiation_CharacterInstantiated);
    this.punVoiceClient.Client.StateChanged += new Action<ClientState, ClientState>(this.VoiceClientStateChanged);
    PhotonNetwork.NetworkingClient.StateChanged += new Action<ClientState, ClientState>(this.PunClientStateChanged);
    this.canvas = this.GetComponentInChildren<Canvas>();
    if ((UnityEngine.Object) this.punSwitch != (UnityEngine.Object) null)
    {
      this.punSwitchText = this.punSwitch.GetComponentInChildren<Text>();
      this.punSwitch.onClick.AddListener(new UnityAction(this.PunSwitchOnClick));
    }
    if ((UnityEngine.Object) this.voiceSwitch != (UnityEngine.Object) null)
    {
      this.voiceSwitchText = this.voiceSwitch.GetComponentInChildren<Text>();
      this.voiceSwitch.onClick.AddListener(new UnityAction(this.VoiceSwitchOnClick));
    }
    if ((UnityEngine.Object) this.calibrateButton != (UnityEngine.Object) null)
    {
      this.calibrateButton.onClick.AddListener(new UnityAction(this.CalibrateButtonOnClick));
      this.calibrateText = this.calibrateButton.GetComponentInChildren<Text>();
    }
    if ((UnityEngine.Object) this.punState != (UnityEngine.Object) null)
      this.debugGO = this.punState.transform.parent.gameObject;
    this.volumeBeforeMute = AudioListener.volume;
    this.previousDebugLevel = this.punVoiceClient.Client.LoadBalancingPeer.DebugOut;
    if ((UnityEngine.Object) this.globalSettings != (UnityEngine.Object) null)
    {
      this.globalSettings.SetActive(true);
      this.InitToggles(this.globalSettings.GetComponentsInChildren<Toggle>());
    }
    if ((UnityEngine.Object) this.devicesInfoText != (UnityEngine.Object) null)
    {
      using (AudioInEnumerator source = new AudioInEnumerator(this.punVoiceClient.Logger))
      {
        using (IDeviceEnumerator audioInEnumerator = Platform.CreateAudioInEnumerator(this.punVoiceClient.Logger))
        {
          if (source.Count<DeviceInfo>() + audioInEnumerator.Count<DeviceInfo>() == 0)
          {
            this.devicesInfoText.enabled = true;
            this.devicesInfoText.color = Color.red;
            this.devicesInfoText.text = "No microphone device detected!";
          }
          else
          {
            this.devicesInfoText.text = "Mic Unity: " + string.Join(", ", source.Select<DeviceInfo, string>((Func<DeviceInfo, string>) (x => x.ToString())));
            Text devicesInfoText = this.devicesInfoText;
            devicesInfoText.text = $"{devicesInfoText.text}\nMic Photon: {string.Join(", ", audioInEnumerator.Select<DeviceInfo, string>((Func<DeviceInfo, string>) (x => x.ToString())))}";
          }
        }
      }
    }
    this.VoiceClientStateChanged(ClientState.PeerCreated, this.punVoiceClient.ClientState);
    this.PunClientStateChanged(ClientState.PeerCreated, PhotonNetwork.NetworkingClient.State);
  }

  private void PunSwitchOnClick()
  {
    switch (PhotonNetwork.NetworkClientState)
    {
      case ClientState.PeerCreated:
      case ClientState.Disconnected:
        PhotonNetwork.ConnectUsingSettings();
        break;
      case ClientState.Joined:
        PhotonNetwork.Disconnect();
        break;
    }
  }

  private void VoiceSwitchOnClick()
  {
    if (this.punVoiceClient.ClientState == ClientState.Joined)
    {
      this.punVoiceClient.Disconnect();
    }
    else
    {
      if (this.punVoiceClient.ClientState != ClientState.PeerCreated && this.punVoiceClient.ClientState != ClientState.Disconnected)
        return;
      this.punVoiceClient.ConnectAndJoinRoom();
    }
  }

  private void CalibrateButtonOnClick()
  {
    if (!(bool) (UnityEngine.Object) this.recorder.RecorderInUse || this.recorder.RecorderInUse.VoiceDetectorCalibrating)
      return;
    this.recorder.RecorderInUse.VoiceDetectorCalibrate(this.calibrationMilliSeconds);
  }

  private void Update()
  {
    if (!((UnityEngine.Object) this.recorder != (UnityEngine.Object) null) || !((UnityEngine.Object) this.recorder.RecorderInUse != (UnityEngine.Object) null) || this.recorder.RecorderInUse.LevelMeter == null)
      return;
    this.voiceDebugText.text = $"Amp: avg. {this.recorder.RecorderInUse.LevelMeter.CurrentAvgAmp:0.000000}, peak {this.recorder.RecorderInUse.LevelMeter.CurrentPeakAmp:0.000000}";
  }

  private void PunClientStateChanged(ClientState fromState, ClientState toState)
  {
    this.punState.text = $"PUN: {toState}";
    switch (toState)
    {
      case ClientState.PeerCreated:
      case ClientState.Disconnected:
        this.punSwitch.interactable = true;
        this.punSwitchText.text = "PUN Connect";
        break;
      case ClientState.Joined:
        this.punSwitch.interactable = true;
        this.punSwitchText.text = "PUN Disconnect";
        break;
      default:
        this.punSwitch.interactable = false;
        this.punSwitchText.text = "PUN busy";
        break;
    }
    this.UpdateUiBasedOnVoiceState(this.punVoiceClient.ClientState);
  }

  private void VoiceClientStateChanged(ClientState fromState, ClientState toState)
  {
    this.UpdateUiBasedOnVoiceState(toState);
  }

  private void UpdateUiBasedOnVoiceState(ClientState voiceClientState)
  {
    this.voiceState.text = $"PhotonVoice: {voiceClientState}";
    switch (voiceClientState)
    {
      case ClientState.PeerCreated:
      case ClientState.Disconnected:
        if (PhotonNetwork.InRoom)
        {
          this.voiceSwitch.interactable = true;
          this.voiceSwitchText.text = "Voice Connect";
          this.voiceDebugText.text = string.Empty;
        }
        else
        {
          this.voiceSwitch.interactable = false;
          this.voiceSwitchText.text = "Voice N/A";
          this.voiceDebugText.text = string.Empty;
        }
        this.calibrateButton.interactable = false;
        this.voiceSwitchText.text = "Voice Connect";
        this.calibrateText.text = "Unavailable";
        this.inGameSettings.SetActive(false);
        break;
      case ClientState.Joined:
        this.voiceSwitch.interactable = true;
        this.inGameSettings.SetActive(true);
        this.voiceSwitchText.text = "Voice Disconnect";
        this.InitToggles(this.inGameSettings.GetComponentsInChildren<Toggle>());
        if ((UnityEngine.Object) this.recorder != (UnityEngine.Object) null && (UnityEngine.Object) this.recorder.RecorderInUse != (UnityEngine.Object) null)
        {
          this.calibrateButton.interactable = !this.recorder.RecorderInUse.VoiceDetectorCalibrating;
          this.calibrateText.text = this.recorder.RecorderInUse.VoiceDetectorCalibrating ? "Calibrating" : $"Calibrate ({this.calibrationMilliSeconds / 1000}s)";
          break;
        }
        this.calibrateButton.interactable = false;
        this.calibrateText.text = "Unavailable";
        break;
      default:
        this.voiceSwitch.interactable = false;
        this.voiceSwitchText.text = "Voice busy";
        break;
    }
  }

  protected void OnApplicationQuit()
  {
    this.punVoiceClient.Client.StateChanged -= new Action<ClientState, ClientState>(this.VoiceClientStateChanged);
    PhotonNetwork.NetworkingClient.StateChanged -= new Action<ClientState, ClientState>(this.PunClientStateChanged);
  }

  public delegate void OnDebugToggle(bool debugMode);
}
