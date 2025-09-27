// Decompiled with JetBrains decompiler
// Type: MagicBeanVine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MagicBeanVine : MonoBehaviour
{
  public Transform vineOriginTransform;
  public float maxWidth = 1.5f;
  public float maxLength = 20f;
  public float initialLength = 0.5f;
  private float currentLength = 0.01f;
  public float growingSpeed = 1f;
  public float rotationSpeed = 10f;
  public AnimationCurve xzScaleCurve;
  public AnimationCurve rotationSpeedCurve;

  private void Awake()
  {
    this.currentLength = this.initialLength;
    float num = this.xzScaleCurve.Evaluate(this.currentLength / this.maxLength) * this.maxWidth;
    this.vineOriginTransform.transform.localScale = new Vector3(num, this.currentLength, num);
  }

  private void FixedUpdate()
  {
    if ((double) this.currentLength >= (double) this.maxLength)
      return;
    this.currentLength = Mathf.MoveTowards(this.currentLength, this.maxLength, this.growingSpeed * Time.fixedDeltaTime);
    float time = this.currentLength / this.maxLength;
    float num = this.xzScaleCurve.Evaluate(time) * this.maxWidth;
    this.vineOriginTransform.transform.localScale = new Vector3(num, this.currentLength, num);
    this.vineOriginTransform.transform.Rotate(0.0f, this.rotationSpeed * this.rotationSpeedCurve.Evaluate(time), 0.0f);
  }
}
