// Decompiled with JetBrains decompiler
// Type: PropSpawner_Line
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PropSpawner_Line : LevelGenStep
{
  public float height = 200f;
  public float rayLength = 5000f;
  public int nrOfSpawns = 500;
  public bool rayCastSpawn = true;
  public GameObject[] props;
  public bool syncTransforms = true;
  public HelperFunctions.LayerType layerType = HelperFunctions.LayerType.TerrainMap;
  [SerializeReference]
  public List<PropSpawnerMod> modifiers = new List<PropSpawnerMod>();
  [SerializeReference]
  public List<PropSpawnerConstraint> constraints = new List<PropSpawnerConstraint>();
  [SerializeReference]
  public List<PropSpawnerConstraintPost> postConstraints = new List<PropSpawnerConstraintPost>();

  private void OnDrawGizmosSelected()
  {
    Gizmos.DrawLine(this.transform.position - this.height * 0.5f * this.transform.up, this.transform.position + this.height * 0.5f * this.transform.up);
  }

  public override void Go()
  {
    this.Clear();
    this.Add();
  }

  public void Add()
  {
    int num1 = 50000;
    int num2 = 0;
    Physics.SyncTransforms();
    while (num2 < this.nrOfSpawns && num1 > 0)
    {
      --num1;
      if (this.TryToSpawn())
      {
        ++num2;
        if (this.syncTransforms)
          Physics.SyncTransforms();
      }
    }
  }

  public override void Clear()
  {
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) this.transform.GetChild(index).gameObject);
  }

  public void GoAll() => this.GetComponentInParent<PropGrouper>().RunAll();

  public void ClearAll() => this.GetComponentInParent<PropGrouper>().ClearAll();

  private bool TryToSpawn()
  {
    PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
    if (randomPoint == null)
      return false;
    for (int index = 0; index < this.constraints.Count; ++index)
    {
      if (!this.constraints[index].mute && !this.constraints[index].CheckConstraint(randomPoint))
        return false;
    }
    return (Object) this.Spawn(randomPoint) != (Object) null;
  }

  private GameObject Spawn(PropSpawner.SpawnData spawnData)
  {
    GameObject spawned = HelperFunctions.SpawnPrefab(this.props[Random.Range(0, this.props.Length)], spawnData.pos, HelperFunctions.GetRandomRotationWithUp(Vector3.up), this.transform);
    for (int index = 0; index < this.modifiers.Count; ++index)
    {
      if (!this.modifiers[index].mute)
        this.modifiers[index].ModifyObject(spawned, spawnData);
    }
    for (int index = 0; index < this.postConstraints.Count; ++index)
    {
      if (!this.postConstraints[index].mute && !this.postConstraints[index].CheckConstraint(spawned, spawnData))
      {
        Object.DestroyImmediate((Object) spawned);
        return (GameObject) null;
      }
    }
    return spawned;
  }

  private PropSpawner.SpawnData GetRandomPoint()
  {
    Vector3 from = this.transform.position + this.transform.up * Mathf.Lerp(-0.5f, 0.5f, Random.value) * this.height;
    Vector3 normalized = Vector3.ProjectOnPlane(Random.onUnitSphere, this.transform.up).normalized;
    if (!this.rayCastSpawn)
      return new PropSpawner.SpawnData()
      {
        pos = from,
        normal = normalized,
        rayDir = normalized,
        hit = new RaycastHit(),
        spawnerTransform = this.transform
      };
    RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + normalized * this.rayLength, this.layerType);
    if (!(bool) (Object) raycastHit.transform)
      return (PropSpawner.SpawnData) null;
    return new PropSpawner.SpawnData()
    {
      pos = raycastHit.point,
      normal = raycastHit.normal,
      rayDir = normalized,
      hit = raycastHit,
      spawnerTransform = this.transform
    };
  }
}
