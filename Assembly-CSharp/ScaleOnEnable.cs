// Decompiled with JetBrains decompiler
// Type: ScaleOnEnable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#nullable disable
public class ScaleOnEnable : MonoBehaviour
{
  public float time = 0.25f;
  public Ease easeType = Ease.OutBounce;
  public CanvasGroup canvasGroup;

  private void OnEnable()
  {
    this.transform.localScale = Vector3.zero;
    this.transform.DOScale(Vector3.one, this.time).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(this.easeType);
    if (!(bool) (Object) this.canvasGroup)
      return;
    this.canvasGroup.alpha = 0.0f;
    this.canvasGroup.DOFade(1f, this.time).SetEase<TweenerCore<float, float, FloatOptions>>(this.easeType);
  }
}
