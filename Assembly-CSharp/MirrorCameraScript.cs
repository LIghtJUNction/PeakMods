// Decompiled with JetBrains decompiler
// Type: MirrorCameraScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MirrorCameraScript : MonoBehaviour
{
  public GameObject MirrorObject;
  public bool VRMode;
  private Renderer mirrorRenderer;
  private Material mirrorMaterial;
  private MirrorScript mirrorScript;
  private Camera cameraObject;
  private RenderTexture reflectionTexture;
  private Matrix4x4 reflectionMatrix;
  private int oldReflectionTextureSize;
  private static bool renderingMirror;

  private void Start()
  {
    this.mirrorScript = this.GetComponentInParent<MirrorScript>();
    this.cameraObject = this.GetComponent<Camera>();
    if (this.mirrorScript.AddFlareLayer)
      this.cameraObject.gameObject.AddComponent<FlareLayer>();
    this.mirrorRenderer = this.MirrorObject.GetComponent<Renderer>();
    if (Application.isPlaying)
    {
      foreach (Material sharedMaterial in this.mirrorRenderer.sharedMaterials)
      {
        if (sharedMaterial.name == "MirrorMaterial")
        {
          this.mirrorRenderer.sharedMaterial = sharedMaterial;
          break;
        }
      }
    }
    this.mirrorMaterial = this.mirrorRenderer.sharedMaterial;
    this.CreateRenderTexture();
  }

  private void CreateRenderTexture()
  {
    if ((Object) this.reflectionTexture == (Object) null || this.oldReflectionTextureSize != this.mirrorScript.TextureSize)
    {
      if ((bool) (Object) this.reflectionTexture)
        Object.DestroyImmediate((Object) this.reflectionTexture);
      this.reflectionTexture = new RenderTexture(this.mirrorScript.TextureSize, this.mirrorScript.TextureSize, 16 /*0x10*/);
      this.reflectionTexture.filterMode = FilterMode.Bilinear;
      this.reflectionTexture.antiAliasing = 1;
      this.reflectionTexture.name = "MirrorRenderTexture_" + this.GetInstanceID().ToString();
      this.reflectionTexture.hideFlags = HideFlags.HideAndDontSave;
      this.reflectionTexture.autoGenerateMips = false;
      this.reflectionTexture.wrapMode = TextureWrapMode.Clamp;
      this.mirrorMaterial.SetTexture("_MainTex", (Texture) this.reflectionTexture);
      this.oldReflectionTextureSize = this.mirrorScript.TextureSize;
    }
    if (!((Object) this.cameraObject.targetTexture != (Object) this.reflectionTexture))
      return;
    this.cameraObject.targetTexture = this.reflectionTexture;
  }

  private void Update()
  {
    if (this.VRMode && (Object) Camera.current == (Object) Camera.main)
      return;
    this.CreateRenderTexture();
  }

  private void UpdateCameraProperties(Camera src, Camera dest)
  {
    dest.clearFlags = src.clearFlags;
    dest.backgroundColor = src.backgroundColor;
    if (src.clearFlags == CameraClearFlags.Skybox)
    {
      Skybox component1 = src.GetComponent<Skybox>();
      Skybox component2 = dest.GetComponent<Skybox>();
      if (!(bool) (Object) component1 || !(bool) (Object) component1.material)
      {
        component2.enabled = false;
      }
      else
      {
        component2.enabled = true;
        component2.material = component1.material;
      }
    }
    dest.orthographic = src.orthographic;
    dest.orthographicSize = src.orthographicSize;
    dest.aspect = (double) this.mirrorScript.AspectRatio <= 0.0 ? src.aspect : this.mirrorScript.AspectRatio;
    dest.renderingPath = src.renderingPath;
  }

  internal void RenderMirror()
  {
    Camera current;
    if (MirrorCameraScript.renderingMirror || !this.enabled || (Object) (current = Camera.current) == (Object) null || (Object) this.mirrorRenderer == (Object) null || (Object) this.mirrorMaterial == (Object) null || !this.mirrorRenderer.enabled)
      return;
    MirrorCameraScript.renderingMirror = true;
    int pixelLightCount = QualitySettings.pixelLightCount;
    if (QualitySettings.pixelLightCount != this.mirrorScript.MaximumPerPixelLights)
      QualitySettings.pixelLightCount = this.mirrorScript.MaximumPerPixelLights;
    try
    {
      this.UpdateCameraProperties(current, this.cameraObject);
      if (this.mirrorScript.MirrorRecursion)
      {
        this.mirrorMaterial.EnableKeyword("MIRROR_RECURSION");
        this.cameraObject.ResetWorldToCameraMatrix();
        this.cameraObject.ResetProjectionMatrix();
        this.cameraObject.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
        this.cameraObject.cullingMask = -17 & this.mirrorScript.ReflectLayers.value;
        GL.invertCulling = true;
        this.cameraObject.Render();
        GL.invertCulling = false;
      }
      else
      {
        this.mirrorMaterial.DisableKeyword("MIRROR_RECURSION");
        Vector3 position1 = this.transform.position;
        Vector3 normal = this.mirrorScript.NormalIsForward ? this.transform.forward : this.transform.up;
        float w = -Vector3.Dot(normal, position1) - this.mirrorScript.ClipPlaneOffset;
        Vector4 plane = new Vector4(normal.x, normal.y, normal.z, w);
        this.CalculateReflectionMatrix(ref plane);
        Vector3 position2 = this.cameraObject.transform.position;
        float farClipPlane = this.cameraObject.farClipPlane;
        Vector3 vector3 = this.reflectionMatrix.MultiplyPoint(position2);
        Matrix4x4 worldToCameraMatrix1 = current.worldToCameraMatrix;
        if (this.VRMode)
        {
          if (current.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
            worldToCameraMatrix1[12] += 11f / 1000f;
          else if (current.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
            worldToCameraMatrix1[12] -= 11f / 1000f;
        }
        Matrix4x4 worldToCameraMatrix2 = worldToCameraMatrix1 * this.reflectionMatrix;
        this.cameraObject.worldToCameraMatrix = worldToCameraMatrix2;
        Vector4 clipPlane = this.CameraSpacePlane(ref worldToCameraMatrix2, ref position1, ref normal, 1f);
        this.cameraObject.projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
        GL.invertCulling = true;
        this.cameraObject.transform.position = vector3;
        this.cameraObject.farClipPlane = this.mirrorScript.FarClipPlane;
        this.cameraObject.cullingMask = -17 & this.mirrorScript.ReflectLayers.value;
        this.cameraObject.Render();
        this.cameraObject.transform.position = position2;
        this.cameraObject.farClipPlane = farClipPlane;
        GL.invertCulling = false;
      }
    }
    finally
    {
      MirrorCameraScript.renderingMirror = false;
      if (QualitySettings.pixelLightCount != pixelLightCount)
        QualitySettings.pixelLightCount = pixelLightCount;
    }
  }

  private void OnDisable()
  {
    if (!(bool) (Object) this.reflectionTexture)
      return;
    Object.DestroyImmediate((Object) this.reflectionTexture);
    this.reflectionTexture = (RenderTexture) null;
  }

  private Vector4 CameraSpacePlane(
    ref Matrix4x4 worldToCameraMatrix,
    ref Vector3 pos,
    ref Vector3 normal,
    float sideSign)
  {
    Vector3 point = pos + normal * this.mirrorScript.ClipPlaneOffset;
    Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
    Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
    return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
  }

  private void CalculateReflectionMatrix(ref Vector4 plane)
  {
    this.reflectionMatrix.m00 = (float) (1.0 - 2.0 * (double) plane[0] * (double) plane[0]);
    this.reflectionMatrix.m01 = -2f * plane[0] * plane[1];
    this.reflectionMatrix.m02 = -2f * plane[0] * plane[2];
    this.reflectionMatrix.m03 = -2f * plane[3] * plane[0];
    this.reflectionMatrix.m10 = -2f * plane[1] * plane[0];
    this.reflectionMatrix.m11 = (float) (1.0 - 2.0 * (double) plane[1] * (double) plane[1]);
    this.reflectionMatrix.m12 = -2f * plane[1] * plane[2];
    this.reflectionMatrix.m13 = -2f * plane[3] * plane[1];
    this.reflectionMatrix.m20 = -2f * plane[2] * plane[0];
    this.reflectionMatrix.m21 = -2f * plane[2] * plane[1];
    this.reflectionMatrix.m22 = (float) (1.0 - 2.0 * (double) plane[2] * (double) plane[2]);
    this.reflectionMatrix.m23 = -2f * plane[3] * plane[2];
    this.reflectionMatrix.m30 = 0.0f;
    this.reflectionMatrix.m31 = 0.0f;
    this.reflectionMatrix.m32 = 0.0f;
    this.reflectionMatrix.m33 = 1f;
  }

  private static void CalculateObliqueMatrix(ref Matrix4x4 projection, ref Vector4 clipPlane)
  {
    Vector4 b = projection.inverse * new Vector4(MirrorCameraScript.Sign(clipPlane.x), MirrorCameraScript.Sign(clipPlane.y), 1f, 1f);
    Vector4 vector4 = clipPlane * (2f / Vector4.Dot(clipPlane, b));
    projection[2] = vector4.x - projection[3];
    projection[6] = vector4.y - projection[7];
    projection[10] = vector4.z - projection[11];
    projection[14] = vector4.w - projection[15];
  }

  private static float Sign(float a)
  {
    if ((double) a > 0.0)
      return 1f;
    return (double) a < 0.0 ? -1f : 0.0f;
  }
}
