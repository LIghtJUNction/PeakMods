// Decompiled with JetBrains decompiler
// Type: Spawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

#nullable disable
public class Spawner : MonoBehaviourPunCallbacks, ISpawner
{
  public int playersInRoomRequirement;
  public int belowAscentRequirement = -1;
  public Spawner.SpawnMode spawnMode = Spawner.SpawnMode.SpawnPool;
  [FormerlySerializedAs("spawnCountMode")]
  public Spawner.SpawnPointMode spawnPointMode;
  [Range(0.0f, 1f)]
  public float baseSpawnChance;
  public GameObject spawnedObjectPrefab;
  public SpawnList spawns;
  public SpawnPool spawnPool;
  public bool canRepeatSpawns;
  public List<Transform> spawnSpots;
  public List<Spawner.WeightedSpawnPointEntry> weightedSpawnSpots = new List<Spawner.WeightedSpawnPointEntry>();
  public Transform spawnUpTowardsTarget;
  public bool spawnTransformIsSpawnerTransform;
  public bool spawnAwayFromUpTarget;
  public bool centerItemsVisually;
  public bool spawnOnStart;
  public bool isKinematic = true;
  public List<Spawner.HeightBasedSpawnListEntry> heightBasedSpawnPools;

  protected bool isWeightedSpawnPoints
  {
    get => this.spawnPointMode == Spawner.SpawnPointMode.WeightedLists;
  }

  private bool isSpawnPool => this.spawnMode == Spawner.SpawnMode.SpawnPool;

  private bool isSingleItem => this.spawnMode == Spawner.SpawnMode.SingleItem;

  private bool isHeightBasedSpawnPool => this.spawnMode == Spawner.SpawnMode.HeightBasedSpawnPools;

  public bool hasSpawnList
  {
    get
    {
      return this.isSpawnPool && (UnityEngine.Object) this.spawns != (UnityEngine.Object) null && this.spawnPool == SpawnPool.None;
    }
  }

  public void ForceSpawn() => this.TrySpawnItems();

