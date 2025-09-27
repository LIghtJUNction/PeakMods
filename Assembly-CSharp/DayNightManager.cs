// Decompiled with JetBrains decompiler
// Type: DayNightManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
public class DayNightManager : MonoBehaviour
{
  public static DayNightManager instance;
  private static readonly int TIMEOFDAY = Shader.PropertyToID("_TimeOfDay");
  private static readonly int FOG = Shader.PropertyToID("EXTRAFOG");
  private static readonly int Name = Shader.PropertyToID("IsDay");
  private static readonly int IsDayReal = Shader.PropertyToID(nameof (IsDayReal));
  private static readonly int SkyTopColor = Shader.PropertyToID(nameof (SkyTopColor));
  private static readonly int SkyMidColor = Shader.PropertyToID(nameof (SkyMidColor));
  private static readonly int SkyBottomColor = Shader.PropertyToID(nameof (SkyBottomColor));
  [Range(0.0f, 48f)]
  public float timeOfDay;
  public float dayLengthInMinutes = 10f;
  public float startingTimeOfDay = 9f;
  public float dayStart = 5f;
  public float dayEnd = 21f;
  public int dayCount = 1;
  public LensFlareComponentSRP lensFlare;
  public AnimationCurve angleOffsetZ;
  public Vector3 highNoonRotation;
  public Transform earth;
  public Light sun;
  public Light moon;
  public DayNightProfile currentProfile;
  public DayNightProfile newProfile;
  private float profileBlend;
  [Header("Special Day")]
  [Range(0.0f, 1f)]
  public float specialDaySunBlend;
  public Color specialSunColor;
  public float specialDaySkyBlend;
  public Color specialTopColor;
  public Color specialMidColor;
  public Color specialBottomColor;
  private List<string> lastShaderFloats = new List<string>();
  private List<string> allShaderFloats = new List<string>();
  private bool isBlending;
  public string timeString;
  public float rotDir = 1f;
  public float snowstormWindFactor;
  public float rainstormWindFactor;
  private PhotonView photonView;
  public float dayNightRatio = 2f;
  public float syncTimer;
  private bool passedMidnight;
  public float HazeDebug;
  public float RimFresnelDebug;
  public float LavaAlphaDebug;
  public float BandFogDebug;
  public float SunSizeDebug;

  public string getShaderValue(DayNightManager.ShaderParams parameter)
  {
    switch (parameter)
    {
      case DayNightManager.ShaderParams.AirDistortion:
        return "_GlobalHazeAmount";
      case DayNightManager.ShaderParams.RimFresnel:
        return "RimFresnelIntensity";
      case DayNightManager.ShaderParams.LavaAlpha:
        return "LavaAlpha";
      case DayNightManager.ShaderParams.BandFog:
        return "BandFogAmount";
      case DayNightManager.ShaderParams.SunSize:
        return "SunSizeMult";
      default:
        return "";
    }
  }

  public void clearAllShaderParams()
  {
    foreach (DayNightManager.ShaderParams parameter in Enum.GetValues(typeof (DayNightManager.ShaderParams)))
      Shader.SetGlobalFloat(this.getShaderValue(parameter), 0.0f);
  }

  public float timeOfDayNormalized => (float) ((double) this.timeOfDay % 24.0 / 24.0);

  public float isDay
  {
    get
    {
      return (double) this.timeOfDay < (double) this.dayStart || (double) this.timeOfDay >= (double) this.dayEnd ? 0.0f : 1f;
    }
  }

  public float dayNightBlend
  {
    get
    {
      return Mathf.Lerp(this.currentProfile.sunGradient.Evaluate(this.timeOfDayNormalized).a, this.newProfile.sunGradient.Evaluate(this.timeOfDayNormalized).a, this.profileBlend);
    }
  }

  private void Awake()
  {
    DayNightManager.instance = this;
    this.newProfile = this.currentProfile;
    this.clearAllShaderParams();
  }

