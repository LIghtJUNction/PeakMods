// Decompiled with JetBrains decompiler
// Type: Vector3Extensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class Vector3Extensions
{
  public static Vector2 XZ(this Vector3 vector) => new Vector2(vector.x, vector.z);

  public static Vector3 Flat(this Vector3 vector) => new Vector3(vector.x, 0.0f, vector.z);

  public static bool Same(this Vector3 v1, Vector3 v2, float threshold = 0.01f)
  {
    return (double) Mathf.Abs(v1.x - v2.x) < (double) threshold && (double) Mathf.Abs(v1.y - v2.y) < (double) threshold && (double) Mathf.Abs(v1.z - v2.z) < (double) threshold;
  }
}
