// Decompiled with JetBrains decompiler
// Type: GuidebookSpread
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
public class GuidebookSpread : MonoBehaviour
{
  public TextMeshProUGUI pageNumberLeft;
  public TextMeshProUGUI pageNumberRight;
  public RectTransform pageLeftTransform;
  public RectTransform pageRightTransform;
  public float page1AlignmentLeft;
  public float page1AlignmentRight;
  public float page1AlignmentTop;
  public float page1AlignmentBottom;

  internal void SetPageLeft(RectTransform prefab)
  {
    if ((Object) this.pageLeftTransform != (Object) null)
      Object.DestroyImmediate((Object) this.pageLeftTransform.gameObject);
    this.pageLeftTransform = Object.Instantiate<RectTransform>(prefab, this.transform);
    this.pageLeftTransform.offsetMax = new Vector2(-this.page1AlignmentRight, -this.page1AlignmentTop);
    this.pageLeftTransform.offsetMin = new Vector2(this.page1AlignmentLeft, this.page1AlignmentBottom);
  }

  internal void SetPageRight(RectTransform prefab)
  {
    if ((Object) this.pageRightTransform != (Object) null)
      Object.DestroyImmediate((Object) this.pageRightTransform.gameObject);
    this.pageRightTransform = Object.Instantiate<RectTransform>(prefab, this.transform);
    this.pageRightTransform.offsetMax = new Vector2(-this.page1AlignmentLeft, -this.page1AlignmentTop);
    this.pageRightTransform.offsetMin = new Vector2(this.page1AlignmentRight, this.page1AlignmentTop);
  }

  internal void ClearContents()
  {
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) this.transform.GetChild(index).gameObject);
  }
}
