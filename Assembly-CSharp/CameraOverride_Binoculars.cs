// Decompiled with JetBrains decompiler
// Type: CameraOverride_Binoculars
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CameraOverride_Binoculars : CameraOverride
{
  public float minFov;
  public float maxFov;
  public float fovChangeRate;
  public float lerpedFOV;

  private void Start() => this.lerpedFOV = this.fov;

  private void Update()
  {
    if ((Object) Character.localCharacter == (Object) null)
      return;
    this.transform.rotation = Quaternion.LookRotation(Character.localCharacter.data.lookDirection);
    this.fov = Mathf.Lerp(this.fov, this.lerpedFOV, Time.deltaTime * 5f);
  }

  public void AdjustFOV(float value)
  {
    this.lerpedFOV += value;
    this.lerpedFOV = Mathf.Clamp(this.lerpedFOV, this.minFov, this.maxFov);
  }
}
