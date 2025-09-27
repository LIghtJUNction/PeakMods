// Decompiled with JetBrains decompiler
// Type: SFX_Settings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class SFX_Settings
{
  [Range(0.0f, 1f)]
  public float volume = 0.5f;
  [Range(0.0f, 1f)]
  [Tooltip("0.2 variation means random between 80% of specified volume and 100% of specified volume")]
  public float volume_Variation = 0.2f;
  public float pitch = 1f;
  [Range(0.0f, 1f)]
  [Tooltip("0.1 variation means random between 95% of specified volume and 105% of specified volume")]
  public float pitch_Variation = 0.1f;
  [Range(0.0f, 1f)]
  public float spatialBlend = 1f;
  [Range(0.0f, 1f)]
  public float dopplerLevel = 1f;
  public float range = 150f;
  public float cooldown = 0.02f;
  public int maxInstances_NOT_IMPLEMENTED = 5;
}
