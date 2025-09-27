// Decompiled with JetBrains decompiler
// Type: HeightmapMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
[RequireComponent(typeof (MeshFilter), typeof (MeshRenderer))]
public class HeightmapMesh : MonoBehaviour
{
  [Tooltip("World-space distance between adjacent grid points.")]
  public float cellSize = 1f;
  [Tooltip("If true, the mesh is centered around the GameObject origin.")]
  public bool center;
  private Mesh _mesh;

  public void Generate(float[,] heights)
  {
    if (heights == null)
    {
      Debug.LogError((object) "heights is null");
    }
    else
    {
      int length1 = heights.GetLength(0);
      int length2 = heights.GetLength(1);
      if (length2 < 2 || length1 < 2)
      {
        Debug.LogError((object) "heights must be at least 2x2.");
      }
      else
      {
        if ((Object) this._mesh == (Object) null)
        {
          this._mesh = new Mesh();
          this._mesh.name = "Heightmap Mesh";
        }
        else
          this._mesh.Clear();
        int length3 = length2 * length1;
        this._mesh.indexFormat = length3 <= (int) ushort.MaxValue ? IndexFormat.UInt16 : IndexFormat.UInt32;
        Vector3[] inVertices = new Vector3[length3];
        Vector2[] uvs = new Vector2[length3];
        int[] triangles = new int[(length2 - 1) * (length1 - 1) * 6];
        Vector3 vector3 = Vector3.zero;
        if (this.center)
          vector3 = new Vector3((float) (-(double) ((float) (length2 - 1) * this.cellSize) * 0.5), 0.0f, (float) (-(double) ((float) (length1 - 1) * this.cellSize) * 0.5));
        for (int index1 = 0; index1 < length1; ++index1)
        {
          for (int index2 = 0; index2 < length2; ++index2)
          {
            int index3 = index1 * length2 + index2;
            float height = heights[index1, index2];
            inVertices[index3] = new Vector3((float) index2 * this.cellSize, height, (float) index1 * this.cellSize) + vector3;
            float x = length2 == 1 ? 0.0f : (float) index2 / (float) (length2 - 1);
            float y = length1 == 1 ? 0.0f : (float) index1 / (float) (length1 - 1);
            uvs[index3] = new Vector2(x, y);
          }
        }
        int num1 = 0;
        for (int index4 = 0; index4 < length1 - 1; ++index4)
        {
          for (int index5 = 0; index5 < length2 - 1; ++index5)
          {
            int num2 = index4 * length2 + index5;
            int num3 = num2 + 1;
            int num4 = num2 + length2;
            int num5 = num4 + 1;
            int[] numArray1 = triangles;
            int index6 = num1;
            int num6 = index6 + 1;
            int num7 = num2;
            numArray1[index6] = num7;
            int[] numArray2 = triangles;
            int index7 = num6;
            int num8 = index7 + 1;
            int num9 = num4;
            numArray2[index7] = num9;
            int[] numArray3 = triangles;
            int index8 = num8;
            int num10 = index8 + 1;
            int num11 = num3;
            numArray3[index8] = num11;
            int[] numArray4 = triangles;
            int index9 = num10;
            int num12 = index9 + 1;
            int num13 = num3;
            numArray4[index9] = num13;
            int[] numArray5 = triangles;
            int index10 = num12;
            int num14 = index10 + 1;
            int num15 = num4;
            numArray5[index10] = num15;
            int[] numArray6 = triangles;
            int index11 = num14;
            num1 = index11 + 1;
            int num16 = num5;
            numArray6[index11] = num16;
          }
        }
        this._mesh.SetVertices(inVertices);
        this._mesh.SetUVs(0, uvs);
        this._mesh.SetTriangles(triangles, 0, true);
        this._mesh.RecalculateNormals();
        this._mesh.RecalculateBounds();
        this.GetComponent<MeshFilter>().sharedMesh = this._mesh;
      }
    }
  }

  [ContextMenu("Generate Test Mesh (Perlin)")]
  private void GenerateTest()
  {
    int length1 = 129;
    int length2 = 129;
    float[,] heights = new float[length2, length1];
    for (int index1 = 0; index1 < length2; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
        heights[index1, index2] = Mathf.PerlinNoise((float) index2 * 0.05f, (float) index1 * 0.05f) * 2f;
    }
    this.Generate(heights);
  }
}
