// Decompiled with JetBrains decompiler
// Type: TextFogEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using TMPro;
using UnityEngine;

#nullable disable
public class TextFogEffect : MonoBehaviour
{
  public bool abs;
  public float amplitude = 0.2f;
  public float period = 0.5f;
  public float offset = 0.1f;
  public Gradient colorGradient;
  public bool skewXtop = true;
  public float skewX;
  public bool skewYtop = true;
  public float skewY;
  public bool roundSin;
  public float chunkiness = 1f;
  public float updateChance = 0.1f;
  public float shiftTime = 0.5f;
  protected TMP_Text m_TextComponent;
  protected TMP_TextInfo textInfo;
  public DOTweenTMPAnimator DTanimator;
  private bool destroyed;

  public virtual float colorSpeedMult => 1f;

  private void Awake()
  {
    this.m_TextComponent = this.GetComponent<TMP_Text>();
    this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
  }

  private void Start() => this.Init();

  private void OnEnable() => this.StartCoroutine(this.TextEffectRoutine());

  private IEnumerator TextEffectRoutine()
  {
    this.textInfo = this.m_TextComponent.textInfo;
    int characterCount = this.textInfo.characterCount;
    while (true)
    {
      this.UpdateCharacter(Random.Range(0, characterCount));
      yield return (object) new WaitForSeconds(this.period);
    }
  }

  public virtual void Init()
  {
    this.textInfo = this.m_TextComponent.textInfo;
    int characterCount = this.textInfo.characterCount;
  }

  private void TryDestroy()
  {
    this.destroyed = true;
    Object.Destroy((Object) this);
  }

  private void LateUpdate()
  {
    int num = this.destroyed ? 1 : 0;
  }

  protected virtual void EffectRoutine()
  {
    this.textInfo = this.m_TextComponent.textInfo;
    int characterCount = this.textInfo.characterCount;
  }

  public void UpdateCharacter(int index)
  {
    if ((double) this.period == 0.0)
      return;
    float num1 = this.offset * (float) index;
    float num2 = (float) (1.0 + (double) Mathf.Sin((Time.time + num1) / this.period) * (double) this.amplitude);
    if (this.roundSin)
    {
      float num3 = Mathf.Round(num2 * this.chunkiness) / this.chunkiness;
    }
    float amplitude = this.amplitude;
    this.DTanimator.DOOffsetChar(index, Random.insideUnitSphere * amplitude, this.shiftTime).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.InOutCubic);
    float time = (float) (((double) Mathf.Sin((float) (((double) Time.time + (double) num1) / ((double) this.period / (double) this.colorSpeedMult))) + 1.0) * 0.5);
    this.DTanimator.SetCharColor(index, (Color32) this.colorGradient.Evaluate(time));
  }
}
