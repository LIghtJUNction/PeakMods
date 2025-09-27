// Decompiled with JetBrains decompiler
// Type: Extens
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class Extens
{
  public static Vector3 EulerRescaled(this Quaternion quaternion)
  {
    Vector3 eulerAngles = quaternion.eulerAngles;
    return new Vector3(Mathf.Repeat(eulerAngles.x + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.y + 180f, 360f) - 180f, Mathf.Repeat(eulerAngles.z + 180f, 360f) - 180f);
  }

  public static Quaternion Inverse(this Quaternion quaterion) => Quaternion.Inverse(quaterion);
}
