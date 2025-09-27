// Decompiled with JetBrains decompiler
// Type: MirageManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MirageManager : MonoBehaviour
{
  public static MirageManager instance;
  public Camera cam;
  private RenderTexture renderTexture;
  public int rtDownscale = 2;

  public void sampleCamera()
  {
    this.cam.transform.position = Camera.main.transform.position;
    this.cam.transform.rotation = Camera.main.transform.rotation;
    this.cam.fieldOfView = Camera.main.fieldOfView;
    this.cam.Render();
  }

  private void Awake()
  {
    this.renderTexture = this.cam.targetTexture;
    this.renderTexture.width = Screen.width / this.rtDownscale;
    this.renderTexture.height = Screen.height / this.rtDownscale;
    MirageManager.instance = this;
  }

  private void setParticleData()
  {
  }

  private void Update()
  {
  }
}
