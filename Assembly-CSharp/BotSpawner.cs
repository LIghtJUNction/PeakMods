// Decompiled with JetBrains decompiler
// Type: BotSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using pworld.Scripts.Extensions;
using UnityEngine;

#nullable disable
public class BotSpawner : MonoBehaviour
{
  public GameObject botPrefab;

  private void Go() => this.SpawnBot(PatrolBoss.me.transform.position);

  public void SpawnBot(Vector3 spawnPosition)
  {
    bool flag = false;
    for (int index = 0; index < 10; ++index)
    {
      if (TrySpawnBot(spawnPosition + ExtMath.RandInsideUnitCircle().xoy() * 2f))
      {
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    Debug.LogWarning((object) "Could not spawn troop");

    bool TrySpawnBot(Vector3 spawnPosition)
    {
      foreach (Collider collider in Physics.OverlapSphere(spawnPosition, 2f))
      {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Terrain") && collider.gameObject.layer != LayerMask.NameToLayer("Prop"))
          return false;
      }
      Object.Instantiate<GameObject>(this.botPrefab, spawnPosition, Quaternion.identity);
      Debug.Log((object) "Spawn Bot");
      return true;
    }
  }
}
