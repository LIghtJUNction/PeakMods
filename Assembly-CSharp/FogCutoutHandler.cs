// Decompiled with JetBrains decompiler
// Type: FogCutoutHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class FogCutoutHandler : MonoBehaviour
{
  public FogCutoutZone[] cutoutZones;
  private FogCutoutZone currentCutoutZone;
  public int index;
  public float fadeTime = 1f;

  public void debugCurrentCutoutZone() => this.setFogCutoutZone(this.index);

  private void OnEnable() => this.setFogCutoutZone(0);

  private void Update()
  {
    if (!(bool) (Object) Character.localCharacter || (double) Character.localCharacter.Center.z <= (double) this.currentCutoutZone.transform.position.z + (double) this.currentCutoutZone.transitionPoint || this.index >= this.cutoutZones.Length)
      return;
    this.setFogCutoutZone(this.index);
    ++this.index;
  }

  public void setFogCutoutZone(int zone)
  {
    this.StartCoroutine(changeZoneRoutine());
    this.currentCutoutZone = this.cutoutZones[zone];

    IEnumerator changeZoneRoutine()
    {
      float normalizedTime = 0.0f;
      while ((double) normalizedTime < 1.0)
      {
        float a = 0.0f;
        if ((Object) this.currentCutoutZone != (Object) null)
          a = this.currentCutoutZone.amount;
        normalizedTime += Time.deltaTime / this.fadeTime;
        Shader.SetGlobalFloat("FogCutoutAmount", Mathf.Lerp(a, 0.0f, normalizedTime));
        yield return (object) null;
      }
      if (zone < this.cutoutZones.Length)
      {
        normalizedTime = 0.0f;
        Shader.SetGlobalVector("FogCutoutPosition", (Vector4) this.cutoutZones[zone].transform.position);
        Shader.SetGlobalFloat("FogCutoutMin", this.cutoutZones[zone].min);
        Shader.SetGlobalFloat("FogCutoutMax", this.cutoutZones[zone].max);
        while ((double) normalizedTime < 1.0)
        {
          normalizedTime += Time.deltaTime / this.fadeTime;
          Shader.SetGlobalFloat("FogCutoutAmount", Mathf.Lerp(0.0f, this.cutoutZones[zone].amount, normalizedTime));
          yield return (object) null;
        }
      }
    }
  }
}
