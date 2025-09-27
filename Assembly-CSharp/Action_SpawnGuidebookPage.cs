// Decompiled with JetBrains decompiler
// Type: Action_SpawnGuidebookPage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class Action_SpawnGuidebookPage : ItemAction
{
  public List<GuidebookSpawnData> possiblePages;

  public override void RunAction()
  {
    if (!(bool) (Object) this.character)
      return;
    this.item.StartCoroutine(this.item.ConsumeDelayed());
    int indexChosen;
    this.character.StartCoroutine(this.SpawnPageDelayed(this.PickGuidebookPage(out indexChosen), indexChosen));
  }

  public IEnumerator SpawnPageDelayed(GuidebookSpawnData itemToSpawn, int index)
  {
    Action_SpawnGuidebookPage spawnGuidebookPage = this;
    Item itemToGrab = itemToSpawn.GetComponent<Item>();
    Character c = spawnGuidebookPage.character;
    float timeout = 2f;
    while ((Object) spawnGuidebookPage != (Object) null)
    {
      timeout -= Time.deltaTime;
      if ((double) timeout <= 0.0)
        yield break;
      yield return (object) null;
    }
    Singleton<AchievementManager>.Instance.TriggerSeenGuidebookPage(index);
    GameUtils.instance.InstantiateAndGrab(itemToGrab, c);
  }

  public GuidebookSpawnData PickGuidebookPage(out int indexChosen)
  {
    int nextPage = Singleton<AchievementManager>.Instance.GetNextPage();
    if (nextPage < 8)
    {
      indexChosen = nextPage;
      return this.possiblePages[indexChosen];
    }
    indexChosen = Random.Range(0, this.possiblePages.Count - 1);
    return this.possiblePages[indexChosen];
  }
}
