// Decompiled with JetBrains decompiler
// Type: LightVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

#nullable disable
[ExecuteInEditMode]
public class LightVolume : MonoBehaviour
{
  public bool showVolumeGizmos = true;
  public float brightness = 1f;
  public float ambienceStrength = 1f;
  public float ambienceMin = 0.05f;
  public Color skyColor = Color.white;
  public Vector3Int gridRes;
  public Vector3 gridOffset;
  public int rayCount = 128 /*0x80*/;
  public float raySpacing = 1.5f;
  [Tooltip("Colliders matching this mask will be used for light tracing, colliders not matching will be ignored")]
  public LayerMask occluderMask = (LayerMask) -1;
  [Tooltip("Radius (in texels) for how much to box blur the output texture")]
  public int blurRadius;
  public GameObject sceneParent;
  public ComputeShader computeShader;
  public RayTracingShader rayTracingShader;
  public Texture3D lightMap;
  public List<BakedVolumeLight> allLightsFound;
  public List<MeshRenderer> allMeshRenderersFound;
  internal static LightVolume instance;

  private void SetShaderVars()
  {
    Shader.SetGlobalFloat("brightness", this.brightness);
    Shader.SetGlobalFloat("ambienceStrength", this.ambienceStrength);
    Shader.SetGlobalFloat("ambienceMin", this.ambienceMin);
    Shader.SetGlobalVector("gridRes", (Vector4) (Vector3) this.gridRes);
    Shader.SetGlobalFloat("raySpacing", this.raySpacing);
    Shader.SetGlobalVector("gridOffset", (Vector4) this.gridOffset);
  }

  public void SetSize()
  {
    Shader.SetGlobalTexture("_LightMap", (Texture) null);
    Bounds totalBounds = LightVolume.GetTotalBounds((UnityEngine.Object) this.sceneParent == (UnityEngine.Object) null ? this.gameObject : this.sceneParent);
    this.gridOffset = totalBounds.center;
    this.gridRes = new Vector3Int(Mathf.CeilToInt((totalBounds.size.x + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.y + 3f) / this.raySpacing), Mathf.CeilToInt((totalBounds.size.z + 3f) / this.raySpacing));
  }

