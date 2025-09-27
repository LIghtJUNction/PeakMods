// Decompiled with JetBrains decompiler
// Type: Mirage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class Mirage : MonoBehaviour
{
  [FormerlySerializedAs("particleSystem")]
  public ParticleSystem ps;
  public RenderTexture rt;
  private ParticleSystemRenderer psr;
  public bool hideObjects;
  public GameObject[] objectsToHide;
  public float hideDelay;
  public float fadeDistance = 10f;
  private List<Vector4> customData = new List<Vector4>();
  public bool hasFaded;

  public void fadeMirage()
  {
    if (Application.isPlaying)
      MirageManager.instance.sampleCamera();
    this.ps.Play();
    this.setParticleData();
    if (this.hideObjects)
      this.StartCoroutine(this.HideObject());
    this.hasFaded = true;
  }

  private IEnumerator HideObject()
  {
    yield return (object) new WaitForSeconds(this.hideDelay);
    for (int index = 0; index < this.objectsToHide.Length; ++index)
      this.objectsToHide[index].SetActive(false);
  }

  private void setParticleData()
  {
    this.StartCoroutine(particleDataRoutine());

    IEnumerator particleDataRoutine()
    {
      yield return (object) new WaitForEndOfFrame();
      this.ps.GetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
      ParticleSystem.Particle[] particles = new ParticleSystem.Particle[this.ps.particleCount];
      this.ps.GetParticles(particles);
      for (int index = 0; index < particles.Length; ++index)
      {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(particles[index].position);
        this.customData[index] = new Vector4(screenPoint.x / (float) Screen.width, screenPoint.y / (float) Screen.height, 0.0f, 1f);
      }
      this.ps.SetCustomParticleData(this.customData, ParticleSystemCustomData.Custom1);
    }
  }

  public void AssignRT(RenderTexture texture)
  {
    this.ps.GetComponent<ParticleSystemRenderer>().material.SetTexture("_MainTex", (Texture) texture);
  }

  private void Start() => this.psr = this.ps.GetComponent<ParticleSystemRenderer>();

  private void Update()
  {
    if ((Object) Character.observedCharacter == (Object) null || (double) Vector3.Distance(Character.observedCharacter.Center, this.transform.position) >= (double) this.fadeDistance || this.hasFaded)
      return;
    this.fadeMirage();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.DrawWireSphere(this.transform.position, this.fadeDistance);
  }
}
