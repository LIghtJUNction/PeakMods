// Decompiled with JetBrains decompiler
// Type: DecorSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class DecorSpawner : LevelGenStep
{
  public GameObject[] props;
  [Range(0.0f, 1f)]
  public float overallSpawnChance;
  public Vector2Int minMaxSpawn;
  public Transform[] spawnPoints;
  private List<GameObject> spawnedObjects = new List<GameObject>();
  [Header("Spawn Customization")]
  public Vector2 scaleMinMax = Vector2.one;

  public override void Go()
  {
    this.Clear();
    this.Add();
  }

  public void Add()
  {
    if ((double) this.overallSpawnChance < 0.99900001287460327 && (double) Random.value > (double) this.overallSpawnChance)
      return;
    int num = Random.Range(this.minMaxSpawn.x, this.minMaxSpawn.y);
    if (num > this.spawnPoints.Length)
      num = this.spawnPoints.Length;
    if (num < 0)
      num = 0;
    List<Vector3> vector3List = new List<Vector3>();
    for (int index = 0; index < this.spawnPoints.Length; ++index)
      vector3List.Add(this.spawnPoints[index].position);
    for (int index1 = 0; index1 < num; ++index1)
    {
      int index2 = Random.Range(0, vector3List.Count);
      GameObject gameObject = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], vector3List[index2], HelperFunctions.GetRandomRotationWithUp(Vector3.up), this.transform);
      gameObject.transform.localScale *= Random.RandomRange(this.scaleMinMax.x, this.scaleMinMax.y);
      vector3List.RemoveAt(index2);
      this.spawnedObjects.Add(gameObject);
    }
  }

  public override void Clear()
  {
    for (int index = 0; index < this.spawnedObjects.Count; ++index)
      Object.DestroyImmediate((Object) this.spawnedObjects[index]);
    this.spawnedObjects.Clear();
  }

  public void getSpawnSpots()
  {
    LazyGizmo[] componentsInChildren = this.GetComponentsInChildren<LazyGizmo>();
    this.spawnPoints = new Transform[componentsInChildren.Length];
    for (int index = 0; index < componentsInChildren.Length; ++index)
      this.spawnPoints[index] = componentsInChildren[index].transform;
  }
}