  private static Bounds GetTotalBounds(GameObject gameObject)
  {
    Bounds totalBounds = new Bounds();
    bool flag = true;
    foreach (MeshRenderer componentsInChild in gameObject.GetComponentsInChildren<MeshRenderer>())
    {
      if (flag)
        totalBounds = componentsInChild.bounds;
      else
        totalBounds.Encapsulate(componentsInChild.bounds);
      flag = false;
    }
    return totalBounds;
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.showVolumeGizmos)
      return;
    Gizmos.color = Color.black;
    Gizmos.DrawWireCube(this.gridOffset - Vector3.one * 0.25f, (Vector3) this.gridRes * this.raySpacing);
    Gizmos.color = Color.white;
    Gizmos.DrawWireCube(this.gridOffset + Vector3.one * 0.25f, (Vector3) this.gridRes * this.raySpacing);
  }

  private void Awake() => LightVolume.instance = this;

  private void Start()
  {
    this.SetShaderVars();
    Shader.SetGlobalTexture("_LightMap", (Texture) this.lightMap);
  }

  private bool RaytracingShaderNotSupported => !SystemInfo.supportsRayTracingShaders;

  public void Bake(Action onComplete = null)
  {
    if (!(bool) (UnityEngine.Object) this.computeShader)
      Debug.LogError((object) "Cannot bake at runtime (serialize the ComputeShader if you want to do this)");
    else if (!(bool) (UnityEngine.Object) this.rayTracingShader)
    {
      Debug.LogError((object) "Cannot bake at runtime (serialize the RayTracingShader if you want to do this)");
    }
    else
    {
      this.SetSize();
      RenderTexture renderTexture = this.RunBlur(this.RunBake());
      renderTexture.name = "LightVolumeRenderTexture";
      this.SetShaderVars();
      Shader.SetGlobalTexture("_LightMap", (Texture) renderTexture);
      this.SaveTex(renderTexture, onComplete);
    }
  }

  private RenderTexture RunBake()
  {
    this.rayTracingShader.SetVector("gridRadius", (Vector4) (new Vector3((float) this.gridRes.x, (float) this.gridRes.y, (float) this.gridRes.z) * (this.raySpacing / 2f)));
    this.rayTracingShader.SetVector("gridOffset", (Vector4) this.gridOffset);
    this.rayTracingShader.SetVector("skyColor", (Vector4) this.skyColor);
    this.rayTracingShader.SetInt("rayCount", this.rayCount);
    ComputeBuffer toDispose1;
    int lightCountForDebug = this.BuildLights(out toDispose1);
    IDisposable toDispose2;
    this.BuildMeshes(out toDispose2, lightCountForDebug);
    RenderTexture renderTexture = LightVolume.Create3DTexture(FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, this.gridRes);
    this.rayTracingShader.SetTexture("lightMap", (Texture) renderTexture);
    for (int val = 0; val < lightCountForDebug + 1; ++val)
    {
      this.rayTracingShader.SetInt("doLightIndex", val);
      this.rayTracingShader.Dispatch("RaygenShader", this.gridRes.x, this.gridRes.y, this.gridRes.z);
    }
    toDispose1.Dispose();
    toDispose2.Dispose();
    return renderTexture;
  }

  private static RenderTexture Create3DTexture(
    FilterMode filterMode,
    RenderTextureFormat format,
    Vector3Int resolution)
  {
    RenderTexture renderTexture = new RenderTexture(resolution.x, resolution.y, 0);
    renderTexture.enableRandomWrite = true;
    renderTexture.format = format;
    renderTexture.dimension = TextureDimension.Tex3D;
    renderTexture.volumeDepth = resolution.z;
    renderTexture.wrapMode = TextureWrapMode.Clamp;
    renderTexture.filterMode = filterMode;
    renderTexture.hideFlags = HideFlags.DontSave;
    return renderTexture.Create() ? renderTexture : throw new Exception("Failed to create texture");
  }

  private int BuildLights(out ComputeBuffer toDispose)
  {
    List<LightVolume.GpuLight> data = new List<LightVolume.GpuLight>();
    GameObject gameObject = (UnityEngine.Object) this.sceneParent == (UnityEngine.Object) null ? this.gameObject : this.sceneParent;
    if (this.allLightsFound == null)
      this.allLightsFound = new List<BakedVolumeLight>();
    this.allLightsFound.Clear();
    LightVolume.GpuLight gpuLight1;
    foreach (BakedVolumeLight componentsInChild in gameObject.GetComponentsInChildren<BakedVolumeLight>())
    {
      this.allLightsFound.Add(componentsInChild);
      Vector3 vector3 = new Vector3(componentsInChild.color.r, componentsInChild.color.g, componentsInChild.color.b);
      float num1;
      switch (componentsInChild.mode)
      {
        case BakedVolumeLight.LightModes.Point:
          num1 = 0.0f;
          break;
        case BakedVolumeLight.LightModes.Spot:
          num1 = componentsInChild.coneSize * ((float) Math.PI / 180f);
          break;
        default:
          throw new Exception();
      }
      float num2 = num1;
      List<LightVolume.GpuLight> gpuLightList = data;
      gpuLight1 = new LightVolume.GpuLight();
      gpuLight1.Position = componentsInChild.transform.position;
      gpuLight1.ConeSize = num2;
      gpuLight1.Direction = componentsInChild.transform.forward;
      gpuLight1.Radius = componentsInChild.GetRadius();
      gpuLight1.Color = vector3 * componentsInChild.intensity;
      gpuLight1.Falloff = componentsInChild.falloff;
      gpuLight1.ConeFalloff = componentsInChild.coneFalloff;
      LightVolume.GpuLight gpuLight2 = gpuLight1;
      gpuLightList.Add(gpuLight2);
    }
    int count = data.Count;
    if (count == 0)
    {
      List<LightVolume.GpuLight> gpuLightList = data;
      gpuLight1 = new LightVolume.GpuLight();
      LightVolume.GpuLight gpuLight3 = gpuLight1;
      gpuLightList.Add(gpuLight3);
    }
    ComputeBuffer buffer = new ComputeBuffer(data.Count, 52);
    buffer.SetData<LightVolume.GpuLight>(data);
    this.rayTracingShader.SetBuffer("lightBuffer", buffer);
    this.rayTracingShader.SetInt("lightBufferLength", count);
    toDispose = buffer;
    return count;
  }

  private void BuildMeshes(out IDisposable toDispose, int lightCountForDebug)
  {
    int num1 = this.occluderMask.value;
    GameObject gameObject = (UnityEngine.Object) this.sceneParent == (UnityEngine.Object) null ? this.gameObject : this.sceneParent;
    if (this.allMeshRenderersFound == null)
      this.allMeshRenderersFound = new List<MeshRenderer>();
    this.allMeshRenderersFound.Clear();
    RayTracingAccelerationStructure accelerationStructure = new RayTracingAccelerationStructure();
    uint num2 = 0;
    int num3 = 0;
    foreach (MeshRenderer componentsInChild in gameObject.GetComponentsInChildren<MeshRenderer>())
    {
      if ((1 << componentsInChild.gameObject.layer & num1) != 0 || (bool) (UnityEngine.Object) componentsInChild.GetComponent<LightingCollider>())
      {
        this.allMeshRenderersFound.Add(componentsInChild);
        MeshFilter component = componentsInChild.GetComponent<MeshFilter>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          Debug.LogError((object) ("Mesh renderer without filter: " + componentsInChild.gameObject.name), (UnityEngine.Object) componentsInChild.gameObject);
        Mesh sharedMesh = component.sharedMesh;
        if (!((UnityEngine.Object) sharedMesh == (UnityEngine.Object) null))
        {
          int subMeshCount = sharedMesh.subMeshCount;
          for (int submesh = 0; submesh < subMeshCount; ++submesh)
            num2 += sharedMesh.GetIndexCount(submesh);
          num3 += sharedMesh.vertexCount;
          RayTracingSubMeshFlags[] subMeshFlags = new RayTracingSubMeshFlags[subMeshCount];
          for (int index = 0; index < subMeshFlags.Length; ++index)
            subMeshFlags[index] = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
          accelerationStructure.AddInstance((Renderer) componentsInChild, subMeshFlags);
        }
      }
    }
    accelerationStructure.Build();
    Debug.Log((object) $"Light Volume Baker found: {lightCountForDebug} lights, {this.allMeshRenderersFound.Count} meshes, {num2} indices, {num3} vertices");
    this.rayTracingShader.SetAccelerationStructure("g_SceneAccelStruct", accelerationStructure);
    toDispose = (IDisposable) accelerationStructure;
  }

  private RenderTexture RunBlur(RenderTexture inputTex)
  {
    if (this.blurRadius <= 0)
      return inputTex;
    this.computeShader.SetInt("blurRadius", this.blurRadius);
    Vector3Int resolution = new Vector3Int(inputTex.width, inputTex.height, inputTex.volumeDepth);
    RenderTexture renderTexture1 = LightVolume.Create3DTexture(inputTex.filterMode, inputTex.format, resolution);
    for (int val = 0; val < 3; ++val)
    {
      this.computeShader.SetTexture(1, "blurInputLightMap", (Texture) inputTex);
      this.computeShader.SetTexture(1, "lightMap", (Texture) renderTexture1);
      this.computeShader.SetInt("blurAxis", val);
      uint num1 = 4;
      uint num2 = 4;
      uint num3 = 4;
      this.computeShader.Dispatch(1, (int) (((long) resolution.x + (long) num1 - 1L) / (long) num1), (int) (((long) resolution.y + (long) num2 - 1L) / (long) num2), (int) (((long) resolution.z + (long) num3 - 1L) / (long) num3));
      RenderTexture renderTexture2 = inputTex;
      RenderTexture renderTexture3 = renderTexture1;
      renderTexture1 = renderTexture2;
      inputTex = renderTexture3;
    }
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) renderTexture1);
    return inputTex;
  }

  private void SaveTex(RenderTexture renderTexture, Action onComplete = null)
  {
    AsyncGPUReadbackRequest gpuReadbackRequest = AsyncGPUReadback.Request((Texture) renderTexture);
    gpuReadbackRequest.WaitForCompletion();
    byte[] numArray = new byte[gpuReadbackRequest.layerDataSize * gpuReadbackRequest.layerCount];
    for (int layer = 0; layer < gpuReadbackRequest.layerCount; ++layer)
      NativeArray<byte>.Copy(gpuReadbackRequest.GetData<byte>(layer), 0, numArray, layer * gpuReadbackRequest.layerDataSize, gpuReadbackRequest.layerDataSize);
    if (!(bool) (UnityEngine.Object) this.lightMap || this.lightMap.width != renderTexture.width || this.lightMap.height != renderTexture.height || this.lightMap.depth != renderTexture.volumeDepth || this.lightMap.graphicsFormat != renderTexture.graphicsFormat)
    {
      if ((bool) (UnityEngine.Object) this.lightMap)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.lightMap);
      this.lightMap = new Texture3D(renderTexture.width, renderTexture.height, renderTexture.volumeDepth, renderTexture.graphicsFormat, TextureCreationFlags.None);
    }
    this.lightMap.name = "LightVolumeBakeTexture";
    this.lightMap.wrapMode = renderTexture.wrapMode;
    this.lightMap.filterMode = renderTexture.filterMode;
    this.lightMap.SetPixelData<byte>(numArray, 0);
    this.lightMap.Apply();
    Shader.SetGlobalTexture("_LightMap", (Texture) this.lightMap);
    if (onComplete != null)
      onComplete();
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) renderTexture);
  }

  public static LightVolume Instance()
  {
    if ((UnityEngine.Object) LightVolume.instance == (UnityEngine.Object) null)
      LightVolume.instance = UnityEngine.Object.FindAnyObjectByType<LightVolume>();
    return LightVolume.instance;
  }

  internal Color SamplePosition(Vector3 worldPos)
  {
    worldPos -= this.gridOffset;
    worldPos += this.raySpacing * (Vector3) this.gridRes * 0.5f;
    worldPos.x /= this.raySpacing;
    worldPos.y /= this.raySpacing;
    worldPos.z /= this.raySpacing;
    return this.lightMap.GetPixel((int) worldPos.x, (int) worldPos.y, (int) worldPos.z);
  }

  public float SamplePositionAlpha(Vector3 worldPos)
  {
    worldPos -= this.gridOffset;
    worldPos += this.raySpacing * (Vector3) this.gridRes * 0.5f;
    worldPos.x /= this.raySpacing;
    worldPos.y /= this.raySpacing;
    worldPos.z /= this.raySpacing;
    return this.lightMap.GetPixel((int) worldPos.x, (int) worldPos.y, (int) worldPos.z).a;
  }

  private struct GpuLight
  {
    public Vector3 Position;
    public float ConeSize;
    public Vector3 Direction;
    public float Radius;
    public Vector3 Color;
    public float Falloff;
    public float ConeFalloff;
  }
}
