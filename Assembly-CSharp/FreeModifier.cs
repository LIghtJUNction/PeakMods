// Decompiled with JetBrains decompiler
// Type: FreeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI.ProceduralImage;

#nullable disable
[ModifierID("Free")]
public class FreeModifier : ProceduralImageModifier
{
  [SerializeField]
  private Vector4 radius;

  public Vector4 Radius
  {
    get => this.radius;
    set
    {
      this.radius = value;
      this._Graphic.SetVerticesDirty();
    }
  }

  public override Vector4 CalculateRadius(Rect imageRect) => this.radius;

  protected void OnValidate()
  {
    this.radius.x = Mathf.Max(0.0f, this.radius.x);
    this.radius.y = Mathf.Max(0.0f, this.radius.y);
    this.radius.z = Mathf.Max(0.0f, this.radius.z);
    this.radius.w = Mathf.Max(0.0f, this.radius.w);
  }
}