  public void BlendProfiles(DayNightProfile profile)
  {
    this.StartCoroutine(blendRoutine());
    List<string> stringList1 = new List<string>();
    if (this.newProfile.globalShaderFloats != null)
    {
      for (int index = 0; index < this.newProfile.globalShaderFloats.Length; ++index)
      {
        this.allShaderFloats.Add(this.getShaderValue(this.newProfile.globalShaderFloats[index].parameter));
        stringList1.Add(this.getShaderValue(this.newProfile.globalShaderFloats[index].parameter));
      }
    }
    if (this.newProfile.animatedGlobalShaderFloats != null)
    {
      for (int index = 0; index < this.newProfile.animatedGlobalShaderFloats.Length; ++index)
      {
        this.allShaderFloats.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[index].parameter));
        stringList1.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[index].parameter));
      }
    }
    List<string> stringList2 = new List<string>();
    for (int index = 0; index < this.lastShaderFloats.Count; ++index)
    {
      if (!stringList1.Contains(this.lastShaderFloats[index]))
        stringList2.Add(this.lastShaderFloats[index]);
    }
    this.lastShaderFloats = stringList2;

    IEnumerator blendRoutine()
    {
      this.isBlending = true;
      float counter = 0.0f;
      this.newProfile = profile;
      while ((double) counter < 1.0)
      {
        for (int index = 0; index < this.lastShaderFloats.Count; ++index)
          Shader.SetGlobalFloat(this.lastShaderFloats[index], 1f - counter);
        if (this.newProfile.globalShaderFloats != null)
        {
          for (int index = 0; index < this.newProfile.globalShaderFloats.Length; ++index)
            Shader.SetGlobalFloat(this.getShaderValue(this.newProfile.globalShaderFloats[index].parameter), Mathf.Lerp(0.0f, this.newProfile.globalShaderFloats[index].paramValue, counter));
        }
        if (this.newProfile.animatedGlobalShaderFloats != null)
        {
          for (int index = 0; index < this.newProfile.animatedGlobalShaderFloats.Length; ++index)
            Shader.SetGlobalFloat(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[index].parameter), Mathf.Lerp(0.0f, this.newProfile.animatedGlobalShaderFloats[index].paramValue.Evaluate(this.timeOfDayNormalized), counter));
        }
        counter += Time.deltaTime * 0.5f;
        this.profileBlend = counter;
        yield return (object) null;
      }
      this.currentProfile = this.newProfile;
      this.profileBlend = 0.0f;
      this.lastShaderFloats.Clear();
      if (this.newProfile.globalShaderFloats != null)
      {
        for (int index = 0; index < this.newProfile.globalShaderFloats.Length; ++index)
          this.lastShaderFloats.Add(this.getShaderValue(this.newProfile.globalShaderFloats[index].parameter));
      }
      if (this.newProfile.animatedGlobalShaderFloats != null)
      {
        for (int index = 0; index < this.newProfile.animatedGlobalShaderFloats.Length; ++index)
          this.lastShaderFloats.Add(this.getShaderValue(this.newProfile.animatedGlobalShaderFloats[index].parameter));
      }
      this.isBlending = false;
    }
  }

  private void Start()
  {
    this.timeOfDay = this.startingTimeOfDay;
    this.UpdateCycle();
    this.photonView = this.GetComponent<PhotonView>();
    float num1 = (float) (((double) this.dayEnd - (double) this.dayStart) / 24.0);
    float num2 = 1f - num1;
    this.dayNightRatio = num1 / num2;
  }

  public void setTimeOfDay(float timeToSet)
  {
    if ((double) timeToSet > 48.0)
      this.timeOfDay = 48f;
    this.timeOfDay = timeToSet;
  }

  private void Update()
  {
    this.HazeDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.AirDistortion));
    this.RimFresnelDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.RimFresnel));
    this.LavaAlphaDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.LavaAlpha));
    this.BandFogDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.BandFog));
    this.SunSizeDebug = Shader.GetGlobalFloat(this.getShaderValue(DayNightManager.ShaderParams.SunSize));
    this.timeOfDay += (float) (1.0 / ((double) this.dayLengthInMinutes * 60.0) * (double) Time.deltaTime * 24.0);
    if ((double) this.timeOfDay > 24.0)
    {
      this.timeOfDay -= 24f;
      this.passedMidnight = true;
    }
    if (this.passedMidnight && (double) this.timeOfDay >= 5.5)
    {
      ++this.dayCount;
      this.passedMidnight = false;
    }
    if (PhotonNetwork.IsMasterClient)
    {
      this.syncTimer += Time.deltaTime;
      if ((double) this.syncTimer > 5.0)
      {
        this.photonView.RPC("RPCA_SyncTime", RpcTarget.All, (object) this.timeOfDay);
        this.syncTimer = 0.0f;
      }
    }
    this.UpdateCycle();
  }

  public string DayCountString()
  {
    return LocalizedText.GetText("DAY").Replace("#", DayNightManager.IntToNumberWord(DayNightManager.instance.dayCount) ?? "");
  }

  public string TimeOfDayString()
  {
    if ((double) this.timeOfDay >= 23.5)
      return "night";
    if ((double) this.timeOfDay >= 17.5)
      return "evening";
    if ((double) this.timeOfDay >= 11.5)
      return "afternoon";
    return (double) this.timeOfDay >= 5.5 ? "morning" : "night";
  }

  private static string IntToNumberWord(int x)
  {
    switch (x)
    {
      case 1:
        return LocalizedText.GetText("One");
      case 2:
        return LocalizedText.GetText("Two");
      case 3:
        return LocalizedText.GetText("Three");
      case 4:
        return LocalizedText.GetText("Four");
      case 5:
        return LocalizedText.GetText("Five");
      case 6:
        return LocalizedText.GetText("Six");
      case 7:
        return LocalizedText.GetText("Seven");
      case 8:
        return LocalizedText.GetText("Eight");
      case 9:
        return LocalizedText.GetText("Nine");
      case 10:
        return LocalizedText.GetText("Ten");
      default:
        return x.ToString() ?? "";
    }
  }

  public string FloatToTimeString(float time)
  {
    time = Mathf.Clamp(time, 0.0f, 24f);
    int num1 = Mathf.FloorToInt(time);
    int num2 = Mathf.FloorToInt((float) (((double) time - (double) num1) * 60.0));
    string str = num1 < 12 ? "AM" : "PM";
    return $"{(num1 % 12 == 0 ? 12 : num1 % 12):D2}:{num2:D2} {str}";
  }

  [PunRPC]
  public void RPCA_SyncTime(float time) => this.timeOfDay = time;

  private void OnValidate() => this.UpdateCycle();

  public void UpdateCycle()
  {
    this.timeString = this.FloatToTimeString(this.timeOfDay);
    float timeOfDayNormalized = this.timeOfDayNormalized;
    Vector3 euler1 = this.highNoonRotation + new Vector3(0.0f, 0.0f, this.angleOffsetZ.Evaluate(timeOfDayNormalized));
    float num1 = timeOfDayNormalized;
    if ((double) this.isDay < 0.5)
    {
      if ((double) num1 > (double) this.dayEnd / 24.0)
        num1 = (float) ((double) this.dayEnd / 24.0 - ((double) num1 - (double) this.dayEnd / 24.0) * (double) this.dayNightRatio);
      else if ((double) num1 < (double) this.dayStart / 24.0)
        num1 = (float) ((double) this.dayStart / 24.0 + ((double) this.dayStart / 24.0 - (double) num1) * (double) this.dayNightRatio);
    }
    Vector3 euler2 = new Vector3((float) (((double) num1 * (double) this.rotDir - 0.5) * 360.0), 0.0f, 0.0f);
    this.earth.transform.rotation = Quaternion.Euler(euler1) * Quaternion.Euler(euler2);
    Color color1 = Color.Lerp(Color.Lerp(this.currentProfile.sunGradient.Evaluate(timeOfDayNormalized), this.newProfile.sunGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialSunColor, this.specialDaySunBlend);
    Color color2 = Color.Lerp(Color.Lerp(this.currentProfile.skyTopGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyTopGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialTopColor, this.specialDaySkyBlend);
    Color color3 = Color.Lerp(Color.Lerp(this.currentProfile.skyMidGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyMidGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialMidColor, this.specialDaySkyBlend);
    Color color4 = Color.Lerp(Color.Lerp(this.currentProfile.skyBottomGradient.Evaluate(timeOfDayNormalized), this.newProfile.skyBottomGradient.Evaluate(timeOfDayNormalized), this.profileBlend), this.specialBottomColor, this.specialDaySkyBlend);
    Shader.SetGlobalColor(DayNightManager.SkyTopColor, color2);
    Shader.SetGlobalColor(DayNightManager.SkyMidColor, color3);
    Shader.SetGlobalColor(DayNightManager.SkyBottomColor, color4);
    Shader.SetGlobalFloat(DayNightManager.TIMEOFDAY, timeOfDayNormalized);
    Shader.SetGlobalFloat(DayNightManager.Name, this.isDay);
    Shader.SetGlobalFloat(DayNightManager.FOG, Mathf.Lerp(this.currentProfile.fogGradient.Evaluate(timeOfDayNormalized).r, this.newProfile.fogGradient.Evaluate(timeOfDayNormalized).r, this.profileBlend));
    this.sun.color = color1;
    this.moon.color = color1;
    if (!this.isBlending && this.currentProfile.animatedGlobalShaderFloats != null)
    {
      for (int index = 0; index < this.currentProfile.animatedGlobalShaderFloats.Length; ++index)
        Shader.SetGlobalFloat(this.getShaderValue(this.currentProfile.animatedGlobalShaderFloats[index].parameter), this.currentProfile.animatedGlobalShaderFloats[index].paramValue.Evaluate(timeOfDayNormalized));
    }
    float num2 = (float) -((double) this.snowstormWindFactor * 1.75 + (double) this.rainstormWindFactor * 1.25);
    float x = Mathf.Lerp(this.currentProfile.sunIntensity, this.newProfile.sunIntensity, this.profileBlend);
    this.sun.intensity = Mathf.Max(0.015f, (float) (((double) color1.a * 2.0 - 1.0) * 0.5) * x + num2);
    float num3 = Mathf.Lerp(this.currentProfile.sunIntensity, this.newProfile.sunIntensity, this.profileBlend);
    this.moon.intensity = Mathf.Max(0.015f, (float) ((1.0 - (double) color1.a * 2.0) * 0.5) * num3 + num2);
    this.lensFlare.intensity = (float) (((double) color1.a - 0.5) * 2.0) + num2;
    if ((double) this.specialDaySunBlend < 0.5)
    {
      if ((double) color1.a < 0.5)
      {
        this.sun.enabled = false;
        this.moon.enabled = true;
        Shader.SetGlobalInt(DayNightManager.IsDayReal, 0);
      }
      else
      {
        this.moon.enabled = false;
        this.sun.enabled = true;
        Shader.SetGlobalInt(DayNightManager.IsDayReal, 1);
      }
    }
    else
    {
      this.moon.enabled = false;
      this.sun.enabled = false;
    }
    this.sun.shadowStrength = math.saturate(x);
  }

  private void OnDisable()
  {
    for (int index = 0; index < this.allShaderFloats.Count; ++index)
      Shader.SetGlobalFloat(this.allShaderFloats[index], 0.0f);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawLine(this.earth.transform.position, this.sun.transform.position);
  }

  public enum ShaderParams
  {
    AirDistortion,
    RimFresnel,
    LavaAlpha,
    BandFog,
    SunSize,
  }

  [Serializable]
  public class ShaderParameters
  {
    public string paramName;
    public string paramValue;
  }
}
