// Decompiled with JetBrains decompiler
// Type: OnlyOneEdgeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI.ProceduralImage;

#nullable disable
[ModifierID("Only One Edge")]
public class OnlyOneEdgeModifier : ProceduralImageModifier
{
  [SerializeField]
  private float radius;
  [SerializeField]
  private OnlyOneEdgeModifier.ProceduralImageEdge side;

  public float Radius
  {
    get => this.radius;
    set
    {
      this.radius = value;
      this._Graphic.SetVerticesDirty();
    }
  }

  public OnlyOneEdgeModifier.ProceduralImageEdge Side
  {
    get => this.side;
    set => this.side = value;
  }

  public override Vector4 CalculateRadius(Rect imageRect)
  {
    switch (this.side)
    {
      case OnlyOneEdgeModifier.ProceduralImageEdge.Top:
        return new Vector4(this.radius, this.radius, 0.0f, 0.0f);
      case OnlyOneEdgeModifier.ProceduralImageEdge.Bottom:
        return new Vector4(0.0f, 0.0f, this.radius, this.radius);
      case OnlyOneEdgeModifier.ProceduralImageEdge.Left:
        return new Vector4(this.radius, 0.0f, 0.0f, this.radius);
      case OnlyOneEdgeModifier.ProceduralImageEdge.Right:
        return new Vector4(0.0f, this.radius, this.radius, 0.0f);
      default:
        return new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    }
  }

  public enum ProceduralImageEdge
  {
    Top,
    Bottom,
    Left,
    Right,
  }
}
