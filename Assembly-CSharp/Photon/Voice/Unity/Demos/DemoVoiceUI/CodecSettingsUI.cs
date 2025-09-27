// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.DemoVoiceUI.CodecSettingsUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using POpusCodec.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos.DemoVoiceUI;

public class CodecSettingsUI : MonoBehaviour
{
  [SerializeField]
  private Dropdown frameDurationDropdown;
  [SerializeField]
  private Dropdown samplingRateDropdown;
  [SerializeField]
  private InputField bitrateInputField;
  [SerializeField]
  private Recorder recorder;
  private static readonly List<string> frameDurationOptions = new List<string>()
  {
    "2.5ms",
    "5ms",
    "10ms",
    "20ms",
    "40ms",
    "60ms"
  };
  private static readonly List<string> samplingRateOptions = new List<string>()
  {
    "8kHz",
    "12kHz",
    "16kHz",
    "24kHz",
    "48kHz"
  };

  private void Awake()
  {
    this.frameDurationDropdown.ClearOptions();
    this.frameDurationDropdown.AddOptions(CodecSettingsUI.frameDurationOptions);
    this.InitFrameDuration();
    this.frameDurationDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnFrameDurationChanged));
    this.samplingRateDropdown.ClearOptions();
    this.samplingRateDropdown.AddOptions(CodecSettingsUI.samplingRateOptions);
    this.InitSamplingRate();
    this.samplingRateDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnSamplingRateChanged));
    this.bitrateInputField.SetSingleOnValueChangedCallback(new UnityAction<string>(this.OnBitrateChanged));
    this.InitBitrate();
  }

  private void Update()
  {
    this.InitFrameDuration();
    this.InitSamplingRate();
    this.InitBitrate();
  }

  private void OnBitrateChanged(string newBitrateString)
  {
    int result;
    if (!int.TryParse(newBitrateString, out result))
      return;
    this.recorder.Bitrate = result;
  }

  private void OnFrameDurationChanged(int index)
  {
    OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
    switch (index)
    {
      case 0:
        frameDuration = OpusCodec.FrameDuration.Frame2dot5ms;
        break;
      case 1:
        frameDuration = OpusCodec.FrameDuration.Frame5ms;
        break;
      case 2:
        frameDuration = OpusCodec.FrameDuration.Frame10ms;
        break;
      case 3:
        frameDuration = OpusCodec.FrameDuration.Frame20ms;
        break;
      case 4:
        frameDuration = OpusCodec.FrameDuration.Frame40ms;
        break;
      case 5:
        frameDuration = OpusCodec.FrameDuration.Frame60ms;
        break;
    }
    this.recorder.FrameDuration = frameDuration;
  }

  private void OnSamplingRateChanged(int index)
  {
    SamplingRate samplingRate = this.recorder.SamplingRate;
    switch (index)
    {
      case 0:
        samplingRate = SamplingRate.Sampling08000;
        break;
      case 1:
        samplingRate = SamplingRate.Sampling12000;
        break;
      case 2:
        samplingRate = SamplingRate.Sampling16000;
        break;
      case 3:
        samplingRate = SamplingRate.Sampling24000;
        break;
      case 4:
        samplingRate = SamplingRate.Sampling48000;
        break;
    }
    this.recorder.SamplingRate = samplingRate;
  }

  private void InitFrameDuration()
  {
    int num = 0;
    switch (this.recorder.FrameDuration)
    {
      case OpusCodec.FrameDuration.Frame5ms:
        num = 1;
        break;
      case OpusCodec.FrameDuration.Frame10ms:
        num = 2;
        break;
      case OpusCodec.FrameDuration.Frame20ms:
        num = 3;
        break;
      case OpusCodec.FrameDuration.Frame40ms:
        num = 4;
        break;
      case OpusCodec.FrameDuration.Frame60ms:
        num = 5;
        break;
    }
    this.frameDurationDropdown.value = num;
  }

  private void InitSamplingRate()
  {
    int num = 0;
    switch (this.recorder.SamplingRate)
    {
      case SamplingRate.Sampling12000:
        num = 1;
        break;
      case SamplingRate.Sampling16000:
        num = 2;
        break;
      case SamplingRate.Sampling24000:
        num = 3;
        break;
      case SamplingRate.Sampling48000:
        num = 4;
        break;
    }
    this.samplingRateDropdown.value = num;
  }

  private void InitBitrate() => this.bitrateInputField.text = this.recorder.Bitrate.ToString();
}
