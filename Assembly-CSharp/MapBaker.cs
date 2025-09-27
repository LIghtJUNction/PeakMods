// Decompiled with JetBrains decompiler
// Type: MapBaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Core;
using Zorro.Core.Editor;

#nullable disable
[CreateAssetMenu(menuName = "Peak/MapBaker")]
public class MapBaker : SingletonAsset<MapBaker>
{
  public int DesiredAmountOfLevels = 2;
  public int DesiredAmountOfLevelsAppended = 2;
  public string[] AllLevels;
  public List<string> BiomeIDs;

  public void GenerateMaps()
  {
  }

  public void AppendMaps()
  {
  }

  private void GenerateMap(int i)
  {
  }

  public string GetLevel(int levelIndex)
  {
    if (this.AllLevels.Length == 0)
    {
      Debug.LogError((object) "No levels found, using WilIsland...");
      return "";
    }
    levelIndex %= this.AllLevels.Length;
    string name = PathUtil.WithoutExtensions(PathUtil.GetFileName(this.AllLevels[levelIndex]));
    if (!Application.isEditor || SceneManager.GetSceneByName(name).IsValid())
      return name;
    Debug.LogError((object) "level not loaded, using WilIsland...");
    return "";
  }

  public string GetBiomeID(int levelIndex)
  {
    if (this.BiomeIDs.Count == 0)
      return "";
    levelIndex %= this.BiomeIDs.Count;
    return this.BiomeIDs[levelIndex];
  }
}
