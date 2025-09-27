// Decompiled with JetBrains decompiler
// Type: StormVisual
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class StormVisual : MonoBehaviour
{
  public ParticleSystem part;
  public MeshRenderer quadRend;
  private Material quadMat;
  private FogConfig fogConfig;
  public AudioLoop stormSFX;
  public bool playerInWindZone;
  private WindChillZone zone;
  public StormVisual.StormType stormType;
  public float windFactor;

  private void Start()
  {
    this.zone = this.GetComponentInParent<WindChillZone>();
    this.fogConfig = this.GetComponentInParent<FogConfig>();
    if (!(bool) (Object) this.quadRend)
      return;
    this.quadMat = this.quadRend.material;
  }

  private void LateUpdate()
  {
    this.playerInWindZone = this.zone.windActive && this.zone.characterInsideBounds;
    if (this.playerInWindZone)
    {
      if (!this.part.isPlaying)
        this.part.Play();
      this.windFactor = Mathf.Lerp(this.windFactor, Mathf.Clamp01(this.zone.hasBeenActiveFor * 0.2f), Time.deltaTime);
    }
    else
    {
      if (this.part.isPlaying)
        this.part.Stop();
      this.windFactor = Mathf.Lerp(this.windFactor, 0.0f, Time.deltaTime);
    }
    if (this.stormType == StormVisual.StormType.Rain)
      DayNightManager.instance.rainstormWindFactor = this.windFactor;
    else if (this.stormType == StormVisual.StormType.Snow)
      DayNightManager.instance.snowstormWindFactor = this.windFactor;
    if (this.zone.characterInsideBounds)
    {
      this.transform.position = Character.observedCharacter.Center;
      if (this.zone.currentWindDirection != Vector3.zero)
        this.transform.rotation = Quaternion.LookRotation(this.zone.currentWindDirection);
      else
        this.transform.rotation = Quaternion.identity;
      if ((bool) (Object) Character.observedCharacter.GetComponent<CharacterAnimations>())
      {
        if (this.gameObject.CompareTag("Storm"))
          Character.observedCharacter.GetComponent<CharacterAnimations>().stormAudio.stormVisual = this;
        if (this.gameObject.CompareTag("Rain"))
          Character.observedCharacter.GetComponent<CharacterAnimations>().stormAudio.rainVisual = this;
      }
      if ((bool) (Object) this.fogConfig && this.zone.windActive)
        this.fogConfig.SetFog();
      if (!(bool) (Object) this.quadMat)
        return;
      this.quadRend.enabled = true;
      this.quadMat.SetFloat("_Alpha", this.windFactor);
    }
    else
    {
      if (!(bool) (Object) this.quadRend)
        return;
      this.quadRend.enabled = false;
    }
  }

  public enum StormType
  {
    Rain,
    Snow,
  }
}
