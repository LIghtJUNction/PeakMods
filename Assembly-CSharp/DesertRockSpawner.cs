// Decompiled with JetBrains decompiler
// Type: DesertRockSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DesertRockSpawner : LevelGenStep
{
  public GameObject[] blockerObjects;
  public GameObject[] enterenceObjects;
  private Transform enterences;
  private Transform inside;

  public override void Clear()
  {
    this.GetRefs();
    for (int index1 = 0; index1 < this.enterences.childCount; ++index1)
    {
      Transform child = this.enterences.GetChild(index1);
      for (int index2 = child.childCount - 1; index2 >= 0; --index2)
        Object.DestroyImmediate((Object) child.GetChild(index2).gameObject);
    }
  }

  public override void Go()
  {
    bool flag = (double) Random.value < 0.5;
    this.Clear();
    int num = Random.Range(0, this.enterences.childCount);
    for (int index = 0; index < this.enterences.childCount; ++index)
    {
      Transform child = this.enterences.GetChild(index);
      if (index == num & flag)
      {
        HelperFunctions.InstantiatePrefab(this.enterenceObjects[Random.Range(0, this.enterenceObjects.Length)], child.position, child.rotation, child).transform.localScale = Vector3.one * 2f;
        this.inside.position = new Vector3(child.position.x, this.inside.position.y, child.position.z);
      }
      else
        HelperFunctions.InstantiatePrefab(this.blockerObjects[Random.Range(0, this.blockerObjects.Length)], child.position, child.rotation, child).transform.localScale = Vector3.one * 2f;
    }
  }

  private void GetRefs()
  {
    this.enterences = this.transform.Find("Enterences");
    this.inside = this.transform.Find("Inside");
  }
}
