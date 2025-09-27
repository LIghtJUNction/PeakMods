// Decompiled with JetBrains decompiler
// Type: BarAffliction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class BarAffliction : MonoBehaviour
{
  public RectTransform rtf;
  public Image icon;
  public float size;
  public CharacterAfflictions.STATUSTYPE afflictionType;

  public float width
  {
    get => this.rtf.sizeDelta.x;
    set => this.rtf.sizeDelta = new Vector2(value, this.rtf.sizeDelta.y);
  }

  public void OnEnable()
  {
    this.icon.transform.localScale = Vector3.zero;
    this.icon.transform.DOScale(1f, 0.5f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutElastic);
  }

  public void ChangeAffliction(StaminaBar bar)
  {
    if ((Object) Character.observedCharacter == (Object) null)
      return;
    float currentStatus = Character.observedCharacter.refs.afflictions.GetCurrentStatus(this.afflictionType);
    this.size = bar.fullBar.sizeDelta.x * currentStatus;
    if ((double) currentStatus > 0.0099999997764825821)
    {
      if ((double) this.size < (double) bar.minAfflictionWidth)
        this.size = bar.minAfflictionWidth;
      this.gameObject.SetActive(true);
    }
    else
      this.gameObject.SetActive(false);
  }

  public void UpdateAffliction(StaminaBar bar)
  {
    this.width = Mathf.Lerp(this.width, this.size, Mathf.Min(Time.deltaTime * 10f, 0.1f));
  }
}
