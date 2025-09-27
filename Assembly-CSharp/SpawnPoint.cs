// Decompiled with JetBrains decompiler
// Type: SpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class SpawnPoint : MonoBehaviour
{
  public int index;
  public bool startPassedOut;
  public static List<SpawnPoint> allSpawnPoints = new List<SpawnPoint>();

  private void Awake() => SpawnPoint.allSpawnPoints.Add(this);

  private void OnDestroy() => SpawnPoint.allSpawnPoints.Remove(this);
}
