// Decompiled with JetBrains decompiler
// Type: EyeBlinkController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering.Universal;

#nullable disable
public class EyeBlinkController : MonoBehaviour
{
  private Character character;
  public UniversalRendererData rend;
  public Material eyeBlinkMaterial;
  public bool enableEyeBlink;
  public AnimationCurve openCurve;
  [Range(0.0f, 1f)]
  public float eyeOpenValue;
  private ScriptableRendererFeature rendererFeature;

  private void Start()
  {
    this.character = this.GetComponentInParent<Character>();
    if (!this.character.IsLocal)
    {
      this.enabled = false;
    }
    else
    {
      foreach (ScriptableRendererFeature rendererFeature in this.rend.rendererFeatures)
      {
        if (rendererFeature.name == "Eye Blink")
          this.rendererFeature = rendererFeature;
      }
      this.rendererFeature.SetActive(true);
      this.setEyeBlinkActive();
    }
  }

  private void setEyeBlinkActive()
  {
    if (!this.character.IsLocal)
      return;
    this.eyeBlinkMaterial.SetFloat("_EyeOpen", this.enableEyeBlink ? 1f : 0.0f);
    if (this.enableEyeBlink)
      return;
    this.rendererFeature.SetActive(false);
    this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
  }

  private void Update()
  {
    if (!this.character.IsLocal)
      return;
    if ((double) this.character.data.passedOutOnTheBeach > 0.0)
    {
      this.eyeOpenValue = 0.0f;
      this.enableEyeBlink = true;
    }
    else
    {
      this.eyeOpenValue = Mathf.MoveTowards(this.eyeOpenValue, 1f, Time.deltaTime * 0.15f);
      if ((double) this.eyeOpenValue >= 0.99900001287460327)
      {
        this.enableEyeBlink = false;
        this.rendererFeature.SetActive(false);
      }
      else
        this.enableEyeBlink = true;
    }
    if (!this.enableEyeBlink)
      return;
    this.rendererFeature.SetActive(true);
    this.eyeBlinkMaterial.SetFloat("_EyeOpen", Mathf.Clamp01(this.openCurve.Evaluate(this.eyeOpenValue)));
  }

  private void OnDisable()
  {
    if ((Object) this.rendererFeature != (Object) null)
      this.rendererFeature.SetActive(false);
    this.eyeBlinkMaterial.SetFloat("_EyeOpen", 1f);
  }
}
