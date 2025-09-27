// Decompiled with JetBrains decompiler
// Type: TerrainSplatMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TerrainSplatMesh : MonoBehaviour
{
  public bool vertexColorMask;
  private Mesh mesh;
  private Vector3[] verts;
  private Color[] colors;

  private Mesh GetMesh()
  {
    if ((Object) this.mesh == (Object) null)
    {
      this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
      this.verts = this.mesh.vertices;
      this.colors = this.mesh.colors;
    }
    return this.mesh;
  }

  internal bool PointIsValid(Vector3 point)
  {
    if (this.vertexColorMask)
    {
      this.GetMesh();
      if ((double) HelperFunctions.GetValue(HelperFunctions.GetVertexColorAtPoint(this.verts, this.colors, this.transform, point)) < 0.89999997615814209)
        return false;
    }
    return true;
  }
}
