// Decompiled with JetBrains decompiler
// Type: UniformModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI.ProceduralImage;

#nullable disable
[ModifierID("Uniform")]
public class UniformModifier : ProceduralImageModifier
{
  [SerializeField]
  private float radius;

  public float Radius
  {
    get => this.radius;
    set
    {
      this.radius = value;
      this._Graphic.SetVerticesDirty();
    }
  }

  public override Vector4 CalculateRadius(Rect imageRect)
  {
    double radius = (double) this.radius;
    return new Vector4((float) radius, (float) radius, (float) radius, (float) radius);
  }
}
