// Decompiled with JetBrains decompiler
// Type: GamefeelHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using Zorro.Settings;

#nullable disable
public class GamefeelHandler : MonoBehaviour
{
  public static GamefeelHandler instance;
  private PhotosensitiveSetting setting;
  public RotationSpring stiff;
  public RotationSpring loose;
  public PerlinShake perlin;

  private void Awake() => GamefeelHandler.instance = this;

  private void Start()
  {
    this.setting = GameHandler.Instance.SettingsHandler.GetSetting<PhotosensitiveSetting>();
  }

  public Vector3 GetRotation()
  {
    Vector3 zero = Vector3.zero;
    for (int index = 0; index < this.transform.childCount; ++index)
      zero += this.transform.GetChild(index).localEulerAngles;
    return zero;
  }

  public void AddRotationShake_Local_Stiff(Vector3 force) => this.stiff.AddForce(force);

  public void AddRotationShake_Local_Loose(Vector3 force) => this.loose.AddForce(force);

  public void AddPerlinShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
  {
    if (this.setting.Value == OffOnMode.ON)
    {
      amount *= 0.5f;
      amount = Mathf.Min(amount, 3f);
    }
    this.perlin.AddShake(amount, duration, scale);
  }

  public void AddPerlinShakeProximity(
    Vector3 position,
    float amount = 1f,
    float duration = 0.2f,
    float scale = 15f,
    float maxProximity = 10f)
  {
    if (this.setting.Value == OffOnMode.ON)
    {
      amount *= 0.5f;
      amount = Mathf.Min(amount, 3f);
    }
    float num = 1f;
    if ((bool) (Object) Character.observedCharacter)
      num = 1f - Mathf.Clamp01(Vector3.Distance(Character.observedCharacter.Center, position) / maxProximity);
    this.perlin.AddShake(amount * num, duration, scale);
  }
}
