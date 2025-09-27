// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.DemoVoiceUI.DemoVoiceUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Voice.Unity.UtilityScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos.DemoVoiceUI;

[RequireComponent(typeof (UnityVoiceClient), typeof (ConnectAndJoin))]
public class DemoVoiceUI : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
{
  [SerializeField]
  private Text connectionStatusText;
  [SerializeField]
  private Text serverStatusText;
  [SerializeField]
  private Text roomStatusText;
  [SerializeField]
  private Text inputWarningText;
  [SerializeField]
  private Text rttText;
  [SerializeField]
  private Text rttVariationText;
  [SerializeField]
  private Text packetLossWarningText;
  [SerializeField]
  private InputField localNicknameText;
  [SerializeField]
  private Toggle debugEchoToggle;
  [SerializeField]
  private Toggle reliableTransmissionToggle;
  [SerializeField]
  private Toggle encryptionToggle;
  [SerializeField]
  private GameObject webRtcDspGameObject;
  [SerializeField]
  private Toggle aecToggle;
  [SerializeField]
  private Toggle aecHighPassToggle;
  [SerializeField]
  private InputField reverseStreamDelayInputField;
  [SerializeField]
  private Toggle noiseSuppressionToggle;
  [SerializeField]
  private Toggle agcToggle;
  [SerializeField]
  private Slider agcCompressionGainSlider;
  [SerializeField]
  private Slider agcTargetLevelSlider;
  [SerializeField]
  private Toggle vadToggle;
  [SerializeField]
  private Toggle muteToggle;
  [SerializeField]
  private Toggle streamAudioClipToggle;
  [SerializeField]
  private Toggle audioToneToggle;
  [SerializeField]
  private Toggle dspToggle;
  [SerializeField]
  private Toggle highPassToggle;
  [SerializeField]
  private Toggle photonVadToggle;
  [SerializeField]
  private MicrophoneSelector microphoneSelector;
  [SerializeField]
  private GameObject androidMicSettingGameObject;
  [SerializeField]
  private Toggle androidAgcToggle;
  [SerializeField]
  private Toggle androidAecToggle;
  [SerializeField]
  private Toggle androidNsToggle;
  [SerializeField]
  private bool defaultTransmitEnabled;
  [SerializeField]
  private bool fullScreen;
  [SerializeField]
  private InputField roomNameInputField;
  [SerializeField]
  private int rttYellowThreshold = 100;
  [SerializeField]
  private int rttRedThreshold = 160 /*0xA0*/;
  [SerializeField]
  private int rttVariationYellowThreshold = 25;
  [SerializeField]
  private int rttVariationRedThreshold = 50;
  private GameObject compressionGainGameObject;
  private GameObject targetLevelGameObject;
  private Text compressionGainText;
  private Text targetLevelText;
  private GameObject aecOptionsGameObject;
  public Transform RemoteVoicesPanel;
  protected UnityVoiceClient voiceConnection;
  private WebRtcAudioDsp voiceAudioPreprocessor;
  private ConnectAndJoin connectAndJoin;
  private readonly Color warningColor = new Color(0.9f, 0.5f, 0.0f, 1f);
  private readonly Color okColor = new Color(0.0f, 0.6f, 0.2f, 1f);
  private readonly Color redColor = new Color(1f, 0.0f, 0.0f, 1f);
  private readonly Color defaultColor = new Color(0.0f, 0.0f, 0.0f, 1f);
  private Func<IAudioDesc> toneInputFactory = (Func<IAudioDesc>) (() => (IAudioDesc) new AudioUtil.ToneAudioReader<float>(channels: 2));

