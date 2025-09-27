// Decompiled with JetBrains decompiler
// Type: FRILerp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FRILerp : MonoBehaviour
{
  private void Start()
  {
  }

  public static Vector3 Lerp(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
  {
    return Vector3.Lerp(from, target, 1f - Mathf.Exp((float) (-(double) speed * (useTimeScale ? (double) Time.deltaTime : (double) Time.unscaledDeltaTime))));
  }

  public static Vector3 PLerp(Vector3 from, Vector3 target, float speed, float dt)
  {
    return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
  }

  public static Quaternion PLerp(Quaternion from, Quaternion target, float speed, float dt)
  {
    return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
  }

  public static float PLerp(float from, float target, float speed, float dt)
  {
    return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
  }

  public static Vector3 LerpFixed(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
  {
    return Vector3.Lerp(from, target, 1f - Mathf.Exp((float) (-(double) speed * (useTimeScale ? (double) Time.fixedDeltaTime : (double) Time.unscaledDeltaTime))));
  }

  public static Vector3 LerpUnclamped(Vector3 from, Vector3 target, float speed)
  {
    return Vector3.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
  }

  public static float Lerp(float from, float target, float speed, bool useTimeScale = true)
  {
    return Mathf.Lerp(from, target, 1f - Mathf.Exp((float) (-(double) speed * (useTimeScale ? (double) Time.fixedDeltaTime : (double) Time.unscaledDeltaTime))));
  }

  public static float LerpUnclamped(float from, float target, float speed)
  {
    return Mathf.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
  }

  public static Vector3 Slerp(Vector3 from, Vector3 target, float speed)
  {
    return Vector3.Slerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
  }

  public static Vector3 SlerpUnclamped(Vector3 from, Vector3 target, float speed)
  {
    return Vector3.SlerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
  }

  public static Quaternion Lerp(Quaternion from, Quaternion target, float speed)
  {
    return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
  }

  public static Quaternion LerpUnclamped(Quaternion from, Quaternion target, float speed)
  {
    return Quaternion.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
  }
}
