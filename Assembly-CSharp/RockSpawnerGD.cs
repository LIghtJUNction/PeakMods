// Decompiled with JetBrains decompiler
// Type: RockSpawnerGD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RockSpawnerGD : MonoBehaviour
{
  public Vector2 colliderScale;
  public SpawnObject[] objectsToSpawn;
  public List<GameObject> spawnedObjects;
  public List<SpawnObject> deck;
  public Vector2 castShape;
  public BoxCollider shape;
  public float yBias;
  [Range(1f, 99f)]
  public int layerCount;

  public void createDeck()
  {
    this.deck.Clear();
    for (int index1 = 0; index1 < this.objectsToSpawn.Length; ++index1)
    {
      for (int index2 = 0; index2 < this.objectsToSpawn[index1].maxCount; ++index2)
        this.deck.Add(this.objectsToSpawn[index1]);
    }
    this.shuffleDeck();
  }

  public void shuffleDeck()
  {
    for (int index1 = 0; index1 < this.deck.Count; ++index1)
    {
      SpawnObject spawnObject = this.deck[index1];
      int index2 = Random.Range(index1, this.objectsToSpawn.Length);
      this.deck[index1] = this.deck[index2];
      this.deck[index2] = spawnObject;
    }
  }

  public SpawnObject DrawFromDeck()
  {
    SpawnObject spawnObject = this.deck[0];
    this.deck.RemoveAt(0);
    return spawnObject;
  }

  public void spawnObjects()
  {
    this.clearList();
    this.createDeck();
    int count = this.deck.Count;
    int num = count / this.layerCount;
    if (this.layerCount > count)
      num = count;
    for (int index = 0; index < count; ++index)
    {
      float p = (float) ((double) index * (double) this.yBias + 1.0);
      RaycastHit hitInfo;
      if (Physics.Raycast(this.transform.position - this.transform.up + (this.transform.right * Random.Range(-1f, 1f) * this.shape.size.x / 2f + this.transform.forward * (float) ((double) Mathf.Pow(Random.Range(-1f, 1f), p) * (double) this.shape.size.z / 2.0)), -this.transform.up, out hitInfo))
      {
        SpawnObject spawnObject = this.DrawFromDeck();
        GameObject gameObject = Object.Instantiate<GameObject>(spawnObject.prefab);
        gameObject.transform.position = hitInfo.point + new Vector3(Random.Range(-spawnObject.posJitter.x, spawnObject.posJitter.x), Random.Range(-spawnObject.posJitter.y, spawnObject.posJitter.y), Random.Range(-spawnObject.posJitter.z, spawnObject.posJitter.z));
        gameObject.transform.eulerAngles += new Vector3(Random.Range(-spawnObject.randomRot.x, spawnObject.randomRot.x), Random.Range(-spawnObject.randomRot.y, spawnObject.randomRot.y), Random.Range(-spawnObject.randomRot.z, spawnObject.randomRot.z));
        gameObject.transform.localScale += new Vector3(Random.Range(-spawnObject.randomScale.x, spawnObject.randomScale.x), Random.Range(-spawnObject.randomScale.y, spawnObject.randomScale.y), Random.Range(-spawnObject.randomScale.z, spawnObject.randomScale.z));
        gameObject.transform.localScale += Vector3.one * Random.Range(-spawnObject.uniformScale, spawnObject.uniformScale);
        gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, Vector3.one - new Vector3((float) Random.Range(0.0f, spawnObject.inversion.x).PCeilToInt(), (float) Random.Range(0.0f, spawnObject.inversion.y).PCeilToInt(), (float) Random.Range(0.0f, spawnObject.inversion.z).PCeilToInt()).normalized * 2f);
        gameObject.transform.localScale *= spawnObject.scaleMultiplier;
        this.spawnedObjects.Add(gameObject);
        gameObject.transform.parent = this.transform;
        if (index % num == 0)
          Physics.SyncTransforms();
      }
    }
  }

  public void clearList()
  {
    for (int index = 0; index < this.spawnedObjects.Count; ++index)
      Object.DestroyImmediate((Object) this.spawnedObjects[index]);
    this.spawnedObjects.Clear();
  }

  public void OnValidate()
  {
    this.shape.size = new Vector3(this.colliderScale.x, 0.0f, this.colliderScale.y);
  }

  private void OnDrawGizmosSelected()
  {
  }
}
