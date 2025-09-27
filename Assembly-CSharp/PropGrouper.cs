// Decompiled with JetBrains decompiler
// Type: PropGrouper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PropGrouper : MonoBehaviour
{
  public PropGrouper.PropGrouperTiming timing;

  public void RunAll(bool updateLightmap = true)
  {
    if (!this.Verify())
      return;
    this.ClearAll();
    LevelGenStep[] componentsInChildren = this.GetComponentsInChildren<LevelGenStep>();
    List<LevelGenStep> levelGenStepList = new List<LevelGenStep>();
    // ISSUE: variable of a compiler-generated type
    PropGrouper.\u003C\u003Ec__DisplayClass2_0 cDisplayClass20;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass20.late = new List<LevelGenStep>();
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      switch (componentsInChildren[index].GetComponentInParent<PropGrouper>().timing)
      {
        case PropGrouper.PropGrouperTiming.Early:
          levelGenStepList.Add(componentsInChildren[index]);
          break;
        case PropGrouper.PropGrouperTiming.Late:
          // ISSUE: reference to a compiler-generated field
          cDisplayClass20.late.Add(componentsInChildren[index]);
          break;
      }
    }
    foreach (LevelGenStep levelGenStep in levelGenStepList)
      levelGenStep.Go();
  }

  private bool Verify()
  {
    foreach (PropSpawner componentsInChild in this.GetComponentsInChildren<PropSpawner>())
    {
      if (componentsInChild.props == null)
      {
        Debug.LogError((object) ("Missing spawns on " + componentsInChild.name), (Object) componentsInChild.gameObject);
        return false;
      }
      foreach (Object prop in componentsInChild.props)
      {
        if (prop == (Object) null)
        {
          Debug.LogError((object) ("Missing prefab on " + componentsInChild.name), (Object) componentsInChild.gameObject);
          return false;
        }
      }
    }
    return true;
  }

  public void ClearAll()
  {
    LevelGenStep[] componentsInChildren = this.GetComponentsInChildren<LevelGenStep>();
    int num = 0;
    foreach (LevelGenStep levelGenStep in componentsInChildren)
    {
      if (!((Object) levelGenStep == (Object) null))
      {
        levelGenStep.Clear();
        ++num;
      }
    }
  }

  public enum PropGrouperTiming
  {
    Early,
    Late,
  }
}
