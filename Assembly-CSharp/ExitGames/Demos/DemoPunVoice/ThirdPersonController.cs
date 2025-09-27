// Decompiled with JetBrains decompiler
// Type: ExitGames.Demos.DemoPunVoice.ThirdPersonController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace ExitGames.Demos.DemoPunVoice;

public class ThirdPersonController : BaseController
{
  [SerializeField]
  private float movingTurnSpeed = 360f;

  protected override void Move(float h, float v)
  {
    this.rigidBody.linearVelocity = v * this.speed * this.transform.forward;
    this.transform.rotation *= Quaternion.AngleAxis(this.movingTurnSpeed * h * Time.deltaTime, Vector3.up);
  }
}