  private void Start()
  {
    this.connectAndJoin = this.GetComponent<ConnectAndJoin>();
    this.voiceConnection = this.GetComponent<UnityVoiceClient>();
    this.voiceAudioPreprocessor = this.voiceConnection.PrimaryRecorder.GetComponent<WebRtcAudioDsp>();
    this.compressionGainGameObject = this.agcCompressionGainSlider.transform.parent.gameObject;
    this.compressionGainText = this.compressionGainGameObject.GetComponentInChildren<Text>();
    this.targetLevelGameObject = this.agcTargetLevelSlider.transform.parent.gameObject;
    this.targetLevelText = this.targetLevelGameObject.GetComponentInChildren<Text>();
    this.aecOptionsGameObject = this.aecHighPassToggle.transform.parent.gameObject;
    this.SetDefaults();
    this.InitUiCallbacks();
    this.GetSavedNickname();
    this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
    this.voiceConnection.SpeakerLinked += new Action<Speaker>(this.OnSpeakerCreated);
    this.voiceConnection.Client.AddCallbackTarget((object) this);
  }

  protected virtual void SetDefaults() => this.muteToggle.isOn = !this.defaultTransmitEnabled;

  private void OnDestroy()
  {
    this.voiceConnection.SpeakerLinked -= new Action<Speaker>(this.OnSpeakerCreated);
    this.voiceConnection.Client.RemoveCallbackTarget((object) this);
  }

  private void GetSavedNickname()
  {
    string str = PlayerPrefs.GetString("vNick");
    if (string.IsNullOrEmpty(str))
      return;
    this.localNicknameText.text = str;
    this.voiceConnection.Client.NickName = str;
  }

  protected virtual void OnSpeakerCreated(Speaker speaker)
  {
    speaker.gameObject.transform.SetParent(this.RemoteVoicesPanel, false);
    speaker.GetComponent<RemoteSpeakerUI>().Init((VoiceConnection) this.voiceConnection);
    speaker.OnRemoteVoiceRemoveAction += new Action<Speaker>(this.OnRemoteVoiceRemove);
  }

  private void OnRemoteVoiceRemove(Speaker speaker)
  {
    if (!((UnityEngine.Object) speaker != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) speaker.gameObject);
  }

  private void ToggleMute(bool isOn)
  {
    this.muteToggle.targetGraphic.enabled = !isOn;
    if (isOn)
      this.voiceConnection.Client.LocalPlayer.Mute();
    else
      this.voiceConnection.Client.LocalPlayer.Unmute();
  }

  protected virtual void ToggleIsRecording(bool isRecording)
  {
    this.voiceConnection.PrimaryRecorder.RecordingEnabled = isRecording;
  }

  private void ToggleDebugEcho(bool isOn)
  {
    this.voiceConnection.PrimaryRecorder.DebugEchoMode = isOn;
  }

  private void ToggleReliable(bool isOn)
  {
    this.voiceConnection.PrimaryRecorder.ReliableMode = isOn;
  }

  private void ToggleEncryption(bool isOn) => this.voiceConnection.PrimaryRecorder.Encrypt = isOn;

  private void ToggleAEC(bool isOn)
  {
    this.voiceAudioPreprocessor.AEC = isOn;
    this.aecOptionsGameObject.SetActive(isOn);
    this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
  }

  private void ToggleNoiseSuppression(bool isOn)
  {
    this.voiceAudioPreprocessor.NoiseSuppression = isOn;
  }

