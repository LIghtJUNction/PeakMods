// Decompiled with JetBrains decompiler
// Type: Action_Torch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Action_Torch : OnItemStateChangedAction
{
  public ParticleSystem[] particles;
  public Light torchLight;
  public AnimationCurve lightCurve;
  public float lightSpeed = 1f;
  public float lightIntensity = 10f;

  public override void RunAction(ItemState state)
  {
    if (state != ItemState.Held)
      return;
    for (int index = 0; index < this.particles.Length; ++index)
    {
      ParticleSystem.MainModule main = this.particles[index].main;
      Debug.Log((object) ("char is null? " + ((Object) this.character == (Object) null).ToString()));
      main.customSimulationSpace = this.character.refs.animationPositionTransform;
    }
  }

  private void Update()
  {
    this.torchLight.intensity = this.lightCurve.Evaluate(Time.time * this.lightSpeed) * this.lightIntensity;
  }
}
