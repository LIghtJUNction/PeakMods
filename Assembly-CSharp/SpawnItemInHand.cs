// Decompiled with JetBrains decompiler
// Type: SpawnItemInHand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class SpawnItemInHand : MonoBehaviour
{
  public Item item;

  private IEnumerator Start()
  {
    while (!(bool) (Object) Character.localCharacter)
      yield return (object) null;
    yield return (object) null;
    yield return (object) null;
    yield return (object) null;
    yield return (object) new WaitForSeconds(1.5f);
    Character.localCharacter.refs.items.SpawnItemInHand(this.item.gameObject.name);
  }
}
