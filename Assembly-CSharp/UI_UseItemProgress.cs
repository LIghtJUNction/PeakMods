// Decompiled with JetBrains decompiler
// Type: UI_UseItemProgress
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class UI_UseItemProgress : MonoBehaviour
{
  public Image fill;
  public Image empty;

  private bool constantUseInteractableExists
  {
    get => Interaction.instance.currentHeldInteractible != null;
  }

  private void Update()
  {
    if ((Object) Character.localCharacter == (Object) null)
      return;
    int num = (Object) Character.localCharacter.data.currentItem != (Object) null ? 1 : 0;
    bool flag = this.UpdateFillAmount();
    if (!this.fill.enabled & flag)
    {
      this.transform.DOKill();
      this.transform.localScale = Vector3.zero;
      this.transform.DOScale(1f, 0.25f).SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutBack);
    }
    this.fill.enabled = flag;
    this.empty.enabled = this.fill.enabled;
  }

  private bool UpdateFillAmount()
  {
    bool flag = (Object) Character.localCharacter.data.currentItem != (Object) null;
    if ((double) Character.localCharacter.refs.items.climbingSpikeCastProgress > 0.0)
    {
      this.fill.fillAmount = Character.localCharacter.refs.items.climbingSpikeCastProgress;
      return true;
    }
    if (flag && Character.localCharacter.data.currentItem.shouldShowCastProgress)
    {
      float progress = Character.localCharacter.data.currentItem.progress;
      if ((double) progress > 0.0)
      {
        this.fill.fillAmount = progress;
        return true;
      }
    }
    else if (this.constantUseInteractableExists && (double) Interaction.instance.constantInteractableProgress > 0.0)
    {
      this.fill.fillAmount = Interaction.instance.constantInteractableProgress;
      return true;
    }
    return false;
  }
}
