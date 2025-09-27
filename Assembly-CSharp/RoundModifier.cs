// Decompiled with JetBrains decompiler
// Type: RoundModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI.ProceduralImage;

#nullable disable
[ModifierID("Round")]
public class RoundModifier : ProceduralImageModifier
{
  public override Vector4 CalculateRadius(Rect imageRect)
  {
    double num = (double) Mathf.Min(imageRect.width, imageRect.height) * 0.5;
    return new Vector4((float) num, (float) num, (float) num, (float) num);
  }
}
