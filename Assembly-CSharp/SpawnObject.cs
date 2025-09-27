// Decompiled with JetBrains decompiler
// Type: SpawnObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class SpawnObject
{
  public int maxCount;
  public GameObject prefab;
  public Vector3 inversion;
  public Vector3 randomRot;
  public Vector3 randomScale;
  public float uniformScale;
  public float scaleMultiplier = 1f;
  public Vector3 posJitter;
}
