// Decompiled with JetBrains decompiler
// Type: Mirror
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;

#nullable disable
[DefaultExecutionOrder(99999)]
public class Mirror : MonoBehaviour
{
  public Camera mirrorCamera;
  public Transform mirrorTransform;
  private Character player;
  private Camera mainCam;
  public RenderTexture renderTexture;
  private BoxCollider box;
  public bool useCameraTransform;
  public bool useCameraRot;
  public float offsetScale;
  public float mirrorCamDistance;
  public float verticalOffset;
  public bool isInitialized;
  public float mirrorWidth;
  public float mirrorHeight;
  public float nearplaneOffset;
  private float depth;

  private void Start()
  {
    Vector2 quadSize = this.getQuadSize();
    this.mirrorWidth = quadSize.x;
    this.mirrorHeight = quadSize.y;
  }

  private Vector2 getQuadSize()
  {
    Vector2 quadSize = new Vector2();
    Renderer component = this.mirrorTransform.GetComponent<Renderer>();
    quadSize.x = Mathf.Abs(component.bounds.size.z);
    quadSize.y = component.bounds.size.y;
    return quadSize;
  }

  private void LateUpdate()
  {
    if ((Object) this.player == (Object) null && (Object) Character.localCharacter != (Object) null)
      this.player = Character.localCharacter;
    if ((Object) this.player == (Object) null)
      return;
    if ((Object) Camera.main != (Object) null && !this.isInitialized)
    {
      this.mainCam = Camera.main;
      --this.mirrorCamera.depth;
      this.mirrorCamera.targetTexture = this.renderTexture;
      this.isInitialized = true;
    }
    Vector3 vector3_1 = this.mainCam.transform.position - this.mirrorTransform.position;
    if ((Object) this.mirrorCamera == (Object) null)
      return;
    Vector3 up = this.mirrorTransform.up;
    Vector3 position = this.mirrorTransform.position;
    Vector3 inDirection = this.mainCam.transform.position - position;
    Vector3 vector3_2 = Vector3.Reflect(inDirection, up);
    this.depth = inDirection.x;
    if (this.useCameraTransform)
      this.mirrorCamera.transform.position = position + vector3_2 + this.mirrorTransform.forward * this.verticalOffset;
    Vector3 upwards = Vector3.Reflect(this.mainCam.transform.up, up);
    Quaternion quaternion = Quaternion.LookRotation(Vector3.Reflect(this.mainCam.transform.forward, up), upwards);
    if (this.useCameraRot)
      this.mirrorCamera.transform.rotation = quaternion;
    this.mirrorCamera.projectionMatrix = Mirror.MirrorProjectionMatrix(this.mirrorCamera, this.mirrorCamera.farClipPlane, this.nearplaneOffset + Mathf.Abs(this.mirrorCamera.transform.position.x - this.mirrorTransform.position.x), this.mirrorTransform.position, this.mirrorTransform.up, this.mirrorWidth, this.mirrorHeight);
  }

  private void OnPreRender() => GL.invertCulling = true;

  private void OnPostRender() => GL.invertCulling = false;

  public void OnPreCull()
  {
  }

  public static Matrix4x4 MirrorProjectionMatrix(
    Camera cam,
    float far,
    float near,
    Vector3 mirrorCenter,
    Vector3 mirrorForward,
    float mirrorWidth,
    float mirrorHeight)
  {
    Transform transform = cam.transform;
    Vector3 vector3_1 = -Vector3.Cross(mirrorForward, Vector3.up).normalized;
    Vector3 vector3_2 = transform.InverseTransformPoint(mirrorCenter + -vector3_1 * (mirrorWidth / 2f));
    Vector3 vector3_3 = transform.InverseTransformPoint(mirrorCenter + vector3_1 * (mirrorWidth / 2f));
    Vector3 vector3_4 = transform.InverseTransformPoint(mirrorCenter + Vector3.up * (mirrorHeight / 2f));
    Vector3 vector3_5 = transform.InverseTransformPoint(mirrorCenter + Vector3.down * (mirrorHeight / 2f));
    Vector3 normalized1 = vector3_2.normalized;
    Vector3 normalized2 = vector3_3.normalized;
    Vector3 normalized3 = vector3_4.normalized;
    Vector3 normalized4 = vector3_5.normalized;
    Plane plane = new Plane(Vector3.forward, Vector3.forward * near);
    float enter1;
    float enter2;
    float enter3;
    float enter4;
    if (!plane.Raycast(new Ray(Vector3.zero, normalized1), out enter1) || !plane.Raycast(new Ray(Vector3.zero, normalized2), out enter2) || !plane.Raycast(new Ray(Vector3.zero, normalized3), out enter3) || !plane.Raycast(new Ray(Vector3.zero, normalized4), out enter4))
      return Matrix4x4.identity;
    double x1 = (double) (normalized1 * enter1).x;
    float x2 = (normalized2 * enter2).x;
    float y1 = (normalized3 * enter3).y;
    float y2 = (normalized4 * enter4).y;
    double right = (double) x2;
    double bottom = (double) y2;
    double top = (double) y1;
    double near1 = (double) near;
    double far1 = (double) far;
    return (Matrix4x4) float4x4.PerspectiveOffCenter((float) x1, (float) right, (float) bottom, (float) top, (float) near1, (float) far1);
  }

  public void Update()
  {
  }

  private static Matrix4x4 PerspectiveOffCenter(
    float left,
    float right,
    float bottom,
    float top,
    float near,
    float far)
  {
    float num1 = (float) (2.0 * (double) near / ((double) right - (double) left));
    float num2 = (float) (2.0 * (double) near / ((double) top - (double) bottom));
    float num3 = (float) (((double) right + (double) left) / ((double) right - (double) left));
    float num4 = (float) (((double) top + (double) bottom) / ((double) top - (double) bottom));
    float num5 = (float) (-((double) far + (double) near) / ((double) far - (double) near));
    float num6 = (float) (-(2.0 * (double) far * (double) near) / ((double) far - (double) near));
    float num7 = -1f;
    return new Matrix4x4()
    {
      [0, 0] = num1,
      [0, 1] = 0.0f,
      [0, 2] = num3,
      [0, 3] = 0.0f,
      [1, 0] = 0.0f,
      [1, 1] = num2,
      [1, 2] = num4,
      [1, 3] = 0.0f,
      [2, 0] = 0.0f,
      [2, 1] = 0.0f,
      [2, 2] = num5,
      [2, 3] = num6,
      [3, 0] = 0.0f,
      [3, 1] = 0.0f,
      [3, 2] = num7,
      [3, 3] = 0.0f
    };
  }
}
