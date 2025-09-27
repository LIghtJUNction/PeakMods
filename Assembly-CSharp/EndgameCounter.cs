// Decompiled with JetBrains decompiler
// Type: EndgameCounter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

#nullable disable
public class EndgameCounter : MonoBehaviour
{
  public CanvasGroup counterGroup;
  public CanvasGroup winGroup;
  public CanvasGroup loseGroup;
  public TextMeshProUGUI counter;

  public void UpdateCounter(int value)
  {
    this.counterGroup.gameObject.SetActive(true);
    this.counterGroup.DOFade(1f, 0.25f);
    this.counter.text = value.ToString() ?? "";
    this.counter.transform.localScale = Vector3.one * 2f;
    this.counter.alpha = 0.0f;
    this.counter.DOScale(1f, 0.25f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutCubic);
    ShortcutExtensionsTMPText.DOFade(this.counter, 1f, 0.25f).SetEase<TweenerCore<Color, Color, ColorOptions>>(Ease.OutCubic);
  }

  public void Win()
  {
    this.winGroup.gameObject.SetActive(true);
    this.winGroup.alpha = 0.0f;
    this.winGroup.DOFade(1f, 1f);
  }

  public void Lose()
  {
    this.loseGroup.gameObject.SetActive(true);
    this.loseGroup.alpha = 0.0f;
    this.loseGroup.DOFade(1f, 1f);
  }

  public void Disable() => this.counterGroup.gameObject.SetActive(false);
}
