// Decompiled with JetBrains decompiler
// Type: ExplosionEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ExplosionEffect : MonoBehaviour
{
  public float speed = 1f;
  public GameObject explosionOrb;
  public float baseScale = 1f;
  public float explosionRadius = 5f;
  public float spawnRadiusFactor = 0.8f;
  public float childSizeFactor = 0.9f;
  public float minDelay = 0.4f;
  public float maxDelay = 0.5f;
  public float minSpeed = 0.75f;
  public float maxSpeed = 1.25f;
  public List<ExplosionOrb> explosionPoints = new List<ExplosionOrb>();

  private void Start()
  {
    this.GetPoints();
    foreach (ExplosionOrb explosionPoint in this.explosionPoints)
      this.StartCoroutine(IExplode(explosionPoint));

    IEnumerator IExplode(ExplosionOrb item)
    {
      yield return (object) new WaitForSeconds(item.delay / (this.speed * item.speed));
      GameObject gameObject = Object.Instantiate<GameObject>(this.explosionOrb, item.position, HelperFunctions.GetRandomRotationWithUp(item.direction));
      gameObject.GetComponentInChildren<Animator>().speed = this.speed * item.speed;
      gameObject.transform.localScale = Vector3.one * item.size * this.baseScale;
      MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
      MaterialPropertyBlock properties = new MaterialPropertyBlock();
      componentInChildren.GetPropertyBlock(properties);
      properties.SetFloat("_Random", Random.value);
      componentInChildren.SetPropertyBlock(properties);
    }
  }

  private void GetPoints()
  {
    this.explosionPoints.Clear();
    this.explosionPoints.Add(new ExplosionOrb()
    {
      position = this.transform.position,
      delay = 0.0f,
      direction = Vector3.up,
      size = 1f
    });
    for (int index = 0; index < 4; ++index)
    {
      Vector3 vector3 = Random.onUnitSphere * this.explosionRadius * this.spawnRadiusFactor;
      vector3.y = Mathf.Abs(vector3.y);
      this.explosionPoints.Add(new ExplosionOrb()
      {
        position = this.transform.position + vector3,
        delay = Random.Range(this.minDelay, this.maxDelay),
        direction = vector3,
        size = this.childSizeFactor,
        speed = Random.Range(this.minSpeed, this.maxSpeed)
      });
    }
    for (int index1 = this.explosionPoints.Count - 1; index1 >= 1; --index1)
    {
      for (int index2 = 0; index2 < 2; ++index2)
      {
        Vector3 position = this.explosionPoints[index1].position;
        Vector3 vector3 = Random.onUnitSphere * this.explosionRadius * this.explosionPoints[index1].size * this.spawnRadiusFactor;
        vector3.y = Mathf.Abs(vector3.y);
        this.explosionPoints.Add(new ExplosionOrb()
        {
          position = position + vector3,
          delay = this.explosionPoints[index1].delay + Random.Range(this.minDelay, this.maxDelay),
          direction = vector3,
          size = this.childSizeFactor * this.childSizeFactor,
          speed = this.explosionPoints[index1].speed * Random.Range(this.minSpeed, this.maxSpeed)
        });
      }
    }
  }

  public void OnDrawGizmosSelected()
  {
    foreach (ExplosionOrb explosionPoint in this.explosionPoints)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(explosionPoint.position, this.explosionRadius * explosionPoint.size);
    }
  }
}
