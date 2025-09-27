// Decompiled with JetBrains decompiler
// Type: SFX_Player
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
public class SFX_Player : MonoBehaviour
{
  public AudioMixerGroup defaultMixerGroup;
  private GameObject defaultSource;
  public List<SFX_Player.SFX_Source> sources = new List<SFX_Player.SFX_Source>();
  private List<SFX_Player.SoundEffectHandle> currentlyPlayed = new List<SFX_Player.SoundEffectHandle>();
  public static SFX_Player instance;
  private int nrOfSoundsPlayed;

  private void Start()
  {
    this.defaultSource = this.GetComponentInChildren<AudioSource>().gameObject;
    SFX_Player.instance = this;
    for (int index = 0; index < 20; ++index)
      this.CreateNewSource();
  }

  public SFX_Player.SoundEffectHandle PlaySFX(
    SFX_Instance SFX,
    Vector3 position,
    Transform followTransform = null,
    SFX_Settings overrideSettings = null,
    float volumeMultiplier = 1f,
    bool loop = false)
  {
    if ((UnityEngine.Object) SFX == (UnityEngine.Object) null)
      return (SFX_Player.SoundEffectHandle) null;
    if (SFX.clips.Length == 0)
      return (SFX_Player.SoundEffectHandle) null;
    if (!SFX.ReadyToPlay())
      return (SFX_Player.SoundEffectHandle) null;
    if ((double) SFX.settings.spatialBlend > 0.0 && (double) Vector3.Distance(MainCamera.instance.transform.position, position) > (double) SFX.settings.range / 2.0)
      return (SFX_Player.SoundEffectHandle) null;
    if (this.nrOfSoundsPlayed + 1 >= AudioSettings.GetConfiguration().numRealVoices)
      this.StopOldest();
    SFX.OnPlayed();
    SFX_Player.SoundEffectHandle handle = new SFX_Player.SoundEffectHandle();
    handle.Init(this.StartCoroutine(this.IPlaySFX(SFX, position, followTransform, overrideSettings, volumeMultiplier, loop, handle)));
    return handle;
  }

  private void StopOldest() => this.currentlyPlayed[0].source.StopPlaying();

  private IEnumerator IPlaySFX(
    SFX_Instance SFX,
    Vector3 position,
    Transform followTransform,
    SFX_Settings overrideSettings,
    float volumeMultiplier,
    bool loop,
    SFX_Player.SoundEffectHandle handle)
  {
    SFX_Player.SFX_Source source = this.GetAvailibleSource();
    AudioClip clip = SFX.GetClip();
    if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Trying to play null sound >:I");
    }
    else
    {
      SFX_Settings settings = SFX.settings;
      if (overrideSettings != null)
        settings = overrideSettings;
      float c = 0.0f;
      float t = clip.length;
      source.source.clip = clip;
      source.source.transform.position = position;
      source.source.volume = settings.volume * UnityEngine.Random.Range(1f - settings.volume_Variation, 1f) * volumeMultiplier;
      source.source.pitch = settings.pitch + UnityEngine.Random.Range((float) (-(double) settings.pitch_Variation * 0.5), settings.pitch_Variation * 0.5f);
      source.source.maxDistance = settings.range;
      source.source.spatialBlend = settings.spatialBlend;
      source.source.dopplerLevel = settings.dopplerLevel;
      source.source.loop = loop;
      source.source.outputAudioMixerGroup = this.defaultMixerGroup;
      Vector3 relativePos = Vector3.zero;
      if ((bool) (UnityEngine.Object) followTransform)
        relativePos = followTransform.InverseTransformPoint(position);
      source.StartPlaying(handle);
      while ((double) c < (double) t | loop)
      {
        c += Time.deltaTime * settings.pitch;
        if ((bool) (UnityEngine.Object) followTransform)
          source.source.transform.position = followTransform.TransformPoint(relativePos);
        yield return (object) null;
      }
      source.StopPlaying();
    }
  }

  private SFX_Player.SFX_Source GetAvailibleSource()
  {
    for (int index = 0; index < this.sources.Count; ++index)
    {
      if (!this.sources[index].isPlaying)
        return this.sources[index];
    }
    return this.CreateNewSource();
  }

  private SFX_Player.SFX_Source CreateNewSource()
  {
    SFX_Player.SFX_Source newSource = new SFX_Player.SFX_Source();
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.defaultSource, this.transform.position, this.transform.rotation, this.transform);
    newSource.source = gameObject.GetComponent<AudioSource>();
    newSource.player = this;
    this.sources.Add(newSource);
    return newSource;
  }

  private void OnPlayed(SFX_Player.SoundEffectHandle handle)
  {
    ++this.nrOfSoundsPlayed;
    this.currentlyPlayed.Add(handle);
  }

  private void OnStopped(SFX_Player.SoundEffectHandle handle)
  {
    --this.nrOfSoundsPlayed;
    this.currentlyPlayed.Remove(handle);
  }

  public static void StopPlaying(SFX_Player.SoundEffectHandle handle, float fadeTime = 0.0f)
  {
    SFX_Player.SFX_Source sourceFromHandle = SFX_Player.GetSFXSourceFromHandle(handle);
    if (sourceFromHandle == null)
      return;
    if ((double) fadeTime == 0.0)
      sourceFromHandle.StopPlaying();
    else
      SFX_Player.instance.StartCoroutine(SFX_Player.FadeOut(sourceFromHandle, fadeTime));
  }

  private static IEnumerator FadeOut(SFX_Player.SFX_Source source, float fadeTime)
  {
    float c = 0.0f;
    float length = fadeTime;
    float startVolume = source.source.volume;
    while ((double) c < (double) length)
    {
      c += Time.deltaTime;
      source.source.volume = Mathf.Lerp(startVolume, 0.0f, c / length);
      yield return (object) null;
    }
    source.StopPlaying();
  }

  private static SFX_Player.SFX_Source GetSFXSourceFromHandle(SFX_Player.SoundEffectHandle handle)
  {
    foreach (SFX_Player.SFX_Source source in SFX_Player.instance.sources)
    {
      if (source.handle == handle)
        return source;
    }
    return (SFX_Player.SFX_Source) null;
  }

  [Serializable]
  public class SFX_Source
  {
    public AudioSource source;
    public bool isPlaying;
    public SFX_Player.SoundEffectHandle handle;
    public SFX_Player player;

    public void StopPlaying()
    {
      if (!this.isPlaying)
        return;
      if (this.handle.corutine != null)
        this.player.StopCoroutine(this.handle.corutine);
      this.player.OnStopped(this.handle);
      this.source.Stop();
      this.isPlaying = false;
      this.handle.source = (SFX_Player.SFX_Source) null;
      this.handle = (SFX_Player.SoundEffectHandle) null;
    }

    public void StartPlaying(SFX_Player.SoundEffectHandle setHandle)
    {
      if (this.isPlaying)
        return;
      this.player.OnPlayed(setHandle);
      this.source.Play();
      this.isPlaying = true;
      this.handle = setHandle;
      this.handle.source = this;
    }
  }

  public class SoundEffectHandle
  {
    public Coroutine corutine;
    public SFX_Player.SFX_Source source;

    public void Init(Coroutine c) => this.corutine = c;
  }
}
