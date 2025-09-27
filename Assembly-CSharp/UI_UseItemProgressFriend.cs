// Decompiled with JetBrains decompiler
// Type: UI_UseItemProgressFriend
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class UI_UseItemProgressFriend : MonoBehaviour
{
  public RectTransform rect;
  public Image fill;
  public RawImage icon;
  public int giverID;
  private float _maxTime;
  private float _currentTime;
  private bool _dead;

  public void Init(FeedData feedData)
  {
    this.giverID = feedData.giverID;
    this._maxTime = feedData.totalItemTime;
    Item obj;
    if (ItemDatabase.TryGetItem(feedData.itemID, out obj))
      this.icon.texture = (Texture) obj.UIData.GetIcon();
    Vector2 sizeDelta = this.rect.sizeDelta;
    this.rect.sizeDelta = Vector2.zero;
    this.rect.DOSizeDelta(sizeDelta, 0.5f).SetEase<TweenerCore<Vector2, Vector2, VectorOptions>>(Ease.OutBack);
  }

  private void Update()
  {
    if (this._dead)
      return;
    this._currentTime += Time.deltaTime;
    this.fill.fillAmount = this._currentTime / this._maxTime;
  }

  public void Kill()
  {
    this._dead = true;
    Object.Destroy((Object) this.gameObject);
  }
}
