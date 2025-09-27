// Decompiled with JetBrains decompiler
// Type: SFX_Instance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "SoundEffectInstance", menuName = "Landfall/SoundEffectInstance")]
public class SFX_Instance : ScriptableObject
{
  public AudioClip[] clips;
  public SFX_Settings settings;
  internal float lastTimePlayed;

  public AudioClip GetClip() => this.clips[Random.Range(0, this.clips.Length)];

  public void Play(Vector3 pos = default (Vector3)) => SFX_Player.instance.PlaySFX(this, pos);

  internal void OnPlayed() => this.lastTimePlayed = Time.unscaledTime;

  internal bool ReadyToPlay()
  {
    return (double) this.lastTimePlayed > (double) Time.unscaledTime + (double) this.settings.cooldown || (double) this.lastTimePlayed + (double) this.settings.cooldown < (double) Time.unscaledTime;
  }
}
