// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.ProceduralImage.ProceduralImageInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.UI.ProceduralImage;

public struct ProceduralImageInfo(
  float width,
  float height,
  float fallOffDistance,
  float pixelSize,
  Vector4 radius,
  float borderWidth)
{
  public float width = Mathf.Abs(width);
  public float height = Mathf.Abs(height);
  public float fallOffDistance = Mathf.Max(0.0f, fallOffDistance);
  public Vector4 radius = radius;
  public float borderWidth = Mathf.Max(borderWidth, 0.0f);
  public float pixelSize = Mathf.Max(0.0f, pixelSize);
}
