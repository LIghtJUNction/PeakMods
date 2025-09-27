// Decompiled with JetBrains decompiler
// Type: PerlinShake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PerlinShake : MonoBehaviour
{
  public List<PerlinShakeInstance> shakes = new List<PerlinShakeInstance>();

  private void Start()
  {
  }

  private void Update()
  {
    Vector2 zero = Vector2.zero;
    for (int index = this.shakes.Count - 1; index >= 0; --index)
    {
      zero.x += (float) (((double) Mathf.PerlinNoise(Time.time * this.shakes[index].scale, 0.0f) - 0.5) * (double) this.shakes[index].amount * ((double) this.shakes[index].duration / (double) this.shakes[index].startDuration));
      zero.y += (float) (((double) Mathf.PerlinNoise(0.0f, Time.time * this.shakes[index].scale) - 0.5) * (double) this.shakes[index].amount * ((double) this.shakes[index].duration / (double) this.shakes[index].startDuration));
      this.shakes[index].duration -= Time.deltaTime;
      if ((double) this.shakes[index].duration < 0.0)
        this.shakes.RemoveAt(index);
    }
    this.transform.localEulerAngles = (Vector3) zero;
  }

  public void AddShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
  {
    this.shakes.Add(new PerlinShakeInstance()
    {
      amount = amount,
      duration = duration,
      startDuration = duration,
      scale = scale
    });
  }

  public void AddShake(Vector3 pos, float amount = 1f, float duration = 0.2f, float scale = 15f, float range = 50f)
  {
    float num = Mathf.InverseLerp(range, 0.0f, Vector3.Distance(MainCamera.instance.transform.position, pos));
    if ((double) num <= 1.0 / 1000.0)
      return;
    this.AddShake(amount * num, duration * num, scale);
  }
}
