// Decompiled with JetBrains decompiler
// Type: DebugSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DebugSpawner : MonoBehaviour
{
  public GameObject objToSpawn;

  private void Update()
  {
    if (!Application.isEditor || !Input.GetKeyDown(KeyCode.Alpha0))
      return;
    this.Spawn();
  }

  private void Spawn()
  {
    Object.Instantiate<GameObject>(this.objToSpawn, HelperFunctions.LineCheck(MainCamera.instance.transform.position, MainCamera.instance.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.TerrainMap).point, Quaternion.identity);
  }
}
