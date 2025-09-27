// Decompiled with JetBrains decompiler
// Type: AmbienceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class AmbienceManager : MonoBehaviour
{
  private static readonly int Maxfog = Shader.PropertyToID("MAXFOG");
  private static readonly int Usefog = Shader.PropertyToID("USEFOG");
  public Color ambienceColor;
  public Gradient ambienceGradient;
  public Color fogColor;
  public float brightness = 1f;
  public Material skyboxMaterial;
  public DayNightManager dayNight;
  public float maxFog = 500f;
  public bool useFog = true;
  public static AmbienceManager instance;

  private void Awake() => AmbienceManager.instance = this;

  private void OnValidate()
  {
    if (Application.isPlaying)
      return;
    this.Start();
  }

  private void UpdateFog()
  {
    if (Application.isPlaying)
      this.useFog = true;
    Shader.SetGlobalFloat(AmbienceManager.Usefog, this.useFog ? 1f : 0.0f);
    Shader.SetGlobalFloat(AmbienceManager.Maxfog, this.maxFog);
  }

  private void Start() => this.UpdateFog();

  private void Update()
  {
    this.UpdateFog();
    RenderSettings.skybox = this.skyboxMaterial;
    RenderSettings.fogColor = this.fogColor;
    if (!(bool) (Object) this.dayNight)
      return;
    Color color = this.ambienceGradient.Evaluate(this.dayNight.timeOfDayNormalized);
    RenderSettings.ambientLight = color * this.brightness * color.a;
  }

  public void ToggleFog()
  {
    this.useFog = !this.useFog;
    Shader.SetGlobalFloat(AmbienceManager.Usefog, this.useFog ? 1f : 0.0f);
  }
}
