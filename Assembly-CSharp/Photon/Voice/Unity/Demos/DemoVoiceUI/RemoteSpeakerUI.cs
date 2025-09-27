// Decompiled with JetBrains decompiler
// Type: Photon.Voice.Unity.Demos.DemoVoiceUI.RemoteSpeakerUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Photon.Voice.Unity.Demos.DemoVoiceUI;

public class RemoteSpeakerUI : MonoBehaviour, IInRoomCallbacks
{
  [SerializeField]
  private Text nameText;
  [SerializeField]
  protected Image remoteIsMuting;
  [SerializeField]
  private Image remoteIsTalking;
  [SerializeField]
  private InputField playDelayInputField;
  [SerializeField]
  private Text bufferLagText;
  [SerializeField]
  private Slider volumeSlider;
  [SerializeField]
  private Text photonVad;
  [SerializeField]
  private Text webrtcVad;
  [SerializeField]
  private Text aec;
  [SerializeField]
  private Text agc;
  [SerializeField]
  private Text mic;
  protected Speaker speaker;
  private AudioSource audioSource;
  protected VoiceConnection voiceConnection;
  protected LoadBalancingClient loadBalancingClient;
  private int smoothedLag;

  protected Player Actor
  {
    get
    {
      return this.loadBalancingClient == null || this.loadBalancingClient.CurrentRoom == null ? (Player) null : this.loadBalancingClient.CurrentRoom.GetPlayer(this.speaker.RemoteVoice.PlayerId);
    }
  }

  protected virtual void Start()
  {
    this.speaker = this.GetComponent<Speaker>();
    this.audioSource = this.GetComponent<AudioSource>();
    this.playDelayInputField.text = this.speaker.PlayDelay.ToString();
    this.playDelayInputField.SetSingleOnEndEditCallback(new UnityAction<string>(this.OnPlayDelayChanged));
    this.SetNickname();
    this.SetMutedState();
    this.SetProperties();
    this.volumeSlider.minValue = 0.0f;
    this.volumeSlider.maxValue = 1f;
    this.volumeSlider.SetSingleOnValueChangedCallback(new UnityAction<float>(this.OnVolumeChanged));
    this.volumeSlider.value = 1f;
    this.OnVolumeChanged(1f);
  }

  private void OnVolumeChanged(float newValue) => this.audioSource.volume = newValue;

  private void OnPlayDelayChanged(string str)
  {
    int result;
    if (int.TryParse(str, out result))
      this.speaker.PlayDelay = result;
    else
      Debug.LogErrorFormat("Failed to parse {0}", (object) str);
  }

  private void Update()
  {
    this.remoteIsTalking.enabled = this.speaker.IsPlaying;
    if (this.speaker.IsPlaying)
    {
      int lag = this.speaker.Lag;
      this.smoothedLag = (lag + this.smoothedLag * 99) / 100;
      this.bufferLagText.text = $"Buffer Lag: {(object) this.smoothedLag}/{(object) lag}";
    }
    else
      this.bufferLagText.text = $"Buffer Lag: {(object) this.smoothedLag}/-";
  }

  private void OnDestroy()
  {
    if (this.loadBalancingClient == null)
      return;
    this.loadBalancingClient.RemoveCallbackTarget((object) this);
  }

  private void SetNickname()
  {
    string str = this.speaker.name;
    if (this.Actor != null)
    {
      str = this.Actor.NickName;
      if (string.IsNullOrEmpty(str))
        str = "user " + (object) this.Actor.ActorNumber;
    }
    this.nameText.text = str;
  }

  private void SetMutedState() => this.SetMutedState(this.Actor.IsMuted());

  private void SetProperties()
  {
    this.photonVad.enabled = this.Actor.HasPhotonVAD();
    this.webrtcVad.enabled = this.Actor.HasWebRTCVAD();
    this.aec.enabled = this.Actor.HasAEC();
    this.agc.enabled = this.Actor.HasAGC();
    Text agc = this.agc;
    int num = this.Actor.GetAGCGain();
    string str1 = num.ToString();
    num = this.Actor.GetAGCLevel();
    string str2 = num.ToString();
    string str3 = $"AGC Gain: {str1} Level: {str2}";
    agc.text = str3;
    Recorder.MicType? mic1 = this.Actor.GetMic();
    this.mic.enabled = mic1.HasValue;
    Text mic2 = this.mic;
    string str4;
    if (!mic1.HasValue)
    {
      str4 = "";
    }
    else
    {
      Recorder.MicType? nullable = mic1;
      Recorder.MicType micType = Recorder.MicType.Unity;
      str4 = nullable.GetValueOrDefault() == micType & nullable.HasValue ? "Unity MIC" : "Photon MIC";
    }
    mic2.text = str4;
  }

  protected virtual void SetMutedState(bool isMuted) => this.remoteIsMuting.enabled = isMuted;

  protected virtual void OnActorPropertiesChanged(Player targetPlayer, Hashtable changedProps)
  {
    if (!((Object) this.speaker != (Object) null) || this.speaker.RemoteVoice == null || targetPlayer.ActorNumber != this.speaker.RemoteVoice.PlayerId)
      return;
    this.SetMutedState();
    this.SetNickname();
    this.SetProperties();
  }

  public virtual void Init(VoiceConnection vC)
  {
    this.voiceConnection = vC;
    this.loadBalancingClient = (LoadBalancingClient) this.voiceConnection.Client;
    this.loadBalancingClient.AddCallbackTarget((object) this);
  }

  void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
  {
  }

  void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
  {
  }

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
}
