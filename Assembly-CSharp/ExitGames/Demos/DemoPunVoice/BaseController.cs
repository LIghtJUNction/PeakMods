// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.BaseController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

[RequireComponent(typeof (PhotonView))]
[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (Animator))]
public abstract class BaseController : MonoBehaviour
{
  public Camera ControllerCamera;
  protected Rigidbody rigidBody;
  protected Animator animator;
  protected Transform camTrans;
  private float h;
  private float v;
  [SerializeField]
  protected float speed = 5f;
  [SerializeField]
  private float cameraDistance;

  protected virtual void OnEnable()
  {
    ChangePOV.CameraChanged += new ChangePOV.OnCameraChanged(this.ChangePOV_CameraChanged);
  }

  protected virtual void OnDisable()
  {
    ChangePOV.CameraChanged -= new ChangePOV.OnCameraChanged(this.ChangePOV_CameraChanged);
  }

  protected virtual void ChangePOV_CameraChanged(Camera camera)
  {
    if ((Object) camera != (Object) this.ControllerCamera)
    {
      this.enabled = false;
      this.HideCamera(this.ControllerCamera);
    }
    else
      this.ShowCamera(this.ControllerCamera);
  }

  protected virtual void Start()
  {
    if (this.GetComponent<PhotonView>().IsMine)
    {
      this.Init();
      this.SetCamera();
    }
    else
      this.enabled = false;
  }

  protected virtual void Init()
  {
    this.rigidBody = this.GetComponent<Rigidbody>();
    this.animator = this.GetComponent<Animator>();
  }

  protected virtual void SetCamera()
  {
    this.camTrans = this.ControllerCamera.transform;
    this.camTrans.position += this.cameraDistance * this.transform.forward;
  }

  protected virtual void UpdateAnimator(float h, float v)
  {
    this.animator.SetBool("IsWalking", (double) h != 0.0 || (double) v != 0.0);
  }

  protected virtual void FixedUpdate()
  {
    this.h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
    this.v = CrossPlatformInputManager.GetAxisRaw("Vertical");
    this.UpdateAnimator(this.h, this.v);
    this.Move(this.h, this.v);
  }

  protected virtual void ShowCamera(Camera camera)
  {
    if (!((Object) camera != (Object) null))
      return;
    camera.gameObject.SetActive(true);
  }

  protected virtual void HideCamera(Camera camera)
  {
    if (!((Object) camera != (Object) null))
      return;
    camera.gameObject.SetActive(false);
  }

  protected abstract void Move(float h, float v);
}
