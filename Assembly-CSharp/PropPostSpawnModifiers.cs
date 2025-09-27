// Decompiled with JetBrains decompiler
// Type: PropPostSpawnModifiers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PropPostSpawnModifiers : MonoBehaviour
{
  [SerializeReference]
  public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();

  public void ApplyModifiers()
  {
    PropSpawner[] componentsInChildren = this.GetComponentsInChildren<PropSpawner>();
    List<GameObject> gameObjectList = new List<GameObject>();
    foreach (Component component in componentsInChildren)
    {
      Transform transform = component.transform;
      int childCount = transform.childCount;
      for (int index = 0; index < childCount; ++index)
        gameObjectList.Add(transform.GetChild(index).gameObject);
    }
    foreach (GameObject gameObject in gameObjectList)
    {
      foreach (PropSpawnerMod modifier in this.modifiers)
      {
        GameObject spawned = gameObject;
        modifier.ModifyObject(spawned, new PropSpawner.SpawnData()
        {
          hit = new RaycastHit(),
          normal = Vector3.up,
          placement = Vector2.zero,
          pos = gameObject.transform.position,
          rayDir = Vector3.zero,
          spawnCount = 0,
          spawnerTransform = (Transform) null
        });
      }
    }
  }

  public enum PropModTiming
  {
    Early,
    Late,
  }
}
