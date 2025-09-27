// Decompiled with JetBrains decompiler
// Type: GrassChunking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;

#nullable disable
public static class GrassChunking
{
  public static readonly float CHUNK_SIZE = 35f;
  public static readonly float CHUNK_SIZE_INV = 1f / GrassChunking.CHUNK_SIZE;

  public static int3 GetChunkFromPosition(float3 p)
  {
    int x = Mathf.FloorToInt(p.x * GrassChunking.CHUNK_SIZE_INV);
    int num1 = Mathf.FloorToInt(p.y * GrassChunking.CHUNK_SIZE_INV);
    int num2 = Mathf.FloorToInt(p.z * GrassChunking.CHUNK_SIZE_INV);
    int y = num1;
    int z = num2;
    return new int3(x, y, z);
  }

  public static bool ShouldDrawChunk(int3 cameraChunk, int3 renderChunk)
  {
    return Mathf.Abs(cameraChunk.x - renderChunk.x) <= 1 && Mathf.Abs(cameraChunk.y - renderChunk.y) <= 1 && Mathf.Abs(cameraChunk.z - renderChunk.z) <= 1;
  }
}
