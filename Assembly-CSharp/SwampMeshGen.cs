// Decompiled with JetBrains decompiler
// Type: SwampMeshGen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (HeightmapMesh), typeof (MeshFilter), typeof (MeshRenderer))]
public class SwampMeshGen : MonoBehaviour
{
  [Header("Grid")]
  [Min(2f)]
  public int resolution = 64 /*0x40*/;
  public float cellSize = 1f;
  public bool center = true;
  [Header("Raycast")]
  public float maxRaycastLength = 10f;
  public LayerMask layerMask = (LayerMask) -1;
  public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
  [Header("Clamp (optional)")]
  public bool clampHeights;
  public float minHeight = -5f;
  public float maxHeight = 5f;
  [Header("Smoothing")]
  [Tooltip("Number of box-blur passes to apply after clamping.")]
  [Min(0.0f)]
  public int blurIterations = 1;
  [Tooltip("Radius in cells for box blur. 0 disables blur.")]
  [Min(0.0f)]
  public int blurRadius = 1;
  [Header("Offset")]
  public float heightOffset;
  [Header("Debug")]
  public bool drawRayGizmos;
  public bool drawSamplePoints;
  private HeightmapMesh _hm;

  private void Awake() => this._hm = this.GetComponent<HeightmapMesh>();

  public void Generate()
  {
    if (this.resolution < 2)
    {
      Debug.LogError((object) "Resolution must be at least 2.");
    }
    else
    {
      if ((Object) this._hm == (Object) null)
        this._hm = this.GetComponent<HeightmapMesh>();
      this._hm.cellSize = this.cellSize;
      this._hm.center = this.center;
      int resolution1 = this.resolution;
      int resolution2 = this.resolution;
      float[,] numArray = new float[resolution2, resolution1];
      Vector3 vector3_1 = Vector3.zero;
      if (this.center)
        vector3_1 = new Vector3((float) (-(double) ((float) (resolution1 - 1) * this.cellSize) * 0.5), 0.0f, (float) (-(double) ((float) (resolution2 - 1) * this.cellSize) * 0.5));
      for (int index1 = 0; index1 < resolution2; ++index1)
      {
        for (int index2 = 0; index2 < resolution1; ++index2)
        {
          Vector3 vector3_2 = new Vector3((float) index2 * this.cellSize, 0.0f, (float) index1 * this.cellSize);
          RaycastHit hitInfo;
          float num = !Physics.Raycast(new Ray(this.transform.position + vector3_1 + vector3_2, Vector3.down), out hitInfo, this.maxRaycastLength, (int) this.layerMask, this.triggerInteraction) ? -this.maxRaycastLength : hitInfo.point.y - this.transform.position.y;
          numArray[index1, index2] = num;
        }
      }
      if (this.clampHeights)
      {
        for (int index3 = 0; index3 < resolution2; ++index3)
        {
          for (int index4 = 0; index4 < resolution1; ++index4)
            numArray[index3, index4] = Mathf.Clamp(numArray[index3, index4], this.minHeight, this.maxHeight);
        }
      }
      for (int index5 = 0; index5 < resolution2; ++index5)
      {
        for (int index6 = 0; index6 < resolution1; ++index6)
          numArray[index5, index6] += this.heightOffset;
      }
      if (this.blurIterations > 0 && this.blurRadius > 0)
        this.BoxBlurInPlace(numArray, this.blurRadius, this.blurIterations);
      this._hm.Generate(numArray);
    }
  }

  private void BoxBlurInPlace(float[,] data, int radius, int iterations)
  {
    int length1 = data.GetLength(0);
    int length2 = data.GetLength(1);
    float[,] numArray = new float[length1, length2];
    for (int index1 = 0; index1 < iterations; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
      {
        float num1 = 0.0f;
        int num2 = radius * 2 + 1;
        for (int index3 = -radius; index3 <= radius; ++index3)
        {
          int index4 = Mathf.Clamp(index3, 0, length2 - 1);
          num1 += data[index2, index4];
        }
        for (int index5 = 0; index5 < length2; ++index5)
        {
          numArray[index2, index5] = num1 / (float) num2;
          int index6 = index5 - radius;
          int index7 = index5 + radius + 1;
          float num3 = index6 < 0 || index6 >= length2 ? (index6 >= 0 ? num1 - data[index2, length2 - 1] : num1 - data[index2, 0]) : num1 - data[index2, index6];
          num1 = index7 < 0 || index7 >= length2 ? (index7 >= 0 ? num3 + data[index2, length2 - 1] : num3 + data[index2, 0]) : num3 + data[index2, index7];
        }
      }
      for (int index8 = 0; index8 < length2; ++index8)
      {
        float num4 = 0.0f;
        int num5 = radius * 2 + 1;
        for (int index9 = -radius; index9 <= radius; ++index9)
        {
          int index10 = Mathf.Clamp(index9, 0, length1 - 1);
          num4 += numArray[index10, index8];
        }
        for (int index11 = 0; index11 < length1; ++index11)
        {
          data[index11, index8] = num4 / (float) num5;
          int index12 = index11 - radius;
          int index13 = index11 + radius + 1;
          float num6 = index12 < 0 || index12 >= length1 ? (index12 >= 0 ? num4 - numArray[length1 - 1, index8] : num4 - numArray[0, index8]) : num4 - numArray[index12, index8];
          num4 = index13 < 0 || index13 >= length1 ? (index13 >= 0 ? num6 + numArray[length1 - 1, index8] : num6 + numArray[0, index8]) : num6 + numArray[index13, index8];
        }
      }
    }
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.drawRayGizmos && !this.drawSamplePoints)
      return;
    int num1 = Mathf.Max(2, this.resolution);
    int num2 = Mathf.Max(2, this.resolution);
    Vector3 vector3_1 = Vector3.zero;
    if (this.center)
      vector3_1 = new Vector3((float) (-(double) ((float) (num1 - 1) * this.cellSize) * 0.5), 0.0f, (float) (-(double) ((float) (num2 - 1) * this.cellSize) * 0.5));
    for (int index1 = 0; index1 < num2; ++index1)
    {
      for (int index2 = 0; index2 < num1; ++index2)
      {
        Vector3 vector3_2 = new Vector3((float) index2 * this.cellSize, 0.0f, (float) index1 * this.cellSize);
        Vector3 vector3_3 = this.transform.position + vector3_1 + vector3_2;
        if (this.drawSamplePoints)
          Gizmos.DrawSphere(vector3_3, Mathf.Min(0.05f, this.cellSize * 0.1f));
        if (this.drawRayGizmos)
          Gizmos.DrawLine(vector3_3, vector3_3 + Vector3.down * this.maxRaycastLength);
      }
    }
  }
}
