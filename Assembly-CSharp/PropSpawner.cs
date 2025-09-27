// Decompiled with JetBrains decompiler
// Type: PropSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class PropSpawner : LevelGenStep
{
  public Vector2 area;
  public Vector3 rayDirectionOffset;
  public float rayLength = 5000f;
  public bool raycastPosition = true;
  public int nrOfSpawns = 500;
  [Range(0.0f, 1f)]
  public float chanceToUseSpawner = 1f;
  public int currentSpawns;
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
    Vector3 vector3_1 = this.transform.position + this.area.y * 0.5f * this.transform.up;
    Vector3 from1 = this.transform.position - this.area.y * 0.5f * this.transform.up;
    Vector3 from2 = this.transform.position - this.area.x * 0.5f * this.transform.right;
    Vector3 vector3_2 = this.transform.position + this.area.x * 0.5f * this.transform.right;
    Gizmos.DrawLine(from1, vector3_1);
    Gizmos.DrawLine(from2, vector3_2);
    Gizmos.color = Color.cyan;
    Gizmos.DrawLine(from1, from1 + this.transform.forward * this.rayLength);
    Gizmos.DrawLine(vector3_1, vector3_1 + this.transform.forward * this.rayLength);
    Gizmos.DrawLine(from2, from2 + this.transform.forward * this.rayLength);
    Gizmos.DrawLine(vector3_2, vector3_2 + this.transform.forward * this.rayLength);
    Matrix4x4 matrix = Gizmos.matrix;
    Gizmos.matrix = Matrix4x4.TRS(this.transform.position + this.transform.forward * this.rayLength / 2f, this.transform.rotation, Vector3.one);
    Gizmos.DrawWireCube(Vector3.zero, this.area.xyn(this.rayLength));
    Gizmos.matrix = matrix;
  }

  public override void Go()
  {
    this.Clear();
    this.Add();
  }

  public void Add()
  {
    if ((double) this.chanceToUseSpawner < 0.99900001287460327 && (double) Random.value > (double) this.chanceToUseSpawner)
      return;
    int num = 50000;
    int currentSpawnCount = 0;
    while (currentSpawnCount < this.nrOfSpawns && num > 0)
    {
      --num;
      if (this.TryToSpawn(currentSpawnCount))
      {
        ++currentSpawnCount;
        if (this.syncTransforms)
          Physics.SyncTransforms();
      }
    }
    this.currentSpawns = this.transform.childCount;
    this.SpawnDecor();
  }

  private void SpawnDecor()
  {
    foreach (LevelGenStep componentsInChild in this.GetComponentsInChildren<DecorSpawner>())
      componentsInChild.Go();
  }

  public override void Clear()
  {
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) this.transform.GetChild(index).gameObject);
  }

  public void GoAll() => this.GetComponentInParent<PropGrouper>().RunAll();

  public void ClearAll() => this.GetComponentInParent<PropGrouper>().ClearAll();

  private bool TryToSpawn(int currentSpawnCount)
  {
    PropSpawner.SpawnData randomPoint = this.GetRandomPoint();
    if (randomPoint == null)
      return false;
    for (int index = 0; index < this.constraints.Count; ++index)
    {
      if (!this.constraints[index].mute && !this.constraints[index].CheckConstraint(randomPoint))
        return false;
    }
    randomPoint.spawnCount = currentSpawnCount;
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
    Vector3 position = this.transform.position;
    Vector2 vector2 = new Vector2(Random.value, Random.value);
    Vector3 from = position + this.transform.right * Mathf.Lerp((float) (-(double) this.area.x * 0.5), this.area.x * 0.5f, vector2.x) + this.transform.up * Mathf.Lerp((float) (-(double) this.area.y * 0.5), this.area.y * 0.5f, vector2.y);
    if (!this.raycastPosition)
      return new PropSpawner.SpawnData()
      {
        pos = from,
        normal = -this.transform.forward,
        rayDir = this.transform.forward,
        hit = new RaycastHit(),
        spawnerTransform = this.transform,
        placement = vector2
      };
    RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + (this.transform.forward + this.rayDirectionOffset).normalized * this.rayLength, this.layerType);
    if (!(bool) (Object) raycastHit.transform)
      return (PropSpawner.SpawnData) null;
    return new PropSpawner.SpawnData()
    {
      pos = raycastHit.point,
      normal = raycastHit.normal,
      rayDir = this.transform.forward,
      hit = raycastHit,
      spawnerTransform = this.transform,
      placement = vector2
    };
  }

  public class SpawnData
  {
    public Transform spawnerTransform;
    public Vector3 pos;
    public Vector3 normal;
    public Vector3 rayDir;
    public RaycastHit hit;
    public Vector2 placement;
    public int spawnCount;
  }
}
