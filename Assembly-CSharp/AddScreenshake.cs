// Decompiled with JetBrains decompiler
// Type: AddScreenshake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class AddScreenshake : MonoBehaviour
{
  public float amount = 5f;
  public float duration = 0.3f;
  public float scale = 12f;
  public bool auto;
  public bool positional;
  public float range = 15f;

  private void Start()
  {
    if (!this.auto)
      return;
    this.Shake();
  }

  public void Shake()
  {
    if (this.positional)
      GamefeelHandler.instance.AddPerlinShakeProximity(this.transform.position, this.amount, this.duration, this.scale, this.range);
    else
      GamefeelHandler.instance.AddPerlinShake(this.amount, this.duration, this.scale);
  }
}
