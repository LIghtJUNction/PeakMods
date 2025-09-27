// Decompiled with JetBrains decompiler
// Type: ReverbMix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Audio;

#nullable disable
public class ReverbMix : MonoBehaviour
{
  public AudioMixerGroup audioMixerGroup;
  private float startReverbStrength;
  public float reverbStrength;

  private void Start()
  {
    this.audioMixerGroup.audioMixer.GetFloat("EffectsStrength", out this.startReverbStrength);
    this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.reverbStrength);
  }

  private void Update()
  {
  }

  private void OnDisable()
  {
    this.audioMixerGroup.audioMixer.SetFloat("EffectsStrength", this.startReverbStrength);
  }
}
