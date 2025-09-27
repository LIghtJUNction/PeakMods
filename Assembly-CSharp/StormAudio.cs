// Decompiled with JetBrains decompiler
// Type: StormAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class StormAudio : MonoBehaviour
{
  public AmbienceAudio aM;
  public AudioLoop loopStorm;
  public AudioLowPassFilter lPStorm;
  public AudioLoop loopRainHeavy;
  public AudioLoop loopRainSoft;
  public StormVisual stormVisual;
  public StormVisual rainVisual;

  private void Start()
  {
    GameObject gameObjectWithTag1 = GameObject.FindGameObjectWithTag("Storm");
    GameObject gameObjectWithTag2 = GameObject.FindGameObjectWithTag("Rain");
    if ((bool) (Object) gameObjectWithTag1)
      this.stormVisual = gameObjectWithTag1.GetComponent<StormVisual>();
    if (!(bool) (Object) gameObjectWithTag2)
      return;
    this.rainVisual = gameObjectWithTag2.GetComponent<StormVisual>();
  }

  private void Update()
  {
    if ((bool) (Object) this.stormVisual)
      this.StormPlay(this.stormVisual, this.loopStorm, this.lPStorm);
    this.RainPlay();
  }

  private void RainPlay()
  {
    this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.0f, Time.deltaTime * 0.25f);
    this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.0f, Time.deltaTime * 0.05f);
    if (!(bool) (Object) this.rainVisual)
      return;
    if (!this.rainVisual.playerInWindZone)
    {
      this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.0f, Time.deltaTime * 0.25f);
      this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.0f, Time.deltaTime * 0.05f);
    }
    if (!this.rainVisual.playerInWindZone || !(bool) (Object) this.aM)
      return;
    if ((double) this.aM.obstruction < 0.60000002384185791)
    {
      this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.25f, Time.deltaTime * 2f);
      this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.005f, Time.deltaTime * 2f);
    }
    if ((double) this.aM.obstruction < 0.60000002384185791)
      return;
    this.loopRainHeavy.volume = Mathf.Lerp(this.loopRainHeavy.volume, 0.15f, Time.deltaTime * 2f);
    this.loopRainSoft.volume = Mathf.Lerp(this.loopRainSoft.volume, 0.25f, Time.deltaTime * 2f);
  }

  private void StormPlay(StormVisual sV, AudioLoop aL, AudioLowPassFilter lFilter)
  {
    if (!(bool) (Object) sV || !(bool) (Object) aL || !(bool) (Object) lFilter)
      return;
    if (!sV.playerInWindZone)
    {
      aL.volume = Mathf.Lerp(aL.volume, 0.0f, Time.deltaTime * 0.25f);
      aL.pitch = Mathf.Lerp(aL.pitch, 0.25f, Time.deltaTime * 0.25f);
      lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
    }
    if (!sV.playerInWindZone)
      return;
    aL.pitch = Mathf.Lerp(aL.pitch, 1f, Time.deltaTime * 0.25f);
    if ((double) this.aM.obstruction >= 0.60000002384185791)
    {
      lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 500f, Time.deltaTime * 0.25f);
      aL.volume = Mathf.Lerp(aL.volume, 0.05f, Time.deltaTime * 0.25f);
    }
    else
    {
      lFilter.cutoffFrequency = Mathf.Lerp(lFilter.cutoffFrequency, 8000f, Time.deltaTime * 1f);
      aL.volume = Mathf.Lerp(aL.volume, 0.25f, Time.deltaTime * 0.25f);
    }
  }
}
