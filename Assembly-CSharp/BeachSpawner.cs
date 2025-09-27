// Decompiled with JetBrains decompiler
// Type: BeachSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BeachSpawner : MonoBehaviour
{
  public GameObject[] palmTrees;
  public float radius;
  public Vector2Int treeSpawnRange;
  public List<GameObject> spawned;
  public Transform treeParent;
  public LayerMask layerMask;

  private void Spawn()
  {
    this.Clear();
    int num1 = Random.Range(this.treeSpawnRange.x, this.treeSpawnRange.y);
    int num2 = 20;
    for (int index = 0; index < num1; ++index)
    {
      if (!this.TrySpawn(this.palmTrees[Random.Range(0, this.palmTrees.Length)]) && num2 > 0)
      {
        --num2;
        --index;
      }
    }
  }

  private void Clear()
  {
    foreach (Object @object in this.spawned)
      Object.DestroyImmediate(@object);
    this.spawned.Clear();
  }

  private bool TrySpawn(GameObject go)
  {
    float f = Random.Range(0.0f, 360f);
    float num = Random.Range(0.0f, this.radius);
    Vector3 vector3 = new Vector3(Mathf.Cos(f), 0.0f, Mathf.Sin(f)) * num + this.treeParent.position;
    RaycastHit hitInfo;
    if (Physics.Linecast(vector3 + Vector3.up * 100f, vector3 - Vector3.up * 100f, out hitInfo, this.layerMask.value, QueryTriggerInteraction.UseGlobal))
    {
      Debug.Log((object) hitInfo.collider.gameObject.name, (Object) hitInfo.collider.gameObject);
      if (hitInfo.collider.gameObject.CompareTag("Sand"))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(go, hitInfo.point, Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f));
        gameObject.transform.SetParent(this.treeParent);
        this.spawned.Add(gameObject);
        return true;
      }
    }
    return false;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(0.73f, 0.57f, 0.0f);
    Gizmos.DrawWireSphere(this.treeParent.position, this.radius);
  }
}
