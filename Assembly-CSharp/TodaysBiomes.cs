// Decompiled with JetBrains decompiler
// Type: TodaysBiomes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class TodaysBiomes : MonoBehaviour
{
  public GameObject shoreIcon;
  public GameObject tropicsIcon;
  public GameObject alpineIcon;
  public GameObject mesaIcon;
  public GameObject kilnIcon;

  private void Start()
  {
    this.shoreIcon.SetActive(false);
    this.tropicsIcon.SetActive(false);
    this.alpineIcon.SetActive(false);
    this.mesaIcon.SetActive(false);
    this.kilnIcon.SetActive(false);
    this.StartCoroutine(TrySetBiomes());

    IEnumerator TrySetBiomes()
    {
      bool set = false;
      while (!set)
      {
        NextLevelService service = GameHandler.GetService<NextLevelService>();
        if (service.Data.IsSome)
        {
          this.SetBiomes(SingletonAsset<MapBaker>.Instance.GetBiomeID(service.Data.Value.CurrentLevelIndex));
          set = true;
        }
        else
          yield return (object) new WaitForSeconds(1f);
      }
    }
  }

  private void SetBiomes(string biomes)
  {
    if (biomes.Contains('S'))
      this.shoreIcon.SetActive(true);
    if (biomes.Contains('T'))
      this.tropicsIcon.SetActive(true);
    if (biomes.Contains('A'))
      this.alpineIcon.SetActive(true);
    if (biomes.Contains('M'))
      this.mesaIcon.SetActive(true);
    if (!biomes.Contains('K'))
      return;
    this.kilnIcon.SetActive(true);
  }
}
