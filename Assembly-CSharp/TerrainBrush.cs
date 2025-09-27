// Decompiled with JetBrains decompiler
// Type: TerrainBrush
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TerrainBrush : MonoBehaviour
{
  public TerrainBrush.BrushType brushType;
  public Texture2D texture;
  public TerrainSplat.SplatColor color;
  [Range(0.0f, 1f)]
  public float strength = 1f;
  public Color detailColor = new Color(1f, 1f, 1f, 1f);
  public Vector2 minMaxSlider = new Vector2(0.0f, 1f);
  private TerrainSplat splat;

  private void Start()
  {
  }

  public void Generate() => Object.FindAnyObjectByType<TerrainSplat>().Generate(this.brushType);

  private Bounds GetBounds()
  {
    Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
    bounds.Encapsulate(this.transform.position + this.transform.right * 0.5f * this.transform.localScale.x * 1.4f);
    bounds.Encapsulate(this.transform.position + this.transform.right * -0.5f * this.transform.localScale.x * 1.4f);
    bounds.Encapsulate(this.transform.position + this.transform.forward * 0.5f * this.transform.localScale.z * 1.4f);
    bounds.Encapsulate(this.transform.position + this.transform.forward * -0.5f * this.transform.localScale.z * 1.4f);
    return bounds;
  }

  private Vector3 GetPos(float pX, float pY)
  {
    return this.transform.position + this.transform.right * this.transform.localScale.x * Mathf.Lerp(-0.5f, 0.5f, pX) + this.transform.forward * this.transform.localScale.z * Mathf.Lerp(-0.5f, 0.5f, pY);
  }

  internal void ApplySplatData(Color[,] colors, Bounds totalBounds)
  {
    foreach (Vector2Int indexesInBound in HelperFunctions.GetIndexesInBounds(colors.GetLength(0), colors.GetLength(1), this.GetBounds(), totalBounds))
    {
      Vector3 worldPos = HelperFunctions.IDToWorldPos(indexesInBound.x, indexesInBound.y, colors.GetLength(0), colors.GetLength(1), totalBounds);
      colors[indexesInBound.x, indexesInBound.y] = this.brushType != TerrainBrush.BrushType.Splat ? this.SampleDetailColor(worldPos, colors[indexesInBound.x, indexesInBound.y]) : this.SampleSplatColor(worldPos, colors[indexesInBound.x, indexesInBound.y]);
    }
  }

  private Color SampleSplatColor(Vector3 pos, Color beforeColor)
  {
    float num = this.SampleMask(pos);
    Color color = Color.Lerp(beforeColor * 2f, TerrainSplat.GetColor(this.color) * 2f, num * this.strength);
    float magnitude = new Vector4(color.r, color.g, color.b, color.a).magnitude;
    return color / magnitude;
  }

  private Color SampleDetailColor(Vector3 pos, Color beforeColor)
  {
    float num = this.SampleMask(pos);
    Color detailColor = this.detailColor;
    detailColor.a *= num;
    Color color;
    if ((double) beforeColor.a <= 0.0099999997764825821)
    {
      color = detailColor;
    }
    else
    {
      float t = detailColor.a / beforeColor.a;
      color = Color.Lerp(beforeColor, detailColor, t) with
      {
        a = Mathf.Lerp(beforeColor.a, detailColor.a, t)
      };
    }
    return color;
  }

  private float SampleMask(Vector3 pos)
  {
    Vector3 vector3 = this.transform.InverseTransformPoint(pos);
    float num1 = Mathf.InverseLerp(-0.5f, 0.5f, vector3.x);
    float num2 = Mathf.InverseLerp(-0.5f, 0.5f, vector3.z);
    return Mathf.Clamp01(Mathf.InverseLerp(this.minMaxSlider.x, this.minMaxSlider.y, this.texture.GetPixel(Mathf.RoundToInt(num1 * (float) this.texture.width), Mathf.RoundToInt(num2 * (float) this.texture.height)).r));
  }

  public enum BrushType
  {
    Splat,
    Detail,
    All,
  }
}
