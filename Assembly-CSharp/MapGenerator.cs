// Decompiled with JetBrains decompiler
// Type: MapGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MapGenerator : MonoBehaviour
{
  public int seed;
  public List<MapGenerationStage> stages;

  public void GenerateAll()
  {
    if (this.seed != 0)
    {
      Debug.Log((object) "Set Seed");
      Random.InitState(this.seed);
    }
    for (int index = 0; index < this.stages.Count; ++index)
    {
      if (this.stages[index].gameObject.activeInHierarchy)
      {
        this.stages[index].Generate();
        Debug.Log((object) $"{index.ToString()} {Random.state.GetHashCode().ToString()}");
      }
    }
  }

  public void ClearAll()
  {
    for (int index = 0; index < this.stages.Count; ++index)
    {
      if (this.stages[index].gameObject.activeInHierarchy)
        this.stages[index].ClearSpawnedObjects();
    }
  }
}
