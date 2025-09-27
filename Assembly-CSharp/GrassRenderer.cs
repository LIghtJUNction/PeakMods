// Decompiled with JetBrains decompiler
// Type: GrassRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Zorro.Core.Compute;

#nullable disable
[ExecuteInEditMode]
public class GrassRenderer : MonoBehaviour
{
  public int3 CurrentChunk = int3.zero;
  public ComputeShader grassComputeShader;
  private ComputeKernel grassGeometryKernel;
  private ComputeBuffer GeometryBuffer;
  private ComputeBuffer ArgumentsBuffer;
  private ComputeBuffer GrassPointsBuffer;
  private const int MAX_GRASS = 10000;
  private const int DRAW_STRIDE = 148;
  private const int INDIRECT_DRAW_ARGS_STIDE = 16 /*0x10*/;
  private int[] argsBufferReset = new int[4]{ 0, 1, 0, 0 };
  public Material m_grassRenderMaterial;
  private GrassDataProvider DataProvider;

  private void OnEnable()
  {
    this.GeometryBuffer?.Dispose();
    this.ArgumentsBuffer?.Dispose();
    this.GrassPointsBuffer?.Dispose();
    this.DataProvider = this.GetComponent<GrassDataProvider>();
  }

  private void OnDisable()
  {
    this.GeometryBuffer?.Dispose();
    this.ArgumentsBuffer?.Dispose();
    this.GrassPointsBuffer?.Dispose();
    this.GeometryBuffer = (ComputeBuffer) null;
    this.ArgumentsBuffer = (ComputeBuffer) null;
    this.GrassPointsBuffer = (ComputeBuffer) null;
  }

  private void Update()
  {
  }

  private void Render()
  {
    if (!(bool) (UnityEngine.Object) this.DataProvider)
      this.DataProvider = this.GetComponent<GrassDataProvider>();
    if (this.GrassPointsBuffer == null || this.DataProvider.IsDirty())
    {
      this.GrassPointsBuffer?.Dispose();
      this.GrassPointsBuffer = this.DataProvider.GetData();
    }
    Camera camera = (Camera) null;
    if (Application.isPlaying)
      camera = MainCamera.instance.cam;
    if (!GrassChunking.ShouldDrawChunk(GrassChunking.GetChunkFromPosition((float3) camera.transform.position), this.CurrentChunk))
      return;
    if (this.GeometryBuffer == null)
    {
      this.GeometryBuffer = new ComputeBuffer(10000, 148, ComputeBufferType.Append);
      this.ArgumentsBuffer = new ComputeBuffer(1, 16 /*0x10*/, ComputeBufferType.DrawIndirect);
    }
    this.GeometryBuffer.SetCounterValue(0U);
    this.ArgumentsBuffer.SetData((Array) this.argsBufferReset);
    this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GeometryBuffer", this.GeometryBuffer);
    this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "IndirectArgsBuffer", this.ArgumentsBuffer);
    this.grassComputeShader.SetBuffer(this.grassGeometryKernel.kernelID, "GrassPoints", this.GrassPointsBuffer);
    this.grassComputeShader.SetFloat("_Time", Time.realtimeSinceStartup);
    this.grassComputeShader.SetVector("_CameraWSPos", (Vector4) camera.transform.position);
    MaterialPropertyBlock properties = new MaterialPropertyBlock();
    properties.SetBuffer("GeometryBuffer", this.GeometryBuffer);
    this.grassComputeShader.GetKernelThreadGroupSizes(this.grassGeometryKernel.kernelID, out uint _, out uint _, out uint _);
    this.grassGeometryKernel.Dispatch(new int3(this.GrassPointsBuffer.count, 1, 1));
    Graphics.DrawProceduralIndirect(this.m_grassRenderMaterial, new Bounds(this.transform.position, Vector3.one * 500f), MeshTopology.Triangles, this.ArgumentsBuffer, properties: properties, castShadows: ShadowCastingMode.Off);
  }
}
