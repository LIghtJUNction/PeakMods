// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.BackgroundMusicController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos;

public class BackgroundMusicController : MonoBehaviour
{
  [SerializeField]
  private Text volumeText;
  [SerializeField]
  private Slider volumeSlider;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private float initialVolume = 0.125f;

  private void Awake()
  {
    this.volumeSlider.minValue = 0.0f;
    this.volumeSlider.maxValue = 1f;
    this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
    this.volumeSlider.value = this.initialVolume;
    this.OnVolumeChanged(this.initialVolume);
  }

  private void OnVolumeChanged(float newValue) => this.audioSource.volume = newValue;
}
