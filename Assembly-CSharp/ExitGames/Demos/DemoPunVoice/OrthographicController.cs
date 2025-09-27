// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.OrthographicController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

public class OrthographicController : ThirdPersonController
{
  public float smoothing = 5f;
  private Vector3 offset;

  protected override void Init()
  {
    base.Init();
    this.ControllerCamera = Camera.main;
  }

  protected override void SetCamera()
  {
    base.SetCamera();
    this.offset = this.camTrans.position - this.transform.position;
  }

  protected override void Move(float h, float v)
  {
    base.Move(h, v);
    this.CameraFollow();
  }

  private void CameraFollow()
  {
    this.camTrans.position = Vector3.Lerp(this.camTrans.position, this.transform.position + this.offset, this.smoothing * Time.deltaTime);
  }
}
