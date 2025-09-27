// Decompiled with JetBrains decompiler
// Type: WobbleSpinBounce
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WobbleSpinBounce : MonoBehaviour
{
  public Transform target;
  [Header("Rotate")]
  public Vector3 rotateSpeed;
  public Vector3 wobbleSpeed;
  public Vector3 wobbleAmount;
  [Header("Position")]
  public Vector3 bounceSize;
  public Vector3 bounceSpeed;
  private Vector3 startPos;
  private Vector3 startRot;

  private void Start()
  {
    if ((Object) this.target == (Object) null)
      this.target = this.transform;
    this.startPos = this.target.position;
    this.startRot = this.transform.eulerAngles;
  }

  private void Update()
  {
    this.target.Rotate(this.rotateSpeed);
    if (this.bounceSize != Vector3.zero)
      this.target.transform.position = this.startPos + new Vector3(Mathf.Sin(Time.time * this.bounceSpeed.x) * this.bounceSize.x, Mathf.Sin(Time.time * this.bounceSpeed.y) * this.bounceSize.y, Mathf.Sin(Time.time * this.bounceSpeed.z) * this.bounceSize.z);
    if (!(this.wobbleAmount != Vector3.zero))
      return;
    this.target.transform.eulerAngles = this.startRot + new Vector3(Mathf.Sin(Time.time * this.wobbleSpeed.x) * this.wobbleAmount.x, Mathf.Sin(Time.time * this.wobbleSpeed.y) * this.wobbleAmount.y, Mathf.Sin(Time.time * this.wobbleSpeed.z) * this.wobbleAmount.z);
  }
}
