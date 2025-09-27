// Decompiled with JetBrains decompiler
// Type: PSC_NearObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PSC_NearObject : PropSpawnerConstraint
{
  public bool inverted;
  public GameObject[] objects;
  public float radius;
  private bool outVal;

  public override bool CheckConstraint(PropSpawner.SpawnData spawnData)
  {
    this.outVal = this.inverted;
    foreach (Collider collider in Physics.OverlapSphere(spawnData.hit.point, this.radius))
    {
      for (int index = 0; index < this.objects.Length; ++index)
      {
        if (collider.transform.parent.name == this.objects[index].name)
          this.outVal = !this.inverted;
      }
    }
    return this.outVal;
  }
}