  private void ToggleAGC(bool isOn)
  {
    this.voiceAudioPreprocessor.AGC = isOn;
    this.compressionGainGameObject.SetActive(isOn);
    this.targetLevelGameObject.SetActive(isOn);
    this.voiceConnection.Client.LocalPlayer.SetAGC(isOn, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
  }

  private void ToggleVAD(bool isOn)
  {
    this.voiceAudioPreprocessor.VAD = isOn;
    this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(isOn);
  }

  private void ToggleHighPass(bool isOn) => this.voiceAudioPreprocessor.HighPass = isOn;

  private void ToggleDsp(bool isOn)
  {
    this.voiceAudioPreprocessor.enabled = isOn;
    this.voiceConnection.PrimaryRecorder.RestartRecording();
    this.webRtcDspGameObject.SetActive(isOn);
    this.voiceConnection.Client.LocalPlayer.SetWebRTCVAD(this.voiceAudioPreprocessor.VAD);
    this.voiceConnection.Client.LocalPlayer.SetAEC(this.voiceAudioPreprocessor.AEC);
    this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
  }

  private void ToggleAudioClipStreaming(bool isOn)
  {
    if (isOn)
    {
      this.audioToneToggle.SetValue(false);
      this.voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.AudioClip;
    }
    else
    {
      if (this.audioToneToggle.isOn)
        return;
      this.microphoneSelector.SwitchToSelectedMic();
    }
  }

  private void ToggleAudioToneFactory(bool isOn)
  {
    if (isOn)
    {
      this.streamAudioClipToggle.SetValue(false);
      this.voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.Factory;
      this.voiceConnection.PrimaryRecorder.InputFactory = this.toneInputFactory;
    }
    else
    {
      if (this.streamAudioClipToggle.isOn)
        return;
      this.microphoneSelector.SwitchToSelectedMic();
    }
  }

  private void TogglePhotonVAD(bool isOn)
  {
    this.voiceConnection.PrimaryRecorder.VoiceDetection = isOn;
    this.voiceConnection.Client.LocalPlayer.SetPhotonVAD(isOn);
  }

  private void ToggleAecHighPass(bool isOn)
  {
    this.voiceAudioPreprocessor.AecHighPass = isOn;
    this.voiceConnection.Client.LocalPlayer.SetAEC(isOn);
  }

  private void OnAgcCompressionGainChanged(float agcCompressionGain)
  {
    this.voiceAudioPreprocessor.AgcCompressionGain = (int) agcCompressionGain;
    this.compressionGainText.text = "Compression Gain: " + (object) agcCompressionGain;
    this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, (int) agcCompressionGain, this.voiceAudioPreprocessor.AgcTargetLevel);
  }

  private void OnAgcTargetLevelChanged(float agcTargetLevel)
  {
    this.voiceAudioPreprocessor.AgcTargetLevel = (int) agcTargetLevel;
    this.targetLevelText.text = "Target Level: " + (object) agcTargetLevel;
    this.voiceConnection.Client.LocalPlayer.SetAGC(this.voiceAudioPreprocessor.AGC, this.voiceAudioPreprocessor.AgcCompressionGain, (int) agcTargetLevel);
  }

  private void OnReverseStreamDelayChanged(string newReverseStreamString)
  {
    int result;
    if (int.TryParse(newReverseStreamString, out result) && result > 0)
      this.voiceAudioPreprocessor.ReverseStreamDelayMs = result;
    else
      this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
  }

  private void OnMicrophoneChanged(Recorder.MicType micType, DeviceInfo deviceInfo)
  {
    this.voiceConnection.Client.LocalPlayer.SetMic(micType);
    this.androidMicSettingGameObject.SetActive(micType == Recorder.MicType.Photon);
  }

  private void OnAndroidMicSettingsChanged(bool isOn)
  {
    this.voiceConnection.PrimaryRecorder.SetAndroidNativeMicrophoneSettings(this.androidAecToggle.isOn, this.androidAgcToggle.isOn, this.androidNsToggle.isOn);
  }

  private void UpdateSyncedNickname(string nickname)
  {
    nickname = nickname.Trim();
    this.voiceConnection.Client.LocalPlayer.NickName = nickname;
    PlayerPrefs.SetString("vNick", nickname);
  }

  private void JoinOrCreateRoom(string roomName)
  {
    if (string.IsNullOrEmpty(roomName))
    {
      this.connectAndJoin.RoomName = string.Empty;
      this.connectAndJoin.RandomRoom = true;
    }
    else
    {
      this.connectAndJoin.RoomName = roomName.Trim();
      this.connectAndJoin.RandomRoom = false;
    }
    if (this.voiceConnection.Client.InRoom)
    {
      this.voiceConnection.Client.OpLeaveRoom(false);
    }
    else
    {
      if (this.voiceConnection.Client.IsConnected)
        return;
      this.voiceConnection.ConnectUsingSettings((AppSettings) null);
    }
  }

