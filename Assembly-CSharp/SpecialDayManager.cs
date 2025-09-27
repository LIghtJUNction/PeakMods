// Decompiled with JetBrains decompiler
// Type: SpecialDayManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SpecialDayManager : MonoBehaviour
{
  public SpecialDayZone[] zones;
  public float debug;
  private float startFog;
  public float debugblend;

  private void Start()
  {
    this.zones = Object.FindObjectsByType<SpecialDayZone>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    this.startFog = AmbienceManager.instance.maxFog;
  }

  private void Update()
  {
    float a1 = 0.0f;
    if (!(bool) (Object) Character.observedCharacter)
      return;
    for (int index1 = 0; index1 < this.zones.Length; ++index1)
    {
      if (this.zones[index1].outerBounds.Contains(Character.observedCharacter.Center))
      {
        Color specialSunColor = DayNightManager.instance.specialSunColor;
        Color specialTopColor = DayNightManager.instance.specialTopColor;
        Color specialMidColor = DayNightManager.instance.specialMidColor;
        Color specialBottomColor = DayNightManager.instance.specialBottomColor;
        float maxFog = AmbienceManager.instance.maxFog;
        float num1 = (float) (1.0 - (double) (Vector3.Distance(Character.observedCharacter.Center, this.zones[index1].bounds.ClosestPoint(Character.observedCharacter.Center)) / this.zones[index1].blendSize) * 2.0);
        this.debugblend = num1;
        float num2 = Mathf.Clamp01(num1);
        if (this.zones[index1].overrideSun)
        {
          float num3 = Mathf.Lerp(this.zones[index1].nightLightIntensity, this.zones[index1].daylLightIntensity, DayNightManager.instance.dayNightBlend);
          DayNightManager.instance.specialSunColor = Color.Lerp(specialSunColor, this.zones[index1].specialSunColor * num3, num2);
        }
        if (this.zones[index1].useCustomSun)
        {
          DayNightManager.instance.specialDaySunBlend = Mathf.Max(a1, num2);
          if ((Object) this.zones[index1].specialLight != (Object) null)
          {
            this.zones[index1].specialLight.enabled = true;
            Color color = Color.Lerp(specialSunColor, this.zones[index1].specialSunColor, num2) * num2;
            this.zones[index1].specialLight.color = color;
            DayNightManager.instance.specialSunColor *= 1f - num2;
          }
        }
        Shader.SetGlobalFloat("SpecialDayBlend", Mathf.Lerp(0.0f, 1f, num2));
        if (this.zones[index1].useCustomColorVals)
        {
          DayNightManager.instance.specialDaySkyBlend = Mathf.Max(a1, num2);
          DayNightManager.instance.specialTopColor = Color.Lerp(specialTopColor, this.zones[index1].specialTopColor, num2);
          DayNightManager.instance.specialMidColor = Color.Lerp(specialMidColor, this.zones[index1].specialMidColor, num2);
          DayNightManager.instance.specialBottomColor = Color.Lerp(specialBottomColor, this.zones[index1].specialBottomColor, num2);
        }
        if (this.zones[index1].overrideFog)
        {
          float num4 = Mathf.Lerp(this.startFog, Mathf.Lerp(maxFog, this.zones[index1].fogDensity, num2), Mathf.Max(a1, num2));
          AmbienceManager.instance.maxFog = num4;
        }
        if (this.zones[index1].globalShaderVals.Length != 0)
        {
          float a2 = 0.0f;
          for (int index2 = 0; index2 < this.zones[index1].globalShaderVals.Length; ++index2)
          {
            float num5 = Mathf.Lerp(a2, this.zones[index1].globalShaderVals[index2].value, num2) * Mathf.Max(a1, num2);
            Shader.SetGlobalFloat(DayNightManager.instance.getShaderValue(this.zones[index1].globalShaderVals[index2].parameter), num5);
            a2 = num5;
          }
        }
        a1 = num2;
      }
      else if ((Object) this.zones[index1].specialLight != (Object) null)
        this.zones[index1].specialLight.enabled = false;
    }
  }

  private void OnDisable()
  {
    Shader.SetGlobalFloat("SpecialDayBlend", 0.0f);
    for (int index1 = 0; index1 < this.zones.Length; ++index1)
    {
      if (this.zones[index1].globalShaderVals.Length != 0)
      {
        for (int index2 = 0; index2 < this.zones[index1].globalShaderVals.Length; ++index2)
          Shader.SetGlobalFloat(DayNightManager.instance.getShaderValue(this.zones[index1].globalShaderVals[index2].parameter), 0.0f);
      }
    }
  }
}
