// Decompiled with JetBrains decompiler
// Type: TerrainSplat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

#nullable disable
[ExecuteInEditMode]
public class TerrainSplat : MonoBehaviour
{
  public float TerrainTriplanarScale = 0.2f;
  public Texture2D TerrainTextureR;
  public Color TerrainColorR;
  public Vector2 TerrainSmoothR = new Vector2(0.0f, 1f);
  public Texture2D TerrainTextureG;
  public Color TerrainColorG;
  public Vector2 TerrainSmoothG = new Vector2(0.0f, 1f);
  public Texture2D TerrainTextureB;
  public Color TerrainColorB;
  public Vector2 TerrainSmoothB = new Vector2(0.0f, 1f);
  public Texture2D TerrainTextureA;
  public Color TerrainColorA;
  public Vector2 TerrainSmoothA = new Vector2(0.0f, 1f);
  public int splatRess;
  public TerrainSplat.SplatColor baseColor;
  public bool displayBrushes;
  public Texture2D splatMap;
  public Texture2D heightMap;
  public Texture2D detailMap;
  private Bounds bounds;
  private Color[,] splatColors;
  private Color[,] detailColors;
  private Color[,] heights;

  private void SetTerrainVariables()
  {
    Shader.SetGlobalFloat("TerrainTriplanarScale", this.TerrainTriplanarScale);
    Shader.SetGlobalTexture("TerrainTextureR", (Texture) this.TerrainTextureR);
    Shader.SetGlobalColor("TerrainColorR", this.TerrainColorR.linear);
    Shader.SetGlobalVector("TerrainSmoothR", (Vector4) this.TerrainSmoothR);
    Shader.SetGlobalTexture("TerrainTextureG", (Texture) this.TerrainTextureG);
    Shader.SetGlobalColor("TerrainColorG", this.TerrainColorG.linear);
    Shader.SetGlobalVector("TerrainSmoothG", (Vector4) this.TerrainSmoothG);
    Shader.SetGlobalTexture("TerrainTextureB", (Texture) this.TerrainTextureB);
    Shader.SetGlobalColor("TerrainColorB", this.TerrainColorB.linear);
    Shader.SetGlobalVector("TerrainSmoothB", (Vector4) this.TerrainSmoothB);
    Shader.SetGlobalTexture("TerrainTextureA", (Texture) this.TerrainTextureA);
    Shader.SetGlobalColor("TerrainColorA", this.TerrainColorA.linear);
    Shader.SetGlobalVector("TerrainSmoothA", (Vector4) this.TerrainSmoothA);
  }

  private void Start() => this.Generate(TerrainBrush.BrushType.All);

  private void GenerateAll() => this.Generate(TerrainBrush.BrushType.All);

  public void Generate(TerrainBrush.BrushType brushType)
  {
    this.SetTerrainVariables();
    this.GetBounds();
    if (brushType == TerrainBrush.BrushType.All)
    {
      this.SampleHeightMap();
      this.CreateHeighMap();
    }
    this.CreateColorData(brushType);
    this.ApplyBrushes(brushType);
    if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
      this.splatMap = this.CreateTexture(this.splatMap, this.splatColors);
    if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
      this.detailMap = this.CreateTexture(this.detailMap, this.detailColors);
    this.SetShaderData(brushType);
  }

  private void SampleHeightMap()
  {
    this.heights = new Color[this.splatRess, this.splatRess];
    for (int x = 0; x < this.splatRess; ++x)
    {
      for (int y = 0; y < this.splatRess; ++y)
        this.heights[x, y] = this.SampleHeight(x, y);
    }
  }

  private Color SampleHeight(int x, int y)
  {
    return new Color(HelperFunctions.GetGroundPos(this.GetPosFromIndex(x, y) + Vector3.up * 1000f, HelperFunctions.LayerType.Terrain).y / 10f, 0.0f, 0.0f, 0.0f);
  }

  private void CreateHeighMap()
  {
    if ((bool) (Object) this.heightMap)
      Object.DestroyImmediate((Object) this.heightMap);
    this.heightMap = new Texture2D(this.splatRess, this.splatRess, TextureFormat.RFloat, 0, true);
    this.heightMap.filterMode = FilterMode.Bilinear;
    this.heightMap.wrapMode = TextureWrapMode.Clamp;
    this.heightMap.SetPixels(HelperFunctions.GridToFlatArray<Color>(this.heights));
    this.heightMap.Apply();
  }