  private void PhotonVoiceCreated(PhotonVoiceCreatedParams p) => this.InitUiValues();

  protected virtual void Update()
  {
    this.connectionStatusText.text = this.voiceConnection.Client.State.ToString();
    this.serverStatusText.text = $"{this.voiceConnection.Client.CloudRegion}/{this.voiceConnection.Client.CurrentServerAddress}";
    if (this.voiceConnection.PrimaryRecorder.IsCurrentlyTransmitting)
    {
      float currentAvgAmp = this.voiceConnection.PrimaryRecorder.LevelMeter.CurrentAvgAmp;
      if ((double) currentAvgAmp > 1.0)
        currentAvgAmp /= 32768f;
      if ((double) currentAvgAmp > 0.1)
      {
        this.inputWarningText.text = "Input too loud!";
        this.inputWarningText.color = this.warningColor;
      }
      else
      {
        this.inputWarningText.text = string.Empty;
        this.ResetTextColor(this.inputWarningText);
      }
    }
    if ((double) this.voiceConnection.FramesReceivedPerSecond > 0.0)
    {
      this.packetLossWarningText.text = $"{this.voiceConnection.FramesLostPercent:0.##}% Packet Loss";
      this.packetLossWarningText.color = (double) this.voiceConnection.FramesLostPercent > 1.0 ? this.warningColor : this.okColor;
    }
    else
    {
      this.packetLossWarningText.text = string.Empty;
      this.ResetTextColor(this.packetLossWarningText);
    }
    this.rttText.text = "RTT:" + (object) this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime;
    this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTime, this.rttText, this.rttYellowThreshold, this.rttRedThreshold);
    this.rttVariationText.text = "VAR:" + (object) this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance;
    this.SetTextColor(this.voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance, this.rttVariationText, this.rttVariationYellowThreshold, this.rttVariationRedThreshold);
  }

  private void SetTextColor(int textValue, Text text, int yellowThreshold, int redThreshold)
  {
    if (textValue > redThreshold)
      text.color = this.redColor;
    else if (textValue > yellowThreshold)
      text.color = this.warningColor;
    else
      text.color = this.okColor;
  }

  private void ResetTextColor(Text text) => text.color = this.defaultColor;

  private void InitUiCallbacks()
  {
    this.muteToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleMute));
    this.debugEchoToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDebugEcho));
    this.reliableTransmissionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleReliable));
    this.encryptionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleEncryption));
    this.streamAudioClipToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioClipStreaming));
    this.audioToneToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAudioToneFactory));
    this.photonVadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.TogglePhotonVAD));
    this.vadToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleVAD));
    this.aecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAEC));
    this.agcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAGC));
    this.dspToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleDsp));
    this.highPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleHighPass));
    this.aecHighPassToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleAecHighPass));
    this.noiseSuppressionToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.ToggleNoiseSuppression));
    this.agcCompressionGainSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcCompressionGainChanged));
    this.agcTargetLevelSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnAgcTargetLevelChanged));
    this.localNicknameText.SetSingleOnEndEditCallback(new UnityAction<string>(this.UpdateSyncedNickname));
    this.roomNameInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.JoinOrCreateRoom));
    this.reverseStreamDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnReverseStreamDelayChanged));
    this.androidAgcToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
    this.androidAecToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
    this.androidNsToggle.SetSingleOnValueChangedCallback(new UnityAction<bool>(this.OnAndroidMicSettingsChanged));
  }

  private void InitUiValues()
  {
    this.muteToggle.SetValue(this.voiceConnection.Client.LocalPlayer.IsMuted());
    this.debugEchoToggle.SetValue(this.voiceConnection.PrimaryRecorder.DebugEchoMode);
    this.reliableTransmissionToggle.SetValue(this.voiceConnection.PrimaryRecorder.ReliableMode);
    this.encryptionToggle.SetValue(this.voiceConnection.PrimaryRecorder.Encrypt);
    this.streamAudioClipToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.AudioClip);
    this.audioToneToggle.SetValue(this.voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.Factory && this.voiceConnection.PrimaryRecorder.InputFactory == this.toneInputFactory);
    this.photonVadToggle.SetValue(this.voiceConnection.PrimaryRecorder.VoiceDetection);
    this.androidAgcToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAGC);
    this.androidAecToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneAEC);
    this.androidNsToggle.SetValue(this.voiceConnection.PrimaryRecorder.AndroidMicrophoneNS);
    if ((UnityEngine.Object) this.webRtcDspGameObject != (UnityEngine.Object) null)
    {
      this.dspToggle.gameObject.SetActive(true);
      this.dspToggle.SetValue(this.voiceAudioPreprocessor.enabled);
      this.webRtcDspGameObject.SetActive(this.dspToggle.isOn);
      this.aecToggle.SetValue(this.voiceAudioPreprocessor.AEC);
      this.aecHighPassToggle.SetValue(this.voiceAudioPreprocessor.AecHighPass);
      this.reverseStreamDelayInputField.text = this.voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
      this.aecOptionsGameObject.SetActive(this.voiceAudioPreprocessor.AEC);
      this.noiseSuppressionToggle.isOn = this.voiceAudioPreprocessor.NoiseSuppression;
      this.agcToggle.SetValue(this.voiceAudioPreprocessor.AGC);
      this.agcCompressionGainSlider.SetValue((float) this.voiceAudioPreprocessor.AgcCompressionGain);
      this.agcTargetLevelSlider.SetValue((float) this.voiceAudioPreprocessor.AgcTargetLevel);
      this.compressionGainGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
      this.targetLevelGameObject.SetActive(this.voiceAudioPreprocessor.AGC);
      this.vadToggle.SetValue(this.voiceAudioPreprocessor.VAD);
      this.highPassToggle.SetValue(this.voiceAudioPreprocessor.HighPass);
    }
    else
      this.dspToggle.gameObject.SetActive(false);
  }

  private void SetRoomDebugText()
  {
    string empty = string.Empty;
    if (this.voiceConnection.Client.InRoom)
    {
      foreach (Player player in this.voiceConnection.Client.CurrentRoom.Players.Values)
        empty += player.ToStringFull();
      this.roomStatusText.text = $"{this.voiceConnection.Client.CurrentRoom.Name} {empty}";
    }
    else
      this.roomStatusText.text = string.Empty;
    this.roomStatusText.text = this.voiceConnection.Client.CurrentRoom == null ? string.Empty : $"{this.voiceConnection.Client.CurrentRoom.Name} {empty}";
  }

  protected virtual void OnActorPropertiesChanged(Player targetPlayer, Hashtable changedProps)
  {
    if (targetPlayer.IsLocal)
    {
      bool isOn = targetPlayer.IsMuted();
      this.voiceConnection.PrimaryRecorder.TransmitEnabled = !isOn;
      this.muteToggle.SetValue(isOn);
    }
    this.SetRoomDebugText();
  }

  protected void OnApplicationQuit()
  {
    this.voiceConnection.Client.RemoveCallbackTarget((object) this);
  }

  void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer) => this.SetRoomDebugText();

  void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer) => this.SetRoomDebugText();

  void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
  {
  }

  void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
  {
    this.OnActorPropertiesChanged(targetPlayer, changedProps);
  }

  void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
  {
  }

  void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
  {
  }

  void IMatchmakingCallbacks.OnCreatedRoom()
  {
  }

  void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
  {
  }

  void IMatchmakingCallbacks.OnJoinedRoom()
  {
    this.SetRoomDebugText();
    this.voiceConnection.Client.LocalPlayer.SetMic(this.voiceConnection.PrimaryRecorder.MicrophoneType);
  }

  void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
  {
  }

  void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
  {
  }

  void IMatchmakingCallbacks.OnLeftRoom()
  {
    this.SetRoomDebugText();
    this.SetDefaults();
  }
}
