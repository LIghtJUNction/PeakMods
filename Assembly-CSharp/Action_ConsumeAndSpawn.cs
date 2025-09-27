// Decompiled with JetBrains decompiler
// Type: Action_ConsumeAndSpawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class Action_ConsumeAndSpawn : ItemAction
{
  public Item itemToSpawn;

  public override void RunAction()
  {
    if (!(bool) (Object) this.character)
      return;
    this.item.StartCoroutine(this.item.ConsumeDelayed());
    this.character.StartCoroutine(this.SpawnItemDelayed());
  }

  public IEnumerator SpawnItemDelayed()
  {
    Action_ConsumeAndSpawn actionConsumeAndSpawn = this;
    Character c = actionConsumeAndSpawn.character;
    Item item = actionConsumeAndSpawn.itemToSpawn;
    float timeout = 2f;
    while ((Object) actionConsumeAndSpawn != (Object) null)
    {
      timeout -= Time.deltaTime;
      if ((double) timeout <= 0.0)
        yield break;
      yield return (object) null;
    }
    GameUtils.instance.InstantiateAndGrab(item, c);
  }
}
