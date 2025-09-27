// Decompiled with JetBrains decompiler
// Type: VoiceObscuranceFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Unity.Mathematics;
using UnityEngine;

#nullable disable
public class VoiceObscuranceFilter : MonoBehaviour
{
  public LayerMask layer;
  private RaycastHit hit;
  public Transform head;
  public AudioLowPassFilter lowPass;
  public AudioReverbFilter reverb;
  public AudioEchoFilter echo;
  public float reverbAddition;
  private Animator anim;

  private void Start()
  {
    this.anim = this.GetComponent<Animator>();
    if ((bool) (Object) GameObject.Find("Airport"))
    {
      this.lowPass.enabled = false;
      this.echo.enabled = false;
      this.reverb.enabled = false;
    }
    if (!(bool) (Object) this.head)
      this.head = MainCamera.instance.transform;
    if (!(bool) (Object) this.head)
      return;
    this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(this.transform.position));
    this.lowPass.cutoffFrequency = !Physics.Linecast(this.transform.position, this.head.position, out this.hit, (int) this.layer) ? Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 100f * Time.deltaTime) : Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 100f * Time.deltaTime);
    if ((double) Vector3.Distance(this.transform.position, this.head.position) > 60.0)
    {
      if ((Object) this.anim != (Object) null)
        this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
      this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
      this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
      this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
      this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
    }
    else
    {
      if ((Object) this.anim != (Object) null)
        this.anim.SetFloat("Obscurance", this.reverbAddition);
      this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.0f, 1f * Time.deltaTime);
      this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
      this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.0f, 1f * Time.deltaTime);
      this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
    }
  }

  private void Update()
  {
    if (!(bool) (Object) this.head)
      this.head = MainCamera.instance.transform;
    if (!(bool) (Object) this.head)
      return;
    this.reverbAddition = math.saturate(LightVolume.Instance().SamplePositionAlpha(this.transform.position));
    this.lowPass.cutoffFrequency = !Physics.Linecast(this.transform.position, this.head.position, out this.hit, (int) this.layer) ? Mathf.Lerp(this.lowPass.cutoffFrequency, 7500f, 1f * Time.deltaTime) : Mathf.Lerp(this.lowPass.cutoffFrequency, 1500f, 1f * Time.deltaTime);
    if ((double) Vector3.Distance(this.transform.position, this.head.position) > 60.0)
    {
      if ((Object) this.anim != (Object) null)
        this.anim.SetFloat("Obscurance", 1f, Time.deltaTime, 0.5f);
      this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.35f, 5f * Time.deltaTime);
      this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 0.5f, 5f * Time.deltaTime);
      this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.3f, 5f * Time.deltaTime);
      this.echo.delay = Mathf.Lerp(this.echo.delay, 500f, 5f * Time.deltaTime);
    }
    else
    {
      if ((Object) this.anim != (Object) null)
        this.anim.SetFloat("Obscurance", this.reverbAddition);
      this.echo.wetMix = Mathf.Lerp(this.echo.wetMix, 0.0f, 1f * Time.deltaTime);
      this.echo.dryMix = Mathf.Lerp(this.echo.dryMix, 1f, 1f * Time.deltaTime);
      this.echo.decayRatio = Mathf.Lerp(this.echo.decayRatio, 0.0f, 1f * Time.deltaTime);
      this.echo.delay = Mathf.Lerp(this.echo.delay, 10f, 1f * Time.deltaTime);
    }
  }
}
