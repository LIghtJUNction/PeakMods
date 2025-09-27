// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.DemoVoiceUI.MicrophoneSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity.UtilityScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos.DemoVoiceUI;

public class MicrophoneSelector : VoiceComponent
{
  public MicrophoneSelector.MicrophoneSelectorEvent onValueChanged = new MicrophoneSelector.MicrophoneSelectorEvent();
  private List<MicRef> micOptions;
  [SerializeField]
  private Dropdown micDropdown;
  [SerializeField]
  private Slider micLevelSlider;
  [SerializeField]
  private Recorder recorder;
  [SerializeField]
  [FormerlySerializedAs("RefreshButton")]
  private GameObject refreshButton;
  private Image fillArea;
  private Color defaultFillColor = Color.white;
  private Color speakingFillColor = Color.green;
  private IDeviceEnumerator unityMicEnum;
  private IDeviceEnumerator photonMicEnum;

  protected override void Awake()
  {
    base.Awake();
    this.unityMicEnum = (IDeviceEnumerator) new AudioInEnumerator(this.Logger);
    this.photonMicEnum = Platform.CreateAudioInEnumerator(this.Logger);
    this.photonMicEnum.OnReady = (Action) (() =>
    {
      this.SetupMicDropdown();
      this.SetCurrentValue();
    });
    this.refreshButton.GetComponentInChildren<Button>().onClick.AddListener(new UnityAction(this.RefreshMicrophones));
    this.fillArea = this.micLevelSlider.fillRect.GetComponent<Image>();
    this.defaultFillColor = this.fillArea.color;
  }

  private void Update()
  {
    if (!((UnityEngine.Object) this.recorder != (UnityEngine.Object) null))
      return;
    this.micLevelSlider.value = this.recorder.LevelMeter.CurrentPeakAmp;
    this.fillArea.color = this.recorder.IsCurrentlyTransmitting ? this.speakingFillColor : this.defaultFillColor;
  }

  private void OnEnable()
  {
    MicrophonePermission.MicrophonePermissionCallback += new Action<bool>(this.OnMicrophonePermissionCallback);
  }

  private void OnMicrophonePermissionCallback(bool granted) => this.RefreshMicrophones();

  private void OnDisable()
  {
    MicrophonePermission.MicrophonePermissionCallback -= new Action<bool>(this.OnMicrophonePermissionCallback);
  }

  private void SetupMicDropdown()
  {
    this.micDropdown.ClearOptions();
    this.micOptions = new List<MicRef>();
    List<string> options = new List<string>();
    this.micOptions.Add(new MicRef(MicType.Unity, DeviceInfo.Default));
    options.Add(string.Format("[Unity] [Default]"));
    foreach (DeviceInfo device in (IEnumerable<DeviceInfo>) this.unityMicEnum)
    {
      this.micOptions.Add(new MicRef(MicType.Unity, device));
      options.Add($"[Unity] {device}");
    }
    this.micOptions.Add(new MicRef(MicType.Photon, DeviceInfo.Default));
    options.Add(string.Format("[Photon] [Default]"));
    foreach (DeviceInfo device in (IEnumerable<DeviceInfo>) this.photonMicEnum)
    {
      this.micOptions.Add(new MicRef(MicType.Photon, device));
      options.Add($"[Photon] {device}");
    }
    this.micDropdown.AddOptions(options);
    this.micDropdown.onValueChanged.RemoveAllListeners();
    this.micDropdown.onValueChanged.AddListener((UnityAction<int>) (x => this.SwitchToSelectedMic()));
  }

  public void SwitchToSelectedMic()
  {
    MicRef micOption = this.micOptions[this.micDropdown.value];
    switch (micOption.MicType)
    {
      case MicType.Unity:
        this.recorder.SourceType = Recorder.InputSourceType.Microphone;
        this.recorder.MicrophoneType = Recorder.MicType.Unity;
        this.recorder.MicrophoneDevice = micOption.Device;
        break;
      case MicType.Photon:
        this.recorder.SourceType = Recorder.InputSourceType.Microphone;
        this.recorder.MicrophoneType = Recorder.MicType.Photon;
        this.recorder.MicrophoneDevice = micOption.Device;
        break;
    }
    this.onValueChanged?.Invoke(micOption.MicType, micOption.Device);
  }

  private void SetCurrentValue()
  {
    if (this.micOptions == null)
    {
      Debug.LogWarning((object) "micOptions list is null");
    }
    else
    {
      this.micDropdown.gameObject.SetActive(true);
      this.refreshButton.SetActive(true);
      for (int index = 0; index < this.micOptions.Count; ++index)
      {
        MicRef micOption = this.micOptions[index];
        if (micOption.MicType == MicType.Unity && this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Unity || micOption.MicType == MicType.Photon && this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Photon)
        {
          this.micDropdown.value = index;
          break;
        }
      }
    }
  }

  public void RefreshMicrophones()
  {
    this.unityMicEnum.Refresh();
    this.photonMicEnum.Refresh();
  }

  private void PhotonVoiceCreated() => this.RefreshMicrophones();

  public class MicrophoneSelectorEvent : UnityEvent<MicType, DeviceInfo>
  {
  }
}
