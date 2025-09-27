// Decompiled with JetBrains decompiler
// Type: DynamicGridCellSizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
[ExecuteAlways]
[RequireComponent(typeof (GridLayoutGroup))]
public class DynamicGridCellSizer : MonoBehaviour
{
  public RectTransform gridRectTransform;
  public int iconCount;
  public int maxIconsPerRow = 8;
  private GridLayoutGroup grid;
  private int childCount = -1;

  private void Awake() => this.grid = this.GetComponent<GridLayoutGroup>();

  private void Update()
  {
    if (this.transform.childCount == this.childCount)
      return;
    this.childCount = this.transform.childCount;
    this.ResizeCells();
  }

  public void ResizeCells()
  {
    this.iconCount = this.grid.transform.childCount;
    Rect rect = this.gridRectTransform.rect;
    double width = (double) rect.width;
    rect = this.gridRectTransform.rect;
    float height = rect.height;
    int num1 = Mathf.Max(1, Mathf.CeilToInt((float) this.iconCount / (float) this.maxIconsPerRow));
    Debug.Log((object) ("Rows!" + num1.ToString()));
    int num2 = Mathf.CeilToInt((float) this.iconCount / (float) num1);
    double left = (double) this.grid.padding.left;
    float num3 = Mathf.Min((float) (width - left - (double) this.grid.padding.right - (double) this.grid.spacing.x * (double) (num2 - 1)) / (float) num2, (float) ((double) height - (double) this.grid.padding.top - (double) this.grid.padding.bottom - (double) this.grid.spacing.y * (double) (num1 - 1)) / (float) num1);
    this.grid.cellSize = new Vector2(num3, num3);
  }
}