  public List<PhotonView> TrySpawnItems()
  {
    List<PhotonView> photonViewList = new List<PhotonView>();
    if (!PhotonNetwork.IsMasterClient || !this.spawnOnStart)
      return photonViewList;
    if (this.playersInRoomRequirement > PhotonNetwork.PlayerList.Length)
    {
      Debug.Log((object) $"Not spawning: {this.gameObject.name} because of players in room req: {this.playersInRoomRequirement}");
      return photonViewList;
    }
    if (this.belowAscentRequirement != -1 && Ascents.currentAscent >= this.belowAscentRequirement)
    {
      Debug.Log((object) $"Not spawning: {this.gameObject.name} because ascent is too high: {Ascents.currentAscent}");
      return photonViewList;
    }
    if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) this.baseSpawnChance)
      photonViewList.AddRange((IEnumerable<PhotonView>) this.SpawnItems(this.GetSpawnSpots()));
    return photonViewList;
  }

  protected virtual List<Transform> GetSpawnSpots()
  {
    switch (this.spawnPointMode)
    {
      case Spawner.SpawnPointMode.SingleList:
        return this.spawnSpots;
      case Spawner.SpawnPointMode.WeightedLists:
        return this.weightedSpawnSpots.RandomSelection<Spawner.WeightedSpawnPointEntry>((Func<Spawner.WeightedSpawnPointEntry, int>) (w => w.weight)).spawnSpots;
      default:
        return new List<Transform>();
    }
  }

  public virtual List<PhotonView> SpawnItems(List<Transform> spawnSpots)
  {
    List<PhotonView> photonViewList = new List<PhotonView>();
    Debug.Log((object) $"Spawning: {this.gameObject}");
    if (!PhotonNetwork.IsMasterClient || spawnSpots.Count == 0)
      return photonViewList;
    List<GameObject> objectsToSpawn = this.GetObjectsToSpawn(spawnSpots.Count, this.canRepeatSpawns);
    for (int index = 0; index < spawnSpots.Count && index < objectsToSpawn.Count; ++index)
    {
      if (!((UnityEngine.Object) objectsToSpawn[index] == (UnityEngine.Object) null))
      {
        Item component = PhotonNetwork.InstantiateItemRoom(objectsToSpawn[index].name, spawnSpots[index].position, spawnSpots[index].rotation).GetComponent<Item>();
        photonViewList.Add(component.GetComponent<PhotonView>());
        if ((bool) (UnityEngine.Object) this.spawnUpTowardsTarget)
          component.transform.up = (this.spawnUpTowardsTarget.position - component.transform.position).normalized;
        if (this.centerItemsVisually)
        {
          Vector3 vector3 = spawnSpots[index].position - component.Center();
          component.transform.position += vector3;
        }
        component.ForceSyncForFrames();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && this.isKinematic)
          component.GetComponent<PhotonView>().RPC("SetKinematicRPC", RpcTarget.AllBuffered, (object) true, (object) component.transform.position, (object) component.transform.rotation);
      }
    }
    return photonViewList;
  }

  protected SpawnPool GetSpawnPool()
  {
    if (this.isHeightBasedSpawnPool)
    {
      for (int index = this.heightBasedSpawnPools.Count - 1; index >= 0; --index)
      {
        Spawner.HeightBasedSpawnListEntry heightBasedSpawnPool = this.heightBasedSpawnPools[index];
        if (index == 0 || (double) this.transform.position.z > (double) heightBasedSpawnPool.minimumZ && (!heightBasedSpawnPool.hasBiomeRequirement || Singleton<MapHandler>.Instance.BiomeIsPresent(heightBasedSpawnPool.biomeRequirement)))
          return heightBasedSpawnPool.spawnPool;
      }
    }
    return this.spawnPool;
  }

  private List<GameObject> GetObjectsToSpawn(int spawnCount, bool canRepeat = false)
  {
    if (this.isSingleItem)
    {
      List<GameObject> objectsToSpawn = new List<GameObject>();
      for (int index = 0; index < spawnCount; ++index)
        objectsToSpawn.Add(this.spawnedObjectPrefab);
      return objectsToSpawn;
    }
    if (this.isSpawnPool)
      return LootData.GetRandomItems(this.spawnPool, spawnCount, canRepeat);
    if (this.isHeightBasedSpawnPool)
    {
      for (int index = this.heightBasedSpawnPools.Count - 1; index >= 0; --index)
      {
        Spawner.HeightBasedSpawnListEntry heightBasedSpawnPool = this.heightBasedSpawnPools[index];
        if (index == 0 || (double) this.transform.position.z > (double) heightBasedSpawnPool.minimumZ && (!heightBasedSpawnPool.hasBiomeRequirement || Singleton<MapHandler>.Instance.BiomeIsPresent(heightBasedSpawnPool.biomeRequirement)))
          return LootData.GetRandomItems(heightBasedSpawnPool.spawnPool, spawnCount, canRepeat);
      }
    }
    List<GameObject> objectsToSpawn1 = new List<GameObject>();
    for (int index = 0; index < spawnCount; ++index)
      objectsToSpawn1.Add((GameObject) null);
    return objectsToSpawn1;
  }

  private void FindOutdatedSpawners()
  {
    bool flag = false;
    Spawner[] objectsOfType = UnityEngine.Object.FindObjectsOfType<Spawner>();
    string message = "";
    foreach (Spawner spawner in objectsOfType)
    {
      if (spawner.hasSpawnList)
      {
        message = $"{message}Found outdated spawner: {spawner.gameObject.name}\n";
        flag = true;
      }
    }
    if (!flag)
      Debug.Log((object) "NO OUTDATED SPAWNERS! YIPPEEEE");
    else
      Debug.Log((object) message);
  }

  [ContextMenu("Test Weighted Spawn Points")]
  private void TestWeightedSpawnPoints()
  {
    Dictionary<int, int> dictionary = new Dictionary<int, int>();
    int num = 1000;
    for (int index = 0; index < num; ++index)
    {
      int key = this.weightedSpawnSpots.IndexOf(this.weightedSpawnSpots.RandomSelection<Spawner.WeightedSpawnPointEntry>((Func<Spawner.WeightedSpawnPointEntry, int>) (w => w.weight)));
      if (dictionary.ContainsKey(key))
        dictionary[key]++;
      else
        dictionary.Add(key, 1);
    }
    string message = $"Test spawned {num} times.\n";
    foreach (int key in dictionary.Keys)
      message += $"Chose #{key} {dictionary[key]} times. ({(ValueType) (float) ((double) dictionary[key] / (double) num * 100.0)}%)\n";
    Debug.Log((object) message);
  }

  public void DebugSpawnRates()
  {
    SpawnPool spawnPool = this.GetSpawnPool();
    if (spawnPool == SpawnPool.None)
      return;
    LootData.PrintOdds(spawnPool);
  }

  private bool hasMultipleFlagsSet
  {
    get
    {
      int num = 0;
      foreach (SpawnPool flag in Enum.GetValues(typeof (SpawnPool)))
      {
        if (flag != SpawnPool.None && this.spawnPool.HasFlag((Enum) flag))
        {
          if (num >= 1)
            return true;
          ++num;
        }
      }
      return false;
    }
  }

  public enum SpawnMode
  {
    SingleItem,
    SpawnPool,
    HeightBasedSpawnPools,
    Guidebook,
  }

  public enum SpawnPointMode
  {
    SingleList,
    WeightedLists,
  }

  [Serializable]
  public class HeightBasedSpawnListEntry
  {
    public SpawnPool spawnPool;
    public float minimumHeight;
    public float minimumZ;
    public bool hasBiomeRequirement;
    public Biome.BiomeType biomeRequirement;
  }

  [Serializable]
  public class WeightedSpawnPointEntry
  {
    public List<Transform> spawnSpots;
    public int weight;
    [SerializeField]
    internal float percentageOdds;
  }
}
