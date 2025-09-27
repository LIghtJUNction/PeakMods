// Decompiled with JetBrains decompiler
// Type: SpineCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
public class SpineCheck : CustomSpawnCondition
{
  public HelperFunctions.LayerType layerType;
  public UnityEvent successEvent;

  public override bool CheckCondition(PropSpawner.SpawnData data)
  {
    Transform transform = this.transform.Find("Spine");
    for (int index = 0; index < transform.childCount - 1; ++index)
    {
      if ((bool) (Object) HelperFunctions.LineCheck(transform.GetChild(index).position, transform.GetChild(index + 1).position, this.layerType).transform)
        return false;
    }
    this.successEvent.Invoke();
    return true;
  }
}
