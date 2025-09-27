// Decompiled with JetBrains decompiler
// Type: MainCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MainCamera : MonoBehaviour
{
  public static MainCamera instance;
  internal Camera cam;
  internal CameraOverride camOverride;
  private int sinceOverride = 10;

  private void Awake()
  {
    this.cam = this.GetComponent<Camera>();
    MainCamera.instance = this;
  }

  public void SetCameraOverride(CameraOverride setOverride)
  {
    this.camOverride = setOverride;
    this.sinceOverride = 0;
  }

  private void Update()
  {
    AudioListener.volume = Mathf.Lerp(AudioListener.volume, 1f, 0.1f * Time.deltaTime);
  }

  private void LateUpdate()
  {
  }
}
