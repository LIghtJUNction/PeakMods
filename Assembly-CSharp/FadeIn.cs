// Decompiled with JetBrains decompiler
// Type: FadeIn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class FadeIn : MonoBehaviour
{
  public Image fade;

  private void Awake()
  {
    this.fade.color = this.fade.color with { a = 1f };
    DOTweenModuleUI.DOFade(this.fade, 0.0f, 2f).OnComplete<TweenerCore<Color, Color, ColorOptions>>(new TweenCallback(this.Disable));
  }

  private void Disable() => this.gameObject.SetActive(false);
}
