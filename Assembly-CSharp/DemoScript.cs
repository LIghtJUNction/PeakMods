// Decompiled with JetBrains decompiler
// Type: DemoScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class DemoScript : MonoBehaviour
{
  public List<GameObject> Mirrors;
  public GameObject LightBulb;
  public Toggle RecursionToggle;
  private float rotationModifier = -1f;
  private float moveModifier = 1f;
  private Material lightBulbMaterial;
  private DemoScript.RotationAxes axes;
  private float sensitivityX = 15f;
  private float sensitivityY = 15f;
  private float minimumX = -360f;
  private float maximumX = 360f;
  private float minimumY = -60f;
  private float maximumY = 60f;
  private float rotationX;
  private float rotationY;
  private Quaternion originalRotation;

  private void Start()
  {
    this.originalRotation = this.transform.localRotation;
    Renderer component = this.LightBulb.GetComponent<Renderer>();
    if (Application.isPlaying)
      component.sharedMaterial = component.material;
    this.lightBulbMaterial = component.sharedMaterial;
  }

  private void Update()
  {
    this.RotateMirror();
    this.MoveLightBulb();
    this.UpdateMouseLook();
    this.UpdateMovement();
  }

  public void MirrorRecursionToggled() => this.ChangeMirrorRecursion();

  public void ChangeMirrorRecursion()
  {
    foreach (GameObject mirror in this.Mirrors)
      mirror.GetComponent<MirrorScript>().MirrorRecursion = this.RecursionToggle.isOn;
  }

  private void UpdateMovement()
  {
    float num = 4f * Time.deltaTime;
    if (Input.GetKey(KeyCode.W))
      this.transform.Translate(0.0f, 0.0f, num);
    else if (Input.GetKey(KeyCode.S))
      this.transform.Translate(0.0f, 0.0f, -num);
    if (Input.GetKey(KeyCode.A))
      this.transform.Translate(-num, 0.0f, 0.0f);
    else if (Input.GetKey(KeyCode.D))
      this.transform.Translate(num, 0.0f, 0.0f);
    if (!Input.GetKeyDown(KeyCode.M))
      return;
    this.RecursionToggle.isOn = !this.RecursionToggle.isOn;
  }

  private void RotateMirror()
  {
    GameObject mirror = this.Mirrors[0];
    float y = mirror.transform.rotation.eulerAngles.y;
    if ((double) y > 65.0 && (double) y < 100.0)
    {
      this.rotationModifier = -this.rotationModifier;
      float num = y - 65f;
      mirror.transform.Rotate(0.0f, -num, 0.0f);
    }
    else if ((double) y > 100.0 && (double) y < 295.0)
    {
      this.rotationModifier = -this.rotationModifier;
      float yAngle = 295f - y;
      mirror.transform.Rotate(0.0f, yAngle, 0.0f);
    }
    else
      mirror.transform.Rotate(0.0f, (float) ((double) this.rotationModifier * (double) Time.deltaTime * 20.0), 0.0f);
  }

  private void MoveLightBulb()
  {
    float x1 = this.LightBulb.transform.position.x;
    float x2;
    if ((double) x1 > 5.0)
    {
      this.moveModifier = -this.moveModifier;
      x2 = 5f;
    }
    else if ((double) x1 < -5.0)
    {
      this.moveModifier = -this.moveModifier;
      x2 = -5f;
    }
    else
      x2 = x1 + Time.deltaTime * this.moveModifier;
    Light component = this.LightBulb.GetComponent<Light>();
    this.LightBulb.transform.position = new Vector3(x2, this.LightBulb.transform.position.y, this.LightBulb.transform.position.z);
    float num = Mathf.Min(1f, component.intensity);
    this.lightBulbMaterial.SetColor("_EmissionColor", new Color(num, num, num));
  }

  private void UpdateMouseLook()
  {
    if (this.axes == DemoScript.RotationAxes.MouseXAndY)
    {
      this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
      this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
      this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
      this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
      this.transform.localRotation = this.originalRotation * Quaternion.AngleAxis(this.rotationX, Vector3.up) * Quaternion.AngleAxis(this.rotationY, -Vector3.right);
    }
    else if (this.axes == DemoScript.RotationAxes.MouseX)
    {
      this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
      this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
      this.transform.localRotation = this.originalRotation * Quaternion.AngleAxis(this.rotationX, Vector3.up);
    }
    else
    {
      this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
      this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
      this.transform.localRotation = this.originalRotation * Quaternion.AngleAxis(-this.rotationY, Vector3.right);
    }
  }

  public static float ClampAngle(float angle, float min, float max)
  {
    if ((double) angle < -360.0)
      angle += 360f;
    if ((double) angle > 360.0)
      angle -= 360f;
    return Mathf.Clamp(angle, min, max);
  }

  private enum RotationAxes
  {
    MouseXAndY,
    MouseX,
    MouseY,
  }
}
