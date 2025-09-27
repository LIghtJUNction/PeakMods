// Decompiled with JetBrains decompiler
// Type: LavaPost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class LavaPost : MonoBehaviour
{
  private MeshRenderer rend;
  public Transform lava1;
  public Transform lava2;
  private float lava1Height;
  private float lava2Height;
  public Transform thresholdTransform;
  public Transform lavaFadeIn;
  public Transform lavaStart;
  private float alpha;
  private bool firstIsActive;
  private float currentLavaHeight;
  private float lastLavaHeight;
  private bool blending;

  private void Start()
  {
    this.rend = this.GetComponent<MeshRenderer>();
    Shader.SetGlobalFloat("LavaAlpha", 0.0f);
    this.lava1Height = this.lava1.position.y;
    this.lava2Height = this.lava2.position.y;
    this.currentLavaHeight = this.lava1Height;
    this.lastLavaHeight = this.lava2Height;
  }

  private void LateUpdate()
  {
    if ((Object) this.lava1 == (Object) null)
      return;
    bool flag = (double) MainCamera.instance.transform.position.z < (double) this.thresholdTransform.position.z;
    if (this.firstIsActive != flag)
    {
      this.alpha = Mathf.MoveTowards(this.alpha, 0.0f, Time.deltaTime);
      if ((double) this.alpha < 1.0 / 1000.0)
        this.firstIsActive = flag;
    }
    else
      this.alpha = Mathf.MoveTowards(this.alpha, 1f, Time.deltaTime);
    float newLavaHeight = this.firstIsActive ? this.lava1Height : this.lava2Height;
    if ((double) this.lastLavaHeight != (double) newLavaHeight)
    {
      this.lastLavaHeight = newLavaHeight;
      this.StopAllCoroutines();
      this.StartCoroutine(this.lavaMove(newLavaHeight));
    }
    if (!this.blending)
      Shader.SetGlobalFloat("LavaHeight", this.firstIsActive ? this.lava1.position.y : this.lava2.position.y);
    if ((double) MainCamera.instance.transform.position.z < (double) this.lavaFadeIn.position.z)
      this.rend.enabled = false;
    else
      this.rend.enabled = true;
    Shader.SetGlobalFloat("LavaStart", this.lavaStart.position.z);
  }

  public IEnumerator lavaMove(float newLavaHeight)
  {
    this.blending = true;
    float normalizedTime = 0.0f;
    while ((double) normalizedTime < 1.0)
    {
      this.currentLavaHeight = Mathf.Lerp(this.currentLavaHeight, newLavaHeight, normalizedTime / 3f);
      Shader.SetGlobalFloat("LavaHeight", this.currentLavaHeight);
      normalizedTime += Time.deltaTime;
      yield return (object) null;
    }
    this.blending = false;
  }
}
