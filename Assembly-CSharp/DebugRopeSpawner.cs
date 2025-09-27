// Decompiled with JetBrains decompiler
// Type: DebugRopeSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DebugRopeSpawner : MonoBehaviour
{
  public GameObject ropeSegment;
  public int segments = 10;
  public float spacing = 0.4f;

  public void Spawn()
  {
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) this.transform.GetChild(index).gameObject);
    for (int index = 0; index < this.segments; ++index)
    {
      GameObject gameObject = HelperFunctions.SpawnPrefab(this.ropeSegment, this.transform.position + this.transform.up * -this.spacing * (float) index, this.transform.rotation, this.transform);
      if (index > 0)
        gameObject.GetComponent<ConfigurableJoint>().connectedBody = this.transform.GetChild(index - 1).GetComponent<Rigidbody>();
    }
  }
}
