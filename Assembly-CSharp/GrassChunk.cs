// Decompiled with JetBrains decompiler
// Type: GrassChunk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

#nullable disable
public class GrassChunk : GrassDataProvider
{
  public List<GrassPoint> GrassPoints;
  public bool isDirty = true;

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireCube(this.transform.position + Vector3.one * 50f, Vector3.one * GrassChunking.CHUNK_SIZE);
  }

  public override bool IsDirty() => this.isDirty;

  public override ComputeBuffer GetData()
  {
    ComputeBuffer data = new ComputeBuffer(this.GrassPoints.Count, UnsafeUtility.SizeOf<GrassPoint>());
    data.SetData<GrassPoint>(this.GrassPoints);
    this.isDirty = false;
    return data;
  }

  public void SetData(List<GrassPoint> grassPoints)
  {
    this.GrassPoints = grassPoints;
    this.isDirty = true;
  }
}
