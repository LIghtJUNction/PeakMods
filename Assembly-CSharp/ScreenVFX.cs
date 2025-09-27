// Decompiled with JetBrains decompiler
// Type: ScreenVFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#nullable disable
public class ScreenVFX : MonoBehaviour
{
  private static readonly int INTENSITY = Shader.PropertyToID("_Intensity");
  public Renderer renderer;
  public float sequenceInitialDuration = 0.25f;
  public float duration = 0.5f;
  public float delay = 0.25f;
  private Tweener tween;
  private DG.Tweening.Sequence sequence;

  private void Start()
  {
    if (!GUIManager.instance.photosensitivity)
      return;
    this.duration *= 2f;
  }

  public void Test() => this.Play(1f);

  public void Play(float amount)
  {
    this.gameObject.SetActive(true);
    amount = 1f;
    if (GUIManager.instance.photosensitivity)
      amount *= 0.3f;
    if (this.tween != null)
      this.tween.Kill();
    if (GUIManager.instance.photosensitivity)
    {
      if (this.sequence != null)
        this.sequence.Kill();
      this.sequence = DOTween.Sequence();
      this.sequence.Append((Tween) this.renderer.material.DOFloat(amount, ScreenVFX.INTENSITY, this.sequenceInitialDuration)).Append((Tween) this.renderer.material.DOFloat(0.0f, ScreenVFX.INTENSITY, this.duration)).SetDelay<DG.Tweening.Sequence>(this.delay).OnComplete<DG.Tweening.Sequence>(new TweenCallback(this.Disable));
    }
    else
    {
      this.renderer.material.SetFloat(ScreenVFX.INTENSITY, amount);
      this.tween = (Tweener) this.renderer.material.DOFloat(0.0f, ScreenVFX.INTENSITY, this.duration).SetDelay<TweenerCore<float, float, FloatOptions>>(this.delay).OnComplete<TweenerCore<float, float, FloatOptions>>(new TweenCallback(this.Disable));
    }
  }

  public void StartFX(float photosensitive = 0.5f)
  {
    this.gameObject.SetActive(true);
    float endValue = 1f;
    double duration = (double) this.duration;
    if (GUIManager.instance.photosensitivity)
      endValue *= photosensitive;
    this.renderer.material.SetFloat(ScreenVFX.INTENSITY, 0.0f);
    this.renderer.material.DOFloat(endValue, ScreenVFX.INTENSITY, this.duration);
  }

  public void EndFX()
  {
    this.renderer.material.DOFloat(0.0f, ScreenVFX.INTENSITY, this.duration).OnComplete<TweenerCore<float, float, FloatOptions>>(new TweenCallback(this.Disable));
  }

  private void Disable() => this.gameObject.SetActive(false);
}
