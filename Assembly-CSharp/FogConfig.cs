// Decompiled with JetBrains decompiler
// Type: FogConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class FogConfig : MonoBehaviour
{
  public static FogConfig currentFog;
  public float windSkyBrightnessValue = 0.2f;
  public float windTextureInfluence = 0.2f;
  public Color windTint = Color.white;
  public Texture windTexture;
  public float windSphereScale = 5f;
  public Vector2 windSpeed;
  public float windFogDensity = 50f;
  public float WindFogTextureDensity = 15f;
  public float windMixInfluence;
  public float maxVal = 1f;
  public bool straightDown;
  private float sinceSet = 10f;

  private void Start() => Shader.SetGlobalFloat("_WeatherBlend", 0.0f);

  private void Update()
  {
    this.sinceSet += Time.deltaTime;
    if (!((Object) FogConfig.currentFog == (Object) this) || (double) this.sinceSet <= 0.10000000149011612 || (double) this.sinceSet >= 10.0)
      return;
    float globalFloat = Shader.GetGlobalFloat("_WeatherBlend");
    if ((double) globalFloat <= 0.0)
      return;
    Shader.SetGlobalFloat("_WeatherBlend", Mathf.MoveTowards(globalFloat, 0.0f, Time.deltaTime * 0.3f));
  }

  public void SetFog()
  {
    FogConfig.currentFog = this;
    this.sinceSet = 0.0f;
    Shader.SetGlobalTexture("_WindTexture", this.windTexture);
    Shader.SetGlobalFloat("_WeatherBlend", Mathf.MoveTowards(Shader.GetGlobalFloat("_WeatherBlend"), this.maxVal, Time.deltaTime * 0.3f));
    Shader.SetGlobalColor("WindTint", this.windTint);
    Shader.SetGlobalFloat("WindSkyBrightnessValue", this.windSkyBrightnessValue);
    Shader.SetGlobalFloat("WindTextureInfluence", this.windTextureInfluence);
    Shader.SetGlobalFloat("WindFogDensity", this.windFogDensity);
    Shader.SetGlobalFloat("WindFogTextureDensity", this.WindFogTextureDensity);
    Shader.SetGlobalFloat("WindMixInfluence", this.windMixInfluence);
    Shader.SetGlobalVector("WindSpeed", new Vector4(this.windSpeed.x, this.windSpeed.y, 0.0f, 0.0f));
    Vector3 forward = this.transform.forward;
    Vector3 vector3 = -Vector3.Cross(Vector3.up, forward);
    float num = Vector3.Angle(Vector3.up, forward);
    if (this.straightDown)
    {
      vector3 = Vector3.forward;
      num = 180f;
    }
    Shader.SetGlobalVector("WindRotationAxis", new Vector4(vector3.x, vector3.y, vector3.z, 0.0f));
    Shader.SetGlobalFloat("WindRotationAngle", num);
    Shader.SetGlobalFloat("WindSphereScale", this.windSphereScale);
  }
}
