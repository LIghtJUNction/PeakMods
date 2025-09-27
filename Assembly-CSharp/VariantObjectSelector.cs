// Decompiled with JetBrains decompiler
// Type: VariantObjectSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class VariantObjectSelector : MonoBehaviour, IGenConfigStep
{
  public void RunStep() => this.SelectVariations();

  public void SelectVariations()
  {
    VariantObject[] componentsInChildren = this.GetComponentsInChildren<VariantObject>(true);
    int message = Mathf.RoundToInt(5f * Mathf.Pow(Random.value, 6f));
    if ((double) Random.value < 0.5 && message == 0)
      message = 1;
    Debug.Log((object) message);
    List<VariantObject> unPicked = new List<VariantObject>();
    unPicked.AddRange((IEnumerable<VariantObject>) componentsInChildren);
    List<VariantObject> variantObjectList = new List<VariantObject>();
    while (message > 0)
    {
      --message;
      VariantObject randomVariant = this.GetRandomVariant(unPicked);
      if (!((Object) randomVariant == (Object) null))
      {
        Debug.Log((object) randomVariant.gameObject.name, (Object) randomVariant.gameObject);
        variantObjectList.Add(randomVariant);
        unPicked.Remove(randomVariant);
      }
    }
    foreach (Component component in variantObjectList)
      component.gameObject.SetActive(true);
  }

  private VariantObject GetRandomVariant(List<VariantObject> unPicked)
  {
    float maxInclusive = 0.0f;
    foreach (VariantObject variantObject in unPicked)
      maxInclusive += variantObject.spawnChance;
    float num = Random.Range(0.0f, maxInclusive);
    foreach (VariantObject randomVariant in unPicked)
    {
      num -= randomVariant.spawnChance;
      if ((double) num <= 9.9999997473787516E-05)
        return randomVariant;
    }
    return (VariantObject) null;
  }
}
