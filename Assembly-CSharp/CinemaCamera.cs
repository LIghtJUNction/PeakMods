// Decompiled with JetBrains decompiler
// Type: CinemaCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CinemaCamera : MonoBehaviour
{
  public bool on;
  public Transform cam;
  private GameObject rootObj;
  private Fog fog;
  private GUIManager gM;
  private Camera oldCam;
  private Vector3 vel;
  private Vector3 rot;
  private bool t;
  public Transform ambience;

  private void Start()
  {
    this.fog = Object.FindFirstObjectByType<Fog>();
    this.gM = Object.FindAnyObjectByType<GUIManager>();
    this.oldCam = Camera.main;
    this.rootObj = this.transform.root.gameObject;
  }

  private void Update()
  {
    if (this.t)
      this.rootObj.SetActive(false);
    if ((Application.isEditor || Debug.isDebugBuild) && Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.M))
      this.on = true;
    if (!this.on)
      return;
    this.ambience.parent = this.transform;
    if ((bool) (Object) this.fog)
      this.fog.gameObject.SetActive(false);
    if ((bool) (Object) this.oldCam)
      this.oldCam.gameObject.SetActive(false);
    this.transform.parent = (Transform) null;
    this.cam.parent = (Transform) null;
    this.cam.gameObject.SetActive(true);
    this.vel = Vector3.Lerp(this.vel, Vector3.zero, 1f * Time.deltaTime);
    this.rot = Vector3.Lerp(this.rot, Vector3.zero, 2.5f * Time.deltaTime);
    float num = 0.05f;
    this.rot.y += (float) ((double) Input.GetAxis("Mouse X") * (double) num * 0.05000000074505806);
    this.rot.x += (float) ((double) Input.GetAxis("Mouse Y") * (double) num * 0.05000000074505806);
    if (Input.GetKey(KeyCode.D))
      this.vel.x += num * Time.deltaTime;
    if (Input.GetKey(KeyCode.A))
      this.vel.x -= num * Time.deltaTime;
    if (Input.GetKey(KeyCode.W))
      this.vel.z += num * Time.deltaTime;
    if (Input.GetKey(KeyCode.S))
      this.vel.z -= num * Time.deltaTime;
    if (Input.GetKey(KeyCode.Space))
      this.vel.y += num * Time.deltaTime;
    if (Input.GetKey(KeyCode.LeftControl))
      this.vel.y -= num * Time.deltaTime;
    if (Input.GetKey(KeyCode.CapsLock))
      this.vel = Vector3.Lerp(this.vel, Vector3.zero, 5f * Time.deltaTime);
    this.cam.transform.Rotate(Vector3.up * this.rot.y, Space.World);
    this.cam.transform.Rotate(this.transform.right * -this.rot.x);
    this.cam.transform.Translate(Vector3.right * this.vel.x, Space.Self);
    this.cam.transform.Translate(Vector3.forward * this.vel.z, Space.Self);
    this.cam.transform.Translate(Vector3.up * this.vel.y, Space.World);
    this.t = true;
  }
}
