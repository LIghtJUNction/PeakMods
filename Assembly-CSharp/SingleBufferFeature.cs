// Decompiled with JetBrains decompiler
// Type: SingleBufferFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#nullable disable
public class SingleBufferFeature : ScriptableRendererFeature
{
  public SingleBufferFeature.Settings settings = new SingleBufferFeature.Settings();
  private SingleBufferFeature.CustomRenderPass m_ScriptablePass;

  public override void Create()
  {
    this.m_ScriptablePass = new SingleBufferFeature.CustomRenderPass(this.settings, this.name);
    this.m_ScriptablePass.renderPassEvent = this.settings._event;
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
    CameraType cameraType = renderingData.cameraData.cameraType;
    if (cameraType == CameraType.Preview || !this.settings.showInSceneView && cameraType == CameraType.SceneView)
      return;
    renderer.EnqueuePass((ScriptableRenderPass) this.m_ScriptablePass);
  }

  protected override void Dispose(bool disposing) => this.m_ScriptablePass.Dispose();

  public class CustomRenderPass : ScriptableRenderPass
  {
    private SingleBufferFeature.Settings settings;
    private FilteringSettings filteringSettings;
    private ProfilingSampler _profilingSampler;
    private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();
    private RTHandle rtCustomColor;
    private RTHandle rtTempColor;

    public CustomRenderPass(SingleBufferFeature.Settings settings, string name)
    {
      this.settings = settings;
      this.filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.transparent), (int) settings.layerMask);
      this.shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
      this.shaderTagsList.Add(new ShaderTagId("UniversalForward"));
      this.shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
      this._profilingSampler = new ProfilingSampler(name);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
      RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor with
      {
        depthBufferBits = 0
      };
      RenderingUtils.ReAllocateIfNeeded(ref this.rtTempColor, in descriptor, name: "_TemporaryColorTexture");
      if (this.settings.colorTargetDestinationID != "")
        RenderingUtils.ReAllocateIfNeeded(ref this.rtCustomColor, in descriptor, name: this.settings.colorTargetDestinationID);
      else
        this.rtCustomColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
      this.ConfigureTarget(this.rtCustomColor, renderingData.cameraData.renderer.cameraDepthTargetHandle);
      this.ConfigureClear(ClearFlag.Color, new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
      CommandBuffer commandBuffer = CommandBufferPool.Get();
      using (new ProfilingScope(commandBuffer, this._profilingSampler))
      {
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
        SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
        DrawingSettings drawingSettings = this.CreateDrawingSettings(this.shaderTagsList, ref renderingData, sortingCriteria);
        if ((UnityEngine.Object) this.settings.overrideMaterial != (UnityEngine.Object) null)
        {
          drawingSettings.overrideMaterialPassIndex = this.settings.overrideMaterialPass;
          drawingSettings.overrideMaterial = this.settings.overrideMaterial;
        }
        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref this.filteringSettings);
        if (this.settings.colorTargetDestinationID != "")
          commandBuffer.SetGlobalTexture(this.settings.colorTargetDestinationID, (RenderTargetIdentifier) this.rtCustomColor);
        if ((UnityEngine.Object) this.settings.blitMaterial != (UnityEngine.Object) null)
        {
          RTHandle colorTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
          if (colorTargetHandle != null)
          {
            if (this.rtTempColor != null)
            {
              Blitter.BlitCameraTexture(commandBuffer, colorTargetHandle, this.rtTempColor, this.settings.blitMaterial, 0);
              Blitter.BlitCameraTexture(commandBuffer, this.rtTempColor, colorTargetHandle);
            }
          }
        }
      }
      context.ExecuteCommandBuffer(commandBuffer);
      commandBuffer.Clear();
      CommandBufferPool.Release(commandBuffer);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }

    public void Dispose()
    {
      if (this.settings.colorTargetDestinationID != "")
        this.rtCustomColor?.Release();
      this.rtTempColor?.Release();
    }
  }

  [Serializable]
  public class Settings
  {
    public bool showInSceneView = true;
    public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;
    [Header("Draw Renderers Settings")]
    public LayerMask layerMask = (LayerMask) 1;
    public Material overrideMaterial;
    public int overrideMaterialPass;
    public string colorTargetDestinationID = "";
    [Header("Blit Settings")]
    public Material blitMaterial;
  }
}
