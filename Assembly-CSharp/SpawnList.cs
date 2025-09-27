// Decompiled with JetBrains decompiler
// Type: SpawnList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class SpawnList : MonoBehaviour
{
  public List<SpawnEntry> items;

  private void RefreshPercentageOdds()
  {
    int num = 0;
    foreach (SpawnEntry spawnEntry in this.items)
      num += spawnEntry.weight;
    foreach (SpawnEntry spawnEntry in this.items)
    {
      spawnEntry.percentageOdds = (float) spawnEntry.weight / (float) num;
      spawnEntry.percentageOdds = (float) Mathf.FloorToInt(spawnEntry.percentageOdds * 1000f) / 10f;
    }
  }

  private void SortByWeight()
  {
    this.items = this.items.OrderByDescending<SpawnEntry, int>((Func<SpawnEntry, int>) (item => item.weight)).ToList<SpawnEntry>();
  }

  public GameObject GetSingleSpawn()
  {
    return this.items.RandomSelection<SpawnEntry>((Func<SpawnEntry, int>) (i => i.weight)).prefab;
  }

  public List<GameObject> GetSpawns(int count, bool canRepeat = true)
  {
    List<SpawnEntry> enumerable = new List<SpawnEntry>((IEnumerable<SpawnEntry>) this.items);
    List<GameObject> spawns = new List<GameObject>();
    for (int index = 0; index < count; ++index)
    {
      SpawnEntry spawnEntry = enumerable.RandomSelection<SpawnEntry>((Func<SpawnEntry, int>) (i => i.weight));
      if (spawnEntry != null)
      {
        spawns.Add(spawnEntry.prefab);
        if (!canRepeat)
        {
          if (enumerable.Count <= 1)
            enumerable = new List<SpawnEntry>((IEnumerable<SpawnEntry>) this.items);
          enumerable.Remove(spawnEntry);
        }
      }
      else
        spawns.Add((GameObject) null);
    }
    return spawns;
  }

  private void FindReferencesInScene()
  {
    Spawner[] objectsOfType = UnityEngine.Object.FindObjectsOfType<Spawner>();
    bool flag = false;
    foreach (Spawner spawner in objectsOfType)
    {
      if (spawner.spawns.gameObject.name == this.gameObject.name)
      {
        Debug.Log((object) $"Found {this.gameObject.name} on {spawner.gameObject.name}");
        flag = true;
      }
    }
    if (flag)
      return;
    Debug.Log((object) (this.gameObject.name + " not present in scene."));
  }
}
