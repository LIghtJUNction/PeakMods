// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.Highlighter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

[RequireComponent(typeof (Canvas))]
public class Highlighter : MonoBehaviour
{
  private Canvas canvas;
  private PhotonVoiceView photonVoiceView;
  [SerializeField]
  private Image recorderSprite;
  [SerializeField]
  private Image speakerSprite;
  [SerializeField]
  private Text bufferLagText;
  private bool showSpeakerLag;

  private void OnEnable()
  {
    ChangePOV.CameraChanged += new ChangePOV.OnCameraChanged(this.ChangePOV_CameraChanged);
    VoiceDemoUI.DebugToggled += new VoiceDemoUI.OnDebugToggle(this.VoiceDemoUI_DebugToggled);
  }

  private void OnDisable()
  {
    ChangePOV.CameraChanged -= new ChangePOV.OnCameraChanged(this.ChangePOV_CameraChanged);
    VoiceDemoUI.DebugToggled -= new VoiceDemoUI.OnDebugToggle(this.VoiceDemoUI_DebugToggled);
  }

  private void VoiceDemoUI_DebugToggled(bool debugMode) => this.showSpeakerLag = debugMode;

  private void ChangePOV_CameraChanged(Camera camera) => this.canvas.worldCamera = camera;

  private void Awake()
  {
    this.canvas = this.GetComponent<Canvas>();
    if ((Object) this.canvas != (Object) null && (Object) this.canvas.worldCamera == (Object) null)
      this.canvas.worldCamera = Camera.main;
    this.photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
  }

  private void Update()
  {
    this.recorderSprite.enabled = this.photonVoiceView.IsRecording;
    this.speakerSprite.enabled = this.photonVoiceView.IsSpeaking;
    this.bufferLagText.enabled = this.showSpeakerLag && this.photonVoiceView.IsSpeaking;
    if (!this.bufferLagText.enabled)
      return;
    this.bufferLagText.text = $"{this.photonVoiceView.SpeakerInUse.Lag}";
  }

  private void LateUpdate()
  {
    if ((Object) this.canvas == (Object) null || (Object) this.canvas.worldCamera == (Object) null)
      return;
    this.transform.rotation = Quaternion.Euler(0.0f, this.canvas.worldCamera.transform.eulerAngles.y, 0.0f);
  }
}
