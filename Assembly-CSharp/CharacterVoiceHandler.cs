// Decompiled with JetBrains decompiler
// Type: CharacterVoiceHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice;
using Photon.Voice.Unity;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

#nullable disable
public class CharacterVoiceHandler : MonoBehaviour
{
  private Character m_character;
  [SerializeField]
  private AudioMixer m_mixer;
  [SerializeField]
  private AudioMixerGroup m_mixerGroup1;
  [SerializeField]
  private AudioMixerGroup m_mixerGroup2;
  [SerializeField]
  private AudioMixerGroup m_mixerGroup3;
  [SerializeField]
  private AudioMixerGroup m_mixerGroup4;
  private AudioSource m_source;
  private string m_parameter;
  private MicrophoneSetting microphoneSetting;
  private PushToTalkSetting pushToTalkSetting;
  private string m_setMicrophoneDevice;
  private Recorder m_Recorder;
  private bool m_currentlyTransmitting;
  private float audioLevel = 0.5f;
  private bool firstTime = true;
  public const float DEFAULT_VOICE_VOLUME = 0.5f;

  internal AudioSource audioSource { get; private set; }

  private void OnEnable()
  {
    GlobalEvents.OnCharacterAudioLevelsUpdated += new Action(this.UpdateAudioLevel);
  }

  private void OnDisable()
  {
    GlobalEvents.OnCharacterAudioLevelsUpdated -= new Action(this.UpdateAudioLevel);
  }

  private void UpdateAudioLevel()
  {
    if (AudioLevels.PlayerAudioLevels.ContainsKey(this.m_character.photonView.OwnerActorNr))
    {
      float playerAudioLevel = AudioLevels.PlayerAudioLevels[this.m_character.photonView.OwnerActorNr];
      this.audioLevel = playerAudioLevel;
      Debug.Log((object) $"{this.m_character.characterName} set audio levels to {playerAudioLevel}");
    }
    else
      this.audioLevel = 0.5f;
  }

  private void Start()
  {
    this.m_Recorder = this.GetComponent<Recorder>();
    this.m_character = this.GetComponentInParent<Character>();
    this.microphoneSetting = GameHandler.Instance.SettingsHandler.GetSetting<MicrophoneSetting>();
    this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
    this.audioSource = this.GetComponent<AudioSource>();
    this.m_source = this.GetComponent<AudioSource>();
    if (this.m_character.IsLocal)
      return;
    byte group = PlayerHandler.AssignMixerGroup(this.m_character);
    if (group != byte.MaxValue)
    {
      this.m_source.outputAudioMixerGroup = this.GetMixerGroup(group);
      this.m_parameter = this.GetMixerGroupParameter(group);
    }
    this.UpdateAudioLevel();
  }

  private AudioMixerGroup GetMixerGroup(byte group)
  {
    switch (group)
    {
      case 0:
        return this.m_mixerGroup1;
      case 1:
        return this.m_mixerGroup2;
      case 2:
        return this.m_mixerGroup3;
      case 3:
        return this.m_mixerGroup4;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private string GetMixerGroupParameter(byte group)
  {
    return $"Voice{((int) group + 1).ToString()}Effects";
  }

  private void Update()
  {
    this.m_source.volume = this.m_character.data.fullyConscious ? this.audioLevel : ((UnityEngine.Object) this.m_character.Ghost != (UnityEngine.Object) null ? this.audioLevel : 0.0f);
    this.PushToTalk();
    if (!this.m_character.IsLocal || this.m_character.isBot)
      return;
    string id = this.microphoneSetting.Value.id;
    if (!(id != this.m_setMicrophoneDevice) || string.IsNullOrEmpty(id))
      return;
    this.m_setMicrophoneDevice = id;
    this.m_Recorder.MicrophoneDevice = new DeviceInfo(id);
    Debug.Log((object) ("Setting microphone to " + id));
  }

  private void PushToTalk()
  {
    bool flag = this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.VoiceActivation || this.m_character.input.pushToTalkPressed && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk || !this.m_character.input.pushToTalkPressed && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToMute;
    if (flag == this.m_currentlyTransmitting && !this.firstTime)
      return;
    this.firstTime = false;
    this.m_currentlyTransmitting = flag;
    this.m_Recorder.TransmitEnabled = flag;
  }

  private void LateUpdate()
  {
    bool flag = false;
    if ((UnityEngine.Object) Singleton<PeakHandler>.Instance != (UnityEngine.Object) null && Singleton<PeakHandler>.Instance.isPlayingCinematic)
      flag = true;
    this.m_source.spatialBlend = flag ? 0.0f : 1f;
    if (this.m_character.IsLocal)
      return;
    Vector3 position = this.m_character.refs.head.transform.position;
    if ((UnityEngine.Object) this.m_character.Ghost != (UnityEngine.Object) null)
      position = this.m_character.Ghost.transform.position;
    this.transform.position = position;
    float num = math.saturate(1f - math.remap(0.0f, 0.3f, 0.0f, 1f, math.saturate(LightVolume.Instance().SamplePositionAlpha(position))));
    if (!flag)
      return;
    num = 0.0f;
  }
}
