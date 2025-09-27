// Decompiled with JetBrains decompiler
// Type: LogoCombineSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#nullable disable
[ExecuteInEditMode]
public class LogoCombineSequence : MonoBehaviour
{
  public float streakAmount;
  public float stretchAmount;
  public float chromaticAmplitude;
  public float lensScale;
  public float lensIntensity;
  public float bloomIntensity;
  public Material material;
  public Volume volume;
  private ChromaticAberration chromaticAberration;
  private Bloom bloom;
  private LensDistortion lensDistortion;

  private void Start()
  {
    if (this.volume.profile.TryGet<ChromaticAberration>(out this.chromaticAberration))
      this.chromaticAberration.intensity.value = 0.0f;
    if (this.volume.profile.TryGet<Bloom>(out this.bloom))
      this.bloom.intensity.value = 0.0f;
    if (!this.volume.profile.TryGet<LensDistortion>(out this.lensDistortion))
      return;
    this.lensDistortion.intensity.value = 0.0f;
  }

  private void Update()
  {
    if ((Object) this.chromaticAberration != (Object) null)
      this.chromaticAberration.intensity.value = this.chromaticAmplitude;
    if ((Object) this.bloom != (Object) null)
      this.bloom.intensity.value = this.bloomIntensity;
    if ((Object) this.lensDistortion != (Object) null)
    {
      this.lensDistortion.intensity.value = this.lensIntensity;
      this.lensDistortion.scale.value = this.lensScale;
    }
    this.material.SetFloat("_StreakAmount", this.streakAmount);
    this.material.SetFloat("_StretchAmount", this.stretchAmount);
  }

  private void OnValidate()
  {
    if ((Object) this.bloom != (Object) null)
      this.bloom.intensity.value = this.bloomIntensity;
    if ((Object) this.chromaticAberration != (Object) null)
      this.chromaticAberration.intensity.value = this.chromaticAmplitude;
    if ((Object) this.lensDistortion != (Object) null)
    {
      this.lensDistortion.intensity.value = this.lensIntensity;
      this.lensDistortion.scale.value = this.lensScale;
    }
    this.material.SetFloat("_StreakAmount", this.streakAmount);
    this.material.SetFloat("_StretchAmount", this.stretchAmount);
  }
}