  private void SetShaderData(TerrainBrush.BrushType brushType)
  {
    if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Detail)
      Shader.SetGlobalTexture("TerrainDetail", (Texture) this.detailMap);
    if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
      Shader.SetGlobalTexture(nameof (TerrainSplat), (Texture) this.splatMap);
    if (brushType == TerrainBrush.BrushType.All)
      Shader.SetGlobalTexture("TerrainHeight", (Texture) this.heightMap);
    Shader.SetGlobalVector("TerrainCenter", (Vector4) this.bounds.center);
    Shader.SetGlobalVector("TerrainSize", (Vector4) this.bounds.size);
  }

  private void OnDestroy()
  {
    if (!(bool) (Object) this.splatMap)
      return;
    Object.DestroyImmediate((Object) this.splatMap);
  }

  private void CreateColorData(TerrainBrush.BrushType brushType)
  {
    if (brushType == TerrainBrush.BrushType.All || brushType == TerrainBrush.BrushType.Splat)
    {
      this.splatColors = new Color[this.splatRess, this.splatRess];
      for (int index1 = 0; index1 < this.splatRess; ++index1)
      {
        for (int index2 = 0; index2 < this.splatRess; ++index2)
          this.splatColors[index1, index2] = TerrainSplat.GetColor(this.baseColor);
      }
    }
    if (brushType != TerrainBrush.BrushType.All && brushType != TerrainBrush.BrushType.Detail)
      return;
    this.detailColors = new Color[this.splatRess, this.splatRess];
    for (int index3 = 0; index3 < this.splatRess; ++index3)
    {
      for (int index4 = 0; index4 < this.splatRess; ++index4)
        this.detailColors[index3, index4] = new Color(0.5f, 0.5f, 0.5f, 0.0f);
    }
  }

  private void ApplyBrushes(TerrainBrush.BrushType brushType)
  {
    foreach (TerrainBrush terrainBrush in HelperFunctions.SortBySiblingIndex<TerrainBrush>((IEnumerable<TerrainBrush>) Object.FindObjectsByType<TerrainBrush>(FindObjectsSortMode.InstanceID)).ToArray<TerrainBrush>())
    {
      if (brushType == TerrainBrush.BrushType.All || brushType == terrainBrush.brushType)
        this.ApplySplatBrush(terrainBrush);
    }
  }

  private void ApplySplatBrush(TerrainBrush item)
  {
    if (item.brushType == TerrainBrush.BrushType.Splat)
      item.ApplySplatData(this.splatColors, this.bounds);
    else
      item.ApplySplatData(this.detailColors, this.bounds);
  }

  private void GetBounds()
  {
    this.bounds = HelperFunctions.GetTotalBounds((IEnumerable<Renderer>) HelperFunctions.GetComponentListFromComponentArray<TerrainSplatMesh, Renderer>((IEnumerable<TerrainSplatMesh>) Object.FindObjectsByType<TerrainSplatMesh>(FindObjectsSortMode.None)).ToArray());
  }

  private Texture2D CreateTexture(Texture2D texture, Color[,] colors)
  {
    if ((bool) (Object) texture)
      Object.DestroyImmediate((Object) texture);
    texture = new Texture2D(this.splatRess, this.splatRess, DefaultFormat.LDR, TextureCreationFlags.None);
    texture.filterMode = FilterMode.Bilinear;
    texture.wrapMode = TextureWrapMode.Clamp;
    texture.SetPixels(HelperFunctions.GridToFlatArray<Color>(colors));
    texture.Apply();
    return texture;
  }

  private Vector3 GetPosFromIndex(int x, int y)
  {
    return this.GetPos((float) x / ((float) this.splatRess - 1f), (float) y / ((float) this.splatRess - 1f));
  }

  private Vector3 GetPos(float pX, float pY)
  {
    return this.bounds.center + Vector3.right * this.bounds.size.x * Mathf.Lerp(-0.5f, 0.5f, pX) + Vector3.forward * this.bounds.size.z * Mathf.Lerp(-0.5f, 0.5f, pY);
  }

  internal static Color GetColor(TerrainSplat.SplatColor color)
  {
    switch (color)
    {
      case TerrainSplat.SplatColor.Black:
        return new Color(0.0f, 0.0f, 0.0f, 0.0f);
      case TerrainSplat.SplatColor.Red:
        return new Color(1f, 0.0f, 0.0f, 0.0f);
      case TerrainSplat.SplatColor.Green:
        return new Color(0.0f, 1f, 0.0f, 0.0f);
      case TerrainSplat.SplatColor.Blue:
        return new Color(0.0f, 0.0f, 1f, 0.0f);
      case TerrainSplat.SplatColor.Alpha:
        return new Color(0.0f, 0.0f, 0.0f, 1f);
      case TerrainSplat.SplatColor.HalfRed:
        return new Color(0.5f, 0.0f, 0.0f, 0.0f);
      case TerrainSplat.SplatColor.HalfGreen:
        return new Color(0.0f, 0.5f, 0.0f, 0.0f);
      case TerrainSplat.SplatColor.HalfBlue:
        return new Color(0.0f, 0.0f, 0.5f, 0.0f);
      default:
        return new Color(0.0f, 0.0f, 0.0f, 0.5f);
    }
  }

  internal Color GetSplatPixelAtWorldPos(Vector3 point)
  {
    float num1 = Mathf.InverseLerp(this.bounds.min.x, this.bounds.max.x, point.x);
    float num2 = Mathf.InverseLerp(this.bounds.min.z, this.bounds.max.z, point.z);
    Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(num1 * (float) this.splatMap.width), Mathf.RoundToInt(num2 * (float) this.splatMap.height));
    return this.splatMap.GetPixel(vector2Int.x, vector2Int.y);
  }

  public enum SplatColor
  {
    Black,
    Red,
    Green,
    Blue,
    Alpha,
    HalfRed,
    HalfGreen,
    HalfBlue,
    HalfAlpha,
  }
}
