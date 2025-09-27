// Decompiled with JetBrains decompiler
// Type: AnimatedMouth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

#nullable disable
public class AnimatedMouth : MonoBehaviour
{
  public AnimationCurve decibelToAmountCurve = AnimationCurve.EaseInOut(-80f, 0.0f, 12f, 1f);
  public bool isSpeaking;
  public AudioSource audioSource;
  public Vector2 BandPassFilter;
  [FormerlySerializedAs("amplitude")]
  [Range(0.0f, 1f)]
  public float volume;
  [FormerlySerializedAs("amplitudeHighest")]
  public float amplitudePeakLimiter;
  public float minAmplitudeThreshold = 0.5f;
  public float amplitudeHighestDecay = 0.01f;
  public float amplitudeSmoothing = 0.2f;
  public float talkThreshold = 0.1f;
  public float amplitudeMult;
  [HideInInspector]
  public int amplitudeIndex;
  [FormerlySerializedAs("textures")]
  [Header("Mouth Cards")]
  public Texture2D[] mouthTextures;
  public Renderer mouthRenderer;
  public Character character;
  public bool isGhost;
  private float volumePeak;
  private PushToTalkSetting pushToTalkSetting;
  private float[] m_lastSentLocalBuffer;

  private void Start()
  {
    this.amplitudePeakLimiter = this.minAmplitudeThreshold;
    this.character = this.GetComponent<Character>();
    if (!this.isGhost && (UnityEngine.Object) this.character != (UnityEngine.Object) null && this.character.IsLocal)
      Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetMic));
    this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
  }

  private void OnDestroy()
  {
    if (this.isGhost || !((UnityEngine.Object) this.character != (UnityEngine.Object) null) || !this.character.IsLocal || !(bool) (UnityEngine.Object) Singleton<MicrophoneRelay>.Instance)
      return;
    Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetMic));
  }

  public void OnGetMic(float[] buffer) => this.m_lastSentLocalBuffer = buffer;

  private void Update()
  {
    float[] numArray = new float[256 /*0x0100*/];
    if ((UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
      this.audioSource.GetSpectrumData(numArray, 0, FFTWindow.Rectangular);
    if (this.m_lastSentLocalBuffer != null)
      numArray = this.m_lastSentLocalBuffer;
    this.ProcessMicData(numArray);
  }

  public static float MicrophoneLevelMax(float[] data)
  {
    int num1 = 128 /*0x80*/;
    float num2 = 0.0f;
    for (int index = 0; index < num1; ++index)
    {
      float num3 = data[index] * data[index];
      if ((double) num2 < (double) num3)
        num2 = num3;
    }
    return num2;
  }

  public static float MicrophoneLevelMaxDecibels(float level)
  {
    return 20f * Mathf.Log10(Mathf.Abs(level));
  }

  private void ProcessMicData(float[] buffer)
  {
    if (!(bool) (UnityEngine.Object) this.audioSource || !this.isGhost && (UnityEngine.Object) this.character != (UnityEngine.Object) null && (this.character.data.dead || this.character.data.passedOut))
      return;
    float time = AnimatedMouth.MicrophoneLevelMaxDecibels(AnimatedMouth.MicrophoneLevelMax(buffer));
    if ((UnityEngine.Object) this.character != (UnityEngine.Object) null && this.character.IsLocal && (this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk && !this.character.input.pushToTalkPressed || this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToMute && this.character.input.pushToTalkPressed))
      time = -80f;
    float num = this.decibelToAmountCurve.Evaluate(time);
    if ((double) num > (double) this.amplitudePeakLimiter)
      this.amplitudePeakLimiter = num;
    if ((double) this.amplitudePeakLimiter > (double) this.minAmplitudeThreshold)
      this.amplitudePeakLimiter -= this.amplitudeHighestDecay * Time.deltaTime;
    this.volume = num / this.amplitudePeakLimiter;
    if ((double) this.volume > (double) this.volumePeak)
      this.volumePeak = this.volume;
    this.volumePeak = Mathf.Lerp(this.volumePeak, 0.0f, Time.deltaTime * this.amplitudeSmoothing);
    if ((double) this.volumePeak > (double) this.talkThreshold)
    {
      this.mouthRenderer.material.SetInt("_UseTalkSprites", 1);
      this.isSpeaking = true;
    }
    else
    {
      this.isSpeaking = false;
      this.mouthRenderer.material.SetInt("_UseTalkSprites", 0);
    }
    this.amplitudeIndex = (int) ((double) Mathf.Clamp01(this.volumePeak * this.amplitudeMult) * (double) (this.mouthTextures.Length - 1));
    this.mouthRenderer.material.SetTexture("_TalkSprite", (Texture) this.mouthTextures[this.amplitudeIndex]);
  }
}
