// Decompiled with JetBrains decompiler
// Type: LootData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

#nullable disable
public class LootData : MonoBehaviour
{
  public Rarity Rarity;
  public SpawnPool spawnLocations;
  public List<ItemRarityOverride> rarityOverrides = new List<ItemRarityOverride>();
  public bool banInSolo;
  public static Dictionary<SpawnPool, Dictionary<ushort, int>> AllSpawnWeightData = (Dictionary<SpawnPool, Dictionary<ushort, int>>) null;
  public static Dictionary<Rarity, int> RarityWeights = new Dictionary<Rarity, int>()
  {
    {
      Rarity.Common,
      100
    },
    {
      Rarity.Uncommon,
      50
    },
    {
      Rarity.Rare,
      30
    },
    {
      Rarity.Epic,
      20
    },
    {
      Rarity.Legendary,
      15
    },
    {
      Rarity.Mythic,
      5
    },
    {
      Rarity.RidiculouslyRare,
      1
    }
  };

  public static List<Item> GetAllItemsInPool(SpawnPool pool)
  {
    List<Item> allItemsInPool = new List<Item>();
    LootData.PopulateLootData();
    Dictionary<ushort, int> dictionary;
    if (LootData.AllSpawnWeightData.TryGetValue(pool, out dictionary))
    {
      foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
      {
        Item obj;
        if (ItemDatabase.TryGetItem(keyValuePair.Key, out obj))
          allItemsInPool.Add(obj);
      }
    }
    return allItemsInPool;
  }

  public bool IsValidToSpawn()
  {
    return !this.banInSolo || !PhotonNetwork.OfflineMode && (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount > 1);
  }

  private void PrintOdds()
  {
    LootData.PopulateLootData();
    Item component = this.GetComponent<Item>();
    if (!(bool) (UnityEngine.Object) component)
      Debug.LogError((object) "Loot data only works on Items right now.");
    string message = this.gameObject.name + " appears in pools:\n";
    foreach (KeyValuePair<SpawnPool, Dictionary<ushort, int>> keyValuePair in LootData.AllSpawnWeightData)
    {
      if (keyValuePair.Value.ContainsKey(component.itemID))
        message += $"{keyValuePair.Key.ToString()} ({LootData.GetPercentageOdds(component.itemID, keyValuePair.Key)}%)\n";
    }
    Debug.Log((object) message);
  }

  public static void PrintOdds(SpawnPool pool)
  {
    LootData.PopulateLootData();
    string message = pool.ToString() + " contains items:\n";
    Dictionary<ushort, int> dictionary;
    if (LootData.AllSpawnWeightData.TryGetValue(pool, out dictionary))
    {
      foreach (KeyValuePair<ushort, int> keyValuePair in dictionary)
      {
        Item obj;
        if (ItemDatabase.TryGetItem(keyValuePair.Key, out obj))
        {
          LootData component = obj.GetComponent<LootData>();
          message = !(bool) (UnityEngine.Object) component ? message + $"{obj.gameObject.name} ({LootData.GetPercentageOdds(keyValuePair.Key, pool)}%)\n" : message + $"{obj.gameObject.name} ({LootData.GetPercentageOdds(keyValuePair.Key, pool)}% ({component.Rarity.ToString()}))\n";
        }
      }
    }
    Debug.Log((object) message);
  }

  public static GameObject GetRandomItem(SpawnPool spawnPool)
  {
    if (LootData.AllSpawnWeightData == null)
      LootData.PopulateLootData();
    Dictionary<ushort, int> enumerable;
    if (!LootData.AllSpawnWeightData.TryGetValue(spawnPool, out enumerable))
      return (GameObject) null;
    Item obj;
    ItemDatabase.TryGetItem(enumerable.RandomSelection<KeyValuePair<ushort, int>>((Func<KeyValuePair<ushort, int>, int>) (i => i.Value)).Key, out obj);
    return obj.gameObject;
  }

  public static List<GameObject> GetRandomItems(SpawnPool spawnPool, int count, bool canRepeat = false)
  {
    if (LootData.AllSpawnWeightData == null)
      LootData.PopulateLootData();
    Dictionary<ushort, int> dictionary;
    if (!LootData.AllSpawnWeightData.TryGetValue(spawnPool, out dictionary))
      return (List<GameObject>) null;
    Dictionary<ushort, int> enumerable = new Dictionary<ushort, int>((IDictionary<ushort, int>) dictionary);
    List<GameObject> randomItems = new List<GameObject>();
    for (int index = 0; index < count; ++index)
    {
      ushort key = enumerable.RandomSelection<KeyValuePair<ushort, int>>((Func<KeyValuePair<ushort, int>, int>) (i => i.Value)).Key;
      Item obj;
      if (ItemDatabase.TryGetItem(key, out obj))
      {
        if (!obj.IsValidToSpawn())
        {
          Debug.Log((object) (obj.gameObject.name + " IS INVALID TO SPAWN"));
          enumerable.Remove(key);
          --index;
        }
        else
        {
          randomItems.Add(obj.gameObject);
          if (!canRepeat)
            enumerable.Remove(key);
        }
      }
    }
    return randomItems;
  }

  public static float GetPercentageOdds(ushort itemID, SpawnPool pool)
  {
    if (!LootData.AllSpawnWeightData.ContainsKey(pool))
      return 0.0f;
    int num1 = 0;
    int num2 = 0;
    foreach (KeyValuePair<ushort, int> keyValuePair in LootData.AllSpawnWeightData[pool])
    {
      num1 += keyValuePair.Value;
      if ((int) keyValuePair.Key == (int) itemID)
        num2 = keyValuePair.Value;
    }
    return (float) Mathf.FloorToInt((float) ((double) num2 / (double) num1 * 1000.0)) / 10f;
  }

  public static void PopulateLootData()
  {
    LootData.AllSpawnWeightData = new Dictionary<SpawnPool, Dictionary<ushort, int>>();
    Array values = Enum.GetValues(typeof (SpawnPool));
    foreach (KeyValuePair<ushort, Item> keyValuePair in SingletonAsset<ItemDatabase>.Instance.itemLookup)
    {
      LootData component = keyValuePair.Value.GetComponent<LootData>();
      if ((bool) (UnityEngine.Object) component)
      {
        foreach (SpawnPool spawnPool in values)
        {
          if (spawnPool != SpawnPool.None && component.spawnLocations.HasFlag((Enum) spawnPool))
          {
            int rarityWeight = LootData.RarityWeights[component.Rarity];
            if (!LootData.AllSpawnWeightData.ContainsKey(spawnPool))
              LootData.AllSpawnWeightData.Add(spawnPool, new Dictionary<ushort, int>()
              {
                {
                  keyValuePair.Key,
                  rarityWeight
                }
              });
            else
              LootData.AllSpawnWeightData[spawnPool].Add(keyValuePair.Key, rarityWeight);
          }
        }
      }
    }
  }
}
