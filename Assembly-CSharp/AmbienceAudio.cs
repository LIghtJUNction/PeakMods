// Decompiled with JetBrains decompiler
// Type: AmbienceAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class AmbienceAudio : MonoBehaviour
{
  public float obstruction;
  private float coverage;
  public bool ceiling;
  public bool inTomb;
  public LayerMask layer;
  private RaycastHit hit;
  public AudioReverbZone reverb;
  private DayNightManager dayNight;
  private Animator ambienceVolumes;
  private int t;
  public AudioSource stingerSource;
  public AudioClip[] startStinger;
  public AudioClip[] sunRiseStinger;
  public AudioClip[] sunSetStinger;
  public AudioClip[] nightStinger;
  public bool volcano;
  public GameObject volcanoObj;
  public float vulcanoT;
  public float naturelessTerrain;
  public AudioSource mainMusic;
  public AudioClip climbStingerBeach;
  private bool playedBeach;
  public AudioClip climbStingerTropics;
  private bool playedTropics;
  public AudioClip climbStingerAlpine;
  private bool playedAlpineOrMesa;
  public AudioClip climbStingerMesa;
  public AudioClip climbStingerCaldera;
  private bool playedCaldera;
  public AudioClip climbStingerKiln;
  private bool playedKiln;
  public AudioClip climbStingerPeak;
  private bool playedPeak;
  public AudioClip tombClimb;
  public AudioSource bingBongStatue;
  private float priorityMusicTimer;
  public float beachStingerZ;
  public float tropicsStingerZ;
  public float alpineStingerZ;
  public float calderaStingerZ;
  public float kilnStingerY;
  public float peaksTingerZ;
  public Transform voice;
  public AudioReverbFilter reverbFilter;
  public AudioEchoFilter echoFilter;
  public AudioLowPassFilter lowPassFilter;
  private float inKiln;
  private bool tropicsSunTime1;
  private bool tropicsSunTime2;
  public AudioClip tropicsSunrise;
  public AudioClip tropicsSunset;
  private bool alpineSunTime1;
  private bool alpineSunTime2;
  public AudioClip alpineSunrise;
  public AudioClip alpineSunset;
  public AudioClip desertSunrise;
  public AudioClip desertSunset;
  private bool playedTomb;

  private void Start()
  {
    this.ambienceVolumes = this.GetComponent<Animator>();
    this.dayNight = Object.FindAnyObjectByType<DayNightManager>();
    this.stingerSource.clip = this.startStinger[UnityEngine.Random.Range(0, this.startStinger.Length)];
    this.stingerSource.Play();
    this.volcanoObj = GameObject.Find("VolcanoModel");
    if (!(bool) (Object) GameObject.Find("Airport"))
      return;
    this.gameObject.SetActive(false);
    if (!(bool) (Object) this.voice)
      return;
    this.reverbFilter.enabled = false;
    this.echoFilter.enabled = false;
    this.lowPassFilter.enabled = false;
  }

  private void FixedUpdate()
  {
    this.naturelessTerrain -= 0.1f;
    if ((double) this.naturelessTerrain > 0.0)
      this.ambienceVolumes.SetBool("Natureless", true);
    if ((double) this.naturelessTerrain < 0.0)
      this.ambienceVolumes.SetBool("Natureless", false);
    this.ambienceVolumes.SetBool("Tomb", this.inTomb);
    try
    {
      this.reverb.room = (int) math.remap(0.0f, 1f, -4000f, -100f, math.saturate(1f - math.remap(0.0f, 0.3f, 0.0f, 1f, math.saturate(LightVolume.Instance().SamplePositionAlpha(this.transform.position)))));
    }
    catch
    {
      Debug.LogError((object) "You probably need to bake the lightmap");
    }
    if ((bool) (Object) this.volcanoObj)
    {
      this.vulcanoT -= Time.deltaTime;
      if ((double) this.vulcanoT <= 0.0)
      {
        this.volcano = false;
        this.vulcanoT = 0.0f;
        this.reverb.enabled = true;
      }
      if ((double) this.vulcanoT > 0.0)
      {
        this.volcano = true;
        this.reverb.enabled = false;
      }
      if ((double) Vector3.Distance(this.transform.position, this.volcanoObj.transform.position) < 200.0)
        this.vulcanoT = 10f;
      this.ambienceVolumes.SetBool("Volcano", this.volcano);
    }
    if ((bool) (Object) this.ambienceVolumes && (bool) (Object) this.dayNight)
    {
      this.ambienceVolumes.SetFloat("Height", this.transform.position.y);
      this.ambienceVolumes.SetFloat("Time", this.dayNight.timeOfDay);
      if ((double) this.transform.position.z > (double) this.alpineStingerZ - 500.0 && !this.alpineSunTime1 && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
        this.ambienceVolumes.SetBool("Desert", true);
      if (!this.inTomb)
      {
        if ((double) this.dayNight.timeOfDay > 5.5 && (double) this.dayNight.timeOfDay < 6.5 && this.t != 1)
        {
          this.t = 1;
          this.stingerSource.clip = this.sunRiseStinger[UnityEngine.Random.Range(0, this.sunRiseStinger.Length)];
          if ((double) this.transform.position.z > (double) this.tropicsStingerZ - 500.0 && !this.tropicsSunTime2)
          {
            this.stingerSource.clip = this.tropicsSunrise;
            if ((double) this.priorityMusicTimer <= 0.0)
              this.tropicsSunTime2 = true;
          }
          if ((double) this.transform.position.z > (double) this.alpineStingerZ - 500.0 && !this.alpineSunTime2)
          {
            if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
              this.stingerSource.clip = this.alpineSunrise;
            else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
              this.stingerSource.clip = this.desertSunrise;
            if ((double) this.priorityMusicTimer <= 0.0)
              this.alpineSunTime2 = true;
          }
          if (!this.volcano)
            this.stingerSource.Play();
        }
        if ((double) this.dayNight.timeOfDay > 19.5 && (double) this.dayNight.timeOfDay < 20.0 && this.t != 2)
        {
          this.t = 2;
          this.stingerSource.clip = this.sunSetStinger[UnityEngine.Random.Range(0, this.sunSetStinger.Length)];
          if ((double) this.transform.position.z > (double) this.tropicsStingerZ - 500.0 && !this.tropicsSunTime1)
          {
            this.stingerSource.clip = this.tropicsSunset;
            if ((double) this.priorityMusicTimer <= 0.0)
              this.tropicsSunTime1 = true;
          }
          if ((double) this.transform.position.z > (double) this.alpineStingerZ - 500.0 && !this.alpineSunTime1)
          {
            if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
              this.stingerSource.clip = this.alpineSunset;
            else if (Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
              this.stingerSource.clip = this.desertSunset;
            this.ambienceVolumes.SetBool("Desert", true);
            if ((double) this.priorityMusicTimer <= 0.0)
              this.alpineSunTime1 = true;
          }
          if (!this.volcano)
            this.stingerSource.Play();
        }
        if ((double) this.dayNight.timeOfDay > 21.200000762939453 && (double) this.dayNight.timeOfDay < 26.0 && this.t != 3)
        {
          this.t = 3;
          this.stingerSource.clip = this.nightStinger[UnityEngine.Random.Range(0, this.nightStinger.Length)];
          if (!this.volcano)
            this.stingerSource.Play();
        }
      }
    }
    this.priorityMusicTimer -= Time.deltaTime;
    CharacterData component = this.transform.root.GetComponent<CharacterData>();
    if ((double) component.sinceDead > 0.5 && !Character.localCharacter.warping && !component.passedOut && !component.dead && !component.fullyPassedOut && !this.inTomb)
    {
      if ((double) this.transform.position.z > (double) this.beachStingerZ && !this.playedBeach)
      {
        this.playedBeach = true;
        this.mainMusic.clip = this.climbStingerBeach;
        this.mainMusic.volume = 0.35f;
        this.mainMusic.Play();
        this.priorityMusicTimer = 120f;
        Debug.Log((object) "Played beach stinger");
      }
      if ((double) this.transform.position.z > (double) this.tropicsStingerZ && !this.playedTropics)
      {
        this.playedTropics = true;
        this.mainMusic.clip = this.climbStingerTropics;
        this.mainMusic.volume = 0.5f;
        this.mainMusic.Play();
        this.priorityMusicTimer = 120f;
        Debug.Log((object) "Played tropics stinger");
      }
      if ((double) this.transform.position.z > (double) this.alpineStingerZ && !this.playedAlpineOrMesa && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Alpine))
      {
        this.mainMusic.volume = 0.4f;
        this.playedAlpineOrMesa = true;
        this.mainMusic.clip = this.climbStingerAlpine;
        this.mainMusic.Play();
        this.priorityMusicTimer = 120f;
        Debug.Log((object) "Played alpine stinger");
      }
      if ((double) this.transform.position.z > (double) this.alpineStingerZ && !this.playedAlpineOrMesa && Singleton<MapHandler>.Instance.BiomeIsPresent(Biome.BiomeType.Mesa))
      {
        this.mainMusic.volume = 0.4f;
        this.playedAlpineOrMesa = true;
        this.mainMusic.clip = this.climbStingerMesa;
        this.mainMusic.Play();
        this.priorityMusicTimer = 120f;
        Debug.Log((object) "Played mesa stinger");
      }
      if ((double) this.transform.position.z > (double) this.calderaStingerZ && !this.playedCaldera)
      {
        if (!(bool) (Object) this.volcanoObj)
          this.volcanoObj = GameObject.Find("VolcanoModel");
        this.mainMusic.volume = 0.75f;
        this.playedCaldera = true;
        this.mainMusic.clip = this.climbStingerCaldera;
        this.mainMusic.Play();
        this.priorityMusicTimer = 120f;
        Debug.Log((object) "Played caldera stinger");
      }
      if ((double) this.transform.position.y > (double) this.kilnStingerY && !this.playedKiln)
      {
        this.inKiln -= Time.deltaTime;
        if ((double) this.inKiln < -2.0)
        {
          this.mainMusic.volume = 0.6f;
          this.playedKiln = true;
          this.mainMusic.clip = this.climbStingerKiln;
          this.mainMusic.Play();
          this.priorityMusicTimer = 120f;
        }
        Debug.Log((object) "Played kiln stinger");
      }
      else
        this.inKiln = 0.0f;
      if ((double) this.transform.position.z > (double) this.peaksTingerZ && !this.playedPeak)
      {
        this.mainMusic.volume = 1f;
        this.playedPeak = true;
        this.mainMusic.clip = this.climbStingerPeak;
        this.mainMusic.Play();
        this.priorityMusicTimer = 120f;
      }
    }
    else
    {
      this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0.0f, 0.05f);
      this.mainMusic.volume = Mathf.Lerp(this.mainMusic.volume, 0.0f, 0.05f);
    }
    if ((double) this.priorityMusicTimer > 0.0)
      this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0.0f, 0.05f);
    if ((double) this.priorityMusicTimer <= 0.0)
    {
      this.stingerSource.volume = Mathf.Lerp(this.stingerSource.volume, 0.35f, 0.05f);
      this.priorityMusicTimer = 0.0f;
    }
    if (this.inTomb)
    {
      if ((double) this.transform.position.z > 700.0 && !this.playedTomb)
      {
        this.playedTomb = true;
        this.mainMusic.clip = this.tombClimb;
        this.mainMusic.Play();
      }
      this.mainMusic.volume = 0.5f;
    }
    if ((double) this.transform.position.y <= 450.0)
      return;
    this.inTomb = false;
  }

  private void Coverage()
  {
    float num = 8f;
    this.ceiling = false;
    if (Physics.Linecast(this.transform.position, this.transform.position + Vector3.up * 8f * num, out this.hit, (int) this.layer))
      this.ceiling = true;
    if (Physics.Linecast(this.transform.position, this.transform.position + this.transform.forward * num, out this.hit, (int) this.layer))
      ++this.coverage;
    if (Physics.Linecast(this.transform.position, this.transform.position + this.transform.forward * -num, out this.hit, (int) this.layer))
      ++this.coverage;
    if (Physics.Linecast(this.transform.position, this.transform.position + this.transform.right * num, out this.hit, (int) this.layer))
      ++this.coverage;
    if (Physics.Linecast(this.transform.position, this.transform.position + this.transform.right * -num, out this.hit, (int) this.layer))
      ++this.coverage;
    if (!Physics.Linecast(this.transform.position, this.transform.position + this.transform.up * num * 4f, out this.hit, (int) this.layer))
      return;
    this.coverage += 2f;
  }
}
